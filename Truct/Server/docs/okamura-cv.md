# Flow CV ↔ AGV: Nhập kho & Xuất kho (Tóm tắt nghiệp vụ)

## 1) Mục tiêu

Mô tả flow bắt tay giữa **Conveyor/Station (CV/PLC)** và **AGV side** (WCS ghi vùng AGV→CV) cho 2 luồng:

* **Nhập kho (Inbound)**
* **Xuất kho (Outbound)**

WCS đóng vai trò điều phối, đảm bảo:

* CV sẵn sàng & an toàn
* AGV sẵn sàng, tới đúng trạm
* Trong quá trình giao/nhận: bật trạng thái “in-progress/receiving”
* Nếu có **Intrusion** hoặc **Safety không OK** → chặn flow

> **Lưu ý:** Trong tài liệu này:
> - **AGV**: chính là đại diện cho WCS (phía điều phối)
> - **CV**: đại diện cho phía kho Okamura (Conveyor/Station/PLC)

---

## 2) Quy ước trạng thái (theo tag dự án)

### 2.1. Nhóm tín hiệu CV → AGV (WCS đọc)

* **Ready**: CV đã sẵn sàng hoạt động
* **AllowHandover/RequestAGV**: CV cho phép bàn giao (inbound) hoặc yêu cầu AGV tới (outbound)
* **Safety OK**: cảm biến an toàn tắt / cửa đã đóng (trạng thái cho phép thao tác)
* **Conveyor Running**: băng tải/bàn quay đang chạy (feedback)

### 2.2. Nhóm tín hiệu AGV → CV (WCS ghi)

* **AGV Ready**: AGV side sẵn sàng phối hợp
* **AGV Arrived**: AGV đã đến trạm
* **InProgress/Receiving**: AGV đang bàn giao (inbound) hoặc đang nhận hàng (outbound)
* **Intrusion**: phát hiện xâm nhập/unsafe của AGV (an toàn)

---

## 3) Flow NHẬP KHO (Inbound)

### 3.1. Các tag tham gia

**CV → AGV (READ)**

* D2001: `CV_In_Ready`
* D2002: `CV_In_AllowHandover`
* D2003: `CV_In_SafetyOk`
* D2004: `CV_In_ConveyorRunning`

**AGV → CV (WRITE)**

* D2101: `Agv_In_Ready`
* D2102: `Agv_In_Arrived`
* D2103: `Agv_In_HandoverInProgress`
* D2104: `Agv_In_Intrusion`

### 3.2. Trình tự nghiệp vụ (happy path)

1. **AGV side sẵn sàng**

   * WCS set `Agv_In_Ready = 1`

2. **Chờ điều kiện từ CV**

   * `CV_In_Ready == 1`
   * `CV_In_AllowHandover == 1`
   * `CV_In_SafetyOk == 1`
   * `Agv_In_Intrusion == 0` (không intrusion)

3. **AGV đến trạm**

   * WCS set `Agv_In_Arrived = 1`

4. **Bắt đầu bàn giao / thao tác**

   * WCS set `Agv_In_HandoverInProgress = 1`

5. *(Tuỳ hệ thống)* **CV bắt đầu chạy băng tải/bàn quay**

   * WCS có thể theo dõi `CV_In_ConveyorRunning == 1` để xác nhận cơ cấu đã chạy

6. **Kết thúc bàn giao**

   * WCS set `Agv_In_HandoverInProgress = 0`
   * WCS set `Agv_In_Arrived = 0`
   * (giữ `Agv_In_Ready = 1` nếu AGV vẫn online)

### 3.3. Điều kiện chặn/abort

* Nếu `CV_In_SafetyOk != 1` → không cho bắt đầu
* Nếu `Agv_In_Intrusion == 1` → dừng thao tác, chờ clear intrusion
* Nếu CV mất `Ready` giữa chừng → coi là fault, dừng/rollback theo rule dự án

---

## 4) Flow XUẤT KHO (Outbound)

### 4.1. Các tag tham gia

**CV → AGV (READ)**

* D2011: `CV_Out_Ready`
* D2012: `CV_Out_RequestAgvArrive`
* D2013: `CV_Out_SafetyOk`
* D2014: `CV_Out_ConveyorRunning`

**AGV → CV (WRITE)**

* D2111: `Agv_Out_Ready`
* D2112: `Agv_Out_Arrived`
* D2113: `Agv_Out_MovingOrCarrying` *(tuỳ dùng)*
* D2114: `Agv_Out_Intrusion`
* D2115: `Agv_Out_Receiving` (AGV nhận hàng)

### 4.2. Trình tự nghiệp vụ (happy path)

1. **AGV side sẵn sàng**

   * WCS set `Agv_Out_Ready = 1`

2. **Chờ CV yêu cầu AGV**

   * `CV_Out_Ready == 1`
   * `CV_Out_RequestAgvArrive == 1`
   * `CV_Out_SafetyOk == 1`
   * `Agv_Out_Intrusion == 0`

3. **AGV đến trạm xuất**

   * WCS set `Agv_Out_Arrived = 1`

4. **AGV bắt đầu nhận hàng**

   * WCS set `Agv_Out_Receiving = 1`
   * *(Tuỳ hệ thống)* WCS có thể set `Agv_Out_MovingOrCarrying = 1` trong giai đoạn đang thao tác

5. *(Tuỳ hệ thống)* **CV chạy băng tải/bàn quay để đưa hàng ra**

   * Theo dõi `CV_Out_ConveyorRunning == 1` để xác nhận cơ cấu chạy

6. **Kết thúc nhận hàng**

   * WCS set `Agv_Out_Receiving = 0`
   * WCS set `Agv_Out_Arrived = 0`
   * (giữ `Agv_Out_Ready = 1` nếu AGV vẫn online)

### 4.3. Điều kiện chặn/abort

* `CV_Out_SafetyOk != 1` → không start
* `Agv_Out_Intrusion == 1` → stop, chờ clear
* `CV_Out_RequestAgvArrive` mất trong khi đang làm → coi là fault tuỳ rule

---

## 5) Quy tắc an toàn (Safety Rules)

1. **Safety gate bắt buộc trước mọi thao tác**

   * Inbound: `D2003 == 1`
   * Outbound: `D2013 == 1`

2. **Intrusion ưu tiên cao nhất**

   * Inbound: `D2104 == 1` → dừng
   * Outbound: `D2114 == 1` → dừng
   * Chỉ resume khi intrusion trở về `0`

3. **Timeout bắt buộc**

   * Mọi bước “Wait …” phải có timeout rõ ràng để tránh treo flow.

---

## 6) Checklist nghiệm thu flow

* [ ] Inbound: chỉ start khi D2001 & D2002 & D2003 đều = 1
* [ ] Outbound: chỉ start khi D2011 & D2012 & D2013 đều = 1
* [ ] WCS ghi đúng các tag AGV→CV: Ready/Arrived/InProgress(or Receiving)/Intrusion
* [ ] Intrusion = 1 thì flow dừng ngay và không set “InProgress/Receiving”
* [ ] Có cơ chế reset bit sau khi hoàn tất (Arrived/InProgress/Receiving về 0)

---
