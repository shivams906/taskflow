<template>
  <div class="p-6 max-w-4xl mx-auto text-black">
    <!-- Task Header -->
    <div class="flex justify-between items-start mb-6">
      <div>
        <h2 class="text-3xl font-bold">{{ task.title }}</h2>
        <p class="text-gray-300">{{ task.description || "No description" }}</p>
      </div>
      <div class="flex space-x-2">
        <router-link
          :to="`/admin/projects/${projectId}/tasks/${task.id}/edit`"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
          >Edit</router-link
        >
        <button
          @click="deleteTask"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
        >
          Delete
        </button>
      </div>
    </div>

    <h2 class="text-2xl font-bold mb-4">Logs:</h2>

    <!-- Filters -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
      <select
        v-if="showUserFilter"
        v-model="filters.userId"
        class="px-3 py-2 rounded border text-black"
      >
        <option value="">All Users</option>
        <option v-for="user in users" :key="user.id" :value="user.id">
          {{ user.username }}
        </option>
      </select>

      <input
        v-model="filters.startDate"
        type="date"
        class="px-3 py-2 rounded border text-black"
      />
      <input
        v-model="filters.endDate"
        type="date"
        class="px-3 py-2 rounded border text-black"
      />
    </div>

    <!-- Logs Table -->
    <table
      class="w-full text-left bg-white rounded overflow-hidden shadow text-black"
    >
      <thead class="bg-gray-200">
        <tr>
          <th class="px-4 py-2">User</th>
          <th class="px-4 py-2">Start Time</th>
          <th class="px-4 py-2">End Time</th>
          <th class="px-4 py-2">Duration (min)</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="log in filteredLogs"
          :key="log.id"
          class="border-t hover:bg-gray-100"
        >
          <td class="px-4 py-2">{{ log.username }}</td>
          <td class="px-4 py-2">{{ formatDate(log.startTime) }}</td>
          <td class="px-4 py-2">{{ formatDate(log.endTime) }}</td>
          <td class="px-4 py-2">
            {{ calculateMinutes(log.startTime, log.endTime) }}
          </td>
        </tr>

        <tr v-if="filteredLogs.length === 0">
          <td colspan="4" class="px-4 py-4 text-center text-gray-500">
            No logs found.
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Total -->
    <div class="mt-4 text-right font-semibold text-black">
      Total Time: {{ totalTime }} minutes
    </div>
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
const taskId = route.params.taskId;

// --- fetch the single task ---
const task = ref({ id: taskId, projectId: "", title: "", description: "" });
const fetchTask = async () => {
  const res = await api.get(`/api/tasks/${taskId}`, {
    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
  });
  task.value = res.data;
};

const logs = ref([]);
const users = ref([]);
const filters = ref({
  userId: "",
  startDate: "",
  endDate: "",
});
const showUserFilter = ref(false);

const fetchLogs = async () => {
  try {
    const res = await api.get(`/api/tasks/${taskId}/logs`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    logs.value = res.data;

    // Determine if we have multiple users
    const uniqueUsers = [...new Set(res.data.map((log) => log.username))];
    showUserFilter.value = uniqueUsers.length > 1;
  } catch (err) {
    console.error("Failed to load logs", err);
  }
};

const fetchUsers = async () => {
  try {
    const res = await api.get("/api/users", {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    users.value = res.data;
  } catch (err) {
    console.error("Failed to load users", err);
  }
};

const filteredLogs = computed(() => {
  return logs.value.filter((log) => {
    const start = new Date(log.startTime);
    const end = new Date(log.endTime);
    console.log(filters.value.userId);
    const userMatch =
      !filters.value.userId || log.userId === filters.value.userId;
    const startMatch =
      !filters.value.startDate || start >= new Date(filters.value.startDate);
    const endMatch =
      !filters.value.endDate ||
      end <= new Date(filters.value.endDate + "T23:59:59");

    return userMatch && startMatch && endMatch;
  });
});

const formatDate = (dt) =>
  new Date(`${dt}Z`).toLocaleString("en-IN", {
    dateStyle: "medium",
    timeStyle: "short",
  });

const calculateMinutes = (start, end) => {
  return Math.round((new Date(end) - new Date(start)) / (1000 * 60));
};

const totalTime = computed(() => {
  return filteredLogs.value.reduce((sum, log) => {
    return sum + calculateMinutes(log.startTime, log.endTime);
  }, 0);
});

const deleteTask = async () => {
  if (!confirm("Delete this task?")) return;
  await api.delete(`/api/tasks/${taskId}`, {
    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
  });
  toast.success("Task deleted successfully!");
  // after delete, go back to project detail
  router.push(`/admin/projects/${projectId}`);
};

onMounted(() => {
  fetchTask();
  fetchLogs();
  fetchUsers();
});
</script>
