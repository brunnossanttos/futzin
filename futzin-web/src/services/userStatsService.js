import api from './api';

export const userStatsService = {
  async getUserStats(userId) {
    const response = await api.get(`/userstats/${userId}`);
    return response.data;
  },

  async recalculate(userId) {
    await api.post(`/userstats/${userId}/recalculate`);
  }
};
