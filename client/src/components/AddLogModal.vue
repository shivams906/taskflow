<template>
  <div
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
  >
    <div class="bg-white rounded-lg shadow-sm w-full max-w-md p-6">
      <h2 class="text-2xl font-semibold text-gray-900 mb-6 text-center">
        Add Time Log
      </h2>

      <div class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            Start Time
          </label>
          <input
            v-model="startTime"
            type="datetime-local"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            End Time
          </label>
          <input
            v-model="endTime"
            type="datetime-local"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          />
        </div>

        <div class="flex justify-end gap-3 mt-6">
          <button
            @click="$emit('close')"
            class="px-4 py-2 bg-gray-100 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-200 transition-colors"
          >
            Cancel
          </button>
          <button
            @click="addLog"
            class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
          >
            Save
          </button>
        </div>

        <div v-if="error" class="text-red-600 text-sm mt-2">{{ error }}</div>
      </div>
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
