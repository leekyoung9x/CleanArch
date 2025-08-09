// TypeScript Definitions for Milestone History API
// File: milestone-history.types.ts

/**
 * Response structure từ API /api/UserMilestoneClaim/history
 */
export interface MilestoneHistoryResponse {
  status: boolean;
  statusMessage: string;
  data: MilestoneHistoryItem[] | null;
}

/**
 * Chi tiết từng item trong lịch sử milestone
 */
export interface MilestoneHistoryItem {
  /** ID của milestone */
  milestoneId: number;
  
  /** Tên milestone (VD: "Mốc nạp 10,000 VND") */
  milestoneName: string;
  
  /** Điểm yêu cầu để đạt milestone */
  requiredScore: number;
  
  /** Tên gói quà thưởng */
  rewardPackageName: string;
  
  /** Thời gian claim milestone (ISO 8601 UTC) */
  claimedAt: string;
  
  /** Mã gift code được tạo */
  giftCode: string;
  
  /** ID của gift code */
  giftCodeId: number;
}

/**
 * API Client class để gọi milestone history
 */
export class MilestoneHistoryClient {
  private baseUrl: string;
  private token: string | null = null;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl.replace(/\/$/, ''); // Remove trailing slash
  }

  /**
   * Set JWT token cho authentication
   */
  setToken(token: string): void {
    this.token = token;
  }

  /**
   * Get JWT token từ localStorage
   */
  getTokenFromStorage(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('jwt_token');
    }
    return null;
  }

  /**
   * Lấy lịch sử milestone của user hiện tại
   */
  async getMilestoneHistory(): Promise<MilestoneHistoryItem[]> {
    const token = this.token || this.getTokenFromStorage();
    
    if (!token) {
      throw new Error('JWT token không tồn tại');
    }

    const response = await fetch(`${this.baseUrl}/api/UserMilestoneClaim/history`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP Error: ${response.status} - ${response.statusText}`);
    }

    const result: MilestoneHistoryResponse = await response.json();
    
    if (!result.status) {
      throw new Error(result.statusMessage || 'API Error');
    }

    return result.data || [];
  }

  /**
   * Format số thành currency VND
   */
  static formatCurrency(amount: number): string {
    return amount.toLocaleString('vi-VN') + ' VND';
  }

  /**
   * Format date từ ISO string sang Vietnam locale
   */
  static formatDate(isoString: string): string {
    return new Date(isoString).toLocaleString('vi-VN');
  }

  /**
   * Copy gift code vào clipboard
   */
  static async copyGiftCode(giftCode: string): Promise<void> {
    if (navigator.clipboard) {
      await navigator.clipboard.writeText(giftCode);
    } else {
      // Fallback cho browser cũ
      const textArea = document.createElement('textarea');
      textArea.value = giftCode;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
    }
  }
}

/**
 * React Hook để sử dụng milestone history
 */
export interface UseMilestoneHistoryResult {
  history: MilestoneHistoryItem[];
  loading: boolean;
  error: string | null;
  refetch: () => Promise<void>;
}

// React Hook (cần cài react)
// export function useMilestoneHistory(client: MilestoneHistoryClient): UseMilestoneHistoryResult {
//   const [history, setHistory] = useState<MilestoneHistoryItem[]>([]);
//   const [loading, setLoading] = useState<boolean>(true);
//   const [error, setError] = useState<string | null>(null);

//   const fetchHistory = async () => {
//     setLoading(true);
//     setError(null);
    
//     try {
//       const data = await client.getMilestoneHistory();
//       setHistory(data);
//     } catch (err) {
//       setError(err instanceof Error ? err.message : 'Unknown error');
//       setHistory([]);
//     } finally {
//       setLoading(false);
//     }
//   };

//   useEffect(() => {
//     fetchHistory();
//   }, []);

//   return {
//     history,
//     loading,
//     error,
//     refetch: fetchHistory
//   };
// }

/**
 * Utility functions
 */
export const MilestoneHistoryUtils = {
  /**
   * Kiểm tra gift code có hợp lệ không
   */
  isValidGiftCode(code: string): boolean {
    return /^[A-Z0-9]{10}$/.test(code);
  },

  /**
   * Sort history theo thời gian mới nhất trước
   */
  sortByLatest(history: MilestoneHistoryItem[]): MilestoneHistoryItem[] {
    return [...history].sort((a, b) => 
      new Date(b.claimedAt).getTime() - new Date(a.claimedAt).getTime()
    );
  },

  /**
   * Filter history theo milestone ID
   */
  filterByMilestone(history: MilestoneHistoryItem[], milestoneId: number): MilestoneHistoryItem[] {
    return history.filter(item => item.milestoneId === milestoneId);
  },

  /**
   * Tính tổng điểm từ tất cả milestone đã claim
   */
  getTotalScore(history: MilestoneHistoryItem[]): number {
    return history.reduce((total, item) => total + item.requiredScore, 0);
  },

  /**
   * Get milestone mới nhất
   */
  getLatestMilestone(history: MilestoneHistoryItem[]): MilestoneHistoryItem | null {
    if (history.length === 0) return null;
    
    return this.sortByLatest(history)[0];
  }
};

// Example usage:
/*
// 1. Khởi tạo client
const client = new MilestoneHistoryClient('https://api.your-domain.com');
client.setToken('your-jwt-token');

// 2. Sử dụng async/await
try {
  const history = await client.getMilestoneHistory();
  console.log('Lịch sử milestone:', history);
  
  // Format data để hiển thị
  history.forEach(item => {
    console.log(`✅ ${item.milestoneName}`);
    console.log(`   Mốc: ${MilestoneHistoryClient.formatCurrency(item.requiredScore)}`);
    console.log(`   Gift Code: ${item.giftCode}`);
    console.log(`   Nhận lúc: ${MilestoneHistoryClient.formatDate(item.claimedAt)}`);
  });
  
} catch (error) {
  console.error('Lỗi:', error.message);
}

// 3. Copy gift code
document.getElementById('copy-btn')?.addEventListener('click', async () => {
  try {
    await MilestoneHistoryClient.copyGiftCode('ABC1234567');
    alert('Đã copy gift code!');
  } catch (error) {
    console.error('Không thể copy:', error);
  }
});

// 4. Sử dụng utilities
const sortedHistory = MilestoneHistoryUtils.sortByLatest(history);
const totalScore = MilestoneHistoryUtils.getTotalScore(history);
const latestMilestone = MilestoneHistoryUtils.getLatestMilestone(history);
*/
