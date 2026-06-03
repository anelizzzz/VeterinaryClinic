import { defineStore } from 'pinia'
import api from '../api/axios'
import { UserRole, type LoginDto } from '../types/auth'

interface LoginApiResponse {
  token: string
}

export interface AuthUser {
  token: string
  id: number
  name: string
  email: string
  role: UserRole
}

interface AuthState {
  user: AuthUser | null
}

interface JwtPayload {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'?: string
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | number
  sub?: string
  unique_name?: string
  email?: string
  role?: string | number
}

function decodeJwt(token: string): JwtPayload {
  const payload = token.split('.')[1]
  const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
  const padded = base64.padEnd(base64.length + (4 - (base64.length % 4 || 4)) % 4, '=')

  return JSON.parse(atob(padded))
}

function mapRole(rawRole: string | number): UserRole {
  if (rawRole === 0 || rawRole === '0' || rawRole === 'Client') {
    return UserRole.Client
  }

  if (rawRole === 1 || rawRole === '1' || rawRole === 'Admin') {
    return UserRole.Admin
  }

  if (rawRole === 2 || rawRole === '2' || rawRole === 'Doctor') {
    return UserRole.Doctor
  }

  throw new Error('Rol invalid în token.')
}

function buildUserFromToken(token: string): AuthUser {
  const payload = decodeJwt(token)
  console.log('JWT PAYLOAD:', payload)

  const rawId =
    payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
    payload.sub ??
    ''

  const rawName =
    payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
    payload.unique_name ??
    'Utilizator'

  const rawEmail =
    payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ??
    payload.email ??
    ''

  const rawRole =
    payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
    payload.role ??
    ''

  const id = Number(rawId)

  console.log('rawId:', rawId)
  console.log('rawName:', rawName)
  console.log('rawEmail:', rawEmail)
  console.log('rawRole:', rawRole)

  if (Number.isNaN(id)) {
    throw new Error('ID-ul utilizatorului nu a putut fi extras din token.')
  }

  return {
    token,
    id,
    name: rawName,
    email: rawEmail,
    role: mapRole(rawRole)
  }
}

export const useAuthStore = defineStore('Auth', {
  state: (): AuthState => ({
    user: null
  }),

  getters: {
    isAuthenticated: (state): boolean => !!state.user?.token,
    isAdmin: (state): boolean => state.user?.role === UserRole.Admin,
    isDoctor: (state): boolean => state.user?.role === UserRole.Doctor,
    isClient: (state): boolean => state.user?.role === UserRole.Client
  },

  actions: {
    async login(data: LoginDto): Promise<void> {
      const response = await api.post<LoginApiResponse>('/Auth/login', data)

      const token = response.data.token
      const user = buildUserFromToken(token)

      this.user = user

      localStorage.setItem('token', token)
      localStorage.setItem('user', JSON.stringify(user))
    },

    logout(): void {
      this.user = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    },

    loadUser(): void {
      const user = localStorage.getItem('user')

      if (!user) {
        this.user = null
        return
      }

      try {
        this.user = JSON.parse(user) as AuthUser
      } catch (error) {
        console.error('Nu s-a putut încărca userul din localStorage.', error)
        this.user = null
        localStorage.removeItem('user')
        localStorage.removeItem('token')
      }
    }
  }
})