# ReCaptcha Implementation - Memory Bank

## Cấu trúc hiện tại

### Services
- **IReCaptchaService**: Interface định nghĩa contract
- **ReCaptchaService**: Implementation xử lý validation

### Models
- **RecaptchaRequest**: Request model cho API call
- **ResponseCaptcha**: Response model từ Google reCAPTCHA Enterprise
  - RiskAnalysis.Score: Float score từ 0.0 đến 1.0
  - TokenProperties.Valid: Boolean validation status

### Configuration (appsettings.json)
```json
"ReCaptcha": {
  "SecretKey": "6Le1iSkqAAAAAADUHW7NPE1KUtXosW4QeT3X_DWj",
  "SiteKey": "6Lc3vSoqAAAAAEjr6ky2lG0ohbnbSPoRcCikEzsp", 
  "APIKey": "AIzaSyC4GlNNtyY1qfnevFX3fFJF5WXFElMKCWo"
}
```

## Vấn đề Hardcode

### Locations (7 chỗ):
1. **AuthController.cs** - 3 instances
2. **CardController.cs** - 2 instances  
3. **TransactionBankingController.cs** - 2 instances

### Pattern hiện tại:
```csharp
if (recaptchaResponse != null && 
    recaptchaResponse.TokenProperties.Valid && 
    recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
{
    // Logic khi captcha valid
}
```

## Giải pháp Refactor

### 1. Thêm MinimumScore vào config
```json
"ReCaptcha": {
  "SecretKey": "...",
  "SiteKey": "...",
  "APIKey": "...",
  "MinimumScore": 0.9
}
```

### 2. Mở rộng IReCaptchaService
- Thêm method `IsValidCaptchaAsync(string token)`
- Method này sẽ gọi `VerifyTokenAsync` và kiểm tra với config score

### 3. Refactor Controllers
- Thay thế hardcode logic bằng service call
- Đơn giản hóa code validation

## Lợi ích
- Dễ dàng thay đổi threshold score
- Centralized validation logic
- Consistent behavior across controllers
- Better maintainability
