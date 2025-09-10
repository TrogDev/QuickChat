import type ChatParticipant from './ChatParticipant';

export default interface Chat {
  id: string;
  name: string;
  code: string;
  participants: ChatParticipant[];
  createdAt: string;
  lifeTimeSeconds: number;
}
