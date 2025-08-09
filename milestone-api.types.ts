/**
 * TypeScript definitions for Milestone Claim API
 */

// ============================================
// API Response Types
// ============================================

export interface ServiceResult<T = any> {
  status: boolean;
  statusMessage: string;
  data: T | null;
}

export interface ApiResponse<T = any> {
  success: boolean;
  result: T;
  message: string;
}

// ============================================
// Entity Types
// ============================================

export interface UserMilestoneClaim {
  userId: number;
  milestoneId: number;
  claimedAt: string; // ISO date string
  giftCodeId?: number;
  milestoneReward?: MilestoneReward;
}

export interface MilestoneClaimHistoryResponse {
  milestoneId: number;
  milestoneName: string;
  requiredScore: number;
  rewardPackageName: string;
  claimedAt: string; // ISO date string
  giftCode: string;
  giftCodeId?: number;
}

export interface MilestoneReward {
  id: number;
  milestoneName: string;
  requiredScore: number;
  rewardPackageId: number;
}

export interface GiftCodeItem {
  id: number;
  quantity: number;
  options: GiftCodeOption[];
}

export interface GiftCodeOption {
  id: number;
  param: number;
}

// ============================================
// API Client Types
// ============================================

export interface MilestoneApiClient {
  claimMilestone(milestoneId: number): Promise<ServiceResult<string>>;
  checkMilestoneClaimed(userId: number, milestoneId: number): Promise<ApiResponse<boolean>>;
  getUserClaimedMilestones(userId: number): Promise<ApiResponse<UserMilestoneClaim[]>>;
  getRecentClaims(limit?: number): Promise<ApiResponse<UserMilestoneClaim[]>>;
  getMyClaimHistory(): Promise<ServiceResult<MilestoneClaimHistoryResponse[]>>;
}

// ============================================
// Error Types
// ============================================

export type MilestoneErrorCode = 
  | 'NOT_ELIGIBLE'
  | 'ALREADY_CLAIMED'
  | 'NO_CHARACTER'
  | 'MILESTONE_NOT_FOUND'
  | 'GIFT_CODE_ERROR'
  | 'DATABASE_ERROR'
  | 'UNAUTHORIZED';

export interface MilestoneError extends Error {
  code: MilestoneErrorCode;
  statusMessage: string;
}

// ============================================
// API Client Implementation
// ============================================

export class MilestoneAPI implements MilestoneApiClient {
  private baseUrl: string;
  private getToken: () => string | null;

  constructor(baseUrl: string, getToken: () => string | null) {
    this.baseUrl = baseUrl;
    this.getToken = getToken;
  }

  private async makeRequest<T>(
    endpoint: string, 
    options: RequestInit = {}
  ): Promise<T> {
    const token = this.getToken();
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...options.headers,
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      ...options,
      headers,
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Claim milestone reward
   * @param milestoneId - ID of the milestone to claim
   * @returns Promise with gift code or error
   */
  async claimMilestone(milestoneId: number): Promise<ServiceResult<string>> {
    return this.makeRequest<ServiceResult<string>>(
      `/api/UserMilestoneClaim/claim/${milestoneId}`,
      { method: 'POST' }
    );
  }

  /**
   * Check if user has already claimed a milestone
   * @param userId - User ID
   * @param milestoneId - Milestone ID
   * @returns Promise with boolean result
   */
  async checkMilestoneClaimed(
    userId: number, 
    milestoneId: number
  ): Promise<ApiResponse<boolean>> {
    return this.makeRequest<ApiResponse<boolean>>(
      `/api/UserMilestoneClaim/user/${userId}/milestone/${milestoneId}/claimed`
    );
  }

  /**
   * Get all milestones claimed by a user
   * @param userId - User ID
   * @returns Promise with array of claimed milestones
   */
  async getUserClaimedMilestones(userId: number): Promise<ApiResponse<UserMilestoneClaim[]>> {
    return this.makeRequest<ApiResponse<UserMilestoneClaim[]>>(
      `/api/UserMilestoneClaim/user/${userId}`
    );
  }

  /**
   * Get recent milestone claims (admin/monitoring)
   * @param limit - Number of recent claims to return (default: 10)
   * @returns Promise with array of recent claims
   */
  async getRecentClaims(limit = 10): Promise<ApiResponse<UserMilestoneClaim[]>> {
    return this.makeRequest<ApiResponse<UserMilestoneClaim[]>>(
      `/api/UserMilestoneClaim/recent?limit=${limit}`
    );
  }

  /**
   * Get my milestone claim history
   * @returns Promise with array of claim history
   */
  async getMyClaimHistory(): Promise<ServiceResult<MilestoneClaimHistoryResponse[]>> {
    return this.makeRequest<ServiceResult<MilestoneClaimHistoryResponse[]>>(
      `/api/UserMilestoneClaim/history`
    );
  }
}

// ============================================
// Utility Functions
// ============================================

/**
 * Create milestone API client instance
 */
export function createMilestoneAPI(
  baseUrl: string, 
  getToken: () => string | null
): MilestoneAPI {
  return new MilestoneAPI(baseUrl, getToken);
}

/**
 * Parse milestone error from API response
 */
export function parseMilestoneError(response: ServiceResult): MilestoneError {
  let code: MilestoneErrorCode = 'DATABASE_ERROR';
  
  if (response.statusMessage.includes('chưa đủ điều kiện')) {
    code = 'NOT_ELIGIBLE';
  } else if (response.statusMessage.includes('đã nhận thưởng')) {
    code = 'ALREADY_CLAIMED';
  } else if (response.statusMessage.includes('chưa tạo nhân vật')) {
    code = 'NO_CHARACTER';
  } else if (response.statusMessage.includes('không tồn tại')) {
    code = 'MILESTONE_NOT_FOUND';
  } else if (response.statusMessage.includes('gift code')) {
    code = 'GIFT_CODE_ERROR';
  }

  const error = new Error(response.statusMessage) as MilestoneError;
  error.code = code;
  error.statusMessage = response.statusMessage;
  return error;
}

/**
 * Check if user is eligible for milestone
 */
export function isEligibleForMilestone(
  userScore: number, 
  requiredScore: number, 
  claimed: boolean
): boolean {
  return userScore >= requiredScore && !claimed;
}

/**
 * Format gift code for display (add spaces for readability)
 */
export function formatGiftCode(code: string): string {
  return code.replace(/(.{4})/g, '$1 ').trim();
}

/**
 * Copy gift code to clipboard
 */
export async function copyGiftCodeToClipboard(code: string): Promise<boolean> {
  try {
    await navigator.clipboard.writeText(code);
    return true;
  } catch (error) {
    console.error('Failed to copy gift code:', error);
    return false;
  }
}

// ============================================
// React Hooks (Optional)
// ============================================

/**
 * Custom hook for milestone claiming (requires React)
 */
/*
import { useState, useCallback } from 'react';

export function useMilestoneClaim(apiClient: MilestoneAPI) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<MilestoneError | null>(null);

  const claimMilestone = useCallback(async (milestoneId: number) => {
    setLoading(true);
    setError(null);

    try {
      const result = await apiClient.claimMilestone(milestoneId);
      
      if (!result.status) {
        setError(parseMilestoneError(result));
        return null;
      }
      
      return result.data;
    } catch (err) {
      const error = err as Error;
      setError({
        ...error,
        code: 'DATABASE_ERROR',
        statusMessage: error.message
      } as MilestoneError);
      return null;
    } finally {
      setLoading(false);
    }
  }, [apiClient]);

  return { claimMilestone, loading, error };
}

export function useMilestoneStatus(
  apiClient: MilestoneAPI, 
  userId: number, 
  milestoneId: number
) {
  const [claimed, setClaimed] = useState(false);
  const [loading, setLoading] = useState(true);

  const checkStatus = useCallback(async () => {
    setLoading(true);
    try {
      const result = await apiClient.checkMilestoneClaimed(userId, milestoneId);
      setClaimed(result.success ? result.result : false);
    } catch (error) {
      console.error('Failed to check milestone status:', error);
      setClaimed(false);
    } finally {
      setLoading(false);
    }
  }, [apiClient, userId, milestoneId]);

  return { claimed, loading, checkStatus, setClaimed };
}
*/

// ============================================
// Constants
// ============================================

export const MILESTONE_STATUS_MESSAGES = {
  SUCCESS: 'Nhận thưởng thành công',
  NOT_ELIGIBLE: 'Bạn chưa đủ điều kiện nhận mốc này',
  ALREADY_CLAIMED: 'Bạn đã nhận thưởng mốc này rồi',
  NO_CHARACTER: 'Bạn chưa tạo nhân vật',
  MILESTONE_NOT_FOUND: 'Mốc thưởng không tồn tại',
  GIFT_CODE_ERROR: 'Không thể tạo gift code',
  DATABASE_ERROR: 'Lỗi database'
} as const;
