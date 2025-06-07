<template>
  <h1 class="text-2xl font-bold mb-4">Task: {{ task.title }}</h1>
  <div class="flex justify-between items-start mb-6">
    <p class="text-gray-300 mt-1">
      {{ task.description || "No description" }}
    </p>
    <div class="flex space-x-2">
      <router-link
        v-permission:ManageTask="task.permissions"
        :to="{ name: 'editTask', params: { workspaceId, projectId, taskId } }"
        class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
        >Edit</router-link
      >
      <button
        v-permission:DeleteTask="task.permissions"
        @click="deleteTask"
        class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
      >
        Delete
      </button>
    </div>
  </div>
  <!-- Task Info Section -->
  <div class="bg-white p-4 rounded-lg shadow mb-6 text-black">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <!-- Status -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Status</label
        >
        <select
          v-permission:UpdateTaskStatus.disable="task.permissions"
          v-model="task.status"
          @change="updateTaskStatus(task.id)"
          class="border border-gray-300 rounded-md px-3 py-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option v-for="s in statusOptions" :key="s" :value="s">
            {{ s }}
          </option>
        </select>
      </div>

      <!-- Assigned To -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Assigned To</label
        >
        <select
          v-permission:ManageTask.disable="task.permissions"
          v-model="task.assignedToId"
          @change="assignTask(task.id)"
          class="border border-gray-300 rounded-md px-3 py-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option v-for="u in users" :key="u.userId" :value="u.userId">
            {{ u.username }}
          </option>
        </select>
      </div>
    </div>
  </div>

  <TabGroup :selectedIndex="selectedTab" @change="changeTab">
    <TabList class="flex space-x-4">
      <Tab v-slot="{ selected }">
        <span
          :class="
            selected
              ? 'border-b-2 border-blue-600 text-blue-600'
              : 'text-gray-600'
          "
        >
          Discussions
        </span>
      </Tab>

      <Tab v-slot="{ selected }">
        <span
          :class="
            selected
              ? 'border-b-2 border-blue-600 text-blue-600'
              : 'text-gray-600'
          "
        >
          Logs
        </span>
      </Tab>
      <Tab v-slot="{ selected }">
        <span
          :class="
            selected
              ? 'border-b-2 border-blue-600 text-blue-600'
              : 'text-gray-600'
          "
        >
          About
        </span>
      </Tab>
      <Tab v-slot="{ selected }">
        <span
          :class="
            selected
              ? 'border-b-2 border-blue-600 text-blue-600'
              : 'text-gray-600'
          "
        >
          History
        </span>
      </Tab>
    </TabList>

    <TabPanels class="mt-4">
      <TabPanel> </TabPanel>
      <TabPanel>
        <div class="p-6 mx-auto text-black">
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
        </div></TabPanel
      >
      <TabPanel>
        <div class="bg-white p-6 rounded shadow space-y-4 text-gray-800 w-full">
          <InfoRow
            label="Created By"
            :value="task.createdByUsername || 'N/A'"
          />
          <InfoRow label="Created On" :value="formatDate(task.createdAtUtc)" />
          <InfoRow
            label="Last Updated By"
            :value="task.updatedByUsername || 'N/A'"
          />
          <InfoRow
            label="Last Updated On"
            :value="task.updatedAtUtc ? formatDate(task.updatedAtUtc) : 'N/A'"
          />
        </div>
      </TabPanel>
      <TabPanel
        ><ul>
          <li v-for="item in history" :key="item.timestamp">
            {{ formatDate(item.timestamp) }} - {{ item.changeSummary }} by
            {{ item.changedByUserName }}
          </li>
        </ul>
      </TabPanel>
    </TabPanels>
  </TabGroup>
</template>
<script setup>
import { TabGroup, TabList, Tab, TabPanels, TabPanel } from "@headlessui/vue";
import { ref, computed, onMounted, watch, watchEffect } from "vue";
import api from "@/api/axios";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import {
  fetchTimeLogsByTaskFromApi,
  updateTaskStatusInApi,
  assignTaskToUserInApi,
  unassignTaskFromUserInApi,
  deleteTaskFromApi,
  fetchTaskStatusOptionsFromApi,
  fetchTaskByIdFromApi,
} from "@/api/task";
import { fetchProjectUsersFromApi } from "@/api/project";
import { fetchChangeLogsFromApi } from "@/api/changeLog";
import { formatDate } from "@/utils/date";
import InfoRow from "@/components/common/InfoRow.vue";
const toast = useToast();

const route = useRoute();
const router = useRouter();
const workspaceId = route.params.workspaceId;
const projectId = route.params.projectId;
const taskId = route.params.taskId;
const statusOptions = ref([]);

// --- fetch the single task ---
const task = ref({
  id: taskId,
  projectId: "",
  title: "",
  description: "",
  status: "",
  assignedToId: "",
});

const fetchStatusOptions = async () => {
  statusOptions.value = await fetchTaskStatusOptionsFromApi();
};
const fetchTask = async () => {
  task.value = await fetchTaskByIdFromApi(taskId);
};
const history = ref([]);
const logs = ref([]);
const users = ref([]);
const filters = ref({
  userId: "",
  startDate: "",
  endDate: "",
});
const showUserFilter = ref(false);

const TABS = {
  DISCUSSIONS: "discussions",
  LOGS: "logs",
  ABOUT: "about",
  HISTORY: "history",
};
const tabNames = Object.values(TABS);

const selectedTab = ref(0);

watchEffect(() => {
  const tabParam = route.query.tab;
  const index = tabNames.indexOf(tabParam);
  selectedTab.value = index !== -1 ? index : 0;
});

watch(selectedTab, (newIndex) => {
  router.push({
    query: {
      ...route.query,
      tab: tabNames[newIndex],
    },
  });
});

function changeTab(index) {
  selectedTab.value = index;
}

const fetchLogs = async () => {
  try {
    logs.value = await fetchTimeLogsByTaskFromApi(taskId);

    // Determine if we have multiple users
    const uniqueUsers = [...new Set(logs.value.map((log) => log.username))];
    showUserFilter.value = uniqueUsers.length > 1;
  } catch (err) {
    console.error("Failed to load logs", err);
  }
};

const fetchUsers = async () => {
  try {
    users.value = await fetchProjectUsersFromApi(projectId);
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
  await deleteTaskFromApi(taskId);
  toast.success("Task deleted successfully!");
  // after delete, go back to project detail
  router.push(`/admin/projects/${projectId}`);
};

const updateTaskStatus = async (taskId) => {
  const newStatus = task.value.status;
  await updateTaskStatusInApi(taskId, newStatus);
  toast.success("Task status updated successfully!");
};

const assignTask = async (taskId) => {
  const userId = task.value.assignedToId;
  if (userId) {
    await assignTaskToUserInApi(taskId, userId);
    toast.success("Task assigned successfully!");
  } else {
    await unassignTaskFromUserInApi(taskId);
    toast.success("Task unassigned successfully!");
  }
};

const loadHistory = async () => {
  history.value = await fetchChangeLogsFromApi("task", taskId);
};

onMounted(() => {
  fetchStatusOptions();
  fetchTask();
  fetchLogs();
  fetchUsers();
  loadHistory();

  if (!route.query.tab) {
    router.replace({
      query: { ...route.query, tab: tabNames[0] },
    });
  }
});
</script>
