<template>
  <router-link to="/" class="app-header">
    <el-text class="app-header__logo-text">Quick</el-text>
    <img src="../../assets/logo.svg" alt="Logo" class="app-header__logo" />
    <el-text class="app-header__logo-text" type="primary">Chat</el-text>
  </router-link>
</template>

<style scoped>
.app-header {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 8px 0;
  gap: 12px;
  text-decoration: none;
}

.app-header__logo {
  height: 41px;
}

.app-header__logo-text {
  font-size: 24px;
  font-weight: 600;
}
</style>
