<template>
  <div class="mt-4">
    <button
      @click="fetchSecret"
      class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
    >
      Get Secret Message
    </button>
    <p v-if="secretMessage" class="mt-2 text-green-600">
      {{ secretMessage }}
    </p>
    <p v-if="error" class="mt-2 text-red-600">
      {{ error }}
    </p>
  </div>
</template>

<script setup>
import { ref } from "vue";
import api from "@/api/axios";

const secretMessage = ref("");
const error = ref("");

const fetchSecret = async () => {
  try {
    error.value = "";
    const token = localStorage.getItem("token");
    const response = await api.get("/api/auth/secret", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    secretMessage.value = response.data;
  } catch (err) {
    error.value = "Failed to fetch secret. Are you logged in?";
    secretMessage.value = "";
  }
};
</script>
