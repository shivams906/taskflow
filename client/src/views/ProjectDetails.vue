<template>
  <div class="p-6">
    <!-- Header: Project Info and Actions -->
    <div class="flex justify-between items-start mb-6">
      <div>
        <h2 class="text-3xl font-bold text-black">{{ project.title }}</h2>
        <p class="text-gray-300 mt-1">
          {{ project.description || "No description" }}
        </p>
      </div>
      <div class="flex space-x-2">
        <router-link
          :to="`/admin/projects/${project.id}/edit`"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
        >
          Edit
        </router-link>
        <button
          @click="confirmDelete"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
        >
          Delete
        </button>
      </div>
    </div>

    <!-- Tasks Section -->
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-2xl font-bold text-black">Tasks</h3>
      <router-link
        :to="`/admin/projects/${project.id}/create-task`"
        class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
      >
        + Create Task
      </router-link>
    </div>

    <!-- Task Filters -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <input
        v-model="filters.title"
        placeholder="Filter by title"
        class="px-3 py-2 rounded border text-black w-full"
      />
      <select
        v-model="filters.status"
        class="px-3 py-2 rounded border text-black w-full"
      >
        <option value="">All Status</option>
        <option v-for="s in statusOptions" :key="s" :value="s">{{ s }}</option>
      </select>
      <select
        v-model="filters.assignedTo"
        class="px-3 py-2 rounded border text-black w-full"
      >
        <option value="">All Users</option>
        <option v-for="u in users" :key="u.id" :value="u.id">
          {{ u.username }}
        </option>
      </select>
      <input
        v-model="filters.createdFrom"
        type="date"
        class="px-3 py-2 rounded border text-black w-full"
      />
    </div>

    <!-- Tasks Table -->
    <table class="w-full bg-white rounded shadow text-black">
      <thead class="bg-gray-200">
        <tr>
          <th class="px-4 py-2">Title</th>
          <th class="px-4 py-2">Status</th>
          <th class="px-4 py-2">Assigned To</th>
          <th class="px-4 py-2">Created By</th>
          <th class="px-4 py-2">Created At</th>
          <th class="px-4 py-2">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="task in filteredTasks"
          :key="task.id"
          class="border-t hover:bg-gray-100"
        >
          <td class="px-4 py-2 text-center">{{ task.title }}</td>
          <td class="px-4 py-2">
            <select
              v-model="taskStatusUpdates[task.id]"
              @change="updateTaskStatus(task.id)"
              class="border rounded px-2 py-1 text-black w-full"
            >
              <option v-for="s in statusOptions" :key="s" :value="s">
                {{ s }}
              </option>
            </select>
          </td>
          <td class="px-4 py-2">
            <select
              v-model="taskAssignments[task.id]"
              @change="assignTask(task.id)"
              class="border rounded px-2 py-1 text-black w-full"
            >
              <option :value="null">-- Unassigned --</option>
              <option v-for="u in users" :key="u.id" :value="u.id">
                {{ u.username }}
              </option>
            </select>
          </td>
          <td class="px-4 py-2 text-center">
            {{ task.createdBy }}
          </td>
          <td class="px-4 py-2 text-center">
            {{ formatDate(task.createdAtUtc) }}
          </td>

          <td class="px-4 py-2 space-x-2 text-center">
            <button
              @click="viewTask(task.id)"
              class="text-blue-600 hover:underline text-sm"
            >
              View
            </button>
            <button
              @click="editTask(task.id)"
              class="text-green-600 hover:underline text-sm"
            >
              Edit
            </button>
            <button
              @click="deleteTask(task.id)"
              class="text-red-600 hover:underline text-sm"
            >
              Delete
            </button>
          </td>
        </tr>
        <tr v-if="filteredTasks.length === 0">
          <td colspan="5" class="px-4 py-4 text-center text-gray-500">
            No tasks found.
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import api from "@/api/axios";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
const toast = useToast();

const route = useRoute();
const router = useRouter();
const projectId = route.params.id;

const project = ref({
  id: projectId,
  title: "",
  description: "",
  createdAtUtc: "",
});
const users = ref([]);
const tasks = ref([]);
const statusOptions = ref([]);
const filters = ref({ title: "", status: "", assignedTo: "", createdFrom: "" });

const taskStatusUpdates = ref({});
const taskAssignments = ref({});

const fetchProject = async () => {
  const res = await api.get(`/api/projects/${projectId}`, {
    headers: authHeader(),
  });
  project.value = res.data;
};

const fetchUsers = async () => {
  const res = await api.get("/api/users", {
    headers: authHeader(),
  });
  users.value = res.data;
};

const fetchTasks = async () => {
  const res = await api.get(`/api/tasks/project/${projectId}`, {
    headers: authHeader(),
  });
  tasks.value = res.data;
  tasks.value.forEach((t) => {
    taskStatusUpdates.value[t.id] = t.status;
    taskAssignments.value[t.id] = t.assignedToId;
  });
};

const fetchStatusOptions = async () => {
  const res = await api.get("/api/tasks/statuses", {
    headers: authHeader(),
  });
  statusOptions.value = res.data;
};

const filteredTasks = computed(() => {
  return tasks.value.filter((t) => {
    const mTitle = t.title
      .toLowerCase()
      .includes(filters.value.title.toLowerCase());
    const mStatus = !filters.value.status || t.status === filters.value.status;
    const mUser =
      !filters.value.assignedTo || t.assignedToId === filters.value.assignedTo;
    const mDate =
      !filters.value.createdFrom ||
      new Date(t.createdAtUtc) >= new Date(filters.value.createdFrom);
    return mTitle && mStatus && mUser && mDate;
  });
});

const updateTaskStatus = async (taskId) => {
  const newStatus = taskStatusUpdates.value[taskId];
  await api.put(
    `/api/tasks/${taskId}/status`,
    { newStatus },
    { headers: authHeader() }
  );
  toast.success("Task status updated successfully!");
};

const assignTask = async (taskId) => {
  const userId = taskAssignments.value[taskId];
  if (userId) {
    await api.put(
      `/api/tasks/${taskId}/assign/`,
      { userId },
      { headers: authHeader() }
    );
    toast.success("Task assigned successfully!");
  } else {
    await api.put(
      `/api/tasks/${taskId}/unassign`,
      {},
      { headers: authHeader() }
    );
    toast.success("Task unassigned successfully!");
  }
};

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

const viewTask = (taskId) =>
  router.push(`/admin/projects/${projectId}/tasks/${taskId}`);
const editTask = (taskId) =>
  router.push(`/admin/projects/${projectId}/tasks/${taskId}/edit`);
const deleteTask = async (taskId) => {
  if (!confirm("Delete this task?")) return;
  await api.delete(`/api/tasks/${taskId}`, {
    headers: authHeader(),
  });
  toast.success("Task deleted successfully!");
  fetchTasks();
};

const confirmDelete = () => {
  if (confirm("Delete this project?")) {
    api
      .delete(`/api/projects/${projectId}`, {
        headers: authHeader(),
      })
      .then(() => router.push("/admin/projects"));
    toast.success("Project deleted successfully!");
  }
};

const formatDate = (dt) =>
  new Date(`${dt}Z`).toLocaleString("en-IN", {
    dateStyle: "medium",
    timeStyle: "short",
  });

onMounted(async () => {
  await Promise.all([
    fetchProject(),
    fetchUsers(),
    fetchTasks(),
    fetchStatusOptions(),
  ]);
});
</script>
