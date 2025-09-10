<script setup lang="ts">
import ChatHeader from '@/components/modules/chat/ChatHeader.vue';
import ChatMessageList from '@/components/modules/chat/ChatMessageList.vue';
import ChatParticipantList from '@/components/modules/chat/ChatParticipantList.vue';
import JoinChatForm from '@/components/modules/home/JoinChatForm.vue';
import type Chat from '@/models/Chat';
import type ChatParticipant from '@/models/ChatParticipant';
import { chatHubService } from '@/services/api/chatHubService';
import { useAuthStore } from '@/stores/authStore';
import { useChatStore } from '@/stores/chatStore';
import { onBeforeUnmount, onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';

type JoinChatFormExposed = {
  tryJoinChat: () => Promise<ChatParticipant | null>;
};

const route = useRoute();
const chatStore = useChatStore();
const authStore = useAuthStore();

const chat = reactive<{ value: Chat | undefined }>({ value: undefined });
const isDialogVisible = ref<boolean>(false);
const joinChatFormRef = ref<JoinChatFormExposed | undefined>();
const formLock = ref<boolean>(false);

const initChat = async () => {
  chat.value = await chatStore.fetchChatByCode(route.params.code.toString());
  chatHubService.subscribe(chat.value.id);

  if (!chat.value.participants.some((p) => p.userId === authStore.userId)) {
    isDialogVisible.value = true;
  }
};

const handleFormJoinClick = async () => {
  formLock.value = true;

  let result;

  try {
    result = await joinChatFormRef.value?.tryJoinChat();
  } finally {
    formLock.value = false;
  }

  if (result && chat.value) {
    isDialogVisible.value = false;
  }
};

onBeforeUnmount(() => {
  if (!chat.value) {
    return;
  }

  chatHubService.unsubscribe(chat.value.id);
});

initChat();
</script>

<template>
  <div class="chat" v-loading="!chat.value">
    <el-dialog
      v-model="isDialogVisible"
      :title="chat.value?.name"
      width="500"
      :close-on-click-modal="false"
      :close-on-press-escape="false"
      :show-close="false"
    >
      <JoinChatForm ref="joinChatFormRef" v-if="chat.value" :chat="chat.value" />
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" :disabled="formLock" @click="handleFormJoinClick">
            Join
          </el-button>
        </div>
      </template>
    </el-dialog>
    <div class="chat-body">
      <ChatHeader v-if="chat.value" :chat="chat.value" />
      <ChatMessageList v-if="chat.value" :chat="chat.value" />
      <ChatMessageForm v-if="chat.value" :chat-id="chat.value.id" />
    </div>
    <ChatParticipantList v-if="chat.value" :chat="chat.value" />
  </div>
</template>

<style scoped>
.chat {
  display: flex;
  width: 100%;
  height: 100%;
  flex-grow: 1;
  border-radius: 10px;
  border: 1px solid var(--el-border-color);
  overflow: hidden;
}

.chat-body {
  width: 100%;
  display: flex;
  flex-direction: column;
}

.participants {
  background: red;
}
</style>
