import api from '../axios'

export interface AppointmentResponseDto {
  id: number
  date: string
  status: number
  type: number
  notes: string
  clientName: string
  doctorName: string
  petName: string
}

export interface AppointmentCreateDto {
  date: string
  type: number
  notes: string
  clientId: number
  doctorId: number
  petId: number
}

export interface AppointmentUpdateDto {
  date: string
  status: number
  type: number
  notes: string
  doctorId: number
}

export interface AppointmentDetailsClientDto {
  id: number
  name: string
  email: string
  phone: string
  address: string
}

export interface AppointmentDetailsPetDto {
  id: number
  name: string
  species: string
  breed: string
  birthdate: string | null
  vaccines: string
  imageUrl: string
  status: number
}

export interface AppointmentDetailsDoctorDto {
  id: number
  name: string
  email: string
  specialization: string
}

export interface AppointmentDetailsDto {
  id: number
  date: string
  status: number
  type: number
  notes: string
  client: AppointmentDetailsClientDto
  pet: AppointmentDetailsPetDto
  doctor: AppointmentDetailsDoctorDto
}

export const getAllAppointments = async (): Promise<AppointmentResponseDto[]> => {
  const response = await api.get<AppointmentResponseDto[]>('/Appointments')
  return response.data
}

export const getDoctorAppointments = async (): Promise<AppointmentResponseDto[]> => {
  const response = await api.get<AppointmentResponseDto[]>('/Appointments')
  return response.data
}

export const getAppointmentById = async (
  id: number
): Promise<AppointmentResponseDto> => {
  const response = await api.get<AppointmentResponseDto>(`/Appointments/${id}`)
  return response.data
}

export const getAppointmentDetails = async (
  id: number
): Promise<AppointmentDetailsDto> => {
  const response = await api.get<AppointmentDetailsDto>(`/Appointments/${id}/details`)
  return response.data
}

export const createAppointment = async (
  data: AppointmentCreateDto
): Promise<void> => {
  await api.post('/Appointments', data)
}

export const updateAppointment = async (
  id: number,
  data: AppointmentUpdateDto
): Promise<void> => {
  await api.put(`/Appointments/${id}`, data)
}

export const deleteAppointment = async (id: number): Promise<void> => {
  await api.delete(`/Appointments/${id}`)
}