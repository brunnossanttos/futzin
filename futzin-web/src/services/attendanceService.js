import api from './api';

export const attendanceService = {
  async getPeladaAttendances(peladaId) {
    const response = await api.get(`/peladaattendance/pelada/${peladaId}`);
    return response.data;
  },

  async getUserAttendances(userId) {
    const response = await api.get(`/peladaattendance/user/${userId}`);
    return response.data;
  },

  async create(peladaId, userId) {
    const response = await api.post('/peladaattendance', {
      peladaId,
      userId
    });
    return response.data;
  },

  async confirm(peladaId, userId) {
    const response = await api.put(`/peladaattendance/${peladaId}/user/${userId}/confirm`);
    return response.data;
  },

  async decline(peladaId, userId) {
    const response = await api.put(`/peladaattendance/${peladaId}/user/${userId}/decline`);
    return response.data;
  },

  async markAttended(peladaId, userId) {
    await api.put(`/peladaattendance/${peladaId}/user/${userId}/attended`);
  },

  async markNoShow(peladaId, userId) {
    await api.put(`/peladaattendance/${peladaId}/user/${userId}/no-show`);
  }
};
