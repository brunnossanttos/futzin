import axios from 'axios';

const API_URL = 'http://localhost:5266/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authAPI = {
  register: (data) => api.post('/auth/register', data),
  login: (data) => api.post('/auth/login', data),
};

export const usersAPI = {
  getMe: () => api.get('/users/me'),
  getAll: () => api.get('/users'),
  getById: (id) => api.get(`/users/${id}`),
  update: (id, data) => api.put(`/users/${id}`, data),
  delete: (id) => api.delete(`/users/${id}`),
  changePassword: (data) => api.post('/users/change-password', data),
};

export const peladasAPI = {
  getAll: () => api.get('/peladas'),
  getActive: () => api.get('/peladas/active'),
  getUpcoming: () => api.get('/peladas/upcoming'),
  getMyPeladas: () => api.get('/peladas/my-peladas'),
  getMyParticipations: () => api.get('/peladas/my-participations'),
  getById: (id) => api.get(`/peladas/${id}`),
  getDetails: (id) => api.get(`/peladas/${id}/details`),
  create: (data) => api.post('/peladas', data),
  update: (id, data) => api.put(`/peladas/${id}`, data),
  delete: (id) => api.delete(`/peladas/${id}`),
  join: (id, inviteToken) => api.post(`/peladas/${id}/join`, inviteToken ? { inviteToken } : {}),
  leave: (id) => api.post(`/peladas/${id}/leave`),
  getParticipants: (id) => api.get(`/peladas/${id}/participants`),
  updatePaymentStatus: (peladaId, userId, hasPaid) =>
    api.patch(`/peladas/${peladaId}/participants/${userId}/payment`, { hasPaid }),
};

export const teamsAPI = {
  getByPelada: (peladaId) => api.get(`/teams/pelada/${peladaId}`),
  generate: (peladaId) => api.post(`/teams/pelada/${peladaId}/generate`),
  clear: (peladaId) => api.delete(`/teams/pelada/${peladaId}`),
};

export const invitesAPI = {
  getByPelada: (peladaId) => api.get(`/peladas/${peladaId}/invites`),
  invite: (peladaId, email) => api.post(`/peladas/${peladaId}/invites`, { email }),
  remove: (peladaId, inviteId) => api.delete(`/peladas/${peladaId}/invites/${inviteId}`),
};

export default api;
