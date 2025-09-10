import { ApiError } from '@/errors/ApiError';
import { NetworkError } from '@/errors/NetworkError';
import type ApiException from '@/models/ApiException';
import { useAuthStore } from '@/stores/authStore';
import axios, { AxiosError } from 'axios';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 10000,
});

// Error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const axiosErr = error as AxiosError;

    if (axiosErr.response?.data) {
      const data = axiosErr.response.data as ApiException;
      throw new ApiError(data);
    } else if (axiosErr.response) {
      const exception: ApiException = {
        status: 500,
        error: 'InternalServerException',
        title: 'An unhandled error occurred',
        description: null,
      };
      throw new ApiError(exception);
    } else {
      throw new NetworkError();
    }
  },
);

// Auth handling
apiClient.interceptors.request.use((config) => {
  const authStore = useAuthStore();
  config.headers.Authorization = `Bearer ${authStore.token}`;
  return config;
});
