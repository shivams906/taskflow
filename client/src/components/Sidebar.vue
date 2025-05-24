<template>
  <nav class="w-60 bg-black text-white min-h-screen p-5 flex flex-col gap-6">
    <!-- Workspace Selector -->
    <div>
      <label for="workspace" class="block text-sm text-gray-400 mb-1"
        >Workspace</label
      >
      <select
        v-if="workspaces.length > 0"
        v-model="selectedWorkspace"
        @change="changeWorkspace"
        class="w-full bg-gray-800 text-white border border-gray-700 rounded px-3 py-2 text-sm focus:outline-none focus:ring focus:ring-indigo-500"
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
        class="mt-2 w-full bg-gray-800 text-gray-300 border border-dashed border-gray-600 text-xs px-3 py-2 rounded hover:bg-gray-700 transition"
      >
        + Add Workspace
      </button>
      <button
        @click="joinWorkspace"
        class="mt-2 w-full bg-gray-800 text-gray-300 border border-dashed border-gray-600 text-xs px-3 py-2 rounded hover:bg-gray-700 transition"
      >
        + Join Workspace
      </button>
    </div>

    <!-- Navigation Links -->
    <ul v-if="selectedWorkspace" class="flex flex-col gap-4 mt-4">
      <li>
        <router-link
          :to="{
            name: 'workspace',
            params: { workspaceId: selectedWorkspace },
          }"
          exact-active-class="!text-white !font-semibold border-l-4 border-indigo-500 pl-3"
          class="text-gray-400 hover:text-white transition block px-2"
        >
          Dashboard
        </router-link>
      </li>

      <li>
        <router-link
          :to="{
            name: 'myTasks',
            params: { workspaceId: selectedWorkspace },
          }"
          exact-active-class="!text-white !font-semibold border-l-4 border-indigo-500 pl-3"
          class="text-gray-400 hover:text-white transition block px-2"
        >
          My Tasks
        </router-link>
      </li>
      <!-- <li>
        <router-link
          to="/settings"
          exact-active-class="!text-white !font-semibold border-l-4 border-indigo-500 pl-3"
          class="text-gray-400 hover:text-white transition block px-2"
        >
          Settings
        </router-link>
      </li> -->
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
