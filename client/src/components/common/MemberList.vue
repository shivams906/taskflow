<template>
  <div class="space-y-4">
    <ul class="divide-y border rounded bg-white shadow-sm">
      <li
        v-for="member in members"
        :key="member.userId"
        class="p-3 flex justify-between items-center"
      >
        <div>
          <div class="font-medium text-gray-900">{{ member.username }}</div>
        </div>
        <span class="text-sm px-2 py-1 rounded bg-blue-100 text-blue-700">
          {{ member.role }}
        </span>
      </li>
    </ul>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  members: {
    type: Array,
    required: true,
  },
});
</script>
