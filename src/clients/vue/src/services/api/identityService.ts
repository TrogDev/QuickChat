import type UserCredentials from '@/models/UserCredentials';
import { apiClient } from './client';

export async function createAnonymousUser(): Promise<UserCredentials> {
  const { data } = await apiClient.post<UserCredentials>('/users');
  return data;
}
