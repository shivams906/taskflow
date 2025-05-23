import { defineStore } from 'pinia'
import { useWorkspaceStore } from './workspaceStore'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    username: localStorage.getItem('username') || '',

  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
  },
  actions: {
    login(token, name) {
      localStorage.setItem('token', token)
      localStorage.setItem('username', name)
      this.token = token
      this.username = name
    },
    logout() {
      const workspaceStore = useWorkspaceStore();
      workspaceStore.cleanCurrentWorkspace();
      
      localStorage.removeItem('token')
      localStorage.removeItem('username')
      this.token = ""
      this.username = ''
    },
  },
})
