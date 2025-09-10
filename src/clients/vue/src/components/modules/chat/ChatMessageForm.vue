<script setup lang="ts">
import { addMessage } from '@/services/api/messageService';
import { ref } from 'vue';

interface ChatMessageFormProps {
  chatId: string;
}

const props = defineProps<ChatMessageFormProps>();

const text = ref<string>('');

const sendMessage = () => {
  if (!text.value.length) {
    return;
  }

  addMessage(props.chatId, text.value, []);
  text.value = '';
};
</script>

<template>
  <form class="chat-message-form" @submit.prevent="sendMessage">
    <el-input v-model="text" placeholder="Message" class="chat-input">
      <template #append>
        <el-button @click="sendMessage">
          <el-icon>
            <right />
          </el-icon>
        </el-button>
      </template>
    </el-input>
  </form>
</template>

<style scoped>
.chat-message-form {
  display: flex;
  padding: 5px;
  border-top: 1px solid var(--el-border-color);
}
</style>
