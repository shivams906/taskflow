<template>
  <div class="max-w-md mx-auto mt-10 p-4 border rounded shadow">
    <h2 class="text-xl font-bold mb-4">Join a Workspace</h2>
    <input
      v-model="inviteCode"
      placeholder="Enter invite code"
      class="w-full border px-3 py-2 rounded mb-4"
    />
    <button
      @click="joinWorkspace"
      class="bg-green-600 text-white px-4 py-2 rounded"
    >
      Join
    </button>
  </div>
</template>

<script setup>
import { ref } from "vue";
import api from "@/api/axios";
import { useRouter } from "vue-router";
import { useWorkspaceStore } from "../stores/workspaceStore";

const inviteCode = ref("");
const router = useRouter();
const workspaceStore = useWorkspaceStore();

const joinWorkspace = async () => {
  try {
    const res = await api.post(
      "/api/workspaces/join",
      { inviteCode: inviteCode.value },
      {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      }
    );
    alert("Joined workspace successfully!");
    const workspaceId = res.data.id;
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
