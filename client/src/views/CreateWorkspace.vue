<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-2xl mx-auto p-6">
      <div class="bg-white p-6 rounded-lg shadow-sm">
        <h2 class="text-3xl font-semibold text-gray-900 mb-6">
          Create New Workspace
        </h2>

        <form @submit.prevent="createWorkspace" class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Workspace Name</label
            >
            <input
              v-model="name"
              type="text"
              required
              class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
            />
          </div>

          <div class="flex justify-end space-x-3">
            <button
              type="button"
              @click="cancel"
              class="px-4 py-2 bg-gray-100 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-200 transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
            >
              Create
            </button>
          </div>
        </form>

        <p v-if="error" class="text-red-600 text-sm mt-4">{{ error }}</p>
      </div>
    </div>
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
    await store.refreshAndSelectWorkspace(workspaceId);
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
  router.go(-1);
};
</script>
