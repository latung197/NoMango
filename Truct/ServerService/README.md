# ServerService

Solution .NET 10 cho API quản lý xuất hàng và tác vụ nhập dữ liệu ECU từ CSV. Chạy trên Windows; `Core.Utils` hiện target .NET 9.

## Cấu trúc

| Project | Vai trò |
| --- | --- |
| `Core` | Host HTTP nghiệp vụ: API controller và Razor Pages mẫu. |
| `Core.Application` | Service, DTO, quy tắc nghiệp vụ, interface cho ngữ cảnh người dùng. Phụ thuộc `Core.Domain` và `Core.Utils`. |
| `Core.Domain` | Entity và interface repository. |
| `Core.Infrastructure` | EF Core context, repository, HTTP client và đăng ký hạ tầng. Phụ thuộc `Core.Application`, `Core.Domain`, `Core.Utils`. |
| `Core.WorkerService` | Generic Host chạy `WorkerLine3`/`WorkerLine4` dưới dạng Windows Service hoặc console. |
| `Worker.Application` | Đọc CSV line 3/4 và gửi danh sách ECU đến host `Core`. |
| `Core.Utils` | Tiện ích chung. |
| `Core.API` | Host API riêng, hiện chỉ có controller WeatherForecast mẫu. |
| `PCMain` | Ứng dụng WPF, hiện là khung cửa sổ cơ bản. |

Luồng HTTP nghiệp vụ: `Core/Controllers` → `Core.Application/Services` → interface repository trong `Core.Domain` → implementation và `CoreContext` trong `Core.Infrastructure`.

Luồng CSV: `Core.WorkerService` → `Worker.Application` → `POST /api/ecudata/import-list-ecu-data` trên `Core` → database. Worker giữ lại CSV nếu API không trả kết quả `Success` để có thể xử lý lại; chỉ xóa sau khi nhập thành công.

## Cấu hình cục bộ

Các file `appsettings.json` của ba host không chứa mật khẩu, khóa ký hoặc đường dẫn thư mục CSV đang sử dụng. Trên máy phát triển, đặt cấu hình riêng vào `appsettings.Local.json` trong thư mục host tương ứng; các file này đã được git bỏ qua. Khi triển khai, cấp giá trị qua biến môi trường hoặc kho bí mật của môi trường chạy. Biến môi trường và tham số dòng lệnh được ưu tiên hơn file local.

Các khóa chính của host HTTP: `Tokens:Key`, `Tokens:Issuer`, `Tokens:Audience`, `ConnectionStrings:DatabaseType`, `ConnectionStrings:CoreContext`. Với biến môi trường .NET, dùng dấu gạch dưới kép cho cấp lồng nhau, ví dụ `Tokens__Key` và `ConnectionStrings__CoreContext`. Cả SQL Server và PostgreSQL đều đọc `ConnectionStrings:CoreContext`.

Worker dùng `ApiDomain`, `FolderScanLine3/4`, `FileNameLine3/4`, `WorkerUsername`, `WorkerPassword`. Khi thử worker, trỏ các thư mục này tới dữ liệu thử riêng. Worker đăng nhập bằng tài khoản riêng rồi gửi JWT tới endpoint import ECU; tài khoản này cần quyền `ECU_DATA` / import. Không có thông tin đăng nhập hoặc thiếu quyền thì worker giữ CSV.

## Đăng nhập và phân quyền

Host nghiệp vụ `Core` sử dụng các bảng `sys_role`, `sys_command`, `sys_user_role`, `sys_role_command`, `sys_user_command`. Trước khi chạy phiên bản này trên database hiện có, xem và áp dụng script phù hợp: `sql/postgresql/access-control.sql` hoặc `sql/sqlserver/access-control.sql`. Script chỉ tạo bảng còn thiếu, không sửa cấu trúc bảng đã có; đối chiếu bảng hiện có với entity trước khi triển khai. Chưa có migration tự động khi khởi động.

Quyền hiệu lực của tài khoản là hợp quyền từ mọi nhóm đang hoạt động và quyền cấp riêng cho tài khoản. Quyền riêng chỉ bổ sung; xóa quyền riêng không thu hồi quyền vẫn còn từ nhóm. Nhóm tên `ADMIN` được toàn quyền. Trong giai đoạn chuyển đổi, tài khoản cũ có `auth_fl` chứa đúng mã `0` vẫn được nhận là admin để thiết lập nhóm quyền. Chính sách API đọc quyền từ database cho mỗi request, nên đổi nhóm hoặc khóa tài khoản có hiệu lực với JWT đã phát. Tài khoản mới phải được nhập mật khẩu; mật khẩu mới lưu bằng ASP.NET Core Identity PasswordHasher. Mật khẩu cũ được nâng cấp sau lần đăng nhập thành công. JWT mặc định sống 8 giờ; có thể đặt `Tokens:LifetimeMinutes` từ 5 đến 1440 phút. Endpoint login giới hạn 10 request/phút theo IP. JWT đã cấp trước khi đổi mật khẩu còn hiệu lực cho đến khi hết hạn, trừ khi khóa tài khoản.

Các mã chức năng API: `USER`, `EXPORT_PLAN`, `ECU_DATA`, `MASTER_DATA`, `BACKUP`, `HANDY`. Các thao tác dùng mã `R` (xem), `S` (tìm kiếm), `C` (thêm), `U` (sửa), `D` (xóa), `I` (nhập), `E` (xuất), `P` (in), `L` (tải lại), `Y` (sao chép), `A` (duyệt/quản trị). Bộ xử lý phân quyền ánh xạ action controller sang mã chức năng và thao tác trong `Core/Security/FunctionAuthorization.cs`; khi thêm action cần kiểm tra ánh xạ này. Endpoint `POST /api/access-control/seed-api-functions` tạo sáu chức năng API trong `sys_command` mà không ghi đè bản ghi sẵn có.

API quản trị yêu cầu admin: `GET /api/access-control/roles`, `PUT /api/access-control/roles`, `GET /api/access-control/users/{id}`, `PUT /api/access-control/users/{id}/roles`, `PUT /api/access-control/users/{id}/permissions`. `GET /api/access-control/commands` trả danh sách chức năng; `GET /api/access-control/me` trả nhóm, quyền riêng và quyền hiệu lực của tài khoản hiện tại. `PUT roles` nhận `roleId` (0 khi tạo), `roleName`, `description`, `isActive`, `permissions`; mỗi quyền có `menuId0` và các cờ `canView`, `canAdd`, `canEdit`, `canDelete`, `canImport`... Tìm kiếm/xem tài khoản cần `USER / canSearch` hoặc `USER / canView`; tạo, sửa, xóa tài khoản chỉ dành cho admin. Người dùng tự đổi mật khẩu của mình.

Trình tự cấp quyền ban đầu: áp dụng script cho database thử → đăng nhập bằng tài khoản admin cũ → gọi `seed-api-functions` → tạo nhóm và chọn quyền → gán nhóm/quyền riêng cho tài khoản → tạo tài khoản worker có `ECU_DATA / canImport` và cấu hình worker. Các endpoint nghiệp vụ của `ExportPlan`, `EcuData`, `MstData`, `Backup`, `Handy` yêu cầu quyền tương ứng. `Core.API` và controller mẫu `TestController` không thuộc luồng phân quyền này.

## Build và kiểm thử

```powershell
dotnet restore Core.sln
dotnet build Core.sln --no-restore
dotnet test tests/Worker.Application.Tests/Worker.Application.Tests.csproj --no-restore
dotnet test tests/Core.Security.Tests/Core.Security.Tests.csproj --no-restore
```

Chạy host HTTP nghiệp vụ bằng `dotnet run --project Core/Core.csproj`. `Core.API` không chứa các endpoint xuất hàng/ECU. Chỉ chạy `Core.WorkerService` sau khi đã cấu hình API và thư mục CSV phù hợp. Các kiểm thử hiện có xác minh quy tắc giữ/xóa CSV khi API nhập ECU thành công hoặc thất bại.

## Khung ERP

Các API khung ERP chạy trên host `Core`, có tiền tố `/api/erp`. Mỗi module có contract trong `Core.Application/Features/Erp/<Module>`, implementation trong `Core.Infrastructure/Features/Erp/<Module>`, controller trong `Core/Features/Erp/<Module>`. Entity dùng chung nằm ở `Core.Domain/Entity/Erp`, ánh xạ EF ở `Core.Infrastructure/Context/CoreContext.cs`. Khi thêm module mới, giữ cùng cấu trúc này, đăng ký service ở `Core.Infrastructure/DependencyInjection.cs`, thêm quyền trong `sys_command` và test cho quy tắc nghiệp vụ cần bảo vệ.

| Module | API chính | Quyền |
| --- | --- | --- |
| Auth | `POST /auth/login-options`, `POST /auth/login`, `GET /auth/context` | Mật khẩu; context đã chọn |
| Organization | `GET/PUT /organization/units`, `GET/PUT /organization/plants` | Admin |
| Users | `GET/POST /users`, `GET/PUT/DELETE /users/{id}`, `PUT /users/{id}/roles`, `/permissions`, `/plants`, `PUT /users/me/password` | Admin; đổi mật khẩu của chính mình |
| Menu | `GET /menu/mine`, `GET/PUT /menu` | Context ERP; admin chỉnh sửa |
| Notifications | `GET /notifications`, `PUT /notifications/{id}/read`, `POST /notifications` | Context ERP; admin phát thông báo |
| Settings | `GET /settings/effective`, `GET/PUT /settings` | Context ERP; admin chỉnh sửa |

Đăng nhập ERP gồm hai bước: gửi username/password tới `login-options` để lấy các cặp đơn vị và nhà máy được gán; gửi lại username/password cùng `unitCode`, `plantCode` tới `login`. JWT chứa cặp đã chọn. Các API yêu cầu policy `ErpContext` xác minh lại tài khoản, nhà máy, đơn vị và quyền gán theo database trên mỗi request; thay đổi quyền gán hoặc khóa nhà máy có hiệu lực ngay. Admin dùng API đăng nhập cũ để tạo đơn vị, nhà máy, gán nhà máy cho tài khoản trước khi tài khoản đăng nhập ERP lần đầu. Tài khoản admin cũng cần được gán nhà máy nếu đăng nhập ERP.

Menu dùng `sys_command`: `menuid0` là mã node, `menuid` là mã cha (root dùng cùng mã với `menuid0`), `command` là route, `picture1` là icon, `basicright` là thứ tự, `hide_yn` là trạng thái ẩn, `ds_dvcs` là danh sách mã đơn vị được phép hiển thị. `GET /menu/mine` chỉ trả node có ít nhất một quyền thao tác hoặc node cha của chức năng đó, trong phạm vi đơn vị đã chọn. Nhóm quyền và quyền riêng vẫn dùng API `/api/access-control`; gọi `POST /api/access-control/seed-api-functions` sau khi cập nhật để tạo các mã `ERP_ORG`, `ERP_MENU`, `ERP_NOTICE`, `ERP_SETTINGS` nếu chưa có.

Thông báo có thể gửi toàn hệ thống, theo đơn vị, theo nhà máy hoặc tới một tài khoản; trạng thái đã đọc lưu theo tài khoản. Tham số chung có scope `GLOBAL`, `U:<unitCode>` hoặc `P:<plantCode>`, ưu tiên nhà máy rồi đơn vị rồi toàn hệ thống. API effective chỉ trả tham số `isPublic` cho tài khoản thường; admin xem được cả tham số riêng. Không lưu bí mật hệ thống dưới dạng tham số công khai.

Trước khi dùng API ERP trên database hiện có, áp dụng `sql/postgresql/erp-foundation.sql` hoặc `sql/sqlserver/erp-foundation.sql` tương ứng, sau script `access-control.sql`. Các script tạo bảng mới, không tự sửa bảng cũ. Đối chiếu schema thực tế trước khi chạy trên dữ liệu sản xuất. Các API nghiệp vụ cũ (`ExportPlan`, `EcuData`...) hiện vẫn theo phân quyền cũ; module ERP mới cần đọc `erp_unit`, `erp_plant` từ JWT và lọc dữ liệu theo context trước khi triển khai cho nhiều đơn vị.
