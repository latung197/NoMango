# Order Matching

## Khai niem

`TransferRequest` la lenh cho ghep doi van chuyen. Co hai loai:

- `Send`: station dang co khay muon day di. Request co `FromStationCode`, `FromStageCode`, `ToStageCode`, size, loai khay, cassette/product/quantity neu la khay hang.
- `Receive`: station dang can nhan khay. Request co `ToStationCode`, `ToStageCode`, size, loai khay va co the co `FromStageCode`.

`FlowTask` chi duoc tao khi co du tuyen station-to-station thuc te. Khi `Send` va `Receive` match, service tao `FlowTask` qua `FlowTaskStarter`, sau do publish `FlowStarted` neu execution gate khong pause.

`TransferRequest.Source` cho biet nguon tao lenh:

- `Manual`: user thao tac tren UI/API.
- `AutoOpc`: worker tu tao tu trang thai OPC cua station.

## CreateSendAsync

`CreateSendAsync` nhan `from_station`, `to_stage`, loai khay, size va metadata cassette.

Luot xu ly:

1. Lay station nguon va stage nguon tu station.
2. Resolve size theo station; neu station nhieu size ma request khong truyen size thi se bao `SIZE_REQUIRED`.
3. Kiem tra stage dich co station active ho tro size do.
4. Chan duplicate `Send` dang waiting cung from stage, to stage, size, loai khay va from station.
5. Tao `TransferRequest` status `Waiting`, source mac dinh `Manual` hoac `AutoOpc`.
6. Neu dich la cua kho nhap phu hop size, tao/queue task vao kho.
7. Neu co `Receive` counterpart thi match va tao `FlowTask`.
8. Neu chua match, request o trang thai `Waiting` den khi co counterpart hoac timeout.

## CreateReceiveAsync

`CreateReceiveAsync` nhan `to_station`, size, loai khay va tuy truong hop co `from_stage`.

Luot xu ly:

1. Lay station dich va stage dich tu station.
2. Resolve size theo station.
3. Chan station co `Receive` waiting khac.
4. Xac dinh `FromStageCode`:
   - WIP tray: dung chinh stage dich.
   - Empty tray: wildcard nguon rong.
   - Manual hang thuong: bat buoc `from_stage`.
   - Auto OPC hang thuong: wildcard nguon rong.
5. Tao request `Waiting`.
6. Neu khong phai WIP, tim `Send` counterpart:
   - Empty tray: tim `Send` empty tray den stage dich.
   - Auto OPC hang thuong: tim moi `Send` hang den stage dich, cung size.
   - Manual hang thuong: match dung from stage, to stage, size.
7. Neu khong co send, thu fallback lay hang tu kho.
8. Neu van khong co, request tiep tuc waiting.

## Matching

Normal goods match khi:

- `Send.FromStageCode == Receive.FromStageCode`
- `Send.ToStageCode == Receive.ToStageCode`
- Cung size
- `IsEmptyTray == false`
- Ca hai request dang `Waiting`

Empty tray match khi:

- `Send.IsEmptyTray == true`
- `Receive.IsEmptyTray == true`
- `Send.ToStageCode == Receive.ToStageCode`
- Cung size

Auto IN wildcard match khi:

- Receive co `Source = AutoOpc`
- Receive khong phai empty tray
- Bat ky `Send` hang nao den `Receive.ToStageCode`
- Cung size

Khi match thanh cong, ca hai request chuyen `Matched`, gan `MatchedRequestId`, tao `FlowTask`, sau do gan `FlowTaskId`.

## Warehouse fallback

Send den stage cua cua kho nhap se khong cho receive counterpart. Service tao task tu station nguon den warehouse IN neu cua kho ranh. Neu cua kho ban, request giu `Waiting` voi:

- `WarehousePendingKind = Inbound`
- `PendingWarehouseStationCode = warehouse IN`

Receive hang thuong neu khong match send se thu lay hang tu WMS qua warehouse OUT. Neu co ton kho va cua kho/station dich ranh, service tao task tu warehouse OUT den station dich va request chuyen `Completed`. Neu cua kho OUT ban, request giu `Waiting` voi:

- `WarehousePendingKind = Outbound`
- `PendingWarehouseStationCode = warehouse OUT`
- `CranePositions` la vi tri WMS da chon

`WarehouseDoorQueueWorker` quet cac request pending theo `OrderMatching.PollingIntervalSeconds` de retry.

## Timeout

`Send` request co `ExpiresAt = CreatedAt + SendRequestTimeoutMinutes`. `TransferMatchTimeoutWorker` quet send waiting da het han. Khi timeout, service thu dua khay ve kho nhap. Neu tao task vao kho thanh cong, request chuyen `TimedOut`; neu cua kho ban, request duoc queue inbound.

## Auto OPC worker

`AutoStationOrderWorker` chay theo `OrderMatching.PollingIntervalSeconds`.

Auto IN chi ap dung station `Type = IN` va `AutoReceiveEnabled = true`:

- Station active, PLC running hoac khong cau hinh status.
- Snapshot hien `NoCassette`.
- Station khong co active `FlowTask`.
- Station khong co receive waiting.
- Co send waiting den `station.StageCode` va size station ho tro.

Worker tao `Receive` source `AutoOpc`, size va loai khay lay theo send candidate. Receive auto hang thuong dung wildcard nguon.

Auto OUT chi ap dung station `Type = OUT` va `AutoSendEnabled = true`:

- Station active, PLC running hoac khong cau hinh status.
- Snapshot hien `HasCassette`.
- Station khong co active `FlowTask`.
- Co `AutoSendToStage`.
- Khong co send waiting trung from station, to stage, size, loai khay.

Neu `AutoSendIsEmptyTray = true`, worker tao `Send` empty tray source `AutoOpc`, size nho nhat station ho tro.

Neu la khay hang, worker doc QR tu snapshot, lookup cassette active theo code QR, lay `cassette_code`, `product`, `quantity`, `size`, roi tao `Send` source `AutoOpc`.

Neu thieu QR, khong tim thay cassette, thieu stage dich, station ban, station khong running, hoac duplicate request, worker khong tao lenh va ghi log.

## UI realtime

Moi lan request duoc tao/cap nhat status, backend publish `TransferRequestChanged`. CMS nhan CAP event va push SignalR event cung ten.

Main UI xu ly event:

- Request `Waiting` co `FromStageCode` hoac `ToStageCode` trung stage hien tai se duoc merge vao danh sach waiting ngay.
- Request khong con `Waiting` se bi xoa khoi danh sach.
- Neu request da co `FlowTaskId`, UI refresh danh sach `FlowTask`.
- Polling `/transfer-requests/waiting` van chay nhu dong bo du phong.

`Command` hien chip `Tu dong` cho request co `source = AutoOpc`.

## Loi van hanh thuong gap

- Station khong running: worker bo qua, khong tao request.
- IN station dang co khay: auto receive bo qua.
- OUT station khong co khay: auto send bo qua.
- Thieu QR hoac QR khong co cassette active: auto OUT khay hang bo qua.
- Station multi-size nhung manual khong truyen size: API tra `SIZE_REQUIRED`.
- Co request waiting trung: API/worker khong tao them request.
- Cua kho ban: request duoc queue bang `WarehousePendingKind` va worker retry sau.
