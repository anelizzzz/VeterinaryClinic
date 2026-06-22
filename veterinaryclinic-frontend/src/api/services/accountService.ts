import api from '../axios'

export interface UserAccountDto {
  id: number
  name: string
  email: string
  phone: string
  role: number
  isApproved: boolean
  isRejected: boolean
}
export interface UserUpdateDto {
  name: string
  email: string
  phone: string
  role: number
}
export const getAllUsers = async (): Promise<UserAccountDto[]> => {
  const response = await api.get<UserAccountDto[]>('/User')
  return response.data
}
export const getUserById = async (id: number): Promise<UserAccountDto> => {
  const response = await api.get<UserAccountDto>(`/User/${id}`)
  return response.data
}
export const updateUser = async (id: number, data: UserUpdateDto): Promise<void> => {
  await api.put(`/User/${id}`, data)
}
export const deleteUser = async (id: number): Promise<void> => {
  await api.delete(`/User/${id}`)
}
export async function getMyProfile(): Promise<UserAccountDto> {
  const response = await api.get('/User/me')
  return response.data
}

export async function updateMyProfile(payload: UserUpdateDto): Promise<UserAccountDto> {
  const response = await api.put('/User/me', payload)
  return response.data
}