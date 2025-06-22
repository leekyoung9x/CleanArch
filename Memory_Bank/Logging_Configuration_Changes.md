# Logging Configuration Changes - Memory Bank

## Thay đổi được thực hiện
**Ngày:** 22/06/2025
**Mục tiêu:** Sửa cơ chế log để chỉ log level ERROR và FATAL

## Chi tiết thay đổi

### 1. NLog Configuration (nlog.config)
**Trước:**
```xml
<rules>
    <logger name="*" minlevel="Debug" writeTo="console,file" />
    <logger name="*" minlevel="Info" writeTo="elk" />
</rules>
```

**Sau:**
```xml
<rules>
    <!-- Cấu hình ghi log ra console - chỉ ERROR và FATAL -->
    <logger name="*" minlevel="Error" writeTo="console,file" />
    
    <!-- Cấu hình ghi log vào Elasticsearch - chỉ ERROR và FATAL -->
    <logger name="*" minlevel="Error" writeTo="elk" />
</rules>
```

### 2. Log4net Configuration (log4net.config)
**Trước:**
```xml
<root>
    <level value="ALL" />
```

**Sau:**
```xml
<root>
    <level value="ERROR" />
```

### 3. JwtLoggingMiddleware
**Thay đổi:** Xóa Console.WriteLine để tránh rò rỉ JWT token trong logs

**Trước:**
```csharp
Console.WriteLine($"Received token: {token}");
```

**Sau:**
```csharp
// Removed JWT token logging for security reasons
```

## Lợi ích đạt được

### 1. Performance
- ✅ Giảm đáng kể volume logs được ghi
- ✅ Cải thiện performance ứng dụng
- ✅ Giảm I/O operations

### 2. Storage
- ✅ Tiết kiệm storage cho file logs
- ✅ Giảm tải cho Elasticsearch cluster
- ✅ Tiết kiệm chi phí infrastructure

### 3. Security
- ✅ Loại bỏ việc log JWT tokens
- ✅ Tập trung vào logs quan trọng
- ✅ Giảm nguy cơ rò rỉ thông tin

### 4. Monitoring
- ✅ Tập trung vào các lỗi thực sự cần xử lý
- ✅ Dễ dàng phát hiện vấn đề nghiêm trọng
- ✅ Giảm noise trong log monitoring

## Cấu hình hiện tại

### NLog (Chính)
- **Framework:** NLog được sử dụng chính trong Program.cs
- **Targets:** Console, File, Elasticsearch
- **Level:** Error và Fatal only
- **File path:** logfile.txt
- **Elasticsearch:** http://157.245.153.39:9200/log/_doc

### Log4net (Backup)
- **Level:** ERROR only
- **Appenders:** RollingFile, Console, Elasticsearch
- **File path:** C:\Logs\CleanArch_API\Log.%property{log4net:HostName}.log

## Debug Logging cho ConfirmTransaction API
**Ngày cập nhật:** 22/06/2025
**Lý do:** Thêm debug logging cho ConfirmTransaction API để dễ dàng troubleshoot

### Thay đổi bổ sung:

#### 1. NLog Configuration Update
**Thêm rule đặc biệt cho TransactionBankingController:**
```xml
<logger name="CleanArch.Api.Controllers.TransactionBankingController" minlevel="Info" writeTo="console,file" />
```

#### 2. Debug Log Points đã thêm:
1. **API Entry**: Log input parameters (Amount, OTP)
2. **reCAPTCHA Validation**: Log validation process và kết quả
3. **Account/Player Lookup**: Log AccountId, PlayerId
4. **Transaction Lookup**: Log transaction tìm kiếm và kết quả
5. **Bank Service Call**: Log số lượng transactions từ bank API
6. **Transaction Matching**: 
   - Log chi tiết từng bank transaction
   - Log kết quả so sánh (Type, Amount, Description)
   - Log khi tìm thấy match
7. **Account Updates**: Log số tiền cũ/mới, bonus, event points
8. **Database Transaction**: Log từng bước update và kết quả
9. **Exception Handling**: Log chi tiết lỗi trong catch blocks
10. **API Exit**: Log kết quả cuối cùng

### Log Levels sử dụng:
- **INFO**: Flow bình thường, debug information
- **WARN**: Validation failures, business logic warnings
- **ERROR**: Database errors, exceptions

### Structured Logging:
Sử dụng NLog structured logging với parameters:
```csharp
_logger.Info("Message with {Param1} and {Param2}", value1, value2);
```

## Lưu ý quan trọng
1. Project chủ yếu sử dụng **NLog** (được config trong Program.cs)
2. Log4net được giữ lại để backward compatibility
3. **TransactionBankingController** được cấu hình đặc biệt để log INFO level
4. Các controller khác vẫn chỉ log ERROR level
5. Elasticsearch vẫn chỉ nhận ERROR logs để tiết kiệm resources
6. Debug logs chỉ ghi vào console và file, không ghi vào Elasticsearch

## Cách sử dụng Debug Logs:
1. **Console**: Xem real-time logs khi chạy application
2. **File**: Đọc `logfile.txt` để xem lịch sử logs
3. **Structured Search**: Tìm kiếm theo AccountId, PlayerId, TransactionId
4. **Flow Tracking**: Theo dõi từng bước của transaction flow

## Cải thiện Console Logging
**Ngày cập nhật:** 22/06/2025
**Mục đích:** Cải thiện khả năng đọc logs và tránh hiểu nhầm về cấu hình

### Thay đổi mới:

#### 1. **Colored Console Output**
- **ColoredConsole target**: Tự động tô màu theo log level
- **DEBUG**: Màu xám
- **INFO**: Màu xanh lá (cho debug TransactionBankingController)
- **WARN**: Màu vàng  
- **ERROR**: Màu đỏ
- **FATAL**: Màu tím

#### 2. **Improved File Logging**
- **Daily rotation**: `logs/app-2025-06-22.log`
- **Archive management**: Giữ 30 ngày logs
- **Better formatting**: Timestamp đầy đủ + padding levels

#### 3. **Detailed Comments trong nlog.config**
- **Rule priority explanation**: Giải thích thứ tự rules quan trọng
- **Purpose documentation**: Tại sao có rule đặc biệt cho TransactionBankingController
- **Target descriptions**: Mô tả rõ từng target làm gì

#### 4. **Configuration Structure**
```xml
<!-- Rule 1: TransactionBankingController → INFO level (debug) -->
<!-- Rule 2: Other controllers → ERROR level (production) -->  
<!-- Rule 3: All → ERROR level (Elasticsearch) -->
```

### Lợi ích mới:
- ✅ **Visual clarity**: Màu sắc giúp phân biệt log levels ngay lập tức
- ✅ **Better maintenance**: Comments giúp team hiểu configuration
- ✅ **Professional logs**: Format đẹp và consistent
- ✅ **File management**: Auto archive và cleanup logs cũ
- ✅ **No confusion**: Rõ ràng tại sao INFO logs xuất hiện
