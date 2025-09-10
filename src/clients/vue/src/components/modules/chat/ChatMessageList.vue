<script setup lang="ts">
import type Message from '@/models/Message';
import { nextTick, onBeforeUnmount, onMounted, reactive, ref } from 'vue';
import ChatMessage from './ChatMessage.vue';
import { getChatMessages } from '@/services/api/messageService';
import type Chat from '@/models/Chat';
import { chatHubService } from '@/services/api/chatHubService';
import type { ScrollbarInstance } from 'element-plus';

interface ChatMessageListProps {
  chat: Chat;
}

const props = defineProps<ChatMessageListProps>();

const messages = reactive<Message[]>([]);
const innerRef = ref<HTMLDivElement>();
const scrollbarRef = ref<ScrollbarInstance>();
const loadLock = ref<boolean>(false);

const initMessages = async () => {
  const response = await getChatMessages(props.chat.id);
  messages.push(...response);
  await nextTick();
  scrollToBottom();
};

const scrollToBottom = () => {
  scrollbarRef.value!.setScrollTop(innerRef.value!.clientHeight);
};

const onEndReached = async (direction: string) => {
  if (direction !== 'top' || loadLock.value) {
    return;
  }

  loadLock.value = true;

  let response: Message[];

  try {
    response = await getChatMessages(props.chat.id, messages[messages.length - 1].id);
  } finally {
    loadLock.value = false;
  }

  messages.push(...response);

  const prevHeight = innerRef.value!.clientHeight;
  await nextTick();
  scrollbarRef.value!.setScrollTop(innerRef.value!.clientHeight - prevHeight);
};

let handlerId: number = chatHubService.addHandler('MessageAdded', async (message: Message) => {
  if (message.chatId !== props.chat.id) {
    return;
  }

  messages.unshift(message);

  await nextTick();
  scrollToBottom();
});

onBeforeUnmount(() => {
  chatHubService.removeHandler('MessageAdded', handlerId);
});

initMessages();
</script>

<template>
  <el-scrollbar ref="scrollbarRef" @end-reached="onEndReached">
    <div class="chat-messages" ref="innerRef">
      <ChatMessage
        v-for="message in messages"
        :message="message"
        :participants="chat.participants"
        :key="message.id"
      />
    </div>
  </el-scrollbar>
</template>

<style scoped>
.chat-messages {
  display: flex;
  flex-direction: column-reverse;
  justify-content: flex-start;
  height: 100%;
  padding: 15px;
  gap: 12px;
}

.error {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
