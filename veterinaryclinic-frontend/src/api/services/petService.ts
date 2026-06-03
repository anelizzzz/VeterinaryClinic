import api from '../axios'

export interface PetResponseDto {
  id: number
  name: string
  species: string
  breed: string | null
  birthdate: string | null
  status: number
  vaccines: string | null
  imageUrl: string | null
  clientName: string
}

export interface PetCreateDto {
  name: string
  species: string
  breed: string
  birthdate: string | null
  vaccines: string
  imageUrl: string
  clientId?: number
}

export interface PetUpdateDto {
  name: string
  species: string
  breed: string
  birthdate: string | null
  vaccines: string
  status: number
  imageUrl: string
}

export const getPetById = async (id: number): Promise<PetResponseDto> => {
  const response = await api.get<PetResponseDto>(`/Pets/${id}`)
  return response.data
}

export const createPet = async (data: PetCreateDto): Promise<PetResponseDto> => {
  const response = await api.post<PetResponseDto>('/Pets', data)
  return response.data
}

export const updatePet = async (
  id: number,
  data: PetUpdateDto
): Promise<void> => {
  await api.put(`/Pets/${id}`, data)
}

export const getAllPets = async (): Promise<PetResponseDto[]> => {
  const response = await api.get<PetResponseDto[]>('/Pets')
  return response.data
}

export const getDoctorPatients = async (): Promise<PetResponseDto[]> => {
  const response = await api.get<PetResponseDto[]>('/Pets')
  return response.data
}

export const deletePet = async (id: number): Promise<void> => {
  await api.delete(`/Pets/${id}`)
}

export const uploadPetImage = async (petId: number, file: File): Promise<void> => {
  const formData = new FormData()
  formData.append('file', file)

  await api.put(`/Pets/${petId}/image`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}