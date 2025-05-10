<template>
  <div class="p-6">
    <h2 class="text-2xl font-bold text-black mb-6">My Tasks</h2>

    <!-- Filters -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-3 mb-3">
      <input
        v-model="filters.projectTitle"
        placeholder="Filter by project"
        class="px-3 py-2 rounded border text-black"
      />
      <input
        v-model="filters.title"
        placeholder="Filter by task"
        class="px-3 py-2 rounded border text-black"
      />
      <select
        v-model="filters.status"
        class="px-3 py-2 rounded border text-black"
      >
        <option value="">All Status</option>
        <option v-for="s in statusOptions" :key="s" :value="s">{{ s }}</option>
      </select>
      <!-- <input
        v-model="filters.startDate"
        type="date"
        class="px-3 py-2 rounded border text-black"
      />
      <input
        v-model="filters.endDate"
        type="date"
        class="px-3 py-2 rounded border text-black"
      /> -->
    </div>

    <!-- Tasks Table -->
    <table class="w-full bg-white rounded shadow text-black">
      <thead class="bg-gray-200">
        <tr>
          <th class="px-4 py-2">Project</th>
          <th class="px-4 py-2">Task</th>
          <th class="px-4 py-2">Status</th>
          <th class="px-4 py-2">Timer</th>
          <th class="px-4 py-2">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="task in filteredTasks"
          :key="task.id"
          class="border-t hover:bg-gray-100"
        >
          <td class="px-4 py-2 text-center">{{ task.projectTitle }}</td>
          <td class="px-4 py-2 text-center">{{ task.title }}</td>
          <td class="px-4 py-2 text-center">
            <select
              v-model="taskStatusUpdates[task.id]"
              @change="updateStatus(task.id)"
              class="border rounded px-2 py-1 text-black"
            >
              <option v-for="s in statusOptions" :key="s" :value="s">
                {{ s }}
              </option>
            </select>
          </td>
          <td class="px-4 py-2 space-x-2 text-center">
            <div v-if="activeTimers[task.id]">
              <button
                @click="endTimer(task.id)"
                class="text-red-600 hover:underline"
              >
                End
              </button>
              <span class="ml-2 text-sm text-gray-400">
                ({{ formatElapsedTime(elapsedTimers[task.id]) }})
              </span>
            </div>
            <button
              v-else
              @click="startTimer(task.id)"
              class="text-green-600 hover:underline"
            >
              Start
            </button>
          </td>
          <td class="px-4 py-2 text-center">
            <button
              @click="viewLogs(task.id)"
              class="text-blue-600 hover:underline text-sm"
            >
              View Logs
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <div
      v-if="filteredTasks.length === 0"
      class="text-center text-gray-400 mt-8"
    >
      No tasks found.
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import api from "@/api/axios";
import { useRouter } from "vue-router";
import { useToast } from "vue-toastification";

const toast = useToast();

const router = useRouter();
const tasks = ref([]);
const statusOptions = ref([]);
const filters = ref({ projectTitle: "", title: "", status: "" });

const taskStatusUpdates = ref({});
const activeTimers = ref({});
const timeLogs = ref({});
const runningTaskId = ref(null);

const elapsedTimers = ref({});
let intervalId = null;

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

// Fetch user's assigned tasks
const fetchTasks = async () => {
  try {
    const res = await api.get("/api/tasks/my", {
      headers: authHeader(),
    });
    tasks.value = res.data;
    tasks.value.forEach((task) => {
      taskStatusUpdates.value[task.id] = task.status;
    });
  } catch (err) {
    console.error("Failed to load tasks:", err);
  }
};

// Fetch available task statuses
const fetchStatusOptions = async () => {
  try {
    const res = await api.get("/api/tasks/statuses", {
      headers: authHeader(),
    });
    statusOptions.value = res.data;
  } catch (err) {
    console.error("Failed to load status options:", err);
  }
};

// Update task status
const updateStatus = async (taskId) => {
  const newStatus = taskStatusUpdates.value[taskId];
  try {
    await api.put(
      `/api/tasks/${taskId}/status`,
      { newStatus },
      { headers: authHeader() }
    );
    toast.success("Task status updated successfully!");
    await fetchTasks();
  } catch (err) {
    console.error("Failed to update task status:", err);
  }
};

// Start timer for a task
const startTimer = (taskId) => {
  if (runningTaskId.value) {
    // endTimer(runningTaskId.value); // Stop any running timer
    return toast.error("A timer is already running for another task.");
  }
  runningTaskId.value = taskId;
  const now = new Date();
  activeTimers.value[taskId] = {
    startTime: now.toISOString(),
  };
  elapsedTimers.value[taskId] = 0;
  if (intervalId) clearInterval(intervalId);
  intervalId = setInterval(() => {
    if (activeTimers.value[taskId]) {
      const startedAt = new Date(activeTimers.value[taskId].startTime);
      const now = new Date();
      elapsedTimers.value[taskId] = Math.floor((now - startedAt) / 1000);
    }
  }, 1000);
  toast.success("Timer started successfully!");
};

// Stop timer and log time
const endTimer = async (taskId) => {
  if (runningTaskId.value !== taskId) return;
  runningTaskId.value = null;
  const timer = activeTimers.value[taskId];
  if (!timer) return;

  const endTime = new Date().toISOString();

  try {
    await api.post(
      `/api/tasks/${taskId}/log-time`,
      {
        startTime: timer.startTime,
        endTime,
      },
      { headers: authHeader() }
    );
    delete activeTimers.value[taskId];
    delete elapsedTimers.value[taskId];

    if (intervalId) {
      clearInterval(intervalId);
      intervalId = null;
    }
    toast.success("Time logged successfully!");
    await fetchTasks();
  } catch (err) {
    console.error("Failed to log time:", err);
  }
};

// View logs page
const viewLogs = (taskId) => {
  router.push(`/my-tasks/${taskId}/logs`);
};

// Computed filtered tasks
const filteredTasks = computed(() => {
  return tasks.value.filter((task) => {
    const projectTitleMatch = task.projectTitle
      .toLowerCase()
      .includes(filters.value.projectTitle.toLowerCase());
    const titleMatch = task.title
      .toLowerCase()
      .includes(filters.value.title.toLowerCase());
    const statusMatch =
      !filters.value.status || task.status === filters.value.status;
    return projectTitleMatch && titleMatch && statusMatch;
  });
});
const formatElapsedTime = (seconds) => {
  const minutes = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${minutes}m ${secs}s`;
};
onMounted(async () => {
  await fetchTasks();
  await fetchStatusOptions();
});
</script>
