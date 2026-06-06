import api from '../axios'

export interface DoctorResponseDto {
  userId: number
  isApproved: boolean
  id: number
  specialization: string
  bio: string
  schedule: string
  name: string
  email: string
  phone: string
}

export interface DoctorUpdateDto {
  specialization: string
  bio: string
  schedule: string
  name: string
  phone: string
}


export interface DoctorPatientDto {
  id: number
  name: string
  ownerName: string
  species: string
  breed: string
  age: number
}
export interface DoctorPatientDetailsDto {
  id: number
  name: string
  species: string
  breed: string
  birthdate: string
  status: number
  vaccines: string
  imageUrl: string
  clientId: number
  ownerName: string
}

export const getDoctorPatientDetails = async (
  petId: number
): Promise<DoctorPatientDetailsDto> => {
  const response = await api.get<DoctorPatientDetailsDto>(`/Doctors/patients/${petId}`)
  return response.data
}
export const getDoctorPatients = async (): Promise<DoctorPatientDto[]> => {
  const response = await api.get<DoctorPatientDto[]>('/Doctors/my-patients')
  return response.data
}
export const getAllDoctors = async (): Promise<DoctorResponseDto[]> => {
  const response = await api.get<DoctorResponseDto[]>('/Doctors')
  return response.data
}

// pentru pagina de profil doctor
export const getDoctorProfile = async (): Promise<DoctorResponseDto> => {
  const response = await api.get<DoctorResponseDto>('/Doctors/profile')
  return response.data
}

export const updateDoctorProfile = async (
  data: DoctorUpdateDto
): Promise<void> => {
  await api.put('/Doctors/profile', data)
}
// doctorService.ts
export const deleteDoctor = async (id: number): Promise<void> => {
  await api.delete(`/Doctors/${id}`)
}