<script lang="ts" setup>
import type Chat from '@/models/Chat';
import { useAuthStore } from '@/stores/authStore';
import { useChatStore } from '@/stores/chatStore';
import { computed, reactive } from 'vue';
import { useRouter } from 'vue-router';

interface ChatRow {
  joinedAt: string;
  code: string;
  name: string;
  participantsCount: number;
}

const chats = reactive<Chat[]>([]);
const chatRows = computed<ChatRow[]>(() =>
  chats.map((c) => {
    return {
      joinedAt: getJoinedAt(c),
      code: c.code,
      name: c.name,
      participantsCount: c.participants.length,
    };
  }),
);

const chatStore = useChatStore();
const authStore = useAuthStore();

const router = useRouter();

const getJoinedAt: (chat: Chat) => string = (chat: Chat) => {
  const participant = chat.participants.find((p) => p.userId == authStore.userId)!;
  return new Date(participant.joinedAt).toLocaleString();
};

const fetchChats = async () => {
  chats.push(...(await chatStore.fetchUserChats()));
};

const openChat = (chat: Chat) => {
  router.push({ name: 'chat', params: { code: chat.code } });
};

fetchChats();
</script>

<template>
  <el-table
    :data="chatRows"
    height="250"
    class="chats-table"
    v-if="chatRows.length"
    @row-click="openChat"
    row-class-name="chats-table__row"
  >
    <el-table-column label="Previous chats" align="center">
      <el-table-column label="Joined At" width="200" align="center">
        <template #default="scope">
          <div style="display: inline-flex; align-items: center">
            <el-icon>
              <timer />
            </el-icon>
            <el-text style="margin-left: 10px">{{ scope.row.joinedAt }}</el-text>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="code" label="Code" width="130" align="center" />
      <el-table-column prop="name" label="Name" align="center" />
      <el-table-column label="Participants" width="120" align="center">
        <template #default="scope">
          <div style="display: inline-flex; align-items: center">
            <el-icon>
              <user />
            </el-icon>
            <el-text style="margin-left: 10px">{{ scope.row.participantsCount }}</el-text>
          </div>
        </template>
      </el-table-column>
    </el-table-column>
  </el-table>
</template>

<style scoped>
.chats-table {
  width: 100%;
}
</style>

<style>
.chats-table__row {
  cursor: pointer;
}
</style>
