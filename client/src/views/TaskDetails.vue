<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto p-6">
      <!-- Header -->
      <div
        class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8"
      >
        <div>
          <h1 class="text-3xl font-semibold text-gray-900">
            Task: {{ task.title }}
          </h1>
          <p class="mt-2 text-gray-600 text-sm">
            {{ task.description || "No description available" }}
          </p>
        </div>
        <div class="mt-4 sm:mt-0 flex space-x-3">
          <router-link
            v-permission:ManageTask="task.permissions"
            :to="{
              name: 'editTask',
              params: { workspaceId, projectId, taskId },
            }"
            class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
          >
            Edit
          </router-link>
          <button
            v-permission:DeleteTask="task.permissions"
            @click="deleteTask"
            class="inline-flex items-center px-4 py-2 bg-red-600 text-white text-sm font-medium rounded-lg hover:bg-red-700 transition-colors"
          >
            Delete
          </button>
        </div>
      </div>

      <!-- Task Info Section -->
      <div class="bg-white p-6 rounded-lg shadow-sm mb-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Status</label
            >
            <select
              v-permission:UpdateTaskStatus.disable="task.permissions"
              v-model="task.status"
              @change="updateTaskStatus(task.id)"
              class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
            >
              <option v-for="s in statusOptions" :key="s" :value="s">
                {{ s }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Assigned To</label
            >
            <select
              v-permission:ManageTask.disable="task.permissions"
              v-model="task.assignedToId"
              @change="assignTask(task.id)"
              class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
            >
              <option v-for="u in users" :key="u.userId" :value="u.userId">
                {{ u.username }}
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <TabGroup :selectedIndex="selectedTab" @change="changeTab">
        <TabList class="flex space-x-1 bg-white p-1 rounded-lg shadow-sm mb-6">
          <Tab
            v-for="tab in ['Discussions', 'Logs', 'About', 'History']"
            :key="tab"
            v-slot="{ selected }"
          >
            <span
              :class="[
                'px-4 py-2 text-sm font-medium rounded-md transition-colors',
                selected
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-600 hover:bg-gray-100',
              ]"
            >
              {{ tab }}
            </span>
          </Tab>
        </TabList>

        <TabPanels>
          <TabPanel>
            <div class="bg-white p-6 rounded-lg shadow-sm">
              <p class="text-gray-500 italic">
                Discussions content coming soon...
              </p>
            </div>
          </TabPanel>
          <TabPanel>
            <!-- Logs Section -->
            <div class="space-y-6">
              <div
                class="bg-white p-4 rounded-lg shadow-sm flex flex-wrap gap-4"
              >
                <div v-if="showUserFilter" class="flex-1 min-w-[150px]">
                  <label class="block text-sm font-medium text-gray-700 mb-1"
                    >User</label
                  >
                  <select
                    v-model="filters.userId"
                    class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                  >
                    <option value="">All Users</option>
                    <option
                      v-for="user in users"
                      :key="user.id"
                      :value="user.id"
                    >
                      {{ user.username }}
                    </option>
                  </select>
                </div>
                <div class="flex-1 min-w-[200px]">
                  <label class="block text-sm font-medium text-gray-700 mb-1"
                    >Start Date</label
                  >
                  <input
                    v-model="filters.startDate"
                    type="date"
                    class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                  />
                </div>
                <div class="flex-1 min-w-[200px]">
                  <label class="block text-sm font-medium text-gray-700 mb-1"
                    >End Date</label
                  >
                  <input
                    v-model="filters.endDate"
                    type="date"
                    class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                  />
                </div>
              </div>
              <div class="bg-white rounded-lg shadow-sm overflow-hidden">
                <table class="w-full text-sm text-gray-900">
                  <thead class="bg-gray-100 text-gray-700">
                    <tr>
                      <th class="px-6 py-3 text-left font-medium">User</th>
                      <th class="px-6 py-3 text-left font-medium">
                        Start Time
                      </th>
                      <th class="px-6 py-3 text-left font-medium">End Time</th>
                      <th class="px-6 py-3 text-left font-medium">
                        Duration (min)
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="log in filteredLogs"
                      :key="log.id"
                      class="border-t hover:bg-gray-50 transition-colors"
                    >
                      <td class="px-6 py-4">{{ log.username }}</td>
                      <td class="px-6 py-4">{{ formatDate(log.startTime) }}</td>
                      <td class="px-6 py-4">{{ formatDate(log.endTime) }}</td>
                      <td class="px-6 py-4">
                        {{ calculateMinutes(log.startTime, log.endTime) }}
                      </td>
                    </tr>
                    <tr v-if="filteredLogs.length === 0">
                      <td
                        colspan="4"
                        class="px-6 py-4 text-center text-gray-500"
                      >
                        No logs found.
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div
                  class="px-6 py-4 bg-gray-50 border-t text-sm text-gray-900 font-semibold text-right"
                >
                  Total Time: {{ totalTime }} minutes
                </div>
              </div>
            </div>
          </TabPanel>
          <TabPanel>
            <div class="bg-white p-6 rounded-lg shadow-sm space-y-4">
              <InfoRow
                label="Created By"
                :value="task.createdByUsername || 'N/A'"
              />
              <InfoRow
                label="Created On"
                :value="formatDate(task.createdAtUtc)"
              />
              <InfoRow
                label="Last Updated By"
                :value="task.updatedByUsername || 'N/A'"
              />
              <InfoRow
                label="Last Updated On"
                :value="
                  task.updatedAtUtc ? formatDate(task.updatedAtUtc) : 'N/A'
                "
              />
            </div>
          </TabPanel>
          <TabPanel>
            <div class="bg-white p-6 rounded-lg shadow-sm">
              <ul class="space-y-2">
                <li
                  v-for="item in history"
                  :key="item.timestamp"
                  class="text-sm text-gray-700"
                >
                  {{ formatDate(item.timestamp) }} - {{ item.changeSummary }} by
                  {{ item.changedByUserName }}
                </li>
                <li v-if="!history.length" class="text-gray-500 italic">
                  No history available.
                </li>
              </ul>
            </div>
          </TabPanel>
        </TabPanels>
      </TabGroup>
    </div>
  </div>
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
