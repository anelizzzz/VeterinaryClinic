import api from '../axios'

export interface DiagnosisResponseDto {
  id: number
  name: string
  description: string
  treatment: string
  observations: string
  species: string
}

export interface DiagnosisCreateDto {
  name: string
  description: string
  treatment: string
  observations: string
  species: string
}

export interface DiagnosisUpdateDto {
  name: string
  description: string
  treatment: string
  observations: string
  species: string
}

export const getAllDiagnoses = async (): Promise<DiagnosisResponseDto[]> => {
  const response = await api.get<DiagnosisResponseDto[]>('/Diagnosis')
  return response.data
}

export const getDiagnosesBySpecies = async (species: string): Promise<DiagnosisResponseDto[]> => {
  const response = await api.get<DiagnosisResponseDto[]>(`/Diagnosis/by-species/${species}`)
  return response.data
}

export const getDiagnosisById = async (id: number): Promise<DiagnosisResponseDto> => {
  const response = await api.get<DiagnosisResponseDto>(`/Diagnosis/${id}`)
  return response.data
}

export const createDiagnosis = async (data: DiagnosisCreateDto): Promise<DiagnosisResponseDto> => {
  const response = await api.post<DiagnosisResponseDto>('/Diagnosis', data)
  return response.data
}

export const updateDiagnosis = async (id: number, data: DiagnosisUpdateDto): Promise<DiagnosisResponseDto> => {
  const response = await api.put<DiagnosisResponseDto>(`/Diagnosis/${id}`, data)
  return response.data
}

export const deleteDiagnosis = async (id: number): Promise<void> => {
  await api.delete(`/Diagnosis/${id}`)
}