<template>
  <div class="p-6 max-w-xl mx-auto">
    <h2 class="text-2xl font-bold text-black mb-4">Create New Workspace</h2>

    <form @submit.prevent="createWorkspace" class="space-y-4">
      <div>
        <label class="block text-black mb-1">Workspace Name</label>
        <input
          v-model="name"
          type="text"
          class="w-full px-3 py-2 rounded border border-gray-300 text-black"
          required
        />
      </div>

      <div class="flex justify-end space-x-2">
        <button
          type="button"
          @click="cancel"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-200 transition"
        >
          Cancel
        </button>
        <button
          type="submit"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-200 transition"
        >
          Create
        </button>
      </div>
    </form>

    <p v-if="error" class="text-red-400 mt-4">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import { useWorkspaceStore } from "@/stores/workspaceStore";
import { sendWorkspaceDataToApi } from "@/api/workspace";
const toast = useToast();

const name = ref("");
const error = ref("");
const route = useRoute();
const router = useRouter();

const store = useWorkspaceStore();

const createWorkspace = async () => {
  try {
    const data = await sendWorkspaceDataToApi(name.value);

    const workspaceId = data.id;
    toast.success("Workspace created successfully!");
    await store.refreshAndSelectWorkspace(workspaceId); // Refresh the workspace store
    router.push({
      name: "workspace",
      params: { workspaceId },
    });
  } catch (err) {
    error.value = "Failed to create workspace.";
    console.error(err);
  }
};

const cancel = () => {
  router.go(-1); // Go back to the previous page
};
</script>
