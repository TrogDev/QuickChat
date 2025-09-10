import type Chat from '@/models/Chat';
import { apiClient } from './client';
import type ChatParticipant from '@/models/ChatParticipant';

export async function getChatByCode(code: string): Promise<Chat> {
  const { data } = await apiClient.get<Chat>(`/chats/${code}`);
  return data;
}

export async function getUserChats(): Promise<Chat[]> {
  const { data } = await apiClient.get<Chat[]>(`/chats`);
  return data;
}

export async function createChat(name: string): Promise<Chat> {
  const formData = new FormData();
  formData.append('name', name);

  const { data } = await apiClient.post<Chat>('/chats', formData);
  return data;
}

export async function joinChat(chatId: string, name: string): Promise<ChatParticipant> {
  const formData = new FormData();
  formData.append('name', name);

  const { data } = await apiClient.post<ChatParticipant>(`/chats/${chatId}/participants`, formData);
  return data;
}
