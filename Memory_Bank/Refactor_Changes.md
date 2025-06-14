# Refactor Changes - ReCaptcha Score Configuration

## Thay đổi đã thực hiện

### 1. Cập nhật Configuration (appsettings.json)
- **File**: `CleanArch.Api/appsettings.json`
- **Thay đổi**: Thêm `"MinimumScore": 0.9` vào section `ReCaptcha`
- **Mục đích**: Chuyển hardcode score từ code sang configuration

### 2. Cập nhật Interface (IReCaptchaService.cs)
- **File**: `CleanArch.Api/Services/IReCaptchaService.cs`
- **Thay đổi**: Thêm method `Task<bool> IsValidCaptchaAsync(string token)`
- **Mục đích**: Tạo abstraction cho validation logic

### 3. Cập nhật Implementation (ReCaptchaService.cs)
- **File**: `CleanArch.Api/Services/ReCaptchaService.cs`
- **Thay đổi**: Implement method `IsValidCaptchaAsync`
- **Logic**: 
  ```csharp
  public async Task<bool> IsValidCaptchaAsync(string token)
  {
      var response = await VerifyTokenAsync(token);
      var minimumScore = _configuration.GetValue<float>("ReCaptcha:MinimumScore");
      
      return response != null && 
             response.TokenProperties?.Valid == true && 
             response.RiskAnalysis?.Score >= minimumScore;
  }
  ```

### 4. Refactor Controllers

#### AuthController.cs (3 chỗ)
- **Login method**: Thay thế hardcode validation
- **ChangePassword method**: Thay thế hardcode validation  
- **Register method**: Thay thế hardcode validation

#### CardController.cs (2 chỗ)
- **ChargingWS method**: Thay thế hardcode validation
- **CheckTransaction method**: Thay thế hardcode validation (dù có early return)

#### TransactionBankingController.cs (2 chỗ)
- **ConfirmTransaction method**: Thay thế hardcode validation
- **InsertTransaction method**: Thay thế hardcode validation

### 5. Pattern cũ vs Pattern mới

**Trước (Hardcode):**
```csharp
var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
if (recaptchaResponse != null && 
    recaptchaResponse.TokenProperties.Valid && 
    recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
{
    // Logic
}
```

**Sau (Configuration-based):**
```csharp
var isValidCaptcha = await reCaptchaService.IsValidCaptchaAsync(model.token);
if (isValidCaptcha)
{
    // Logic
}
```

## Lợi ích đạt được

1. **Flexibility**: Có thể thay đổi minimum score qua config
2. **Maintainability**: Code gọn gàng, dễ bảo trì
3. **Consistency**: Tất cả validation đều dùng cùng logic
4. **Centralized**: Logic validation tập trung tại service layer
5. **Testability**: Dễ dàng unit test với mock configuration

## Files đã thay đổi
- `CleanArch.Api/appsettings.json`
- `CleanArch.Api/Services/IReCaptchaService.cs`
- `CleanArch.Api/Services/ReCaptchaService.cs`
- `CleanArch.Api/Controllers/AuthController.cs`
- `CleanArch.Api/Controllers/CardController.cs`
- `CleanArch.Api/Controllers/TransactionBankingController.cs`

## Tổng cộng
- **7 chỗ hardcode** đã được refactor thành công
- **6 files** đã được cập nhật
- **100%** coverage cho tất cả các Controller validation
