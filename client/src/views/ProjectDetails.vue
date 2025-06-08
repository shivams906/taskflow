<template>
  <div class="max-w-7xl mx-auto p-6 bg-gray-50 min-h-screen">
    <!-- Project Header -->
    <div
      class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8"
    >
      <div>
        <h1 class="text-3xl font-semibold text-gray-900">
          {{ project.title }}
        </h1>
        <p class="mt-2 text-gray-600 text-sm">
          {{ project.description || "No description available" }}
        </p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <router-link
          v-permission:ManageProject="project.permissions"
          :to="{
            name: 'editProject',
            params: { workspaceId: workspaceId, projectId: project.id },
          }"
          class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
        >
          Edit
        </router-link>
        <button
          v-permission:DeleteProject="project.permissions"
          @click="confirmDelete"
          class="inline-flex items-center px-4 py-2 bg-red-600 text-white text-sm font-medium rounded-lg hover:bg-red-700 transition-colors"
        >
          Delete
        </button>
      </div>
    </div>

    <!-- Tabs -->
    <TabGroup :selectedIndex="selectedTab" @change="changeTab">
      <TabList class="flex space-x-1 bg-white p-1 rounded-lg shadow-sm mb-6">
        <Tab
          v-for="tab in [
            'Dashboard',
            'Tasks',
            'Discussions',
            'Members',
            'About',
            'History',
          ]"
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
            <!-- Placeholder for Dashboard content -->
            <p class="text-gray-500 italic">Dashboard content coming soon...</p>
          </div>
        </TabPanel>
        <TabPanel>
          <div class="space-y-6">
            <!-- Create Task Button -->
            <div
              v-permission:ManageProject="project.permissions"
              class="flex justify-end"
            >
              <router-link
                :to="{
                  name: 'createTask',
                  params: { workspaceId: workspaceId, projectId: project.id },
                }"
                class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                + Create Task
              </router-link>
            </div>

            <!-- Task Filters -->
            <div
              class="bg-white p-4 rounded-lg shadow-sm flex flex-wrap gap-4 items-end"
            >
              <!-- Title Filter -->
              <div class="flex-1 min-w-[200px]">
                <label class="block text-sm font-medium text-gray-700 mb-1"
                  >Title</label
                >
                <input
                  v-model="filters.title"
                  placeholder="Filter by title"
                  class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                />
              </div>

              <!-- Status Filter -->
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

              <!-- Assigned To Filter -->
              <div class="flex-1 min-w-[150px]">
                <label class="block text-sm font-medium text-gray-700 mb-1"
                  >Assigned To</label
                >
                <select
                  v-model="filters.assignedTo"
                  class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                >
                  <option value="">All Members</option>
                  <option
                    v-for="m in members"
                    :key="m.userId"
                    :value="m.userId"
                  >
                    {{ m.username }}
                  </option>
                </select>
              </div>

              <!-- Created By Filter -->
              <div class="flex-1 min-w-[150px]">
                <label class="block text-sm font-medium text-gray-700 mb-1"
                  >Created By</label
                >
                <select
                  v-model="filters.createdBy"
                  class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                >
                  <option value="">All Members</option>
                  <option
                    v-for="m in members"
                    :key="m.userId"
                    :value="m.userId"
                  >
                    {{ m.username }}
                  </option>
                </select>
              </div>

              <!-- Sort Controls -->
              <div class="flex-1 min-w-[150px]">
                <label class="block text-sm font-medium text-gray-700 mb-1"
                  >Sort By</label
                >
                <div class="flex items-center gap-2">
                  <select
                    v-model="sortBy"
                    class="h-9 flex-1 px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                  >
                    <option value="CreatedAtUtc">Created At</option>
                    <option value="Title">Title</option>
                    <option value="UpdatedAtUtc">Last Updated</option>
                  </select>
                  <button
                    @click="sortDesc = !sortDesc"
                    class="h-9 w-9 flex items-center justify-center rounded-md border border-gray-300 hover:bg-gray-100 focus:outline-none focus:ring-1 focus:ring-blue-500"
                    aria-label="Toggle sort direction"
                  >
                    <span class="text-sm text-gray-600">{{
                      sortDesc ? "↓" : "↑"
                    }}</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Tasks Table -->
            <div class="bg-white rounded-lg shadow-sm overflow-hidden">
              <table class="w-full text-sm text-gray-900">
                <thead class="bg-gray-100 text-gray-700">
                  <tr>
                    <th class="px-6 py-3 text-left font-medium">Title</th>
                    <th class="px-6 py-3 text-left font-medium">Status</th>
                    <th class="px-6 py-3 text-left font-medium">Assigned To</th>
                    <th class="px-6 py-3 text-left font-medium">Created At</th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="task in tasks"
                    :key="task.id"
                    class="border-t hover:bg-gray-50 transition-colors"
                  >
                    <td class="px-6 py-4">
                      <button
                        @click="viewTask(task.id)"
                        class="text-blue-600 hover:underline"
                      >
                        {{ task.title }}
                      </button>
                    </td>
                    <td class="px-6 py-4">
                      <select
                        v-permission:UpdateTaskStatus.disable="task.permissions"
                        v-model="taskStatusUpdates[task.id]"
                        @change="updateTaskStatus(task.id)"
                        class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm"
                      >
                        <option v-for="s in statusOptions" :key="s" :value="s">
                          {{ s }}
                        </option>
                      </select>
                    </td>
                    <td class="px-6 py-4">
                      <select
                        v-permission:ManageTask.disable="task.permissions"
                        v-model="taskAssignments[task.id]"
                        @change="assignTask(task.id)"
                        class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm"
                      >
                        <option :value="null">-- Unassigned --</option>
                        <option
                          v-for="m in members"
                          :key="m.userId"
                          :value="m.userId"
                        >
                          {{ m.username }}
                        </option>
                      </select>
                    </td>
                    <td class="px-6 py-4">
                      {{ formatDate(task.createdAtUtc) }}
                    </td>
                  </tr>
                  <tr v-if="tasks.length === 0">
                    <td colspan="4" class="px-6 py-4 text-center text-gray-500">
                      No tasks found.
                    </td>
                  </tr>
                </tbody>
              </table>
              <!-- Pagination -->
              <div
                class="flex justify-between items-center px-6 py-4 bg-gray-50 border-t"
              >
                <button
                  @click="currentPage--"
                  :disabled="currentPage === 1"
                  class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 transition-colors"
                >
                  Previous
                </button>
                <div class="space-x-2">
                  <button
                    v-for="page in totalPages"
                    :key="page"
                    @click="currentPage = page"
                    :class="[
                      'px-3 py-1 text-sm rounded-lg',
                      page === currentPage
                        ? 'bg-blue-600 text-white'
                        : 'border border-gray-300 hover:bg-gray-100',
                    ]"
                  >
                    {{ page }}
                  </button>
                </div>
                <button
                  @click="currentPage++"
                  :disabled="currentPage === totalPages"
                  class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 transition-colors"
                >
                  Next
                </button>
              </div>
            </div>
          </div>
        </TabPanel>
        <TabPanel>
          <div class="bg-white p-6 rounded-lg shadow-sm">
            <!-- Placeholder for Discussions content -->
            <p class="text-gray-500 italic">
              Discussions content coming soon...
            </p>
          </div>
        </TabPanel>
        <TabPanel>
          <div class="space-y-6">
            <div
              v-permission:ManageProject="project.permissions"
              class="bg-white p-6 rounded-lg shadow-sm"
            >
              <h3 class="text-lg font-semibold text-gray-900 mb-4">
                Add Member to Project
              </h3>
              <div class="flex flex-col sm:flex-row gap-4">
                <select
                  v-model="selectedUserId"
                  class="h-9 flex-1 px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
                >
                  <option value="" disabled>Select a user</option>
                  <option
                    v-for="user in availableUsers"
                    :key="user.userId"
                    :value="user.userId"
                  >
                    {{ user.username }}
                  </option>
                </select>
                <button
                  @click="addUserToProject"
                  class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Add Member
                </button>
              </div>
            </div>
            <MemberList
              title="Project Members"
              :members="members"
              class="bg-white p-6 rounded-lg shadow-sm"
            />
          </div>
        </TabPanel>
        <TabPanel>
          <div class="bg-white p-6 rounded-lg shadow-sm space-y-4">
            <InfoRow
              label="Created By"
              :value="project.createdByUsername || 'N/A'"
            />
            <InfoRow
              label="Created On"
              :value="formatDate(project.createdAtUtc)"
            />
            <InfoRow
              label="Last Updated By"
              :value="project.updatedByUsername || 'N/A'"
            />
            <InfoRow
              label="Last Updated On"
              :value="
                project.updatedAtUtc ? formatDate(project.updatedAtUtc) : 'N/A'
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
</template>

<script setup>
import { TabGroup, TabList, Tab, TabPanels, TabPanel } from "@headlessui/vue";
import { ref, computed, onMounted, watch, watchEffect, reactive } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import MemberList from "@/components/common/MemberList.vue";
import InfoRow from "@/components/common/InfoRow.vue";
import { fetchWorkspaceUsersFromApi } from "@/api/workspace";
import { fetchChangeLogsFromApi } from "@/api/changeLog";
import {
  AddProjectUserInApi,
  deleteProjectInApi,
  fetchProjectByIdFromApi,
  fetchProjectUsersFromApi,
} from "../api/project";
import {
  fetchTasksByProjectFromApi,
  updateTaskStatusInApi,
  fetchTaskStatusOptionsFromApi,
  assignTaskToUserInApi,
  unassignTaskFromUserInApi,
} from "@/api/task";
import { formatDate } from "@/utils/date";
import { useDebounceFn } from "@/composables/useDebounceFn";

const toast = useToast();
const route = useRoute();
const router = useRouter();
const workspaceId = route.params.workspaceId;
const projectId = route.params.projectId;

const project = ref({});
const history = ref([]);
const tasks = ref([]);
const statusOptions = ref([]);
const filters = reactive({
  title: "",
  status: "",
  assignedTo: "",
  createdBy: "",
});
const taskStatusUpdates = ref({});
const taskAssignments = ref({});
const members = ref([]);
const availableUsers = ref([]);
const selectedUserId = ref("");

const TABS = {
  DASHBOARD: "dashboard",
  TASKS: "tasks",
  DISCUSSIONS: "discussions",
  MEMBERS: "members",
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
    query: { ...route.query, tab: tabNames[newIndex] },
  });
});

const sortBy = ref("CreatedAtUtc");
const sortDesc = ref(true);
const totalCount = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);

function changeTab(index) {
  selectedTab.value = index;
}

const fetchProjectMembers = async () => {
  try {
    members.value = await fetchProjectUsersFromApi(projectId);
  } catch (err) {
    console.error("Error loading project members", err);
  }
};

const fetchAvailableUsers = async () => {
  try {
    const allUsers = await fetchWorkspaceUsersFromApi(workspaceId);
    const addedIds = new Set(members.value.map((m) => m.userId));
    availableUsers.value = allUsers.filter(
      (user) => !addedIds.has(user.userId)
    );
  } catch (err) {
    console.error("Failed to fetch workspace members", err);
  }
};

const addUserToProject = async () => {
  if (!selectedUserId.value) return;
  try {
    await AddProjectUserInApi(
      route.params.projectId,
      selectedUserId.value,
      "Member"
    );
    toast.success("Member added successfully!");
    selectedUserId.value = "";
    await Promise.all([fetchProjectMembers(), fetchAvailableUsers()]);
  } catch (err) {
    console.error("Failed to add member", err);
    toast.error("Something went wrong.");
  }
};

const fetchProject = async () => {
  project.value = await fetchProjectByIdFromApi(projectId);
};

const fetchTasks = async () => {
  const queryParams = {
    pageNumber: currentPage.value,
    pageSize: pageSize.value,
    filters: {
      ...(filters.title && { title: filters.title }),
      ...(filters.status && { status: filters.status }),
      ...(filters.assignedTo && { assignedToId: filters.assignedTo }),
      ...(filters.createdBy && { createdById: filters.createdBy }),
    },
    sortBy: sortBy.value || "CreatedAtUtc",
    sortDesc: sortDesc.value,
  };
  try {
    const data = await fetchTasksByProjectFromApi(projectId, queryParams);
    tasks.value = data.items;
    totalCount.value = data.totalCount;
    tasks.value.forEach((t) => {
      taskStatusUpdates.value[t.id] = t.status;
      taskAssignments.value[t.id] = t.assignedToId;
    });
  } catch (err) {
    console.error("Failed to fetch tasks", err);
  }
};

const { debounced: debouncedFetchTasks } = useDebounceFn(fetchTasks, 500);

watch(
  [filters, sortBy, sortDesc],
  () => {
    currentPage.value = 1;
    debouncedFetchTasks();
  },
  { deep: true }
);

watch(currentPage, fetchTasks);

const fetchStatusOptions = async () => {
  statusOptions.value = await fetchTaskStatusOptionsFromApi();
};

const updateTaskStatus = async (taskId) => {
  const newStatus = taskStatusUpdates.value[taskId];
  await updateTaskStatusInApi(taskId, newStatus);
  toast.success("Task status updated successfully!");
};

const assignTask = async (taskId) => {
  const userId = taskAssignments.value[taskId];
  if (userId) {
    await assignTaskToUserInApi(taskId, userId);
    toast.success("Task assigned successfully!");
  } else {
    await unassignTaskFromUserInApi(taskId);
    toast.success("Task unassigned successfully!");
  }
};

const viewTask = (taskId) =>
  router.push({ name: "task", params: { workspaceId, projectId, taskId } });

const confirmDelete = () => {
  if (confirm("Delete this project?")) {
    deleteProjectInApi(projectId).then(() => router.push("/admin/projects"));
    toast.success("Project deleted successfully!");
  }
};

const loadHistory = async () => {
  history.value = await fetchChangeLogsFromApi("project", projectId);
};

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value));

onMounted(async () => {
  await Promise.all([
    fetchProject(),
    fetchProjectMembers(),
    fetchAvailableUsers(),
    fetchTasks(),
    fetchStatusOptions(),
    loadHistory(),
  ]);
  if (!route.query.tab) {
    router.replace({ query: { ...route.query, tab: tabNames[0] } });
  }
});
</script>
