import api from "@/api/axios";

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

export const fetchChangeLogsFromApi = async (entityType, entityId) => {
  try {
    const response = await api.get(
      `/api/changelogs/${entityType}/${entityId}`,
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error fetching change logs:", error);
    throw error;
  }
};
