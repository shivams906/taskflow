<template>
  <h1 class="text-2xl font-bold mb-4">Workspace: {{ workspace.name }}</h1>
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
                <th class="px-4 py-2">Name</th>
                <!-- <th class="px-4 py-2">Created By</th> -->
                <th class="px-4 py-2">Created At</th>
                <th class="px-4 py-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="project in projects"
                :key="project.id"
                class="border-t hover:bg-gray-100"
              >
                <td class="px-4 py-2 text-center">{{ project.title }}</td>
                <!-- <td class="px-4 py-2 text-center">{{ project.createdBy }}</td> -->
                <td class="px-4 py-2 text-center">
                  {{ formatDate(project.createdAtUtc) }}
                </td>
                <td class="px-4 py-2 space-x-2 text-center">
                  <button
                    @click="viewTasks(project.id)"
                    class="text-sm text-blue-600 hover:underline"
                  >
                    View
                  </button>
                  <button
                    @click="editProject(project.id)"
                    class="text-sm text-green-600 hover:underline"
                  >
                    Edit
                  </button>
                  <button
                    @click="deleteProject(project.id)"
                    class="text-sm text-red-600 hover:underline"
                  >
                    Delete
                  </button>
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
      <TabPanel></TabPanel>
    </TabPanels>
  </TabGroup>
</template>

<script setup>
import { TabGroup, TabList, Tab, TabPanels, TabPanel } from "@headlessui/vue";
import api from "@/api/axios";
import { useRoute, useRouter } from "vue-router";
import { ref, onMounted, watch, watchEffect } from "vue";
import { useToast } from "vue-toastification";
import MemberList from "@/components/common/MemberList.vue";
import { useWorkspaceStore } from "@/stores/workspaceStore";
// import DashboardPanel from "@/components/workspace/DashboardPanel.vue";
// import ProjectList from "@/components/workspace/ProjectList.vue";
// import MemberList from "@/components/workspace/MemberList.vue";
// import AboutWorkspace from "@/components/workspace/AboutWorkspace.vue";
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

const fetchWorkspaceMembers = async () => {
  try {
    const res = await api.get(`/api/workspaces/${workspaceId}/users`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    members.value = res.data;
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
    const res = await api.get(`/api/workspaces/${workspaceId}`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    workspace.value = res.data;
  } catch (err) {
    console.error("Failed to load workspace", err);
  }
};

const fetchProjects = async () => {
  try {
    const res = await api.get(`/api/workspaces/${workspaceId}/projects`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    projects.value = res.data;
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
    }
  },
  { immediate: true }
);

onMounted(() => {
  fetchWorkspace();
  fetchProjects();
  fetchWorkspaceMembers();
});

const viewTasks = (id) =>
  router.push({ name: "project", params: { workspaceId, projectId: id } });
const editProject = (id) =>
  router.push({ name: "editProject", params: { workspaceId, projectId: id } });
const deleteProject = async (id) => {
  if (!confirm("Delete this project?")) return;
  await api.delete(`/api/projects/${id}`, {
    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
  });
  toast.success("Project deleted successfully!");
  fetchProjects();
};

const formatDate = (dt) =>
  new Date(`${dt}Z`).toLocaleString("en-IN", {
    dateStyle: "medium",
    timeStyle: "short",
  });

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
