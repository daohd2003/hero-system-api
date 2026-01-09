# 🦸 Hero System API

**Hero System API** là một hệ thống backend RESTful mạnh mẽ được xây dựng trên nền tảng **.NET 8**, cung cấp giải pháp toàn diện để quản lý thế giới Siêu anh hùng. Hệ thống tích hợp các công nghệ tiên tiến như **SignalR** cho giao tiếp thời gian thực, **OData** cho truy vấn dữ liệu linh hoạt, và **Background Services** để xử lý các logic game tự động.

## 📋 Mục lục
- [Giới thiệu](#-giới-thiệu)
- [Tính năng nổi bật](#-tính-năng-nổi-bật)
- [Kiến trúc & Công nghệ](#-kiến-trúc--công-nghệ)
- [Cài đặt & Cấu hình](#-cài-đặt--cấu-hình)
- [Hướng dẫn sử dụng](#-hướng-dẫn-sử-dụng)
- [Testing](#-testing)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)

---

## 📖 Giới thiệu

Dự án được thiết kế theo mô hình **Layered Architecture** (Kiến trúc phân lớp) kết hợp với **Repository & Unit of Work Pattern**, đảm bảo tính tách biệt giữa logic nghiệp vụ, truy cập dữ liệu và giao diện API.

Hệ thống không chỉ dừng lại ở các thao tác CRUD cơ bản mà còn cung cấp trải nghiệm tương tác thực (Real-time Chat) và các cơ chế vận hành tự động (Level Decay) mô phỏng một hệ thống Game Server thu nhỏ.

---

## ✨ Tính năng nổi bật

### 1. Quản lý Siêu Anh Hùng (Hero) & Phe Phái (Faction)
* **Quản lý hồ sơ:** Tạo mới, cập nhật sức mạnh, cấp độ và phe phái.
* **Cơ chế xác thực (Authentication):**
    * Đăng nhập bằng JWT (JSON Web Token).
    * **Refresh Token Rotation:** Cơ chế bảo mật cao, tự động cấp mới Refresh Token sau khi sử dụng để chống đánh cắp session.
* **Phân quyền (Authorization):** Hệ thống phân quyền dựa trên Role (Admin/User).

### 2. Hệ thống Nhiệm vụ (Mission)
* Thiết lập các nhiệm vụ với độ khó khác nhau.
* Gán nhiệm vụ cho Hero và ghi nhận kết quả xếp hạng (Rank S, A, B...).
* **Business Logic:** Ngăn chặn việc gán trùng nhiệm vụ đã hoàn thành.

### 3. Giao tiếp thời gian thực (Real-time Communication)
Sử dụng **SignalR** để xây dựng hệ thống chat đa kênh:
* **Feed Chat:** Kênh chat thế giới (Global chat).
* **Private Chat:** Chat mật 1-1 giữa các Hero.
* **System Notification:** Thông báo thời gian thực khi có Hero mới gia nhập server.
* **Lưu trữ:** Lịch sử tin nhắn được đồng bộ và lưu trữ trên **Firebase Realtime Database**.
* **Media:** Hỗ trợ gửi ảnh trong chat thông qua tích hợp **Cloudinary**.

### 4. Truy vấn dữ liệu nâng cao (OData)
Hỗ trợ chuẩn **OData v9**, cho phép client tự định nghĩa truy vấn:
* Chọn cột (`$select`), Lọc dữ liệu (`$filter`), Sắp xếp (`$orderby`).
* Truy vấn lồng (`$expand`) để lấy thông tin Phe phái (Faction) kèm theo Hero.

### 5. Tác vụ nền (Background Services)
* **HeroPowerDecayService:** Một `HostedService` chạy ngầm, tự động kích hoạt vào **8:00 AM** mỗi ngày.
* **Logic:** Kiểm tra các Hero không hoạt động (`LastActiveAt`) trong 7 ngày qua và tự động giảm 5% cấp độ (Level) của họ.

---

## 🛠 Kiến trúc & Công nghệ

* **Framework:** .NET 8 (ASP.NET Core Web API).
* **Database:** Microsoft SQL Server (Entity Framework Core 9 - Code First).
* **Real-time:** SignalR Core.
* **Storage:**
    * **Cloudinary:** Lưu trữ hình ảnh upload.
    * **Firebase Realtime DB:** Lưu trữ lịch sử chat.
* **Querying:** Microsoft.AspNetCore.OData (v9.x).
* **Patterns:** Repository, Unit of Work, Dependency Injection.
* **Mapping:** AutoMapper.
* **Testing:** xUnit, Moq, MockQueryable.
* **Documentation:** Swagger (OpenAPI).

---

## ⚙️ Cài đặt & Cấu hình

### Yêu cầu tiên quyết
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* Visual Studio 2022 hoặc VS Code.

### Các bước cài đặt

1.  **Clone repository:**
    ```bash
    git clone <repository-url>
    cd daohd2003-hero-system-api
    ```

2.  **Cấu hình `appsettings.json`:**
    Cập nhật file `Controllers/appsettings.json` với các key của bạn:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=(local);Database=HeroDb;User Id=sa;Password=YOUR_PASSWORD;Encrypt=False;TrustServerCertificate=True;"
      },
      "JwtSettings": {
        "SecretKey": "YOUR_SUPER_SECRET_KEY_MUST_BE_LONG_ENOUGH",
        "Issuer": "HeroApi",
        "Audience": "HeroClient"
      },
      "Cloudinary": {
        "CloudName": "YOUR_CLOUD_NAME",
        "ApiKey": "YOUR_API_KEY",
        "ApiSecret": "YOUR_API_SECRET"
      },
      "Firebase": {
        "RealtimeDbUrl": "[https://your-project.firebaseio.com](https://your-project.firebaseio.com)"
      }
    }
    ```

3.  **Khởi tạo Database:**
    Mở terminal tại thư mục `Controllers` và chạy lệnh:
    ```bash
    dotnet ef database update --project "../Data Access/DataAccess.csproj"
    ```
    *Lưu ý: Dữ liệu mẫu (Seed Data) gồm Iron Man và Spider Man sẽ được tự động thêm vào.*

4.  **Chạy ứng dụng:**
    ```bash
    dotnet run --project Controllers
    ```
    API sẽ khởi chạy tại `https://localhost:7296` hoặc `http://localhost:5275`.

---

## 🚀 Hướng dẫn sử dụng

### 1. API Documentation (Swagger)
Truy cập `https://localhost:7296/swagger` để xem toàn bộ danh sách API, schemas và test trực tiếp các endpoint (Auth, Hero, Faction, Mission).

### 2. Sử dụng OData
Bạn có thể truy vấn dữ liệu Hero phức tạp thông qua endpoint OData. Ví dụ:
* Lấy tên và level của các Hero có level > 50, kèm theo thông tin Faction:
GET /odata/Heroes?$select=Name,Level&$filter=Level gt 50&$expand=Faction

### 3. Demo Chat Real-time
Dự án có sẵn một giao diện web đơn giản để test tính năng chat.
1.  Khởi chạy API.
2.  Mở trình duyệt và truy cập: `https://localhost:7296/chat.html`.
3.  Sử dụng API `/api/auth/login` để lấy **Access Token**.
4.  Nhập Token vào ô kết nối trên trang chat để bắt đầu:
  * Gửi tin nhắn vào kênh chung (Feed).
  * Nhập `HeroId` của người khác để chat riêng (Private).
  * Gửi ảnh đính kèm.

---

## 🧪 Testing

Dự án áp dụng Unit Testing để đảm bảo chất lượng code cho tầng Business Logic (Services).

Để chạy toàn bộ test case:
```bash
dotnet test
