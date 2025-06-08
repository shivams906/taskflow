<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-2xl mx-auto p-6">
      <div class="bg-white p-6 rounded-lg shadow-sm">
        <h2 class="text-3xl font-semibold text-gray-900 mb-6">Edit Task</h2>

        <form @submit.prevent="updateTask" class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Task Title</label
            >
            <input
              v-model="title"
              type="text"
              required
              class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Description</label
            >
            <textarea
              v-model="description"
              class="w-full px-3 py-2 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
              rows="4"
            ></textarea>
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
              Update
            </button>
          </div>
        </form>

        <p v-if="error" class="text-red-600 text-sm mt-4">{{ error }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import { updateTaskInApi, fetchTaskByIdFromApi } from "@/api/task";
const toast = useToast();

const route = useRoute();
const router = useRouter();
const projectId = route.params.id;
const taskId = route.params.taskId;

const title = ref("");
const description = ref("");
const error = ref("");

const fetchTask = async () => {
  const res = await fetchTaskByIdFromApi(taskId);
  title.value = res.title;
  description.value = res.description;
};

const updateTask = async () => {
  try {
    await updateTaskInApi(taskId, projectId, title.value, description.value);

    toast.success("Task updated successfully!");
    router.go(-1);
  } catch (err) {
    error.value = "Failed to create task";
    console.error(err);
  }
};

const cancel = () => {
  router.go(-1);
};

onMounted(() => {
  fetchTask();
});
</script>
