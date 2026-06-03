import api from '../axios'

export interface AIDiagnosisRequestDto {
  petName: string
  species: string
  breed: string
  age: number
  diagnosis: string
  treatment: string
  observations: string
  labResults: string[]
}

export interface AIDiagnosisResponseDto {
  possibleCauses: string[]
  urgencyLevel: string
  recommendedNextSteps: string
  confidence: number
  disclaimer: string
}

export const generateAiDiagnosis = async (
  data: AIDiagnosisRequestDto
): Promise<AIDiagnosisResponseDto> => {
  const response = await api.post<AIDiagnosisResponseDto>('/AIDiagnosis', data)
  return response.data
}