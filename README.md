# Library Management System

Hệ thống quản lý thư viện với **ASP.NET Core Web API** và **React**, hỗ trợ ba nhóm người dùng: quản trị viên, thủ thư và độc giả. Dự án tách Frontend và Backend, sử dụng PostgreSQL để lưu dữ liệu.

> **Trạng thái:** Mã nguồn đã triển khai các module chính. Chưa xác nhận build, kiểm thử tích hợp hoặc triển khai production thành công. Xem mục [Kiểm tra trước khi chạy](#kiểm-tra-trước-khi-chạy) để biết các điểm cần rà soát.

## 1. Công nghệ

| Thành phần | Công nghệ |
| --- | --- |
| Backend | C#, .NET 8, ASP.NET Core Web API |
| Kiến trúc Backend | Clean Architecture, CQRS, MediatR |
| Truy cập dữ liệu | Entity Framework Core, Npgsql |
| Cơ sở dữ liệu | PostgreSQL |
| Xác thực và phân quyền | JWT Bearer, Role-based Authorization |
| Frontend | React 19, TypeScript, Vite 8 |
| Giao diện | Tailwind CSS 4, Lucide React |
| Gọi API và quản lý dữ liệu | Axios, TanStack Query |
| Điều hướng | React Router |
| Tài liệu API | Swagger (môi trường Development) |

## 2. Vai trò và chức năng

### Admin — Quản trị viên

- Xem thống kê số người dùng, đầu sách, bản sách khả dụng, lượt mượn, lượt quá hạn và khoản phạt chưa thanh toán.
- Xem và tìm kiếm người dùng; chuyển quyền giữa `READER` và `LIBRARIAN`; khóa hoặc mở khóa tài khoản.
- Quản lý sách, thể loại và tác giả qua các API có phân quyền.

### Librarian — Thủ thư

- Thêm, sửa, xóa mềm sách; theo dõi tổng số bản và số bản khả dụng.
- Xem lượt mượn đang hoạt động hoặc quá hạn; xác nhận trả sách.
- Xem tiền phạt chưa thanh toán và ghi nhận đã thu tiền.

### Reader — Độc giả

- Đăng ký, đăng nhập; tìm sách theo từ khóa và xem số lượng bản còn lại.
- Mượn sách, xem lịch sử mượn và hạn trả.
- Tra cứu tiền phạt qua API cá nhân.

**Quy tắc nghiệp vụ:** Người dùng tự đăng ký được cấp vai trò Reader; quyền truy cập được kiểm tra tại Backend. Trả sách có thể làm phát sinh tiền phạt quá hạn. Xác nhận thanh toán tiền phạt là thao tác ghi nhận thủ công, **không phải cổng thanh toán trực tuyến**.

## 3. Cấu trúc mã nguồn

```text
LibraryManagement/                  # Backend
├── LibraryManagement.API/          # Controllers, JWT, Swagger, middleware
├── LibraryManagement.Application/  # Commands, Queries, handlers, interfaces
├── LibraryManagement.Domain/       # Entities và quy tắc miền
├── LibraryManagement.Infrastructure/ # EF Core, PostgreSQL, migrations, services
└── LibraryManagement.sln

library-management-web/             # Frontend
├── src/
│   ├── api/                        # Axios và các API clients
│   ├── components/                 # ProtectedRoute
│   ├── contexts/                   # AuthContext
│   ├── pages/
│   │   ├── admin/
│   │   ├── librarian/
│   │   └── reader/
│   └── App.tsx                      # Routes
├── package.json
└── vite.config.ts
```

Hai thư mục trên có thể đặt cạnh nhau trong một repository hoặc lưu thành hai repository riêng. Nếu dùng chung repository, đặt file README này tại thư mục cha của cả hai.

## 4. Yêu cầu môi trường

- .NET SDK 8.
- PostgreSQL đang chạy và có database dành cho dự án.
- Node.js phiên bản tương thích với Vite 8 (Node.js **20.19+ hoặc 22.12+**; nên dùng phiên bản LTS mới).
- npm.
- EF Core CLI nếu chạy migration bằng lệnh `dotnet ef`.

## 5. Cài đặt Backend

### 5.1. Cấu hình database và JWT

Mở terminal trong thư mục `LibraryManagement/`:

```bash
dotnet restore
```

Cấu hình bằng **User Secrets** tại project API để tránh đưa thông tin nhạy cảm lên Git:

```bash
cd LibraryManagement.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=library_management;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_RANDOM_SECRET_AT_LEAST_32_BYTES_LONG"
dotnet user-secrets set "Jwt:Issuer" "LibraryManagement"
dotnet user-secrets set "Jwt:Audience" "LibraryManagementClient"
```

Các giá trị `Issuer` và `Audience` phải thống nhất giữa nơi tạo và nơi xác thực token. **Không dùng chuỗi ví dụ làm JWT secret khi triển khai thực tế.** Nếu project API chưa có `UserSecretsId`, lệnh `dotnet user-secrets init` sẽ bổ sung cấu hình đó.

### 5.2. Tạo schema bằng migrations

Từ thư mục `LibraryManagement/`:

```bash
dotnet tool install --global dotnet-ef

dotnet ef database update \
  --project LibraryManagement.Infrastructure \
  --startup-project LibraryManagement.API
```

**PowerShell (Windows):** Viết lệnh trên thành một dòng nếu không sử dụng ký tự nối dòng phù hợp:

```powershell
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.API
```

Nếu chưa có database hoặc tài khoản PostgreSQL chưa có quyền tạo database, hãy tạo database và cấp quyền trước. Không chạy lại migration tùy tiện trên database đang chứa dữ liệu quan trọng; nên sao lưu trước.

### 5.3. Chạy API

Từ thư mục `LibraryManagement/`:

```bash
dotnet run --project LibraryManagement.API --launch-profile https
```

Theo `launchSettings.json` của bản mã nguồn được kiểm tra:

- API HTTPS: `https://localhost:7057`
- API HTTP: `http://localhost:5139`
- Swagger khi chạy Development: `https://localhost:7057/swagger`

Nếu chứng chỉ HTTPS local chưa được tin cậy:

```bash
dotnet dev-certs https --trust
```

Các cổng trên là cấu hình local của bản ZIP hiện tại; có thể khác khi bạn thay đổi profile hoặc deploy.

## 6. Cài đặt Frontend

Từ thư mục `library-management-web/`:

```bash
npm install
```

Tạo file `.env.local` tại gốc Frontend:

```dotenv
VITE_API_URL=https://localhost:7057/api
```

Chạy giao diện:

```bash
npm run dev
```

Mở URL được Vite in ra terminal (thông thường `http://localhost:5173`).

Các lệnh bổ sung:

```bash
npm run build    # TypeScript check và tạo bản build
npm run lint     # Kiểm tra lint
npm run preview  # Xem thử bản build
```

**CORS:** Trong `Program.cs` của Backend thuộc bản ZIP được kiểm tra **chưa thấy cấu hình CORS**. Khi Frontend chạy ở `localhost:5173` và API chạy ở cổng khác, trình duyệt có thể chặn request. Trước khi kiểm thử trên browser, cấu hình CORS chỉ cho phép origin Frontend mong muốn và gọi `app.UseCors(...)` trước `UseAuthentication()`/`UseAuthorization()`. Không dùng `AllowAnyOrigin()` cùng credentials trong production.

## 7. Các API chính

| Nhóm | Endpoint | Quyền |
| --- | --- | --- |
| Auth | `POST /api/auth/register` | Công khai |
| Auth | `POST /api/auth/login` | Công khai |
| Auth | `GET /api/auth/me` | Đăng nhập |
| Books | `GET /api/books`, `GET /api/books/search`, `GET /api/books/{id}` | Công khai |
| Books | `POST /api/books`, `PUT /api/books/{id}`, `DELETE /api/books/{id}` | Admin, Librarian |
| Categories | `GET /api/categories`; `POST /api/categories` | GET công khai; POST Admin/Librarian |
| Authors | `GET /api/authors`; `POST /api/authors` | GET công khai; POST Admin/Librarian |
| Borrow | `POST /api/borrow-records` | Reader |
| Borrow | `GET /api/borrow-records/my` | Đăng nhập |
| Borrow | `GET /api/borrow-records/active`, `GET /api/borrow-records/overdue` | Admin, Librarian |
| Borrow | `PUT /api/borrow-records/{id}/return` | Admin, Librarian |
| Penalties | `GET /api/penalties/my` | Reader |
| Penalties | `GET /api/penalties`, `PUT /api/penalties/{id}/pay` | Admin, Librarian |
| Users | `GET /api/users`, `GET /api/users/{id}` | Admin |
| Users | `PUT /api/users/{id}/role`, `PUT /api/users/{id}/status` | Admin |
| Dashboard | `GET /api/admin/dashboard` | Admin |

Với API yêu cầu JWT, gửi header:

```http
Authorization: Bearer YOUR_ACCESS_TOKEN
```

Định dạng request/response cụ thể xem trong Swagger và DTO/Command tương ứng. Ví dụ, cập nhật sách yêu cầu `id` trong body trùng với `{id}` trên URL.

## 8. Luồng sử dụng

1. Người dùng đăng ký tài khoản Reader và đăng nhập.
2. Admin cấp quyền Librarian cho tài khoản phù hợp.
3. Librarian tạo thể loại, tác giả và sách; nhập tổng số bản.
4. Reader tìm sách và gửi yêu cầu mượn khi còn bản khả dụng.
5. Librarian xem danh sách đang mượn, xác nhận trả sách.
6. Nếu có khoản phạt, Librarian xác nhận đã nhận thanh toán.
7. Admin xem các chỉ số tổng hợp trên Dashboard.

Các thao tác cập nhật số lượng sách cần được Backend bảo vệ bằng transaction và kiểm tra đồng thời; không dựa vào số lượng đang hiển thị trên Frontend để quyết định còn sách hay không.

## 9. Kiểm tra trước khi chạy

Đây là danh sách cần hoàn thành trước khi đánh dấu dự án là **đã chạy ổn định**:

- [ ] `dotnet build` thành công.
- [ ] `npm run build` thành công.
- [ ] PostgreSQL kết nối được và migration chạy thành công.
- [ ] Backend đã cấu hình CORS cho origin Frontend.
- [ ] Đăng ký/đăng nhập hoạt động, API bảo vệ trả 401 khi thiếu token và 403 khi sai quyền.
- [ ] Admin đổi quyền/khóa tài khoản; token cũ bị từ chối khi quyền hoặc trạng thái thay đổi.
- [ ] CRUD sách hoạt động, xóa mềm không làm mất lịch sử mượn.
- [ ] Mượn bản cuối cùng đồng thời không gây âm số lượng.
- [ ] Trả sách hai lần không làm tăng số lượng hai lần.
- [ ] Tính phạt quá hạn và xác nhận thanh toán đúng nghiệp vụ.
- [ ] Dashboard khớp số liệu trong database.

**Lưu ý về dữ liệu mẫu:** Không giả định repository đã có tài khoản Admin mẫu hoặc script seed dữ liệu. Kiểm tra cơ chế khởi tạo vai trò và Admin của dự án trước khi thử các màn hình quản trị. Không hard-code mật khẩu Admin trong mã nguồn công khai.

## 10. Bảo mật và triển khai

- Không commit `.env.local`, mật khẩu PostgreSQL, JWT secret hoặc token thật.
- Frontend hiện lưu access token trong `sessionStorage`; cần cân nhắc rủi ro XSS và phương án lưu phiên phù hợp trước khi triển khai production.
- Luôn dùng HTTPS cho môi trường công khai.
- Cấu hình CORS theo đúng domain Frontend.
- Chạy migration theo quy trình có sao lưu; không dùng database production để thử nghiệm.
- Khi deploy, thay `VITE_API_URL` bằng URL API công khai và build lại Frontend.
- Swagger hiện chỉ được bật ở môi trường Development theo cấu hình Backend.

## 11. Hướng phát triển

- Tìm kiếm nâng cao và lọc theo tác giả/thể loại.
- Thông báo email khi sắp đến hạn hoặc quá hạn.
- Dashboard biểu đồ theo thời gian, báo cáo xuất Excel/PDF.
- Kiểm thử tự động cho handler, API và các tình huống mượn trả đồng thời.
- Docker Compose cho API, PostgreSQL và Frontend.

---

**Mục đích dự án:** Thực hành phát triển ứng dụng full-stack, thiết kế REST API, phân quyền, mô hình dữ liệu quan hệ, CQRS/Clean Architecture và xử lý nghiệp vụ mượn trả có ràng buộc đồng thời.
