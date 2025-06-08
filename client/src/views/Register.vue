<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-md mx-auto p-6">
      <div class="bg-white p-6 rounded-lg shadow-sm">
        <h2 class="text-3xl font-semibold text-gray-900 mb-6">Register</h2>
        <form @submit.prevent="register" class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Username</label
            >
            <input
              v-model="username"
              type="text"
              required
              placeholder="Username"
              class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Password</label
            >
            <input
              v-model="password"
              type="password"
              required
              placeholder="Password"
              class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
            />
          </div>
          <div class="flex justify-end">
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
            >
              Register
            </button>
          </div>
        </form>
        <p v-if="error" class="text-red-600 text-sm mt-4">{{ error }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/api/axios";

export default {
  data() {
    return {
      username: "",
      password: "",
      error: "",
    };
  },
  methods: {
    async register() {
      try {
        await api.post("/api/auth/register", {
          username: this.username,
          password: this.password,
        });
        this.$router.push("/login");
      } catch (err) {
        this.error = "Registration failed";
      }
    },
  },
};
</script>
