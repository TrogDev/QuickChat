import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    userId: null as string | null,
    token: null as string | null,
    isAuth: false as boolean,
  }),
  actions: {
    initAuth() {
      this.token = localStorage.token || null;
      this.userId = localStorage.userId || null;
      this.isAuth = this.token !== null;
    },
    setAuth(token: string, userId: string) {
      this.token = token;
      this.userId = userId;
      this.isAuth = true;
      localStorage.token = token;
      localStorage.userId = userId;
    },
  },
});
