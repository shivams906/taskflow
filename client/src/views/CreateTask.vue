<template>
  <div class="p-6 max-w-xl mx-auto">
    <h2 class="text-2xl font-bold text-black mb-4">Create Task</h2>

    <form @submit.prevent="createTask" class="space-y-4">
      <div>
        <label class="block text-black mb-1">Task Title</label>
        <input
          v-model="title"
          type="text"
          required
          class="w-full px-3 py-2 rounded border border-gray-300 text-black"
        />
      </div>

      <div>
        <label class="block text-black mb-1">Description</label>
        <textarea
          v-model="description"
          class="w-full px-3 py-2 rounded border border-gray-300 text-black"
          rows="3"
        ></textarea>
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
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import api from "@/api/axios";
import { useToast } from "vue-toastification";
const toast = useToast();

const route = useRoute();
const router = useRouter();
const projectId = route.params.id;

const title = ref("");
const description = ref("");
const error = ref("");

const createTask = async () => {
  try {
    await api.post(
      `/api/tasks`,
      {
        projectId: projectId,
        title: title.value,
        description: description.value,
      },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    toast.success("Task created successfully!");

    router.go(-1); // Go back to the previous page
  } catch (err) {
    error.value = "Failed to create task";
    console.error(err);
  }
};

const cancel = () => {
  router.go(-1); // Go back to the previous page
};
</script>
