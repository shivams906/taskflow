<template>
  <div class="p-6 max-w-xl mx-auto">
    <h2 class="text-2xl font-bold text-black mb-4">Create New Project</h2>

    <form @submit.prevent="createProject" class="space-y-4">
      <div>
        <label class="block text-black mb-1">Project Title</label>
        <input
          v-model="title"
          type="text"
          class="w-full px-3 py-2 rounded border border-gray-300 text-black"
          required
        />
      </div>

      <div>
        <label class="block text-black mb-1">Description (optional)</label>
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
import { ref } from "vue";
import api from "@/api/axios";
import { useRouter } from "vue-router";
import { useToast } from "vue-toastification";
const toast = useToast();

const title = ref("");
const description = ref("");
const error = ref("");
const router = useRouter();

const createProject = async () => {
  try {
    const res = await api.post(
      "/api/projects",
      {
        title: title.value,
        description: description.value,
      },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    const projectId = res.data.id;
    toast.success("Project created successfully!");
    router.go(-1); // Go back to the previous page
  } catch (err) {
    error.value = "Failed to create project";
    console.error(err);
  }
};

const cancel = () => {
  router.go(-1); // Go back to the previous page
};
</script>
