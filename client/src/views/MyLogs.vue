<template>
  <div class="p-6 max-w-4xl mx-auto text-black">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold">Logs</h2>
      <button
        @click="openAddLogModal"
        class="bg-white text-black px-4 py-2 rounded border border-gray-300 hover:bg-gray-200 transition"
      >
        + Add Log
      </button>
    </div>

    <!-- Filters -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
      <input
        v-model="filters.startDate"
        type="date"
        class="border px-3 py-2 rounded text-black"
      />
      <input
        v-model="filters.endDate"
        type="date"
        class="border px-3 py-2 rounded text-black"
      />
    </div>

    <!-- Logs Table -->
    <table class="w-full bg-white rounded shadow text-black">
      <thead class="bg-gray-200">
        <tr>
          <th class="px-4 py-2">Start Time</th>
          <th class="px-4 py-2">End Time</th>
          <th class="px-4 py-2">Duration</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="log in filteredLogs"
          :key="log.id"
          class="border-t hover:bg-gray-100"
        >
          <td class="px-4 py-2">{{ formatDate(log.startTime) }}</td>
          <td class="px-4 py-2">{{ formatDate(log.endTime) }}</td>
          <td class="px-4 py-2">
            {{ calculateMinutes(log.startTime, log.endTime) }} min
          </td>
        </tr>
        <tr v-if="filteredLogs.length === 0">
          <td colspan="3" class="text-center text-gray-400 py-6">
            No logs found
          </td>
        </tr>
      </tbody>
    </table>

    <AddLogModal
      v-if="showAddModal"
      :taskId="taskId"
      @close="closeAddLogModal"
      @refresh="fetchLogs"
    />
  </div>
</template>
<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute } from "vue-router";
import api from "@/api/axios";
import AddLogModal from "@/components/AddLogModal.vue";

const route = useRoute();
const taskId = route.params.id;
const logs = ref([]);
const filters = ref({ startDate: "", endDate: "" });
const showAddModal = ref(false);

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

const fetchLogs = async () => {
  try {
    const res = await api.get(`/api/tasks/${taskId}/logs?onlyMine=true`, {
      headers: authHeader(),
    });
    logs.value = res.data;
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

const formatDate = (dt) => new Date(`${dt}Z`).toLocaleString();

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
