<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto p-6">
      <h2 class="text-3xl font-semibold text-gray-900 mb-8">My Tasks</h2>

      <!-- Filters -->
      <div class="bg-white p-4 rounded-lg shadow-sm flex flex-wrap gap-4 mb-6">
        <div class="flex-1 min-w-[200px]">
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Project</label
          >
          <input
            v-model="filters.projectTitle"
            placeholder="Filter by project"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          />
        </div>
        <div class="flex-1 min-w-[200px]">
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Task</label
          >
          <input
            v-model="filters.title"
            placeholder="Filter by task"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          />
        </div>
        <div class="flex-1 min-w-[150px]">
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Status</label
          >
          <select
            v-model="filters.status"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          >
            <option value="">All Status</option>
            <option v-for="s in statusOptions" :key="s" :value="s">
              {{ s }}
            </option>
          </select>
        </div>
      </div>

      <!-- Tasks Table -->
      <div class="bg-white rounded-lg shadow-sm overflow-hidden">
        <table class="w-full text-sm text-gray-900">
          <thead class="bg-gray-100 text-gray-700">
            <tr>
              <th class="px-6 py-3 text-left font-medium">Project</th>
              <th class="px-6 py-3 text-left font-medium">Task</th>
              <th class="px-6 py-3 text-left font-medium">Status</th>
              <th class="px-6 py-3 text-left font-medium">Timer</th>
              <th class="px-6 py-3 text-left font-medium">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="task in filteredTasks"
              :key="task.id"
              class="border-t hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4">
                <button
                  @click="viewProject(task.projectId)"
                  class="text-blue-600 hover:underline"
                >
                  {{ task.projectTitle }}
                </button>
              </td>
              <td class="px-6 py-4">
                <button
                  @click="viewTask(task.id, task.projectId)"
                  class="text-blue-600 hover:underline"
                >
                  {{ task.title }}
                </button>
              </td>
              <td class="px-6 py-4">
                <select
                  v-permission:UpdateTaskStatus.disable="task.permissions"
                  v-model="taskStatusUpdates[task.id]"
                  @change="updateStatus(task.id)"
                  class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm"
                >
                  <option v-for="s in statusOptions" :key="s" :value="s">
                    {{ s }}
                  </option>
                </select>
              </td>
              <td class="px-6 py-4 space-x-2">
                <div
                  v-if="activeTimers[task.id]"
                  class="flex items-center space-x-2"
                >
                  <button
                    @click="endTimer(task.id)"
                    class="text-red-600 hover:underline text-sm"
                  >
                    End
                  </button>
                  <span class="text-sm text-gray-500">
                    ({{ formatElapsedTime(elapsedTimers[task.id]) }})
                  </span>
                </div>
                <button
                  v-else
                  @click="startTimer(task.id)"
                  class="text-green-600 hover:underline text-sm"
                >
                  Start
                </button>
              </td>
              <td class="px-6 py-4">
                <button
                  @click="viewLogs(task.id)"
                  class="text-blue-600 hover:underline text-sm"
                >
                  View Logs
                </button>
              </td>
            </tr>
            <tr v-if="filteredTasks.length === 0">
              <td colspan="5" class="px-6 py-4 text-center text-gray-500">
                No tasks found.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import api from "@/api/axios";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import { useWorkspaceStore } from "@/stores/workspaceStore";
import {
  AddTimeLogToTaskInApi,
  fetchTaskStatusOptionsFromApi,
  updateTaskStatusInApi,
} from "@/api/task";
const toast = useToast();
const route = useRoute();
const router = useRouter();
const tasks = ref([]);
const statusOptions = ref([]);
const filters = ref({ projectTitle: "", title: "", status: "" });

const taskStatusUpdates = ref({});
const activeTimers = ref({});
const timeLogs = ref({});
const runningTaskId = ref(null);

const store = useWorkspaceStore();

const workspaceId = route.params.workspaceId;

watch(
  () => route.params.workspaceId,
  (newId) => {
    if (newId && newId !== store.currentWorkspaceId) {
      store.setCurrentWorkspace(newId);
    }
  },
  { immediate: true }
);

const elapsedTimers = ref({});
let intervalId = null;

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

// Fetch user's assigned tasks
const fetchTasks = async () => {
  try {
    const res = await api.get(`/api/workspaces/${workspaceId}/my-tasks`, {
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
    statusOptions.value = await fetchTaskStatusOptionsFromApi();
  } catch (err) {
    console.error("Failed to load status options:", err);
  }
};

// Update task status
const updateStatus = async (taskId) => {
  const newStatus = taskStatusUpdates.value[taskId];
  try {
    await updateTaskStatusInApi(taskId, newStatus);
    toast.success("Task status updated successfully!");
    await fetchTasks();
  } catch (err) {
    console.error("Failed to update task status:", err);
  }
};

// Start timer for a task
const startTimer = (taskId) => {
  if (runningTaskId.value) {
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
    await AddTimeLogToTaskInApi(taskId, timer.startTime, endTime);
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
  router.push({ name: "myLogs", params: { workspaceId, taskId } });
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
const viewTask = (taskId, projectId) =>
  router.push({ name: "task", params: { workspaceId, projectId, taskId } });
const viewProject = (id) =>
  router.push({ name: "project", params: { workspaceId, projectId: id } });
onMounted(async () => {
  await fetchTasks();
  await fetchStatusOptions();
});
</script>
