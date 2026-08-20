# Vòng đời FlowTask — từ step đầu tới step cuối

Tài liệu mô tả logic chạy của một `FlowTask` (nhiệm vụ chuyển hàng AMR) trong WCS, từ lúc khởi tạo tới khi hoàn tất, bao gồm cả nhánh **redirect** khi trạm hạ hàng không sẵn sàng.

---

## 1. Các thành phần chính

| Thành phần | Vai trò |
|-----------|---------|
| `FlowTask` | Entity trạng thái nhiệm vụ (`CurrentStep`, `ToStation`, `RcsTaskId`, `WaitingFor`, …) |
| `FlowStep` | Enum các bước của nhiệm vụ |
| `MainFlow` | State machine: nhận event → validate → `ProceedToNextStep` → gọi handler |
| `*Handler` (UseCases) | Mỗi step có 1 handler thực thi hành động (gọi RCS, OPC, WMS, crane…) |
| `RcsClient` | Giao tiếp HIK RCS: `genAgvSchedulingTask`, `continueTask`, `cancelTask` |
| `RcsCallbackController` | Nhận webhook HIK (`{n}_Complete`) → publish `RobotTaskComplete` |
| `FlowTaskTimeoutWorker` | Quét task quá hạn ở mỗi step → publish `TimeoutFired` |
| `ErrorFlow` | Xử lý `TimeoutFired` / `FlowErrorOccurred` (redirect hoặc cancel) |
| `HikCallbackStep` | Map số webhook HIK (`n`) → `FlowStep` |

---

## 2. Cơ chế điều khiển AMR qua HIK

Khác với cách cũ (truyền toàn bộ lộ trình khi tạo task), nay WCS quyết định **đích đến từng bước**:

- `genAgvSchedulingTask`: tạo task HIK, chỉ truyền **điểm đầu tiên** (waiting point trạm lấy hàng).
- `continueTask(taskCode, nextPositionCode)`: ở mỗi step, truyền `nextPositionCode = { PositionCode, Type="00" }` để chỉ định điểm kế tiếp.

Nhờ đó có thể đổi đích (redirect) giữa chừng mà không cần hủy task.

Helper: `RcsPositions.Berth(positionCode)` tạo `PositionCodePathItem` với `Type="00"`.

---

## 3. Danh sách step và mapping webhook HIK

Thứ tự step (theo `FlowConstant.FlowStepList`):

| # | FlowStep | Handler | Hành động RCS | Webhook HIK |
|---|----------|---------|---------------|-------------|
| 0 | `Initial` | — | — | — |
| 1 | `MoveToPickupWaitingPoint` | `MoveToPickupWaitingPointHandler` | `genAgvSchedulingTask` → From.Waiting | `1_Complete` |
| 2 | `BeforePick` | `BeforePickHandler` | (chờ rèm/băng tải/cassette) | — |
| 3 | `EnterPickupPoint` | `EnterPickupPointHandler` | `continueTask` → From.Main | `3_Complete` |
| 4 | `LiftRack` | `LiftRackHandler` | `continueTask` (nâng rack) | `4_Complete` |
| 5 | `BackToPickupWaitingPoint` | `BackToPickupWaitingPointHandler` | `continueTask` → From.Waiting | `5_Complete` |
| 6 | `AfterPickup` | `AfterPickupHandler` | (xác nhận WMS, OPC) → `FlowResumed` | — |
| 7 | `MoveToDropWaitingPoint` | `MoveToDropWaitingPointHandler` | `continueTask` → To.Waiting | `7_Complete` |
| 8 | `BeforeDrop` | `BeforeDropHandler` | (chờ rèm/băng tải) | — |
| — | *(filler leg)* | `MainFlow.IssueDropFiller` | `continueTask` → To.Waiting ×`DropFillerCount` | `71_Complete` |
| 9 | `EnterDropPoint` | `EnterDropPointHandler` | `continueTask` → To.Main | `9_Complete` |
| 10 | `PutdownRack` | `PutdownRackHandler` | `continueTask` (hạ rack) | `10_Complete` |
| 11 | `BackToDropWaitingPoint` | `BackToDropWaitingPointHandler` | `continueTask` → To.Waiting | `11_Complete` |
| 12 | `AfterDrop` | `AfterDropHandler` | (xác nhận WMS/crane) → `FlowResumed` | — |
| 13 | `Completed` | `EndFlowHandler` | kết thúc | — |

> Lưu ý: số webhook HIK **không** trùng với index FlowStep. Việc map do `HikCallbackStep.TryGetFlowStep(n)` đảm nhiệm, không index thẳng vào `FlowStepList`.
>
> **Filler leg `71` không phải FlowStep** — đây là leg đệm của template HIK, do `MainFlow` xử lý generic (đếm `ConsumedDropFillers`, số lượng = `HikConfig.DropFillerCount`).

---

## 4. Cơ chế chuyển step trong `MainFlow`

Mỗi event (callback, curtain, conveyor, FlowResumed…) đi qua `ProcessFlowEvent`:

```
1. Load task, bỏ qua nếu Status không phải Active/Pending
2. (Chung) Nếu task có `Reroute` đang chờ và HIK báo đúng leg (`RobotTaskComplete.Step == Reroute.ExpectedHikLeg`) → resume tại `Reroute.ResumeStep`
3. ValidateFlowEvent: event có được phép ở step hiện tại? có đúng task/station?
4. Cập nhật WaitingFor:
   - Không có điều kiện chờ → ProceedToNextStep
   - Đủ điều kiện → clear + ProceedToNextStep
   - Còn thiếu → mark, dừng chờ
```

`ProceedToNextStep`:
```
nếu CurrentStep == BeforeDrop và ConsumedDropFillers < DropFillerCount
    → IssueDropFiller (continueTask To.Waiting) và dừng chờ *_Complete  (chưa sang EnterDropPoint)
ngược lại:
    nextStep = GetNextStep(CurrentStep)   // bước kế tiếp theo FlowStepList
    publish FlowChangeStep
    gọi handler tương ứng nextStep (RunStepHandler)
```

> **Cơ chế reroute chung (Phương án 1).** Mọi điều hướng giữa chừng được mô tả bởi một
> `RerouteRequest` đặt trên task:
> - `ExpectedHikLeg` — leg HIK báo AMR đã tới điểm reroute → `MainFlow` resume tại `ResumeStep`.
> - `ResumeStep` — step chạy lại sau khi tới nơi (vd `BeforeDrop` để chờ rèm trạm mới).
>
> `RerouteCount` đếm số lần đã reroute để giới hạn.

> **Filler leg (leg đệm HIK).** Template HIK có (các) leg dư giữa BeforeDrop và EnterDropPoint
> — không mang ý nghĩa nghiệp vụ, chỉ để có chỗ trống cho redirect. Chúng KHÔNG còn là `FlowStep`:
> - Số lượng cấu hình bằng `HikConfig.DropFillerCount` (mặc định 1, tăng tuỳ ý).
> - `MainFlow` tự bơm `continueTask(To.Waiting)` cho từng filler và đếm `ConsumedDropFillers`,
>   tới đủ thì mới sang `EnterDropPoint`. Không có step/handler riêng.
> - Redirect tiêu thụ trước 1 filler để di chuyển sang trạm mới, nên bộ đếm tự khớp và sau khi
>   rèm trạm mới mở sẽ vào thẳng `EnterDropPoint` — không cần logic "skip" đặc biệt.

**Điều kiện chờ (`WaitingFor`)**: `BeforePick` / `BeforeDrop` có thể cần chờ các event như `CurtainOpened`, `ConveyorBoxArrived/Removed`, `BoxArrived/Removed`. Handler set `WaitingFor` qua `StationService.GetWaitingRequirements`. Khi đủ điều kiện, publish `FlowResumed` để đi tiếp.

---

## 5. Luồng bình thường (happy path)

```
[Tạo task] FlowStarted
   │
   ▼
(1) MoveToPickupWaitingPoint ──genAgvSchedulingTask(From.Waiting)──► AMR tới From.Waiting
   │  ◄── 1_Complete
   ▼
(2) BeforePick ── chờ rèm/băng tải tại From ──► FlowResumed
   │
   ▼
(3) EnterPickupPoint ──continueTask(From.Main)──► AMR vào From.Main
   │  ◄── 3_Complete
   ▼
(4) LiftRack ──continueTask(nâng rack)──► AMR nâng hàng   ★ bắt đầu mang hàng
   │  ◄── 4_Complete
   ▼
(5) BackToPickupWaitingPoint ──continueTask(From.Waiting)──► AMR lùi ra From.Waiting
   │  ◄── 5_Complete
   ▼
(6) AfterPickup ── xác nhận WMS/OPC ──► FlowResumed
   │
   ▼
(7) MoveToDropWaitingPoint ──continueTask(To.Waiting)──► AMR tới To.Waiting
   │  ◄── 7_Complete
   ▼
(8) BeforeDrop ── chờ rèm/băng tải tại To ──► FlowResumed
   │
   │  [filler leg] MainFlow tự bơm continueTask(To.Waiting) × DropFillerCount
   │  ◄── 71_Complete  (tiêu thụ filler, KHÔNG phải FlowStep)
   ▼
(9) EnterDropPoint ──continueTask(To.Main)──► AMR vào To.Main
   │  ◄── 9_Complete
   ▼
(10) PutdownRack ──continueTask(hạ rack)──► AMR hạ hàng    ★ kết thúc mang hàng
   │  ◄── 10_Complete
   ▼
(11) BackToDropWaitingPoint ──continueTask(To.Waiting)──► AMR lùi ra To.Waiting
   │  ◄── 11_Complete
   ▼
(12) AfterDrop ── xác nhận WMS/crane ──► FlowResumed
   │
   ▼
(13) Completed ── EndFlow
```

---

## 6. Nhánh redirect — trạm hạ hàng không sẵn sàng

### 6.1. Điều kiện kích hoạt

Tại step `BeforeDrop`, nếu chờ rèm/băng tải quá hạn (`FlowTaskTimeoutWorker` → `TimeoutFired`), `ErrorFlow.Handle(TimeoutFired)` kiểm tra:

```
CurrentStep == BeforeDrop
  && RerouteCount == 0                          // chưa redirect lần nào
  && AmrLoadStateService.CanRedirectDrop(task) // AMR đang mang hàng
```

- **AMR mang hàng** → redirect (chuyển trạm).
- **AMR không mang hàng** → cancel task như cũ.

> Trạng thái "đang mang hàng" hiện suy luận từ `FlowStep` (`AmrLoadStateService`). Khi lắp cảm biến, chỉ cần sửa logic trong service này.

### 6.2. Logic redirect (`DropStationRedirectService.TryRedirectAsync`)

```
1. Lấy danh sách trạm fallback (GetFallbackStationsAsync), bỏ trạm đang bận
2. Chọn trạm B khả dụng:
   - ToStation = B
   - continueTask(B.Waiting)          ◄── tiêu thụ 1 filler leg (71) để di chuyển A.Waiting→B.Waiting
   - ConsumedDropFillers++            ◄── khớp bộ đếm filler
   - RerouteCount++
   - Reroute = RerouteRequest(ExpectedHikLeg=71, ResumeStep=BeforeDrop)
   - giữ CurrentStep = BeforeDrop (refresh timeout window)
3. Publish BizError E005: "Trạm A không thể hạ hàng, chuyển hàng tới B"
```

### 6.3. Khi AMR tới trạm mới B

```
AMR tới B.Waiting ──► HIK 71_Complete
   │
   ▼
MainFlow nhánh reroute chung (Reroute != null && step == Reroute.ExpectedHikLeg(71))
   │
   ▼
RunStepHandler(task, Reroute.ResumeStep=BeforeDrop)          ★ CHỜ RÈM LẠI TẠI B
   │  (CurrentStep quay về BeforeDrop, WaitingFor = rèm@B)
   ▼
Rèm B mở ──► FlowResumed/CurtainOpened
   │
   ▼
ProceedToNextStep: filler check → ConsumedDropFillers(1) == DropFillerCount(1)
   │  → không bơm filler nữa → nextStep = GetNextStep(BeforeDrop) = EnterDropPoint
   ▼
(9) EnterDropPoint ──continueTask(B.Main)──► AMR vào B.Main
   │  ◄── 9_Complete
   ▼
... tiếp tục như happy path (PutdownRack → BackToDropWaitingPoint → AfterDrop → Completed)
```

### 6.4. Số leg HIK tiêu thụ — giống nhau cả 2 case

| Case | Các leg HIK sau khi nâng hàng |
|------|-------------------------------|
| Bình thường | `7` (→To.Waiting), `71` (filler giữ chỗ), `9` (→To.Main), `10`, `11` |
| Redirect | `7` (→A.Waiting), `71` (A.Waiting→B.Waiting), `9` (→B.Main), `10`, `11` |

`71` là filler leg: case thường = giữ chỗ tại waiting; case redirect = di chuyển sang trạm mới.
Cả hai đều tiêu thụ đúng `DropFillerCount` filler nên bộ đếm `ConsumedDropFillers` luôn khớp.

### 6.5. Giới hạn

- **Chỉ redirect 1 lần** (`RerouteCount == 0` mới cho redirect). Nếu trạm B cũng timeout → cancel task.
- Số filler leg khả dụng = `HikConfig.DropFillerCount` (khớp số leg đệm dư trong template HIK).

---

## 7. Sơ đồ trạng thái (rút gọn)

```
Initial → MoveToPickupWaitingPoint → BeforePick → EnterPickupPoint → LiftRack
   → BackToPickupWaitingPoint → AfterPickup → MoveToDropWaitingPoint → BeforeDrop
        │                                  (filler leg ×DropFillerCount, không phải FlowStep)
        ├─(rèm mở)──────────────► [filler] ─► EnterDropPoint ─► PutdownRack
        │                                            ▲
        └─(timeout + mang hàng)─► [redirect B: tiêu thụ 1 filler] ─► (rèm B mở) ─┘
   → BackToDropWaitingPoint → AfterDrop → Completed
```

---

## 8. Xử lý lỗi & timeout

| Tình huống | Xử lý |
|-----------|-------|
| Timeout tại `BeforeDrop` + mang hàng + chưa redirect | Redirect sang trạm fallback (giữ task) |
| Timeout tại `BeforeDrop` + không mang hàng | Cancel task |
| Timeout tại `BeforeDrop` (B) sau redirect | Cancel (đã redirect 1 lần) |
| Timeout các step khác | `ErrorFlow` cancel task + `BizError` |
| `FlowErrorOccurred` (exception trong handler) | `ErrorFlow` cancel task + log |
| RCS `continueTask` thất bại | Throw → `FlowErrorOccurred` → cancel |

Cancel task: `TaskStateStore.CancelTask` set `Status=Cancelled` và gọi `RcsClient.CancelTask` nếu có `RcsTaskId`.
