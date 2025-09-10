import type MessageAttachment from './MessageAttachment';

export default interface Message {
  id: number;
  chatId: string;
  userId: string;
  text: string;
  attachments: MessageAttachment[];
  createdAt: Date;
}
