import api from '../axios'

export const TestType = {
  BloodTest: 0,
  UrineTest: 1,
  XRay: 2,
  Ultrasound: 3
} as const

export type TestType = (typeof TestType)[keyof typeof TestType]

export interface LabResultCreateDto {
  testType: TestType
  filePath: string
  keyValues: string
  interpretation: string
  petId: number
}

export interface LabResultResponseDto {
  id: number
  testType: TestType
  date: string
  filePath: string
  keyValues: string
  interpretation: string
  petName: string
  species: string
  breed: string
  ageYears: number
  vaccines: string
}

export const createLabResult = async (
  data: LabResultCreateDto
): Promise<LabResultResponseDto> => {
  const response = await api.post<LabResultResponseDto>('/LabResults', data)
  return response.data
}

export const getLabResultsByPetId = async (
  petId: number
): Promise<LabResultResponseDto[]> => {
  const response = await api.get<LabResultResponseDto[]>(`/LabResults/pet/${petId}`)
  return response.data
}