// axios.js (new file)
// OR in main.js if you don't want extra file

import axios from 'axios';
import { useAuthStore } from '@/stores/authStore';
import router from '@/router'; // adjust path if needed

// Create axios instance
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL, // Use your API URL from .env file
});

// Intercept response
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      const auth = useAuthStore();
      auth.logout();
      router.push('/login');
    }
    return Promise.reject(error);
  }
);

export default api;
