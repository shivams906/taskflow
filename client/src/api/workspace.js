import api from "@/api/axios";

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

export const fetchWorkspacesFromApi = async () => {
  try {
    const response = await api.get("/api/workspaces", {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching workspaces:", error);
    throw error;
  }
};
export const fetchWorkspaceByIdFromApi = async (workspaceId) => {
  try {
    const response = await api.get(`/api/workspaces/${workspaceId}`, {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching workspace by ID:", error);
    throw error;
  }
};
export const sendWorkspaceDataToApi = async (name) => {
  try {
    const response = await api.post(
      "/api/workspaces",
      { name },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error creating workspace:", error);
    throw error;
  }
};
export const sendUpdatedWorkspaceDataToApi = async (workspaceId, name) => {
  try {
    const response = await api.put(
      `/api/workspaces/${workspaceId}`,
      { name },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error updating workspace:", error);
    throw error;
  }
};
export const deleteWorkspaceFromApi = async (workspaceId) => {
  try {
    const response = await api.delete(`/api/workspaces/${workspaceId}`, {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error deleting workspace:", error);
    throw error;
  }
};
export const sendWorkspaceJoiningRequestToApi = async (inviteCode) => {
  try {
    const response = await api.post(
      `/api/workspaces/join`,
      { inviteCode },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error sending workspace joining request:", error);
    throw error;
  }
};
export const fetchWorkspaceUsersFromApi = async (workspaceId) => {
  try {
    const response = await api.get(`/api/workspaces/${workspaceId}/users`, {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching workspace users:", error);
    throw error;
  }
};
