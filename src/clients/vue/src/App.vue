<script setup lang="ts">
import { ref, type Ref } from 'vue';
import { useAuthStore } from './stores/authStore';
import { createAnonymousUser } from './services/api/identityService';
import { chatHubService } from './services/api/chatHubService';
import Header from './components/layout/Header.vue';

chatHubService;

enum State {
  Loading,
  Error,
  Initialized,
}

const state: Ref<State> = ref(State.Loading);
const authStore = useAuthStore();

const initAuth = async () => {
  authStore.initAuth();

  if (authStore.isAuth) {
    state.value = State.Initialized;
    return;
  }

  try {
    const credentials = await createAnonymousUser();
    authStore.setAuth(credentials.accessToken, credentials.user.id);
    state.value = State.Initialized;
  } catch (err) {
    console.error('Failed to authenticate:', err);
    state.value = State.Error;
  }
};

initAuth();
</script>

<template>
  <el-container class="container">
    <el-header>
      <Header></Header>
    </el-header>
    <el-main class="main">
      <h1 v-if="state === State.Loading">loading...</h1>
      <h1 v-if="state === State.Error">Error!</h1>
      <RouterView v-if="state === State.Initialized" />
    </el-main>
  </el-container>
</template>

<style scoped>
.container {
  height: 100%;
}

.main {
  display: flex;
  flex-direction: column;
}
</style>
