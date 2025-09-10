<script setup lang="ts">
import type ChatParticipant from '@/models/ChatParticipant';
import { ref } from 'vue';
import JoinChatForm from './JoinChatForm.vue';
import { ElMessage } from 'element-plus';
import { ApiError } from '@/errors/ApiError';
import { NetworkError } from '@/errors/NetworkError';
import { useChatStore } from '@/stores/chatStore';
import type Chat from '@/models/Chat';
import { useAuthStore } from '@/stores/authStore';
import { useRouter } from 'vue-router';

type JoinChatFormExposed = {
  tryJoinChat: () => Promise<ChatParticipant | null>;
};

const chatCode = ref<string>('');
const joinChatFormRef = ref<JoinChatFormExposed | undefined>();
const isDialogVisible = ref<boolean>(false);
const formLock = ref<boolean>(false);
const chat = ref<Chat | undefined>(undefined);

const chatStore = useChatStore();
const authStore = useAuthStore();

const router = useRouter();

const handleJoinClick = async () => {
  if (!chatCode.value) {
    ElMessage({
      message: 'Please enter a code before continuing',
      type: 'warning',
      grouping: true,
      plain: true,
    });
    return;
  }

  try {
    chat.value = await chatStore.fetchChatByCode(chatCode.value);
  } catch (e) {
    if (e instanceof ApiError) {
      ElMessage({
        message: e.data.title,
        type: 'error',
        grouping: true,
        plain: true,
      });
    } else if (e instanceof NetworkError) {
      ElMessage({
        message:
          'We couldn’t connect to the server. Please check your internet connection and try again',
        type: 'error',
        grouping: true,
        plain: true,
      });
    } else {
      console.error(e);
      ElMessage({
        message: 'Something went wrong. Please try again later',
        type: 'error',
        grouping: true,
        plain: true,
      });
    }
    return;
  }

  if (chat.value.participants.some((e) => e.userId == authStore.userId)) {
    return;
  }

  isDialogVisible.value = true;
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
    router.push({ name: 'chat', params: { code: chat.value.code } });
  }
};
</script>

<template>
  <el-input v-model="chatCode" placeholder="Paste chat code to join..." class="chat-input">
    <template #append>
      <el-button @click="handleJoinClick">Join</el-button>
    </template>
  </el-input>
  <el-dialog v-model="isDialogVisible" :title="chat?.name" width="500">
    <JoinChatForm ref="joinChatFormRef" v-if="chat" :chat="chat" />
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="isDialogVisible = false">Cancel</el-button>
        <el-button type="primary" :disabled="formLock" @click="handleFormJoinClick">
          Join
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<style scoped>
.chat-input {
  max-width: 250px;
  height: 40px;
}
</style>
