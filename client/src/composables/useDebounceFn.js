import { onBeforeUnmount } from "vue";

export function useDebounceFn(fn, delay = 300) {
  let timeout;

  const debounced = (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      fn(...args);
    }, delay);
  };

  // Optional: cancel function
  const cancel = () => clearTimeout(timeout);

  // Cleanup on component unmount
  onBeforeUnmount(() => cancel());

  return { debounced, cancel };
}
