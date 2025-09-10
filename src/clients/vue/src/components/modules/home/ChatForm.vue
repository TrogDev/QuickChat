<script setup lang="ts">
import { reactive, ref } from 'vue';
import { generateParticipantName } from '@/services/nameGenerator';
import type { FormInstance, FormRules } from 'element-plus';
import { createChat, joinChat } from '@/services/api/chatService';
import type Chat from '@/models/Chat';
import { useChatStore } from '@/stores/chatStore';
import type ChatParticipant from '@/models/ChatParticipant';

const formLabelWidth = '140px';

interface ChatForm {
  chatName: string;
  participantName: string;
}

const formRef = ref<FormInstance>();
const form = reactive<ChatForm>({
  chatName: 'New chat',
  participantName: generateParticipantName(),
});
const rules = reactive<FormRules<ChatForm>>({
  chatName: [{ required: true, message: 'Please input Chat name', trigger: 'blur' }],
  participantName: [{ required: true, message: 'Please input Participant name', trigger: 'blur' }],
});

const chatStore = useChatStore();

const tryCreateChat = async (): Promise<Chat | null> => {
  if (!formRef.value) return null;

  const isValid = await formRef.value.validate();

  if (!isValid) return null;

  const chat: Chat = await createChat(form.chatName);
  const participant: ChatParticipant = await joinChat(chat.id, form.participantName);
  chat.participants.push(participant);
  chatStore.addChat(chat);

  return chat;
};

defineExpose({ tryCreateChat });
</script>

<template>
  <el-form ref="formRef" :model="form" :rules="rules">
    <el-form-item label="Chat name" :label-width="formLabelWidth" prop="chatName">
      <el-input v-model="form.chatName" autocomplete="off" />
    </el-form-item>
    <el-form-item label="Your name" :label-width="formLabelWidth" prop="participantName">
      <el-input v-model="form.participantName" autocomplete="off" />
    </el-form-item>
  </el-form>
</template>
