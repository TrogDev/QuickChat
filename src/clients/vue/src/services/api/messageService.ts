import type Message from '@/models/Message';
import { apiClient } from './client';

export async function getChatMessages(
  chatId: string,
  cursor: number | undefined = undefined,
  limit: number = 50,
): Promise<Message[]> {
  const params = new URLSearchParams();

  if (cursor) {
    params.append('cursor', cursor.toString());
  }
  if (limit) {
    params.append('limit', limit.toString());
  }

  const { data } = await apiClient.get<Message[]>(`/chats/${chatId}/messages?${params}`);
  return data;
}

export async function addMessage(
  chatId: string,
  text: string,
  attachmentIds: string[],
): Promise<void> {
  await apiClient.post<Message[]>(`/chats/${chatId}/messages`, {
    text,
    attachmentIds,
  });
}

export async function editMessage(
  chatId: string,
  messageId: number,
  text: string,
  attachmentIds: string[],
): Promise<void> {
  await apiClient.put<Message[]>(`/chats/${chatId}/messages/${messageId}`, {
    text,
    attachmentIds,
  });
}

export async function deleteMessage(chatId: string, messageId: number): Promise<void> {
  await apiClient.get<Message[]>(`/chats/${chatId}/messages/${messageId}`);
}
