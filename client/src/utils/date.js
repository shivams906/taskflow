// src/utils/date.ts
export const formatDate = (dt) =>
  new Date(`${dt}Z`).toLocaleString("en-IN", {
    dateStyle: "medium",
    timeStyle: "short",
  });
