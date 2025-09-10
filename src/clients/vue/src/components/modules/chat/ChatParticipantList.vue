<script setup lang="ts">
import type ChatParticipant from '@/models/ChatParticipant';
import type Chat from '@/models/Chat';
import ChatParticipantAvatar from './ChatParticipantAvatar.vue';
import { onBeforeUnmount } from 'vue';
import { chatHubService } from '@/services/api/chatHubService';

interface ChatParticipantListProps {
  chat: Chat;
}

const props = defineProps<ChatParticipantListProps>();

let handlerId: number = chatHubService.addHandler(
  'UserJoined',
  (chatId: string, chatParticipant: ChatParticipant) => {
    if (chatId !== props.chat.id) {
      return;
    }
    if (props.chat.participants.some((p) => p.id === chatParticipant.id)) {
      return;
    }

    props.chat.participants.push(chatParticipant);
  },
);

onBeforeUnmount(() => {
  chatHubService.removeHandler('UserJoined', handlerId);
});
</script>

<template>
  <el-scrollbar class="list">
    <div
      v-for="(participant, i) in props.chat.participants"
      :key="participant.id"
      class="participant"
    >
      <el-text class="name">{{ participant.name }}</el-text>
      <ChatParticipantAvatar :index="i" />
    </div>
  </el-scrollbar>
</template>

<style scoped>
.list {
  display: flex;
  flex-direction: column;
  padding: 15px;
  border-left: 1px solid var(--el-border-color);
  flex-shrink: 0;
}

.participant {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 10px;
  padding: 5px 0;
  border-top: 1px solid var(--el-border-color);
}

.participant:nth-child(1) {
  padding-top: 0;
  border-top: none;
}

.name {
  font-size: 14px;
  font-weight: 600;
}
</style>
