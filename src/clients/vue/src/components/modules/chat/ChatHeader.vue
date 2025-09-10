<script setup lang="ts">
import type Chat from '@/models/Chat';
import { CopyDocument } from '@element-plus/icons-vue';
import { ElMessage } from 'element-plus';

interface ChatHeaderProps {
  chat: Chat;
}

const props = defineProps<ChatHeaderProps>();

const copyLink = () => {
  const link = location.origin + '/chats/' + props.chat.code;
  navigator.clipboard.writeText(link).then(() =>
    ElMessage({
      message: 'Link copied to clipboard!',
      grouping: true,
      plain: true,
    }),
  );
};
</script>

<template>
  <div class="chat-header">
    <el-text class="chat-header__name">{{ props.chat.name }}</el-text>
    <el-text class="chat-header__code">
      <b>Code</b>: {{ props.chat.code }}
      <el-button :icon="CopyDocument" circle @click="copyLink" />
    </el-text>
  </div>
</template>

<style scoped>
.chat-header {
  border-bottom: 1px solid var(--el-border-color);
  padding: 15px;
  display: flex;
  justify-content: space-between;
  gap: 15px;
  align-items: flex-end;
}

.chat-header__name {
  font-size: 22px;
  font-weight: 600;
}
</style>
