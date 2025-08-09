# 🎮 CLIENT INTEGRATION GUIDE - Milestone History API

## 📋 API Quick Reference

**Endpoint**: `GET /api/UserMilestoneClaim/history`  
**Auth**: JWT Token Required  
**Purpose**: Lấy lịch sử nhận thưởng milestone của user  

---

## 🔧 Request Format

```javascript
// Headers bắt buộc
{
  "Authorization": "Bearer YOUR_JWT_TOKEN",
  "Content-Type": "application/json"
}

// Method: GET
// Body: Không có (lấy user ID từ JWT token)
```

---

## 📨 Response Format

### ✅ Success Response
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
    }
  ]
}
```

### ❌ Error Response
```json
{
  "status": false,
  "statusMessage": "Token không hợp lệ", // hoặc "Bạn chưa tạo nhân vật"
  "data": null
}
```

### 📭 Empty History
```json
{
  "status": true,
  "statusMessage": "Lấy lịch sử thành công", 
  "data": []
}
```

---

## 💻 Implementation Examples

### 1. JavaScript/Fetch (Recommended)

```javascript
async function loadMilestoneHistory() {
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
      // Thành công
      displayHistory(result.data);
      return result.data;
    } else {
      // Có lỗi
      showError(result.statusMessage);
      return null;
    }
    
  } catch (error) {
    console.error('Network Error:', error);
    showError('Không thể kết nối server');
    return null;
  }
}

function displayHistory(historyList) {
  const container = document.getElementById('history-container');
  
  if (historyList.length === 0) {
    container.innerHTML = '<p>Bạn chưa nhận thưởng mốc nào.</p>';
    return;
  }
  
  const html = historyList.map(item => `
    <div class="milestone-item">
      <h3>${item.milestoneName}</h3>
      <p><strong>Mốc:</strong> ${item.requiredScore.toLocaleString()} VND</p>
      <p><strong>Gói quà:</strong> ${item.rewardPackageName}</p>
      <p><strong>Gift Code:</strong> <code>${item.giftCode}</code> 
         <button onclick="copyCode('${item.giftCode}')">Copy</button></p>
      <p><strong>Nhận lúc:</strong> ${new Date(item.claimedAt).toLocaleString('vi-VN')}</p>
    </div>
  `).join('');
  
  container.innerHTML = html;
}

function copyCode(giftCode) {
  navigator.clipboard.writeText(giftCode);
  alert('Đã copy gift code: ' + giftCode);
}

function showError(message) {
  document.getElementById('error-message').textContent = message;
}

// Gọi khi trang load
loadMilestoneHistory();
```

### 2. Unity C# Example

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class MilestoneHistory
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
public class ApiResponse
{
    public bool status;
    public string statusMessage;
    public MilestoneHistory[] data;
}

public class MilestoneManager : MonoBehaviour
{
    [Header("API Settings")]
    public string apiBaseUrl = "https://your-domain.com";
    
    public void LoadHistory()
    {
        StartCoroutine(GetMilestoneHistory());
    }
    
    IEnumerator GetMilestoneHistory()
    {
        string token = PlayerPrefs.GetString("jwt_token");
        
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("JWT token không tồn tại");
            yield break;
        }
        
        string url = $"{apiBaseUrl}/api/UserMilestoneClaim/history";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
                
                if (response.status)
                {
                    Debug.Log($"Tải được {response.data.Length} milestone");
                    DisplayHistory(response.data);
                }
                else
                {
                    Debug.LogError("API Error: " + response.statusMessage);
                }
            }
            else
            {
                Debug.LogError("Network Error: " + request.error);
            }
        }
    }
    
    void DisplayHistory(MilestoneHistory[] history)
    {
        foreach (var item in history)
        {
            Debug.Log($"✅ {item.milestoneName}");
            Debug.Log($"   Gift Code: {item.giftCode}");
            Debug.Log($"   Claimed: {item.claimedAt}");
        }
    }
}
```

### 3. jQuery/Ajax Example

```javascript
function loadMilestoneHistory() {
  const token = localStorage.getItem('jwt_token');
  
  $.ajax({
    url: '/api/UserMilestoneClaim/history',
    method: 'GET',
    headers: {
      'Authorization': 'Bearer ' + token,
      'Content-Type': 'application/json'
    },
    success: function(response) {
      if (response.status) {
        displayHistoryTable(response.data);
      } else {
        $('#error-msg').text(response.statusMessage);
      }
    },
    error: function(xhr, status, error) {
      console.error('AJAX Error:', error);
      $('#error-msg').text('Không thể kết nối server');
    }
  });
}

function displayHistoryTable(data) {
  const tbody = $('#history-table tbody');
  tbody.empty();
  
  if (data.length === 0) {
    tbody.append('<tr><td colspan="5">Chưa có lịch sử nhận thưởng</td></tr>');
    return;
  }
  
  data.forEach(function(item) {
    const row = `
      <tr>
        <td>${item.milestoneName}</td>
        <td>${item.requiredScore.toLocaleString()} VND</td>
        <td>${item.rewardPackageName}</td>
        <td>
          <code>${item.giftCode}</code>
          <button class="btn-copy" onclick="copyGiftCode('${item.giftCode}')">📋</button>
        </td>
        <td>${new Date(item.claimedAt).toLocaleDateString('vi-VN')}</td>
      </tr>
    `;
    tbody.append(row);
  });
}

function copyGiftCode(code) {
  navigator.clipboard.writeText(code).then(function() {
    alert('Đã copy: ' + code);
  });
}
```

---

## 🎨 UI Display Examples

### HTML Structure

```html
<!DOCTYPE html>
<html>
<head>
  <title>Lịch Sử Milestone</title>
  <style>
    .milestone-item {
      border: 1px solid #ddd;
      padding: 15px;
      margin: 10px 0;
      border-radius: 8px;
      background: #f9f9f9;
    }
    .milestone-item h3 {
      color: #333;
      margin: 0 0 10px 0;
    }
    .milestone-item code {
      background: #e8e8e8;
      padding: 5px;
      border-radius: 4px;
      font-family: monospace;
      font-weight: bold;
    }
    .btn-copy {
      background: #4CAF50;
      color: white;
      border: none;
      padding: 5px 10px;
      margin-left: 10px;
      border-radius: 4px;
      cursor: pointer;
    }
    .error {
      color: red;
      font-weight: bold;
    }
    .empty-state {
      text-align: center;
      color: #666;
      font-style: italic;
    }
  </style>
</head>
<body>
  <h1>🎁 Lịch Sử Nhận Thưởng Milestone</h1>
  
  <div id="error-message" class="error"></div>
  
  <button onclick="loadMilestoneHistory()">🔄 Tải Lại</button>
  
  <div id="history-container">
    <p>Đang tải...</p>
  </div>

  <script>
    // Paste JavaScript code ở trên vào đây
  </script>
</body>
</html>
```

### Table Format Alternative

```html
<table id="history-table" class="table">
  <thead>
    <tr>
      <th>Milestone</th>
      <th>Mốc điểm</th>
      <th>Gói quà</th>
      <th>Gift Code</th>
      <th>Ngày nhận</th>
    </tr>
  </thead>
  <tbody>
    <!-- Data sẽ được load vào đây bằng JavaScript -->
  </tbody>
</table>
```

---

## ⚡ Quick Testing

### Browser Console Test

Mở Developer Tools → Console, paste code sau:

```javascript
// Test API directly
fetch('/api/UserMilestoneClaim/history', {
  headers: {
    'Authorization': 'Bearer ' + localStorage.getItem('jwt_token'),
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => {
  console.log('API Response:', data);
  if (data.status && data.data.length > 0) {
    console.log('✅ Có ' + data.data.length + ' milestone');
    data.data.forEach(item => {
      console.log(`- ${item.milestoneName}: ${item.giftCode}`);
    });
  } else if (data.status && data.data.length === 0) {
    console.log('📭 Chưa có milestone nào');
  } else {
    console.log('❌ Lỗi: ' + data.statusMessage);
  }
})
.catch(error => {
  console.error('Network Error:', error);
});
```

### Postman Collection

```json
{
  "info": {
    "name": "Milestone History API"
  },
  "item": [
    {
      "name": "Get Milestone History",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{jwt_token}}"
          },
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "{{base_url}}/api/UserMilestoneClaim/history",
          "host": ["{{base_url}}"],
          "path": ["api", "UserMilestoneClaim", "history"]
        }
      }
    }
  ]
}
```

---

## 📋 Integration Checklist

### ✅ Bước 1: Authentication
- [ ] Lưu JWT token trong localStorage/PlayerPrefs
- [ ] Thêm Authorization header vào request
- [ ] Handle token hết hạn (401 error)

### ✅ Bước 2: API Call
- [ ] Gọi GET `/api/UserMilestoneClaim/history`
- [ ] Parse JSON response
- [ ] Check `response.status` field

### ✅ Bước 3: Data Handling
- [ ] Handle empty array (chưa có lịch sử)
- [ ] Display milestone info
- [ ] Format dates và numbers
- [ ] Copy gift code functionality

### ✅ Bước 4: Error Handling
- [ ] Network errors
- [ ] API errors (status: false)
- [ ] User không tồn tại
- [ ] Token không hợp lệ

### ✅ Bước 5: UI/UX
- [ ] Loading state
- [ ] Empty state
- [ ] Error messages
- [ ] Refresh functionality

---

## 🚨 Important Notes

1. **JWT Token**: Bắt buộc phải có và hợp lệ
2. **User Context**: API chỉ trả về lịch sử của user hiện tại
3. **Date Format**: `claimedAt` là ISO 8601 UTC
4. **Empty Response**: `data: []` không phải `data: null`
5. **Gift Code**: Unique cho mỗi milestone claim
6. **Sorting**: Mới nhất trước (ORDER BY claimedAt DESC)

---

## ❓ FAQ

**Q: API trả về empty array có nghĩa là gì?**  
A: User chưa claim milestone nào, hoàn toàn bình thường.

**Q: Làm sao biết token đã hết hạn?**  
A: API sẽ trả về `status: false` với message "Token không hợp lệ".

**Q: Gift code có thời hạn không?**  
A: Có, gift code có expiry date (thường là 5 năm từ khi tạo).

**Q: Có thể lọc theo milestone cụ thể không?**  
A: API này trả về tất cả, frontend tự filter nếu cần.

**Q: Dữ liệu có real-time không?**  
A: Không, cần refresh để get data mới nhất.
