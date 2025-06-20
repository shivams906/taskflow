<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto p-6">
      <!-- Header -->
      <div
        class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8"
      >
        <h1 class="text-3xl font-semibold text-gray-900">
          Workspace: {{ workspace.name }}
        </h1>
        <div class="mt-4 sm:mt-0 flex space-x-3">
          <router-link
            v-permission:ManageWorkspace="workspace.permissions"
            :to="{ name: 'editWorkspace', params: { workspaceId } }"
            class="inline-flex items-center"
          >
            <button
              class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
            >
              Edit
            </button>
          </router-link>
          <button
            v-permission:DeleteWorkspace="workspace.permissions"
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
              'Projects',
              // 'Discussions',
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
              <p class="text-gray-500 italic">
                Dashboard content coming soon...
              </p>
            </div>
          </TabPanel>
          <TabPanel>
            <div class="space-y-6">
              <div
                v-permission:ManageWorkspace="workspace.permissions"
                class="flex justify-end"
              >
                <router-link
                  :to="{ name: 'createProject', params: { workspaceId } }"
                  class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                >
                  + Create Project
                </router-link>
              </div>
              <div class="bg-white rounded-lg shadow-sm overflow-hidden">
                <table class="w-full text-sm text-gray-900">
                  <thead class="bg-gray-100 text-gray-700">
                    <tr>
                      <th class="px-6 py-3 text-left font-medium w-2/3">
                        Name
                      </th>
                      <th class="px-6 py-3 text-left font-medium">
                        Created By
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="project in projects"
                      :key="project.id"
                      class="border-t hover:bg-gray-50 transition-colors"
                    >
                      <td class="px-6 py-4">
                        <button
                          @click="viewProject(project.id)"
                          class="text-blue-600 hover:underline"
                        >
                          {{ project.title }}
                        </button>
                      </td>
                      <td class="px-6 py-4">
                        {{ project.createdByUsername || "N/A" }}
                      </td>
                    </tr>
                    <tr v-if="projects.length === 0">
                      <td
                        colspan="2"
                        class="px-6 py-4 text-center text-gray-500"
                      >
                        No projects found.
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </TabPanel>
          <!-- <TabPanel>
            <div class="bg-white p-6 rounded-lg shadow-sm">
              <p class="text-gray-500 italic">
                Discussions content coming soon...
              </p>
            </div>
          </TabPanel> -->
          <TabPanel>
            <MemberList
              :members="members"
              class="bg-white p-6 rounded-lg shadow-sm"
            />
          </TabPanel>
          <TabPanel>
            <div class="bg-white p-6 rounded-lg shadow-sm space-y-4">
              <InfoRow label="Invite Code">
                <template #default>
                  <div class="flex gap-2 items-center">
                    <input
                      type="text"
                      v-model="workspace.inviteCode"
                      readonly
                      class="h-9 w-full px-3 rounded-md border border-gray-300 bg-gray-100 text-gray-700 text-sm focus:outline-none"
                    />
                    <button
                      @click="copyInviteCode"
                      class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                    >
                      Copy
                    </button>
                  </div>
                </template>
              </InfoRow>
              <InfoRow
                label="Created By"
                :value="workspace.createdByUsername || 'N/A'"
              />
              <InfoRow
                label="Created On"
                :value="formatDate(workspace.createdAtUtc)"
              />
              <InfoRow
                label="Last Updated By"
                :value="workspace.updatedByUsername || 'N/A'"
              />
              <InfoRow
                label="Last Updated On"
                :value="
                  workspace.updatedAtUtc
                    ? formatDate(workspace.updatedAtUtc)
                    : 'N/A'
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
import { useRoute, useRouter } from "vue-router";
import api from "@/api/axios";
import { ref, onMounted, watch, watchEffect } from "vue";
import { useToast } from "vue-toastification";
import MemberList from "@/components/common/MemberList.vue";
import { useWorkspaceStore } from "@/stores/workspaceStore";
import { fetchChangeLogsFromApi } from "@/api/changeLog";
import InfoRow from "@/components/common/InfoRow.vue";
import {
  fetchWorkspaceByIdFromApi,
  fetchWorkspaceUsersFromApi,
  deleteWorkspaceFromApi,
} from "@/api/workspace";
import {
  fetchProjectsByWorkspaceFromApi,
  deleteProjectInApi,
} from "@/api/project";
import { formatDate } from "@/utils/date";

const route = useRoute();
const router = useRouter();
const toast = useToast();
const suppressWatch = ref(false);

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
  // DISCUSSIONS: "discussions",
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
  if (suppressWatch.value) return;
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
      await loadHistory();
    }
  }
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
  fetchWorkspace();
  fetchProjects();
  fetchWorkspaceMembers();
  loadHistory();
});

const viewProject = (id) =>
  router.push({ name: "project", params: { workspaceId, projectId: id } });

const copyInviteCode = async () => {
  try {
    await navigator.clipboard.writeText(workspace.value.inviteCode);
    alert("Invite code copied to clipboard!");
  } catch (err) {
    console.error("Copy failed", err);
    alert("Failed to copy invite code.");
  }
};

const confirmDelete = async () => {
  if (confirm("Delete this workspace?")) {
    try {
      suppressWatch.value = true;
      await deleteWorkspaceFromApi(workspaceId);
      workspaceStore.cleanCurrentWorkspace();
      await workspaceStore.fetchWorkspaces();

      toast.success("Workspace deleted successfully!");
      await router.push({ name: "home" });
    } catch (error) {
      toast.error("Failed to delete workspace.");
      console.error(error);
    }
  }
};
</script>
