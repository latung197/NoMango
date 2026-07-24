// src/features/auth/types/auth.types.ts

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken?: string
  user: User
}

export interface User {
  id: number
  username: string
  email: string
  fullName: string
  roles: string[]
}
export interface BaseResponse<T> {
  data: T
  message: string
  success: boolean
}