// directives/permission.ts
import type { DirectiveBinding } from 'vue';

function checkPermission(binding: DirectiveBinding, permissions: string[]): boolean {
  const permissionKey = binding.arg;
  return Array.isArray(permissions) && permissions.includes(permissionKey!);
}

function apply(el: HTMLElement, binding: DirectiveBinding) {
  const permissions = binding.value;
  const hasPermission = checkPermission(binding, permissions);

  if (binding.modifiers.disable) {
    el.toggleAttribute('disabled', !hasPermission);
    el.classList.toggle('disabled', !hasPermission); // optional: add disabled class
  } else {
    el.style.display = hasPermission ? '' : 'none';
  }
}

export default {
  mounted: apply,
  updated: apply,
};
