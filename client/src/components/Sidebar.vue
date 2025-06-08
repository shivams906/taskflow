<template>
  <nav class="w-64 bg-gray-900 text-white min-h-screen p-6 flex flex-col gap-6">
    <!-- Workspace Selector -->
    <div>
      <label
        for="workspace"
        class="block text-sm font-medium text-gray-300 mb-2"
      >
        Workspace
      </label>
      <select
        v-if="workspaces.length > 0"
        v-model="selectedWorkspace"
        @change="changeWorkspace"
        class="h-9 w-full px-3 rounded-md bg-gray-800 border border-gray-700 text-sm text-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500"
      >
        <option
          v-for="workspace in workspaces"
          :key="workspace.id"
          :value="workspace.id"
        >
          {{ workspace.name }}
        </option>
      </select>
      <button
        @click="addWorkspace"
        class="mt-3 w-full px-3 py-2 bg-gray-800 text-gray-300 text-sm font-medium border border-dashed border-gray-600 rounded-lg hover:bg-gray-700 transition-colors"
      >
        + Add Workspace
      </button>
      <button
        @click="joinWorkspace"
        class="mt-2 w-full px-3 py-2 bg-gray-800 text-gray-300 text-sm font-medium border border-dashed border-gray-600 rounded-lg hover:bg-gray-700 transition-colors"
      >
        + Join Workspace
      </button>
    </div>

    <!-- Navigation Links -->
    <ul v-if="selectedWorkspace" class="flex flex-col gap-3 mt-4">
      <li>
        <router-link
          :to="{
            name: 'workspace',
            params: { workspaceId: selectedWorkspace },
          }"
          exact-active-class="text-white font-semibold bg-gray-800 rounded-md"
          class="block px-3 py-2 text-sm text-gray-300 hover:text-white hover:bg-gray-800 rounded-md transition-colors"
        >
          Dashboard
        </router-link>
      </li>
      <li>
        <router-link
          :to="{ name: 'myTasks', params: { workspaceId: selectedWorkspace } }"
          exact-active-class="text-white font-semibold bg-gray-800 rounded-md"
          class="block px-3 py-2 text-sm text-gray-300 hover:text-white hover:bg-gray-800 rounded-md transition-colors"
        >
          My Tasks
        </router-link>
      </li>
    </ul>
  </nav>
</template>

<script setup>
import { computed, ref, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { useToast } from "vue-toastification";
import { useWorkspaceStore } from "@/stores/workspaceStore";

const toast = useToast();
const router = useRouter();
const store = useWorkspaceStore();

onMounted(async () => {
  await store.fetchWorkspaces();

  if (!store.currentWorkspaceId && store.workspaces.length > 0) {
    store.setCurrentWorkspace(store.workspaces[0].id);
  }
});

const selectedWorkspace = computed({
  get: () => store.currentWorkspaceId,
  set: (val) => {
    store.setCurrentWorkspace(val);
    router.push(`/workspaces/${val}`);
  },
});

const workspaces = computed(() => store.workspaces);
const changeWorkspace = () =>
  (selectedWorkspace.value = selectedWorkspace.value);

const addWorkspace = () => {
  router.push("/workspaces/create");
};
const joinWorkspace = () => {
  router.push("/workspaces/join");
};
</script>
