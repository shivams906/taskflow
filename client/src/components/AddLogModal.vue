<template>
  <div
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
  >
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md p-6 text-black">
      <h2 class="text-2xl font-bold mb-4 text-center">Add Time Log</h2>

      <div class="mb-4">
        <label class="block mb-1 font-semibold">Start Time</label>
        <input
          v-model="startTime"
          type="datetime-local"
          class="border px-3 py-2 rounded w-full"
        />
      </div>

      <div class="mb-4">
        <label class="block mb-1 font-semibold">End Time</label>
        <input
          v-model="endTime"
          type="datetime-local"
          class="border px-3 py-2 rounded w-full"
        />
      </div>

      <div class="flex justify-end gap-4 mt-6">
        <button
          @click="$emit('close')"
          class="px-4 py-2 rounded bg-gray-400 text-white hover:bg-gray-500"
        >
          Cancel
        </button>
        <button
          @click="addLog"
          class="px-4 py-2 rounded bg-green-500 text-white hover:bg-green-600"
        >
          Save
        </button>
      </div>

      <div v-if="error" class="text-red-600 text-sm mt-2">{{ error }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "axios";
import { useToast } from "vue-toastification";

const toast = useToast();

const props = defineProps({
  taskId: String,
});

const emits = defineEmits(["close", "refresh"]);

const startTime = ref("");
const endTime = ref("");
const error = ref("");

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

const addLog = async () => {
  if (!startTime.value || !endTime.value) {
    error.value = "Please fill both times.";
    return;
  }

  try {
    await axios.post(
      `https://localhost:7234/api/tasks/${props.taskId}/log-time`,
      {
        startTime: new Date(startTime.value).toISOString(),
        endTime: new Date(endTime.value).toISOString(),
      },
      { headers: authHeader() }
    );

    emits("refresh");
    emits("close");
    toast.success("Log added successfully!");
  } catch (err) {
    console.error("Failed to add log:", err);
    error.value = "Failed to add log.";
  }
};
</script>
