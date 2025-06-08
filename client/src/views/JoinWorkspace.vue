<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-md mx-auto p-6">
      <div class="bg-white p-6 rounded-lg shadow-sm">
        <h2 class="text-3xl font-semibold text-gray-900 mb-6">
          Join a Workspace
        </h2>
        <input
          v-model="inviteCode"
          placeholder="Enter invite code"
          class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900 mb-4"
        />
        <div class="flex justify-end">
          <button
            @click="joinWorkspace"
            class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
          >
            Join
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useWorkspaceStore } from "../stores/workspaceStore";
import { sendWorkspaceJoiningRequestToApi } from "../api/workspace";

const inviteCode = ref("");
const router = useRouter();
const workspaceStore = useWorkspaceStore();

const joinWorkspace = async () => {
  try {
    const data = await sendWorkspaceJoiningRequestToApi(inviteCode.value);
    alert("Joined workspace successfully!");
    const workspaceId = data.id;
    await workspaceStore.refreshAndSelectWorkspace(workspaceId);
    router.push({
      name: "workspace",
      params: { workspaceId },
    });
  } catch (err) {
    console.error("Join failed", err);
    alert("Invalid invite code or already a member.");
  }
};
</script>
