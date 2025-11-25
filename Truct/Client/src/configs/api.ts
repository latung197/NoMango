import { getToken } from "@/utils/authUtils";
import axios, { InternalAxiosRequestConfig } from "axios";
import i18n from "i18next";

const BASE_URL = import.meta.env.VITE_API_URL;

const api = axios.create({
  baseURL: BASE_URL,
  timeout: 100000,
  headers: { "X-Custom-Header": "foobar", 'Content-Type': 'application/json'},
});

api.interceptors.request.use(function (config: InternalAxiosRequestConfig ) {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  config.headers["Accept-Language"] = i18n.language || localStorage.getItem("lang") || "ja";
  return config;
}, function (error) {
  return Promise.reject(error);
});

export default api;
