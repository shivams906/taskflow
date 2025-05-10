<template>
  <nav
    class="flex justify-between items-center bg-black text-white px-6 py-3 shadow"
  >
    <!-- Left: App title -->
    <div class="flex gap-6 items-center">
      <router-link to="/" class="text-xl font-bold text-white">
        TaskFlow
      </router-link>

      <!-- Center nav links -->
      <div v-if="auth.isAuthenticated" class="flex gap-6">
        <router-link to="/admin/projects" class="hover:text-gray-300"
          >Projects</router-link
        >
        <router-link to="/my-tasks" class="hover:text-gray-300"
          >My Tasks</router-link
        >
      </div>
    </div>

    <!-- Right: Auth section -->
    <div class="flex items-center gap-4">
      <div v-if="auth.isAuthenticated" class="flex items-center gap-4">
        <span class="text-gray-300">
          {{ auth.username }}
        </span>
        <button
          @click="logout"
          class="bg-white text-black px-3 py-1 rounded hover:bg-gray-200 transition"
        >
          Logout
        </button>
      </div>

      <div v-else class="flex gap-4">
        <router-link to="/login" class="hover:text-gray-300">Login</router-link>
        <router-link to="/register" class="hover:text-gray-300"
          >Register</router-link
        >
      </div>
    </div>
  </nav>
</template>

<script setup>
import { useAuthStore } from "@/stores/authStore";
import { useRouter } from "vue-router";

const auth = useAuthStore();
const router = useRouter();

const logout = () => {
  auth.logout();
  router.push("/");
};
</script>
