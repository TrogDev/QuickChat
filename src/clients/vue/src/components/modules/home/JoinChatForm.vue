<script setup lang="ts">
import { reactive, ref } from 'vue';
import { generateParticipantName } from '@/services/nameGenerator';
import type { FormInstance, FormRules } from 'element-plus';
import { joinChat } from '@/services/api/chatService';
import type ChatParticipant from '@/models/ChatParticipant';
import type Chat from '@/models/Chat';

const formLabelWidth = '140px';

interface JoinChatForm {
  participantName: string;
}

interface JoinChatFormProps {
  chat: Chat;
}

const props = defineProps<JoinChatFormProps>();
const formRef = ref<FormInstance>();
const form = reactive<JoinChatForm>({
  participantName: generateParticipantName(),
});
const rules = reactive<FormRules<JoinChatForm>>({
  participantName: [{ required: true, message: 'Please input Participant name', trigger: 'blur' }],
});

const tryJoinChat = async (): Promise<ChatParticipant | null> => {
  if (!formRef.value) return null;

  const isValid = await formRef.value.validate();

  if (!isValid) return null;

  const participant: ChatParticipant = await joinChat(props.chat.id, form.participantName);
  props.chat.participants.push(participant);
  return participant;
};

defineExpose({ tryJoinChat });
</script>

<template>
  <el-form ref="formRef" :model="form" :rules="rules">
    <el-form-item label="Your name" :label-width="formLabelWidth" prop="participantName">
      <el-input v-model="form.participantName" autocomplete="off" />
    </el-form-item>
  </el-form>
</template>
