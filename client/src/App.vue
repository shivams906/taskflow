<template>
  <div class="app-layout">
    <Navbar />
    <div class="main-container">
      <div v-if="auth.isAuthenticated" class="sidebar">
        <Sidebar />
      </div>
      <main class="main-content">
        <div v-if="auth.isAuthenticated" class="flex gap-6 mb-6">
          <Breadcrumb />
        </div>
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import Navbar from "./components/Navbar.vue";
import Sidebar from "./components/Sidebar.vue";
import { useAuthStore } from "@/stores/authStore";
import Breadcrumb from "./components/common/Breadcrumb.vue";

const auth = useAuthStore();
</script>

<style>
.app-layout {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-container {
  flex: 1;
  display: flex;
  overflow: hidden;
}

.main-content {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}
</style>
