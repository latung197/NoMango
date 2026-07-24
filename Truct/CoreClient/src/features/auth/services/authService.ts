// src/features/auth/services/authService.ts

import apiClient from '@/services/api/apiClient'

import type {
  LoginRequest,
  LoginResponse,
} from '../types/auth.types'

export const login = async (
  request: LoginRequest,
): Promise<LoginResponse> => {
  const response = await apiClient.post<LoginResponse>(
    '/auth/login',
    request,
  )

  return response.data
}

export const logout = async (): Promise<void> => {
  await apiClient.post('/auth/logout')
}

export const getCurrentUser = async () => {
  const response = await apiClient.get('/auth/me')

  return response.data
}