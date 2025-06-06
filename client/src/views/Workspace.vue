<template>
  <h1 class="text-2xl font-bold mb-4">Workspace: {{ workspace.name }}</h1>
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
          Projects
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
      <TabPanel>
        <!-- Dashboard Content -->
        <!-- <DashboardPanel /> -->
      </TabPanel>
      <TabPanel>
        <!-- Projects -->
        <div class="p-6">
          <div class="flex justify-between items-center mb-4">
            <router-link
              :to="{ name: 'createProject', params: { workspaceId } }"
              class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-100"
            >
              + Create Project
            </router-link>
          </div>
          <!-- Projects Table -->
          <table class="w-full bg-white rounded shadow text-black">
            <thead class="bg-gray-200">
              <tr>
                <th class="px-4 py-2 w-2/3 text-left">Name</th>
                <th class="px-4 py-2">Created By</th>
                <th class="px-4 py-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="project in projects"
                :key="project.id"
                class="border-t hover:bg-gray-100"
              >
                <td class="px-4 py-2 text-left">
                  <button
                    @click="viewProject(project.id)"
                    class="hover:underline"
                  >
                    {{ project.title }}
                  </button>
                </td>
                <td class="px-4 py-2 text-center">
                  {{ project.createdByUsername || "N/A" }}
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
                            @click="editProject(project.id)"
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
                            @click="deleteProject(project.id)"
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
              <tr v-if="projects.length === 0">
                <td colspan="5" class="px-4 py-4 text-center text-gray-500">
                  No projects found.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </TabPanel>
      <TabPanel></TabPanel>
      <TabPanel>
        <!-- Members -->
        <MemberList :members="members" />
      </TabPanel>
      <TabPanel>
        <div class="space-y-4 text-sm text-gray-800">
          <div>
            <label class="font-semibold">Workspace Name:</label>
            <div class="text-gray-900">{{ workspace.name }}</div>
          </div>

          <div class="space-y-4 text-sm text-gray-800">
            <div>
              <label class="font-semibold">Invite Code</label>
              <div class="flex gap-2 items-center">
                <input
                  type="text"
                  v-model="workspace.inviteCode"
                  readonly
                  class="border border-gray-300 rounded px-2 py-1 w-full bg-gray-100 text-gray-700"
                />
                <button
                  @click="copyInviteCode"
                  class="bg-blue-600 text-white text-sm px-3 py-1 rounded"
                >
                  Copy
                </button>
              </div>
            </div>
          </div>

          <div>
            <label class="font-semibold">Created By:</label>
            <div>{{ workspace.createdById }}</div>
          </div>

          <div>
            <label class="font-semibold">Created At:</label>
            <div>{{ formatDate(workspace.createdAtUtc) }}</div>
          </div>

          <div>
            <label class="font-semibold">Last Updated:</label>
            <div>{{ formatDate(workspace.updatedAtUtc) }}</div>
          </div>
        </div>
      </TabPanel>
      <TabPanel>
        <ul>
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
import { useRoute, useRouter } from "vue-router";
import api from "@/api/axios";
import { ref, onMounted, watch, watchEffect } from "vue";
import { useToast } from "vue-toastification";
import MemberList from "@/components/common/MemberList.vue";
import { useWorkspaceStore } from "@/stores/workspaceStore";
import { Menu, MenuButton, MenuItems, MenuItem } from "@headlessui/vue";
import { fetchChangeLogsFromApi } from "@/api/changeLog";
import {
  fetchWorkspaceByIdFromApi,
  fetchWorkspaceUsersFromApi,
} from "@/api/workspace";
import {
  fetchProjectsByWorkspaceFromApi,
  deleteProjectInApi,
} from "@/api/project";
import { formatDate } from "@/utils/date";

const route = useRoute();
const router = useRouter();
const toast = useToast();

// Load workspace data
var workspaceId = route.params.workspaceId;

const workspace = ref({
  id: "",
  name: "",
  inviteCode: "",
  createdById: "",
  createdAtUtc: "",
  UpdatedById: "",
  updatedAtUtc: "",
});
const projects = ref([]);
const workspaceStore = useWorkspaceStore();
const members = ref([]);
const history = ref([]);

const TABS = {
  DASHBOARD: "dashboard",
  PROJECTS: "projects",
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
    query: {
      ...route.query,
      tab: tabNames[newIndex],
    },
  });
});

function changeTab(index) {
  selectedTab.value = index;
}

const fetchWorkspaceMembers = async () => {
  try {
    members.value = await fetchWorkspaceUsersFromApi(workspaceId);
  } catch (err) {
    console.error("Error loading workspace members", err);
  }
};

watchEffect(() => {
  const id = workspaceStore.currentWorkspaceId;
  if (id) fetchWorkspaceMembers(id);
});

const fetchWorkspace = async () => {
  try {
    workspace.value = await fetchWorkspaceByIdFromApi(workspaceId);
  } catch (err) {
    console.error("Failed to load workspace", err);
  }
};

const fetchProjects = async () => {
  try {
    projects.value = await fetchProjectsByWorkspaceFromApi(workspaceId);
  } catch (err) {
    console.error("Failed to load projects", err);
  }
};
watch(
  () => route.params.workspaceId,
  async (newId, oldId) => {
    if (newId != oldId) {
      workspaceId = newId;
      await fetchWorkspace();
      await fetchProjects();
      await fetchWorkspaceMembers();
    }
  },
  { immediate: true }
);

const loadHistory = async () => {
  history.value = await fetchChangeLogsFromApi("workspace", workspaceId);
};

onMounted(() => {
  if (!route.query.tab) {
    router.replace({
      query: { ...route.query, tab: tabNames[0] },
    });
  }
  loadHistory();
});

const viewProject = (id) =>
  router.push({ name: "project", params: { workspaceId, projectId: id } });
const editProject = (id) =>
  router.push({ name: "editProject", params: { workspaceId, projectId: id } });
const deleteProject = async (id) => {
  if (!confirm("Delete this project?")) return;
  await deleteProjectInApi(id);
  toast.success("Project deleted successfully!");
  fetchProjects();
};

const copyInviteCode = async () => {
  try {
    await navigator.clipboard.writeText(workspace.value.inviteCode);
    alert("Invite code copied to clipboard!");
  } catch (err) {
    console.error("Copy failed", err);
    alert("Failed to copy invite code.");
  }
};
</script>

<style scoped>
/* Optional: improve tab UX */
</style>
