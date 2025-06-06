import api from "@/api/axios";

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

export const fetchProjectsByWorkspaceFromApi = async (workspaceId) => {
  const response = await api.get(`/api/workspaces/${workspaceId}/projects`, {
    headers: authHeader(),
  });
  return response.data;
};

export const fetchProjectByIdFromApi = async (projectId) => {
  const response = await api.get(`/api/projects/${projectId}`, {
    headers: authHeader(),
  });
  return response.data;
};

export const createProjectInApi = async (workspaceId, title, description) => {
  const response = await api.post(
    `/api/workspaces/${workspaceId}/projects`,
    { title, description },
    {
      headers: authHeader(),
    }
  );
  return response.data;
};

export const updateProjectInApi = async (projectId, title, description) => {
  const response = await api.put(
    `/api/projects/${projectId}`,
    { title, description },
    {
      headers: authHeader(),
    }
  );
  return response.data;
};

export const deleteProjectInApi = async (projectId) => {
  const response = await api.delete(`/api/projects/${projectId}`, {
    headers: authHeader(),
  });
  return response.data;
};

export const AddProjectUserInApi = async (projectId, userId, role) => {
  const response = await api.post(
    `/api/projects/${projectId}/users`,
    { userId, role },
    {
      headers: authHeader(),
    }
  );
  return response.data;
};

export const fetchProjectUsersFromApi = async (projectId) => {
  const response = await api.get(`/api/projects/${projectId}/users`, {
    headers: authHeader(),
  });
  return response.data;
};
