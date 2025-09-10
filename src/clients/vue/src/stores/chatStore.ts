import { defineStore } from 'pinia';
import { getChatByCode, getUserChats } from '@/services/api/chatService';
import type Chat from '@/models/Chat';

export const useChatStore = defineStore('user', {
  state: () => ({
    chats: {} as Record<string, Chat>,
  }),
  actions: {
    async fetchChatByCode(code: string): Promise<Chat> {
      if (this.chats[code]) return this.chats[code];

      const chat = await getChatByCode(code);
      this.chats[chat.code] = chat;
      return chat;
    },
    async fetchUserChats(): Promise<Chat[]> {
      const chats = await getUserChats();

      for (let chat of chats) {
        this.chats[chat.code] = chat;
      }

      return chats;
    },
    addChat(chat: Chat): void {
      this.chats[chat.code] = chat;
    },
  },
});
