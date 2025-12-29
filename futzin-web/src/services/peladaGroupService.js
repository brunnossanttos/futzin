import api from './api';

export const peladaGroupService = {
  async getById(id) {
    const response = await api.get(`/peladagroup/${id}`);
    return response.data;
  },

  async getUserGroups(userId) {
    const response = await api.get(`/peladagroup/user/${userId}`);
    return response.data;
  },

  async create(data) {
    const response = await api.post('/peladagroup', data);
    return response.data;
  },

  async update(id, data) {
    const response = await api.put(`/peladagroup/${id}`, data);
    return response.data;
  },

  async delete(id) {
    await api.delete(`/peladagroup/${id}`);
  },

  async addMember(groupId, userId, isAdmin = false) {
    const response = await api.post(`/peladagroup/${groupId}/members`, {
      userId,
      isAdmin
    });
    return response.data;
  },

  async removeMember(groupId, userId) {
    await api.delete(`/peladagroup/${groupId}/members/${userId}`);
  }
};
