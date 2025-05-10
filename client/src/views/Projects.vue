<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-2xl font-bold text-black">Projects</h2>
      <router-link
        to="/admin/projects/create"
        class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
      >
        + Create Project
      </router-link>
    </div>

    <!-- Filters -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <input
        v-model="filters.name"
        placeholder="Filter by project name"
        class="px-3 py-2 rounded border text-black w-full"
      />
      <select
        v-model="filters.createdBy"
        class="px-3 py-2 rounded border text-black w-full"
      >
        <option value="">All Creators</option>
        <option v-for="user in creatorOptions" :key="user" :value="user">
          {{ user }}
        </option>
      </select>
      <input
        v-model="filters.createdFrom"
        type="date"
        class="px-3 py-2 rounded border text-black w-full"
      />
      <input
        v-model="filters.createdTo"
        type="date"
        class="px-3 py-2 rounded border text-black w-full"
      />
    </div>

    <!-- Projects Table -->
    <table class="w-full bg-white rounded shadow text-black">
      <thead class="bg-gray-200">
        <tr>
          <th class="px-4 py-2">Name</th>
          <th class="px-4 py-2">Created By</th>
          <th class="px-4 py-2">Created At</th>
          <th class="px-4 py-2">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="project in filteredProjects"
          :key="project.id"
          class="border-t hover:bg-gray-100"
        >
          <td class="px-4 py-2 text-center">{{ project.title }}</td>
          <td class="px-4 py-2 text-center">{{ project.createdBy }}</td>
          <td class="px-4 py-2 text-center">
            {{ formatDate(project.createdAtUtc) }}
          </td>
          <td class="px-4 py-2 space-x-2 text-center">
            <button
              @click="viewTasks(project.id)"
              class="text-sm text-blue-600 hover:underline"
            >
              View
            </button>
            <button
              @click="editProject(project.id)"
              class="text-sm text-green-600 hover:underline"
            >
              Edit
            </button>
            <button
              @click="deleteProject(project.id)"
              class="text-sm text-red-600 hover:underline"
            >
              Delete
            </button>
          </td>
        </tr>
        <tr v-if="filteredProjects.length === 0">
          <td colspan="5" class="px-4 py-4 text-center text-gray-500">
            No projects found.
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import api from "@/api/axios";
import { useRouter } from "vue-router";
import { useToast } from "vue-toastification";
const toast = useToast();

const router = useRouter();
const projects = ref([]);
const filters = ref({
  name: "",
  createdBy: "",
  createdFrom: "",
  createdTo: "",
});

const fetchProjects = async () => {
  try {
    const res = await api.get("/api/projects", {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    projects.value = res.data;
  } catch (err) {
    console.error("Failed to load projects", err);
  }
};

// distinct creators for filter dropdown
const creatorOptions = computed(() => {
  const names = projects.value.map((p) => p.createdBy);
  return [...new Set(names)];
});

const filteredProjects = computed(() => {
  return projects.value.filter((p) => {
    const matchesName = p.title
      .toLowerCase()
      .includes(filters.value.name.toLowerCase());
    const matchesCreator =
      !filters.value.createdBy || p.createdBy === filters.value.createdBy;
    const fromOk =
      !filters.value.createdFrom ||
      new Date(p.createdAtUtc) >= new Date(filters.value.createdFrom);
    const toOk =
      !filters.value.createdTo ||
      new Date(p.createdAtUtc) <=
        new Date(filters.value.createdTo + "T23:59:59");
    return matchesName && matchesCreator && fromOk && toOk;
  });
});

const viewTasks = (id) => router.push(`/admin/projects/${id}`);
const editProject = (id) => router.push(`/admin/projects/${id}/edit`);
const deleteProject = async (id) => {
  if (!confirm("Delete this project?")) return;
  await api.delete(`/api/projects/${id}`, {
    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
  });
  toast.success("Project deleted successfully!");
  fetchProjects();
};

const formatDate = (dt) =>
  new Date(`${dt}Z`).toLocaleString("en-IN", {
    dateStyle: "medium",
    timeStyle: "short",
  });

onMounted(fetchProjects);
</script>
