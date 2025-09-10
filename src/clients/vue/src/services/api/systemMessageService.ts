import type SystemMessage from '@/models/SystemMessage';
import { apiClient } from './client';

export async function getChatSystemMessages(chatId: string): Promise<SystemMessage[]> {
  const { data } = await apiClient.get<SystemMessage[]>(`/chats/${chatId}/system-messages`);
  return data;
}
