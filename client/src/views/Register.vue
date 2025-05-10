<template>
  <div class="p-6 max-w-xl mx-auto">
    <h2 class="text-2xl font-bold text-black mb-4">Register</h2>
    <form @submit.prevent="register" class="space-y-4">
      <div>
        <label class="block text-black mb-1">Username</label>
        <input
          v-model="username"
          type="text"
          required
          placeholder="Username"
          class="w-full px-3 py-2 rounded border border-gray-300 text-black"
        />
      </div>
      <div>
        <label class="block text-black mb-1">Password</label>
        <input
          v-model="password"
          type="password"
          required
          placeholder="Password"
          class="w-full px-3 py-2 rounded border border-gray-300 text-black"
        />
      </div>
      <div class="flex justify-end space-x-2">
        <button
          type="submit"
          class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-200 transition"
        >
          Register
        </button>
      </div>
    </form>
    <p v-if="error" class="text-red-400 mt-4">{{ error }}</p>
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
