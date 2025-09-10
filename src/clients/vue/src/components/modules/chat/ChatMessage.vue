<script setup lang="ts">
import type ChatParticipant from '@/models/ChatParticipant';
import type Message from '@/models/Message';
import { useAuthStore } from '@/stores/authStore';
import { computed } from 'vue';

interface ChatMessageProps {
  message: Message;
  participants: ChatParticipant[];
}

const props = defineProps<ChatMessageProps>();
const authStore = useAuthStore();

const isAuthor = computed<boolean>(() => props.message.userId === authStore.userId);
const author = computed<ChatParticipant>(
  () => props.participants.find((p) => p.userId === props.message.userId)!,
);
const authorIndex = computed<number>(() =>
  props.participants.findIndex((p) => p.userId === props.message.userId),
);
</script>

<template>
  <div class="message" :class="{ 'is-author': isAuthor }">
    <ChatParticipantAvatar :index="authorIndex" />
    <div>
      <el-text class="message__author-name">{{ author.name }}</el-text>
      <div class="message__body">
        <el-text class="message__text">{{ props.message.text }}</el-text>
      </div>
    </div>
  </div>
</template>

<style scoped>
.message {
  display: flex;
  align-items: flex-end;
  gap: 5px;
  max-width: 650px;
}

.message.is-author {
  align-self: flex-end;
  flex-direction: row-reverse;
}

.message__author-name {
  display: block;
}

.message.is-author .message__author-name {
  text-align: right;
}

.message__body {
  padding: 5px 8px;
  border-radius: 3px;
  background-color: var(--el-fill-color);
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 3px;
  margin-top: 5px;
}

.message.is-author .message__body {
  background-color: var(--el-color-primary);
}

.message__text {
  align-self: flex-start !important;
}

.message.is-author .message__text {
  color: var(--el-bg-color) !important;
  align-self: flex-end !important;
}
</style>
