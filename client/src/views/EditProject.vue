<template>
  <div class="p-6 max-w-xl mx-auto">
    <h2 class="text-2xl font-bold text-black mb-4">Edit Project</h2>

    <form @submit.prevent="updateProject" class="space-y-4">
      <div>
        <label class="block mb-1">Project Title</label>
        <input
          v-model="title"
          type="text"
          class="w-full px-3 py-2 border rounded"
          required
        />
      </div>

      <div>
        <label class="block mb-1">Description</label>
        <textarea
          v-model="description"
          class="w-full px-3 py-2 border rounded"
          rows="4"
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
import api from "@/api/axios";
import { useToast } from "vue-toastification";
const toast = useToast();

const route = useRoute();
const router = useRouter();
const projectId = route.params.id;

const title = ref("");
const description = ref("");
const error = ref("");

// load existing project
const fetchProject = async () => {
  try {
    const res = await api.get(`/api/projects/${projectId}`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    title.value = res.data.title;
    description.value = res.data.description;
  } catch (err) {
    console.error("Failed to load project", err);
    error.value = "Could not load project data.";
  }
};

const updateProject = async () => {
  try {
    await api.put(
      `/api/projects/${projectId}`,
      {
        title: title.value,
        description: description.value,
      },
      {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      }
    );
    toast.success("Project updated successfully!");
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

onMounted(fetchProject);
</script>
