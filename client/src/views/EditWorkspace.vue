<template>
  <div class="p-6 max-w-xl mx-auto">
    <h2 class="text-2xl font-bold text-black mb-4">Edit Workspace</h2>

    <form @submit.prevent="updateWorkspace" class="space-y-4">
      <div>
        <label class="block mb-1">Workspace Name</label>
        <input
          v-model="name"
          type="text"
          class="w-full px-3 py-2 border rounded"
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
          Update
        </button>
      </div>
    </form>

    <p v-if="error" class="text-red-400 mt-4">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import {
  fetchWorkspaceByIdFromApi,
  sendUpdatedWorkspaceDataToApi,
} from "@/api/workspace";
const toast = useToast();

const route = useRoute();
const router = useRouter();
const workspaceId = route.params.workspaceId;

const name = ref("");
const error = ref("");

// load existing workspace
const fetchWorkspace = async () => {
  try {
    const data = await fetchWorkspaceByIdFromApi(workspaceId);
    name.value = data.name;
  } catch (err) {
    console.error("Failed to load workspace", err);
    error.value = "Could not load workspace data.";
  }
};

const updateWorkspace = async () => {
  try {
    await sendUpdatedWorkspaceDataToApi(workspaceId, name.value);
    toast.success("Workspace updated successfully!");
    router.go(-1); // Go back to the previous page
  } catch (err) {
    console.error("Update failed", err);
    error.value = "Failed to save changes.";
  }
};

const cancel = () => {
  //   router.push("/admin/projects");
  router.go(-1); // Go back to the previous page
};

onMounted(fetchWorkspace);
</script>
