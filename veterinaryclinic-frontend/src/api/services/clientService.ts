import api from '../axios'

export interface ClientResponseDto {
  userId: number
  isApproved: boolean
  id: number
  address: string
  name: string
  email: string
  phone: string
}

export interface ClientUpdateDto {
  address: string
  name: string
  phone: string
}

export const getAllClients = async (): Promise<ClientResponseDto[]> => {
  const response = await api.get<ClientResponseDto[]>('/Clients')
  return response.data
}

export const getClientProfile = async (): Promise<ClientResponseDto> => {
  const response = await api.get<ClientResponseDto>('/Clients/profile')
  return response.data
}

export const updateClientProfile = async (
  data: ClientUpdateDto
): Promise<void> => {
  await api.put('/Clients/profile', data)
}
// clientService.ts
export const deleteClient = async (id: number): Promise<void> => {
  await api.delete(`/Clients/${id}`)
}