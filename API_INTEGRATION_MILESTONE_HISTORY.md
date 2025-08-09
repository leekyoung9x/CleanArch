# 📖 API Integration Document - Milestone Claim History

## 🎯 Overview
API endpoint để lấy lịch sử nhận thưởng milestone của user hiện tại, bao gồm thông tin chi tiết về milestone, gói quà và gift code đã được tạo.

---

## 📋 API Endpoint Details

### **GET** `/api/UserMilestoneClaim/history`

**Mục đích**: Lấy danh sách tất cả milestone mà user hiện tại đã claim thành công

**Authentication**: Required (JWT Token)

**Content-Type**: `application/json`

---

## 🔐 Authentication

### Headers Required
```http
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json
```

### JWT Token Format
- **Location**: Header `Authorization`
- **Format**: `Bearer {token}`
- **Claims Required**: Token phải chứa `sub` (user ID) để identify user

---

## 📥 Request Format

### HTTP Method: `GET`

### URL: `/api/UserMilestoneClaim/history`

### Headers:
```http
GET /api/UserMilestoneClaim/history HTTP/1.1
Host: your-domain.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

### Parameters: 
**Không có parameters** - API tự động lấy user ID từ JWT token

---

## 📤 Response Format

### Success Response (200 OK)

```json
{
  "status": true,
  "statusMessage": "Lấy lịch sử thành công",
  "data": [
    {
      "milestoneId": 1,
      "milestoneName": "Mốc nạp 10,000 VND",
      "requiredScore": 10000,
      "rewardPackageName": "Gói quà bronze",
      "claimedAt": "2025-08-09T15:30:00.000Z",
      "giftCode": "ABC1234567",
      "giftCodeId": 123
    },
    {
      "milestoneId": 2,
      "milestoneName": "Mốc nạp 50,000 VND",
      "requiredScore": 50000,
      "rewardPackageName": "Gói quà silver", 
      "claimedAt": "2025-08-08T10:15:00.000Z",
      "giftCode": "XYZ9876543",
      "giftCodeId": 124
    }
  ]
}
```

### Empty History Response (200 OK)

```json
{
  "status": true,
  "statusMessage": "Lấy lịch sử thành công",
  "data": []
}
```

---

## ❌ Error Responses

### 1. Unauthorized (401)
```json
{
  "status": false,
  "statusMessage": "Token không hợp lệ",
  "data": null
}
```
**Nguyên nhân**: JWT token không có hoặc đã hết hạn

### 2. User Not Found (400)
```json
{
  "status": false,
  "statusMessage": "Bạn chưa tạo nhân vật",
  "data": null
}
```
**Nguyên nhân**: User ID trong token không tồn tại trong database

### 3. Internal Server Error (500)
```json
{
  "status": false,
  "statusMessage": "Lỗi hệ thống",
  "data": null
}
```
**Nguyên nhân**: Lỗi database hoặc server internal error

---

## 📊 Response Data Schema

### Root Response Object
| Field | Type | Description |
|-------|------|-------------|
| `status` | `boolean` | `true` nếu thành công, `false` nếu có lỗi |
| `statusMessage` | `string` | Thông báo kết quả |
| `data` | `array` hoặc `null` | Danh sách lịch sử hoặc `null` nếu lỗi |

### History Item Object (trong `data` array)
| Field | Type | Description | Example |
|-------|------|-------------|---------|
| `milestoneId` | `number` | ID của milestone | `1` |
| `milestoneName` | `string` | Tên milestone | `"Mốc nạp 10,000 VND"` |
| `requiredScore` | `number` | Điểm yêu cầu để đạt milestone | `10000` |
| `rewardPackageName` | `string` | Tên gói quà thưởng | `"Gói quà bronze"` |
| `claimedAt` | `string` | Thời gian claim (ISO 8601) | `"2025-08-09T15:30:00.000Z"` |
| `giftCode` | `string` | Mã gift code được tạo | `"ABC1234567"` |
| `giftCodeId` | `number` | ID của gift code | `123` |

---

## 💻 Client Integration Examples

### 1. JavaScript/Fetch Example

```javascript
async function getMilestoneHistory() {
  try {
    const token = localStorage.getItem('jwt_token');
    
    const response = await fetch('/api/UserMilestoneClaim/history', {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    const result = await response.json();
    
    if (result.status) {
      console.log('Lịch sử milestone:', result.data);
      return result.data;
    } else {
      console.error('API Error:', result.statusMessage);
      throw new Error(result.statusMessage);
    }
    
  } catch (error) {
    console.error('Network Error:', error);
    throw error;
  }
}

// Sử dụng
getMilestoneHistory()
  .then(history => {
    // Xử lý data
    history.forEach(item => {
      console.log(`✅ ${item.milestoneName}`);
      console.log(`   Gift Code: ${item.giftCode}`);
    });
  })
  .catch(error => {
    // Xử lý lỗi
    alert('Không thể tải lịch sử: ' + error.message);
  });
```

### 2. jQuery/Ajax Example

```javascript
function getMilestoneHistory() {
  const token = localStorage.getItem('jwt_token');
  
  return $.ajax({
    url: '/api/UserMilestoneClaim/history',
    method: 'GET',
    headers: {
      'Authorization': 'Bearer ' + token,
      'Content-Type': 'application/json'
    },
    success: function(result) {
      if (result.status) {
        displayHistory(result.data);
      } else {
        showError(result.statusMessage);
      }
    },
    error: function(xhr, status, error) {
      console.error('AJAX Error:', error);
      showError('Không thể kết nối server');
    }
  });
}

function displayHistory(historyData) {
  const container = $('#history-container');
  container.empty();
  
  if (historyData.length === 0) {
    container.html('<p>Bạn chưa nhận thưởng mốc nào.</p>');
    return;
  }
  
  historyData.forEach(function(item) {
    const html = `
      <div class="history-item">
        <h3>${item.milestoneName}</h3>
        <p>Mốc: ${item.requiredScore.toLocaleString()} VND</p>
        <p>Gói quà: ${item.rewardPackageName}</p>
        <p>Gift Code: <code>${item.giftCode}</code></p>
        <p>Nhận lúc: ${new Date(item.claimedAt).toLocaleString()}</p>
      </div>
    `;
    container.append(html);
  });
}

function showError(message) {
  alert('Lỗi: ' + message);
}
```

### 3. Unity C# Example

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class MilestoneHistoryItem
{
    public int milestoneId;
    public string milestoneName;
    public int requiredScore;
    public string rewardPackageName;
    public string claimedAt;
    public string giftCode;
    public int giftCodeId;
}

[System.Serializable]
public class MilestoneHistoryResponse
{
    public bool status;
    public string statusMessage;
    public MilestoneHistoryItem[] data;
}

public class MilestoneHistoryManager : MonoBehaviour
{
    private string baseUrl = "https://your-api-domain.com";
    
    public IEnumerator GetMilestoneHistory(System.Action<MilestoneHistoryItem[]> onSuccess, System.Action<string> onError)
    {
        string token = PlayerPrefs.GetString("jwt_token");
        
        if (string.IsNullOrEmpty(token))
        {
            onError?.Invoke("Chưa đăng nhập");
            yield break;
        }
        
        using (UnityWebRequest request = UnityWebRequest.Get($"{baseUrl}/api/UserMilestoneClaim/history"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    MilestoneHistoryResponse response = JsonUtility.FromJson<MilestoneHistoryResponse>(request.downloadHandler.text);
                    
                    if (response.status)
                    {
                        onSuccess?.Invoke(response.data);
                    }
                    else
                    {
                        onError?.Invoke(response.statusMessage);
                    }
                }
                catch (System.Exception e)
                {
                    onError?.Invoke("Lỗi parse JSON: " + e.Message);
                }
            }
            else
            {
                onError?.Invoke("Network error: " + request.error);
            }
        }
    }
}

// Sử dụng:
// StartCoroutine(GetMilestoneHistory(
//     onSuccess: (history) => {
//         Debug.Log($"Có {history.Length} milestone đã claim");
//         foreach(var item in history) {
//             Debug.Log($"Gift Code: {item.giftCode}");
//         }
//     },
//     onError: (error) => {
//         Debug.LogError("Lỗi: " + error);
//     }
// ));
```

---

## 🔄 Data Flow

```
[Client App] 
    ↓ (GET với JWT token)
[API Controller] 
    ↓ (Lấy userId từ token)
[Repository Layer] 
    ↓ (Query database với JOIN)
[MySQL Database] 
    ↓ (Return joined data)
[Controller] 
    ↓ (Format response)
[Client App] 
    ↓ (Hiển thị lịch sử)
```

---

## 🛠️ Testing Guide

### 1. Postman Testing

**Request:**
```http
GET {{baseUrl}}/api/UserMilestoneClaim/history
Authorization: Bearer {{jwt_token}}
Content-Type: application/json
```

**Environment Variables:**
- `baseUrl`: https://your-api-domain.com
- `jwt_token`: Your JWT token

### 2. cURL Testing

```bash
curl -X GET "https://your-api-domain.com/api/UserMilestoneClaim/history" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 3. Browser DevTools Testing

```javascript
// Paste vào Browser Console
fetch('/api/UserMilestoneClaim/history', {
  headers: {
    'Authorization': 'Bearer ' + localStorage.getItem('jwt_token'),
    'Content-Type': 'application/json'
  }
})
.then(r => r.json())
.then(console.log);
```

---

## 📝 Integration Checklist

### ✅ Prerequisites
- [ ] User đã đăng nhập và có JWT token hợp lệ
- [ ] JWT token chứa claim `sub` (user ID)
- [ ] User đã tạo character/profile trong hệ thống
- [ ] Database đã có data milestone claims

### ✅ Implementation Steps
- [ ] Implement HTTP client với Authorization header
- [ ] Handle response parsing (JSON)
- [ ] Implement error handling cho các trường hợp lỗi
- [ ] Display data trong UI
- [ ] Handle empty state (chưa có lịch sử)
- [ ] Implement refresh/reload functionality

### ✅ Testing
- [ ] Test với user có lịch sử claims
- [ ] Test với user chưa có lịch sử (empty array)
- [ ] Test với token không hợp lệ (401 error)
- [ ] Test với user không tồn tại (400 error)
- [ ] Test network errors và timeouts

---

## 🚨 Important Notes

1. **Authentication Required**: API này LUÔN yêu cầu JWT token hợp lệ
2. **User Context**: Chỉ trả về lịch sử của user hiện tại (từ token)
3. **Date Format**: `claimedAt` sử dụng ISO 8601 format với timezone UTC
4. **Empty Response**: Nếu chưa có lịch sử, trả về array rỗng `[]` không phải `null`
5. **Gift Code**: Mỗi milestone claim sẽ có 1 gift code unique được tạo tự động
6. **Sorting**: Dữ liệu được sắp xếp theo thời gian claim (mới nhất trước)

---

## 📞 Support

Nếu gặp vấn đề trong quá trình tích hợp:
1. Kiểm tra JWT token có hợp lệ không
2. Verify user đã tạo character trong hệ thống
3. Check database có data milestone claims không
4. Review API response để identify exact error
