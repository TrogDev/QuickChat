<script setup lang="ts">
import CreateChatButton from '@/components/modules/home/CreateChatButton.vue';
import JoinChatButton from '@/components/modules/home/JoinChatButton.vue';
import UserChatsTable from '@/components/modules/home/UserChatsTable.vue';
</script>

<template>
  <div class="hero">
    <el-text class="title">Instant Chats. No Registration.</el-text>
    <el-text class="description"
      >Quickly share text or files between your devices or with anyone else using a simple
      link.</el-text
    >
    <div class="actions">
      <CreateChatButton />
      <JoinChatButton />
    </div>
    <UserChatsTable />
  </div>
</template>

<style scoped>
.hero {
  text-align: center;
  margin-top: 60px;
  max-width: 900px;
  margin-left: auto;
  margin-right: auto;
}

.title {
  font-size: 28px;
  margin-bottom: 10px;
  font-weight: 600;
  display: block;
}

.description {
  font-size: 16px;
  margin-bottom: 30px;
  display: block;
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 16px;
  align-items: center;
  margin-bottom: 30px;
}
</style>
