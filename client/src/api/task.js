import api from "@/api/axios";

const authHeader = () => ({
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

export const fetchTasksByProjectFromApi = async (projectId, queryParams) => {
  try {
    const response = await api.get(`/api/tasks/project/${projectId}`, {
      headers: authHeader(),
      params: queryParams,
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching tasks by project:", error);
    throw error;
  }
};

export const fetchTaskByIdFromApi = async (taskId) => {
  try {
    const response = await api.get(`/api/tasks/${taskId}`, {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching task by ID:", error);
    throw error;
  }
};

export const createTaskInApi = async (projectId, title, description) => {
  try {
    const response = await api.post(
      "/api/tasks",
      { projectId, title, description },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error creating task:", error);
    throw error;
  }
};

export const updateTaskStatusInApi = async (taskId, newStatus) => {
  try {
    const response = await api.put(
      `/api/tasks/${taskId}/status`,
      { newStatus },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error updating task status:", error);
    throw error;
  }
};

export const AddTimeLogToTaskInApi = async (taskId, startTime, endTime) => {
  try {
    const response = await api.post(
      `/api/tasks/${taskId}/log-time`,
      { startTime, endTime },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error adding time log to task:", error);
    throw error;
  }
};

export const fetchTimeLogsByTaskFromApi = async (taskId, onlyMine = false) => {
  try {
    const response = await api.get(`/api/tasks/${taskId}/logs`, {
      headers: authHeader(),
      params: {
        onlyMine,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching time logs by task:", error);
    throw error;
  }
};

export const updateTaskInApi = async (
  taskId,
  projectId,
  title,
  description
) => {
  try {
    const response = await api.put(
      `/api/tasks/${taskId}`,
      {
        projectId,
        title,
        description,
      },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error updating task:", error);
    throw error;
  }
};

export const deleteTaskFromApi = async (taskId) => {
  try {
    const response = await api.delete(`/api/tasks/${taskId}`, {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error deleting task:", error);
    throw error;
  }
};

export const assignTaskToUserInApi = async (taskId, userId) => {
  try {
    const response = await api.post(
      `/api/tasks/${taskId}/assign`,
      { userId },
      {
        headers: authHeader(),
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error assigning task to user:", error);
    throw error;
  }
};

export const unassignTaskFromUserInApi = async (taskId) => {
  try {
    const response = await api.post(`/api/tasks/${taskId}/unassign`, {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error unassigning task from user:", error);
    throw error;
  }
};

export const fetchTaskStatusOptionsFromApi = async () => {
  try {
    const response = await api.get("/api/tasks/statuses", {
      headers: authHeader(),
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching task status options:", error);
    throw error;
  }
};
