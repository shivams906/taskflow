<script setup>
defineProps({
  label: String,
  value: String,
});
</script>

<template>
  <div class="flex justify-between py-2 items-center">
    <div class="font-semibold text-gray-600">{{ label }}</div>
    <div class="flex items-center gap-4">
      <div class="text-right text-gray-900 truncate max-w-[220px]">
        <slot>{{ value }}</slot>
      </div>
      <div>
        <slot name="actions" />
      </div>
    </div>
  </div>
</template>
