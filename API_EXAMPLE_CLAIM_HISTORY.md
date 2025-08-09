# 📋 API Example - Get Milestone Claim History

## JavaScript/Fetch Example

```javascript
// Lấy lịch sử nhận thưởng
async function getMyClaimHistory(token) {
  try {
    const response = await fetch('/api/UserMilestoneClaim/history', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      }
    });
    
    const result = await response.json();
    
    if (result.status) {
      console.log('Lịch sử nhận thưởng:', result.data);
      return result.data;
    } else {
      console.error('Lỗi:', result.statusMessage);
      return [];
    }
  } catch (error) {
    console.error('Network error:', error);
    return [];
  }
}

// Sử dụng
const token = localStorage.getItem('jwt_token');
const history = await getMyClaimHistory(token);

// Hiển thị lịch sử
history.forEach(item => {
  console.log(`✅ ${item.milestoneName} (${item.requiredScore.toLocaleString()} VND)`);
  console.log(`   📦 ${item.rewardPackageName}`);
  console.log(`   🎁 Gift Code: ${item.giftCode}`);
  console.log(`   📅 ${new Date(item.claimedAt).toLocaleDateString()}`);
  console.log('');
});
```

## React Component Example

```jsx
import React, { useState, useEffect } from 'react';

const ClaimHistoryComponent = () => {
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    loadClaimHistory();
  }, []);
  
  const loadClaimHistory = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const token = localStorage.getItem('jwt_token');
      const response = await fetch('/api/UserMilestoneClaim/history', {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      const result = await response.json();
      
      if (result.status) {
        setHistory(result.data);
      } else {
        setError(result.statusMessage);
      }
    } catch (err) {
      setError('Không thể tải lịch sử');
    } finally {
      setLoading(false);
    }
  };
  
  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString('vi-VN');
  };
  
  const formatAmount = (amount) => {
    return amount.toLocaleString('vi-VN') + ' VND';
  };
  
  const copyGiftCode = (code) => {
    navigator.clipboard.writeText(code);
    alert('Đã copy gift code!');
  };
  
  if (loading) return <div>Đang tải...</div>;
  if (error) return <div>Lỗi: {error}</div>;
  
  return (
    <div className="claim-history">
      <h2>Lịch Sử Nhận Thưởng</h2>
      
      {history.length === 0 ? (
        <p>Bạn chưa nhận thưởng mốc nào.</p>
      ) : (
        <div className="history-list">
          {history.map((item, index) => (
            <div key={index} className="history-item">
              <div className="milestone-info">
                <h3>{item.milestoneName}</h3>
                <p className="milestone-score">Mốc: {formatAmount(item.requiredScore)}</p>
                <p className="package-name">Gói quà: {item.rewardPackageName}</p>
              </div>
              
              <div className="claim-details">
                <div className="gift-code">
                  <span>Gift Code: </span>
                  <code>{item.giftCode}</code>
                  <button 
                    onClick={() => copyGiftCode(item.giftCode)}
                    className="copy-btn"
                  >
                    📋 Copy
                  </button>
                </div>
                
                <div className="claim-time">
                  <span>Nhận lúc: {formatDate(item.claimedAt)}</span>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ClaimHistoryComponent;
```

## CSS Styles

```css
.claim-history {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.history-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.history-item {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 16px;
  background: #f9f9f9;
}

.milestone-info h3 {
  margin: 0 0 8px 0;
  color: #333;
}

.milestone-score {
  font-weight: bold;
  color: #2196F3;
  margin: 4px 0;
}

.package-name {
  color: #666;
  margin: 4px 0;
}

.gift-code {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 12px 0 8px 0;
}

.gift-code code {
  background: #e8e8e8;
  padding: 4px 8px;
  border-radius: 4px;
  font-family: monospace;
  font-weight: bold;
}

.copy-btn {
  background: #4CAF50;
  color: white;
  border: none;
  padding: 4px 8px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
}

.copy-btn:hover {
  background: #45a049;
}

.claim-time {
  font-size: 14px;
  color: #888;
}
```

## Response Data Structure

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
      "claimedAt": "2025-08-09T15:30:00",
      "giftCode": "ABC1234567",
      "giftCodeId": 123
    },
    {
      "milestoneId": 2,
      "milestoneName": "Mốc nạp 50,000 VND", 
      "requiredScore": 50000,
      "rewardPackageName": "Gói quà silver",
      "claimedAt": "2025-08-08T10:15:00",
      "giftCode": "XYZ9876543",
      "giftCodeId": 124
    }
  ]
}
```

## Error Cases

```json
// Chưa đăng nhập
{
  "status": false,
  "statusMessage": "Token không hợp lệ",
  "data": null
}

// Chưa tạo nhân vật
{
  "status": false,
  "statusMessage": "Bạn chưa tạo nhân vật", 
  "data": null
}

// Thành công nhưng chưa có lịch sử
{
  "status": true,
  "statusMessage": "Lấy lịch sử thành công",
  "data": []
}
```
