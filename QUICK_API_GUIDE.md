# 🎯 Quick API Guide - Milestone Claim

## Endpoint chính

```
POST /api/UserMilestoneClaim/claim/{milestoneId}
```

## Request
- **Headers**: `Authorization: Bearer <token>`
- **milestoneId**: ID của mốc thưởng (integer)

## Response
```typescript
interface ApiResponse {
  status: boolean;
  statusMessage: string;
  data: string | null; // Gift code nếu thành công
}
```

## JavaScript Example
```javascript
async function claimMilestone(milestoneId, token) {
  const response = await fetch(`/api/UserMilestoneClaim/claim/${milestoneId}`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  const result = await response.json();
  
  if (result.status) {
    console.log('Gift code:', result.data);
    // Copy to clipboard
    navigator.clipboard.writeText(result.data);
  } else {
    console.error('Error:', result.statusMessage);
  }
  
  return result;
}
```

## Response Cases

| Status | statusMessage | Meaning |
|--------|---------------|---------|
| `true` | "Nhận thưởng thành công" | ✅ Success - Gift code in `data` |
| `false` | "Bạn chưa đủ điều kiện nhận mốc này" | ❌ Not eligible yet |
| `false` | "Bạn đã nhận thưởng mốc này rồi" | ❌ Already claimed |
| `false` | "Bạn chưa tạo nhân vật" | ❌ No character created |
| `false` | "Mốc thưởng không tồn tại" | ❌ Invalid milestone ID |

## Helper Endpoints

### Check if claimed
```
GET /api/UserMilestoneClaim/user/{userId}/milestone/{milestoneId}/claimed
```

### Get user's claimed milestones
```
GET /api/UserMilestoneClaim/user/{userId}
```

## Flow Logic
1. Check if user already claimed → Hide button if yes
2. Check if user meets requirement → Enable button if yes  
3. Call claim API → Show gift code if success
4. Handle errors → Show appropriate message

## React Component Example
```jsx
const MilestoneButton = ({ milestoneId, userScore, requiredScore }) => {
  const [claimed, setClaimed] = useState(false);
  const [loading, setLoading] = useState(false);
  
  const canClaim = userScore >= requiredScore && !claimed;
  
  const handleClaim = async () => {
    setLoading(true);
    const result = await claimMilestone(milestoneId, getToken());
    
    if (result.status) {
      setClaimed(true);
      showGiftCode(result.data);
    } else {
      showError(result.statusMessage);
    }
    
    setLoading(false);
  };
  
  return (
    <button 
      onClick={handleClaim} 
      disabled={!canClaim || loading}
    >
      {loading ? 'Processing...' : 
       claimed ? 'Claimed ✅' : 
       canClaim ? 'Claim Reward' : 
       'Not Eligible'}
    </button>
  );
};
```

## Notes
- ✅ JWT token required in Authorization header
- ✅ Each milestone can only be claimed once
- ✅ Gift code expires after 5 years
- ✅ Always check `status` field in response
- ⚠️ Use database transactions for consistency
