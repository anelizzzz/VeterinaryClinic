import api from '../axios'

export const AdoptionStatus = {
  Available: 0,
  Pending: 1,
  Reserved: 2,
  Adopted: 3,
  Rejected: 4,
  Approved: 5
} as const

export type AdoptionStatus = (typeof AdoptionStatus)[keyof typeof AdoptionStatus]

export interface AdoptionAnimalResponseDto {
  id: number
  name: string
  species: string
  breed: string
  age: number
  description: string
  vaccines: string
  imageUrl: string
  status: number
  addedDate: string
}

export interface AdoptionRequestCreateDto {
  adoptionAnimalId: number
  motivation: string
}

export interface AdoptionRequestResponseDto {
  id: number
  clientName: string
  animalName: string
  requestDate: string
  status: number
  motivation: string
}

export const getAllAdoptionAnimals = async (): Promise<AdoptionAnimalResponseDto[]> => {
  const response = await api.get<AdoptionAnimalResponseDto[]>('/AdoptionAnimals')
  return response.data
}

export const getAdoptionAnimalById = async (id: number): Promise<AdoptionAnimalResponseDto> => {
  const response = await api.get<AdoptionAnimalResponseDto>(`/AdoptionAnimals/${id}`)
  return response.data
}

export const createAdoptionRequest = async (
  data: AdoptionRequestCreateDto
): Promise<AdoptionRequestResponseDto> => {
  const response = await api.post<AdoptionRequestResponseDto>('/AdoptionRequests', data)
  return response.data
}

export const getMyAdoptionRequests = async (): Promise<AdoptionRequestResponseDto[]> => {
  const response = await api.get<AdoptionRequestResponseDto[]>('/AdoptionRequests/my')
  return response.data
}

export const getAllAdoptionRequests = async (): Promise<AdoptionRequestResponseDto[]> => {
  const response = await api.get<AdoptionRequestResponseDto[]>('/AdoptionRequests')
  return response.data
}

export const updateAdoptionRequestStatus = async (
  id: number,
  status: AdoptionStatus
): Promise<void> => {
  await api.put(`/AdoptionRequests/${id}/status`, { status })
}
export interface AdoptionAnimalCreateDto {
  name: string
  species: string
  breed: string
  age: number
  description: string
  vaccines: string
  imageUrl: string
}

export const createAdoptionAnimal = async (
  data: AdoptionAnimalCreateDto
): Promise<AdoptionAnimalResponseDto> => {
  const response = await api.post<AdoptionAnimalResponseDto>('/AdoptionAnimals', data)
  return response.data
}

export const deleteAdoptionAnimal = async (id: number): Promise<void> => {
  await api.delete(`/AdoptionAnimals/${id}`)
}