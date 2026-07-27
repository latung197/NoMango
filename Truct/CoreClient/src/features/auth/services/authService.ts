// src/features/auth/services/authService.ts

import api from "../../../configs/api";

import type {
  LoginRequest,
  LoginResponse,
} from '../types/auth.types'

export const login = async (
  request: LoginRequest,
): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>(
    '/auth/login',
    request,
  )

  return response.data
}

export const logout = async (): Promise<void> => {
  await api.post('/auth/logout')
}

export const getCurrentUser = async () => {
  const response = await api.get('/auth/me')

  return response.data
}