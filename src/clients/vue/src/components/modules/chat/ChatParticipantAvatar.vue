<script setup lang="ts">
import { getParticipantAvatarColor } from '@/services/avatarColorService';

interface ChatParticipantAvatarProps {
  index: number;
}

const props = defineProps<ChatParticipantAvatarProps>();
</script>

<template>
  <div class="avatar" :style="{ backgroundColor: getParticipantAvatarColor(index) }"></div>
</template>

<style scoped>
.avatar {
  flex-shrink: 0;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  position: relative;
  overflow: hidden;
}

.avatar::before {
  content: '';
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  background-color: #0003;
  width: 15px;
  height: 15px;
  border-radius: 50%;
  top: 10px;
}

.avatar::after {
  content: '';
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  background-color: #0003;
  width: 25px;
  height: 20px;
  border-radius: 50%;
  top: 25px;
}
</style>
