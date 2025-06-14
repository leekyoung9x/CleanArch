# CleanArch Project - Memory Bank

## Tổng quan Project
- **Tên**: CleanArch
- **Kiến trúc**: Clean Architecture
- **Ngôn ngữ**: C# (.NET)
- **Loại**: Web API + WebSocket

## Cấu trúc Project

### CleanArch.Api
- **Controllers**: API endpoints chính
  - AuthController: Xác thực người dùng
  - CardController: Xử lý thẻ cào
  - TransactionBankingController: Giao dịch ngân hàng
  - BankController, PostController, RankController
- **Services**: Business logic services
  - ReCaptchaService: Xử lý validation captcha
  - AuthService, BankService, CardService, etc.

### CleanArch.Core
- **Entities**: Domain models và DTOs
- **RequestModel**: Models cho API requests
- **ResponseModel**: Models cho API responses

### CleanArch.Infrastructure
- **Repository**: Data access layer
- **Base**: Base classes cho repository pattern

### CleanArch.Application
- **Interfaces**: Abstractions cho business logic

## Cấu hình quan trọng
- **Database**: MySQL (localhost)
- **JWT**: Authentication tokens
- **ReCaptcha**: Google reCAPTCHA Enterprise
- **Elasticsearch**: Logging và analytics
- **Banking API**: Web2M integration
- **Card API**: TheSieuRe integration

## Vấn đề hiện tại
- Score validation cho reCAPTCHA bị hardcode (0.9) trong 7 chỗ
- Cần refactor để sử dụng configuration
