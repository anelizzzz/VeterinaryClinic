export const UserRole = {
  Client: 0,
  Admin: 1,
  Doctor: 2
} as const

export type UserRole = (typeof UserRole)[keyof typeof UserRole]

export interface LoginDto {
  email: string
  password: string
}

export interface RegisterDto {
  name: string
  email: string
  password: string
  phone: string
  role: UserRole
  address?: string
  specialization?: string
  bio?: string
}

export interface AuthResponseDto {
  token: string
}