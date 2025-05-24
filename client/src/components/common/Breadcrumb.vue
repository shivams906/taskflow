<template>
  <nav class="flex items-center gap-1 text-xs text-gray-500 select-none">
    <router-link to="/" class="hover:text-gray-700 hover:underline transition"
      >Home</router-link
    >

    <template v-if="workspace">
      <span class="mx-1 text-gray-300 select-none">/</span>
      <router-link
        :to="{
          name: 'workspace',
          params: { workspaceId: workspace.id },
        }"
        class="hover:text-gray-700 hover:underline transition truncate max-w-[120px]"
      >
        Workspace: {{ workspace.name }}
      </router-link>
    </template>

    <template v-if="project">
      <span class="mx-1 text-gray-300 select-none">/</span>
      <router-link
        :to="{
          name: 'project',
          params: { workspaceId: workspace.id, projectId: project.id },
        }"
        class="hover:text-gray-700 hover:underline transition truncate max-w-[120px]"
      >
        Project: {{ project.title }}
      </router-link>
    </template>

    <template v-if="task">
      <span class="mx-1 text-gray-300 select-none">/</span>
      <span
        class="text-gray-700 truncate max-w-[120px]"
        title="Task: {{ task.title }}"
      >
        {{ task.title }}
      </span>
    </template>
  </nav>
</template>

<script setup>
import { useRoute } from "vue-router";
import { ref, watchEffect } from "vue";
import api from "@/api/axios";

const route = useRoute();

const workspace = ref(null);
const project = ref(null);
const task = ref(null);

watchEffect(async () => {
  const workspaceId = route.params.workspaceId;
  const projectId = route.params.projectId;
  const taskId = route.params.taskId;

  if (workspaceId) {
    try {
      const res = await api.get(`/api/workspaces/${workspaceId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
      workspace.value = res.data;
    } catch (err) {
      console.error("Failed to load workspace", err);
    }
  } else {
    workspace.value = null;
  }

  if (projectId) {
    try {
      const res = await api.get(`/api/projects/${projectId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
      project.value = res.data;
    } catch (err) {
      console.error("Failed to load workspace", err);
    }
  } else {
    project.value = null;
  }

  if (taskId) {
    try {
      const res = await api.get(`/api/tasks/${taskId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
      task.value = res.data;
    } catch (err) {
      console.error("Failed to load workspace", err);
    }
  } else {
    task.value = null;
  }
});
</script>
