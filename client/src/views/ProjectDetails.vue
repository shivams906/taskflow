<template>
  <h1 class="text-2xl font-bold mb-4">Project: {{ project.title }}</h1>
  <div class="flex justify-between items-start mb-6">
    <p class="text-gray-300 mt-1">
      {{ project.description || "No description" }}
    </p>
    <div class="flex space-x-2">
      <router-link
        :to="{
          name: 'editProject',
          params: { workspaceId: workspaceId, projectId: project.id },
        }"
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
  <TabGroup>
    <TabList class="flex space-x-4">
      <Tab v-slot="{ selected }">
        <span
          :class="
            selected
              ? 'border-b-2 border-blue-600 text-blue-600'
              : 'text-gray-600'
          "
        >
          Dashboard
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
          Tasks
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
          Members
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
      <TabPanel
        ><div class="p-6">
          <!-- Header: Project Info and Actions -->

          <!-- Tasks Section -->
          <div class="flex justify-between items-center mb-4">
            <router-link
              :to="{
                name: 'createTask',
                params: { workspaceId: workspaceId, projectId: project.id },
              }"
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
              <option v-for="s in statusOptions" :key="s" :value="s">
                {{ s }}
              </option>
            </select>
            <select
              v-model="filters.assignedTo"
              class="px-3 py-2 rounded border text-black w-full"
            >
              <option value="">All Users</option>
              <option v-for="u in users" :key="u.userId" :value="u.userId">
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
                <!-- <th class="px-4 py-2">Created By</th> -->
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
                <td class="px-4 py-2 text-center">
                  <button @click="viewTask(task.id)" class="hover:underline">
                    {{ task.title }}
                  </button>
                </td>
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
                    <option
                      v-for="u in users"
                      :key="u.userId"
                      :value="u.userId"
                    >
                      {{ u.username }}
                    </option>
                  </select>
                </td>
                <!-- <td class="px-4 py-2 text-center">
            {{ task.createdBy }}
          </td> -->
                <td class="px-4 py-2 text-center">
                  {{ formatDate(task.createdAtUtc) }}
                </td>

                <td class="px-4 py-2 space-x-2 text-center">
                  <Menu as="div" class="relative inline-block text-left">
                    <div>
                      <MenuButton
                        class="inline-flex justify-center w-full p-2 text-sm font-medium text-gray-500 rounded-full hover:bg-gray-200 focus:outline-none"
                      >
                        <!-- Three-dot icon -->
                        <svg
                          class="w-5 h-5"
                          fill="currentColor"
                          viewBox="0 0 20 20"
                        >
                          <path
                            d="M10 3a1.5 1.5 0 110 3 1.5 1.5 0 010-3zm0 5a1.5 1.5 0 110 3 1.5 1.5 0 010-3zm0 5a1.5 1.5 0 110 3 1.5 1.5 0 010-3z"
                          />
                        </svg>
                      </MenuButton>
                    </div>

                    <MenuItems
                      class="absolute right-0 z-10 mt-2 w-40 origin-top-right rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none"
                    >
                      <div class="py-1">
                        <MenuItem v-slot="{ active }">
                          <button
                            @click="editTask(task.id)"
                            :class="[
                              active
                                ? 'bg-gray-100 text-gray-900'
                                : 'text-gray-700',
                              'block w-full text-left px-4 py-2 text-sm',
                            ]"
                          >
                            Edit
                          </button>
                        </MenuItem>

                        <MenuItem v-slot="{ active }">
                          <button
                            @click="deleteTask(task.id)"
                            :class="[
                              active
                                ? 'bg-gray-100 text-red-600'
                                : 'text-red-600',
                              'block w-full text-left px-4 py-2 text-sm',
                            ]"
                          >
                            Delete
                          </button>
                        </MenuItem>
                      </div>
                    </MenuItems>
                  </Menu>
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
      </TabPanel>
      <TabPanel> </TabPanel>
      <TabPanel>
        <div v-if="isAdmin" class="my-4">
          <h3 class="font-semibold mb-2">Add Member to Project</h3>
          <select
            v-model="selectedUserId"
            class="border px-2 py-1 rounded w-full mb-2"
          >
            <option disabled value="">Select a user</option>
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
            class="bg-blue-600 text-white px-4 py-1 rounded text-sm"
          >
            Add Member
          </button>
        </div>

        <MemberList title="Project Members" :members="members" />
      </TabPanel>
      <TabPanel> </TabPanel>
      <TabPanel> </TabPanel>
    </TabPanels>
  </TabGroup>
</template>

<script setup>
import { TabGroup, TabList, Tab, TabPanels, TabPanel } from "@headlessui/vue";
import { ref, computed, onMounted } from "vue";
import api from "@/api/axios";
import { useRoute, useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import MemberList from "@/components/common/MemberList.vue";
import { useAuthStore } from "@/stores/authStore";
import { Menu, MenuButton, MenuItems, MenuItem } from "@headlessui/vue";
const toast = useToast();

const authStore = useAuthStore();

const route = useRoute();
const router = useRouter();
const workspaceId = route.params.workspaceId;
const projectId = route.params.projectId;

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

const members = ref([]);
const availableUsers = ref([]);

const fetchProjectMembers = async () => {
  try {
    const res = await api.get(`/api/projects/${projectId}/users`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    members.value = res.data;
  } catch (err) {
    console.error("Error loading project members", err);
  }
};
const fetchAvailableUsers = async () => {
  try {
    const res = await api.get(`/api/workspaces/${workspaceId}/users`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });

    // Filter out already added project members
    const addedIds = new Set(members.value.map((m) => m.userId));
    availableUsers.value = res.data.filter(
      (user) => !addedIds.has(user.userId)
    );
  } catch (err) {
    console.error("Failed to fetch workspace members", err);
  }
};

const selectedUserId = ref("");
const isAdmin = computed(() => {
  return members.value.some(
    (m) =>
      m.username === authStore.username &&
      (m.role === "Admin" || m.role === "Owner")
  );
});

const addUserToProject = async () => {
  if (!selectedUserId.value) return;

  try {
    await api.post(
      `/api/projects/${route.params.projectId}/users`,
      {
        userId: selectedUserId.value,
        role: "Member",
      },
      {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      }
    );

    alert("Member added!");
    selectedUserId.value = "";
    await fetchProjectMembers(); // reload member list
    await fetchAvailableUsers(); // update dropdown
  } catch (err) {
    console.error("Failed to add member", err);
    alert("Something went wrong.");
  }
};

const fetchProject = async () => {
  const res = await api.get(`/api/projects/${projectId}`, {
    headers: authHeader(),
  });
  project.value = res.data;
};

const fetchUsers = async () => {
  const res = await api.get(`/api/projects/${projectId}/users`, {
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
  router.push({ name: "task", params: { workspaceId, projectId, taskId } });
const editTask = (taskId) =>
  router.push({ name: "editTask", params: { workspaceId, projectId, taskId } });
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
    fetchProjectMembers(),
    fetchAvailableUsers(),
    fetchUsers(),
    fetchTasks(),
    fetchStatusOptions(),
  ]);
});
</script>
