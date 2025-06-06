// stores/workspaceStore.js
import { defineStore } from "pinia";
import { fetchWorkspacesFromApi } from "@/api/workspace";

export const useWorkspaceStore = defineStore("workspace", {
  state: () => ({
    workspaces: [],
    currentWorkspaceId: localStorage.getItem("currentWorkspaceId") || "",
  }),
  actions: {
    async fetchWorkspaces() {
      try {
        this.workspaces = await fetchWorkspacesFromApi();
      } catch (err) {
        console.error("Failed to load workspaces", err);
      }

      // Initialize with first workspace if none selected
      this.currentWorkspaceId ||= this.workspaces[0]?.id || "";
    },
    setCurrentWorkspace(id) {
      if (!id) return;
      this.currentWorkspaceId = id;
      localStorage.setItem("workspaceId", id);
    },
    cleanCurrentWorkspace() {
      this.currentWorkspaceId = null;
      localStorage.removeItem("workspaceId");
    },
    async refreshAndSelectWorkspace(id) {
      await this.fetchWorkspaces();
      this.setCurrentWorkspace(id);
    },
  },
});
