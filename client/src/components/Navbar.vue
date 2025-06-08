<template>
  <nav
    class="flex justify-between items-center bg-gray-900 text-white px-6 py-4 shadow-sm"
  >
    <!-- Left: App title -->
    <div class="flex gap-6 items-center">
      <router-link to="/" class="text-xl font-semibold text-white">
        TaskFlow
      </router-link>
    </div>

    <!-- Right: Auth section -->
    <div class="flex items-center gap-4">
      <div v-if="auth.isAuthenticated" class="flex items-center gap-4">
        <span class="text-gray-300 text-sm">
          {{ auth.username }}
        </span>
        <button
          @click="logout"
          class="px-3 py-1.5 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
        >
          Logout
        </button>
      </div>

      <div v-else class="flex gap-4">
        <router-link
          to="/login"
          class="text-gray-300 text-sm hover:text-white transition-colors"
        >
          Login
        </router-link>
        <router-link
          to="/register"
          class="text-gray-300 text-sm hover:text-white transition-colors"
        >
          Register
        </router-link>
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
