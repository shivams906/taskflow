// stores/workspaceStore.js
import { defineStore } from 'pinia';
import api from '@/api/axios';

export const useWorkspaceStore = defineStore('workspace', {
  state: () => ({
    workspaces: [],
    currentWorkspaceId: localStorage.getItem('currentWorkspaceId') || '',
  }),
  actions: {
    async fetchWorkspaces() {
      // API call or dummy data
        try {
            const res = await api.get("/api/workspaces", {
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
            });
            this.workspaces = res.data;
        } catch (err) {
            console.error("Failed to load workspaces", err);
        }

      // Initialize with first workspace if none selected
      this.currentWorkspaceId ||= this.workspaces[0]?.id || '';
    },
    setCurrentWorkspace(id) {
      if (!id) return;
      this.currentWorkspaceId = id;
      localStorage.setItem('workspaceId', id);
    },
    cleanCurrentWorkspace(){
      this.currentWorkspaceId = null;
      localStorage.removeItem('workspaceId');
    },
    async refreshAndSelectWorkspace(id) {
      await this.fetchWorkspaces();
      this.setCurrentWorkspace(id);
    },
  },
});
