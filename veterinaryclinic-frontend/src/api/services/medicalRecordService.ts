import api from '../axios'

export interface MedicalRecordCreateDto {
  petId: number
  doctorId: number
  diagnosis: string
  treatment: string
  observations: string
}

export interface MedicalRecordResponseDto {
  id: number
  date: string
  petName: string
  doctorName: string
  diagnosis: string
  treatment: string
  observations: string
}

export interface MedicalRecordUpdateDto {
  diagnosis: string
  treatment: string
  observations: string
}

export const createMedicalRecord = async (
  data: MedicalRecordCreateDto
): Promise<MedicalRecordResponseDto> => {
  const response = await api.post<MedicalRecordResponseDto>('/MedicalRecords', data)
  return response.data
}

export const getMedicalRecordsByPetId = async (
  petId: number
): Promise<MedicalRecordResponseDto[]> => {
  const response = await api.get<MedicalRecordResponseDto[]>(`/MedicalRecords/pet/${petId}`)
  return response.data
}