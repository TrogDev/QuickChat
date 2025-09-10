<script setup lang="ts">
import { ref } from 'vue';
import ChatForm from './ChatForm.vue';
import type Chat from '@/models/Chat';
import { useRouter } from 'vue-router';

type ChatFormExposed = {
  tryCreateChat: () => Promise<Chat | null>;
};

const isDialogVisible = ref<boolean>(false);
const formLock = ref<boolean>(false);
const chatFormRef = ref<ChatFormExposed | undefined>();

const router = useRouter();

const createAndOpenChat = async () => {
  formLock.value = true;

  let result;

  try {
    result = await chatFormRef.value?.tryCreateChat();
  } finally {
    formLock.value = false;
  }

  if (result) {
    router.push({ name: 'chat', params: { code: result.code } });
  }
};
</script>

<template>
  <el-button type="primary" size="large" @click="isDialogVisible = true">Create New Chat</el-button>
  <el-dialog v-model="isDialogVisible" title="Create chat" width="500">
    <ChatForm ref="chatFormRef" />
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="isDialogVisible = false">Cancel</el-button>
        <el-button type="primary" :disabled="formLock" @click="createAndOpenChat">
          Create
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>
