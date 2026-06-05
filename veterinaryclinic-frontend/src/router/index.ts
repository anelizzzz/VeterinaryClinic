import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { UserRole } from '../types/auth'

const routes = [
  // LANDING pentru NON-LOGAT
  {
    path: '/',
    name: 'Landing',
    component: () => import('../views/DashboardView.vue')
  },

  // PAGINI PUBLICE
  {
    path: '/despre-noi',
    name: 'DespreNoi',
    component: () => import('../views/DespreNoiView.vue')
  },
  {
    path: '/contact',
    name: 'Contact',
    component: () => import('../views/ContactView.vue')
  },

  // PAGINA ADOPTII
  {
    path: '/adoptii',
    name: 'adoption',
    component: () => import('../views/client/AdoptionView.vue')
  },

  // DASHBOARD pentru LOGAT
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('../views/Homeview.vue'),
    meta: { requiresAuth: true }
  },

  // ADMIN
  {
    path: '/admin/pets',
    name: 'admin-pets',
    component: () => import('../views/admin/PetView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },
  {
    path: '/admin/accounts',
    name: 'admin-accounts',
    component: () => import('../views/admin/AccountsView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },
  {
    path: '/admin/profile',
    name: 'admin-profile',
    component: () => import('../views/admin/AdminProfileView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },
  {
    path: '/admin/appointments',
    name: 'admin-appointments',
    component: () => import('../views/admin/AppointmentView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },
  {
    path: '/admin/accounts/users/edit/:id',
    name: 'admin-edit-user',
    component: () => import('../views/admin/EditUserView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },
  {
    path: '/admin/adoption-requests',
    name: 'admin-adoption-requests',
    component: () => import('../views/admin/AdoptionRequestsView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },

  // DOCTOR
  {
    path: '/doctor/patients',
    name: 'doctor-patients',
    component: () => import('../views/doctor/DoctorPatientsView.vue'),
    meta: { requiresAuth: true, role: UserRole.Doctor }
  },
  {
    path: '/doctor/appointments',
    name: 'doctor-appointments',
    component: () => import('../views/doctor/DoctorAppointmentView.vue'),
    meta: { requiresAuth: true, role: UserRole.Doctor }
  },
  {
    path: '/doctor/appointments/:id',
    name: 'doctor-appointment-details',
    component: () => import('../views/doctor/DoctorAppointmentDetailsView.vue'),
    meta: { requiresAuth: true, role: UserRole.Doctor }
  },
  {
    path: '/doctor/lab-results/create/:petId',
    name: 'doctor-lab-result-create',
    component: () => import('../views/doctor/DoctorLabResultCreateView.vue'),
    meta: { requiresAuth: true, roles: ['Doctor'] }
  },
  {
    path: '/doctor/lab-results/:petId',
    name: 'doctor-lab-results',
    component: () => import('../views/doctor/DoctorLabResultsView.vue'),
    meta: { requiresAuth: true, roles: ['Doctor'] }
  },
  {
    path: '/doctor/medical-records/create/:petId',
    name: 'doctor-medical-record-create',
    component: () => import('../views/doctor/DoctorMedicalRecordCreateView.vue'),
    meta: { requiresAuth: true, roles: ['Doctor'] }
  },
  {
    path: '/doctor/medical-records/:petId',
    name: 'doctor-medical-records',
    component: () => import('../views/doctor/DoctorMedicalRecordsView.vue'),
    meta: { requiresAuth: true, roles: ['Doctor'] }
  },
  {
    path: '/doctor/profile',
    name: 'doctor-profile',
    component: () => import('../views/doctor/DoctorProfileView.vue'),
    meta: { requiresAuth: true, role: UserRole.Doctor }
  },
  {
    path: '/doctor/profile/edit',
    name: 'doctor-profile-edit',
    component: () => import('../views/doctor/DoctorProfileEditView.vue'),
    meta: { requiresAuth: true, role: UserRole.Doctor }
  },
  {
    path: '/doctor/patients/:petId',
    name: 'doctor-patient-details',
    component: () => import('../views/doctor/DoctorPatientDetailsView.vue'),
    meta: { requiresAuth: true, roles: ['Doctor'] }
  },

  // CLIENT
  {
    path: '/my-pets',
    name: 'client-pets',
    component: () => import('../views/client/ClientPetView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/my-appointments',
    name: 'client-appointments',
    component: () => import('../views/client/ClientAppointmentView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/appointments/create',
    name: 'client-appointment-create',
    component: () => import('../views/client/AppointmentView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/client/profile',
    name: 'client-profile',
    component: () => import('../views/client/ClientProfileView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/client/profile/edit',
    name: 'client-profile-edit',
    component: () => import('../views/client/ClientProfileEditView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/client/pets/create',
    name: 'client-create-pet',
    component: () => import('../views/client/CreatePetView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/pets/edit/:id',
    name: 'pet-edit',
    component: () => import('../views/client/PetEditView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/client/adoption-requests',
    name: 'my-adoption-requests',
    component: () => import('../views/client/MyAdoptionRequestsView.vue'),
    meta: { requiresAuth: true, role: UserRole.Client }
  },
  {
    path: '/admin/adoptions',
    name: 'admin-adoptions',
    component: () => import('../views/admin/AdminAdoptionsView.vue'),
    meta: { requiresAuth: true, role: UserRole.Admin }
  },
  {
    path: '/client/pets/:petId/medical-records',
    name: 'client-medical-records',
    component: () => import('../views/client/ClientMedicalRecordsView.vue'),
    meta: { requiresAuth: true, roles: ['Client'] }
  },
  {
    path: '/client/pets/:petId/lab-results',
    name: 'client-lab-results',
    component: () => import('../views/client/ClientLabResultsView.vue'),
    meta: { requiresAuth: true, roles: ['Client'] }
  },

  // COMUNE
  {
    path: '/profile',
    name: 'Profile',
    component: () => import('../views/ProfileView.vue'),
    meta: { requiresAuth: true }
  },

  // AUTH
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/LoginView.vue')
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('../views/RegisterView.vue')
  },

  // FALLBACK
  {
    path: '/:pathMatch(.*)*',
    redirect: '/dashboard'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const authStore = useAuthStore()
  
  authStore.loadUser() // ← adaugă această linie
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return '/login'
  }

  if (
    (to.path === '/login' || to.path === '/register') &&
    authStore.isAuthenticated
  ) {
    return '/dashboard'
  }

  if (to.meta.role !== undefined && authStore.user?.role !== to.meta.role) {
    return '/dashboard'
  }
})

export default router