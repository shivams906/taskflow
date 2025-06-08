<template>
  <div class="w-full min-h-screen bg-gray-50">
    <div class="max-w-7xl mx-auto p-6">
      <!-- Header -->
      <div
        class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8"
      >
        <h2 class="text-3xl font-semibold text-gray-900">Logs</h2>
        <button
          @click="openAddLogModal"
          class="mt-4 sm:mt-0 px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
        >
          + Add Log
        </button>
      </div>

      <!-- Filters -->
      <div class="bg-white p-4 rounded-lg shadow-sm flex flex-wrap gap-4 mb-6">
        <div class="flex-1 min-w-[200px]">
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Start Date</label
          >
          <input
            v-model="filters.startDate"
            type="date"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          />
        </div>
        <div class="flex-1 min-w-[200px]">
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >End Date</label
          >
          <input
            v-model="filters.endDate"
            type="date"
            class="h-9 w-full px-3 rounded-md border border-gray-300 focus:outline-none focus:ring-1 focus:ring-blue-500 text-sm text-gray-900"
          />
        </div>
      </div>

      <!-- Logs Table -->
      <div class="bg-white rounded-lg shadow-sm overflow-hidden">
        <table class="w-full text-sm text-gray-900">
          <thead class="bg-gray-100 text-gray-700">
            <tr>
              <th class="px-6 py-3 text-left font-medium">Start Time</th>
              <th class="px-6 py-3 text-left font-medium">End Time</th>
              <th class="px-6 py-3 text-left font-medium">Duration</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="log in filteredLogs"
              :key="log.id"
              class="border-t hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4">{{ formatDate(log.startTime) }}</td>
              <td class="px-6 py-4">{{ formatDate(log.endTime) }}</td>
              <td class="px-6 py-4">
                {{ calculateMinutes(log.startTime, log.endTime) }} min
              </td>
            </tr>
            <tr v-if="filteredLogs.length === 0">
              <td colspan="3" class="px-6 py-4 text-center text-gray-500">
                No logs found
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <AddLogModal
        v-if="showAddModal"
        :taskId="taskId"
        @close="closeAddLogModal"
        @refresh="fetchLogs"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute } from "vue-router";
import AddLogModal from "@/components/AddLogModal.vue";
import { fetchTimeLogsByTaskFromApi } from "../api/task";
import { formatDate } from "@/utils/date";

const route = useRoute();
const taskId = route.params.taskId;
const logs = ref([]);
const filters = ref({ startDate: "", endDate: "" });
const showAddModal = ref(false);

const fetchLogs = async () => {
  try {
    logs.value = await fetchTimeLogsByTaskFromApi(taskId, true);
  } catch (err) {
    console.error("Failed to load logs:", err);
  }
};

const openAddLogModal = () => {
  showAddModal.value = true;
};

const closeAddLogModal = () => {
  showAddModal.value = false;
};

const calculateMinutes = (start, end) => {
  return Math.round((new Date(end) - new Date(start)) / (1000 * 60));
};

const filteredLogs = computed(() => {
  return logs.value.filter((log) => {
    const startOk =
      !filters.value.startDate ||
      new Date(log.startTime) >= new Date(filters.value.startDate);
    const endOk =
      !filters.value.endDate ||
      new Date(log.endTime) <= new Date(filters.value.endDate + "T23:59:59");
    return startOk && endOk;
  });
});

onMounted(fetchLogs);
</script>
