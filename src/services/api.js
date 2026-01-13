import axios from 'axios';

// Base API client configured for backend
const api = axios.create({
    baseURL: 'http://localhost:5041/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json'
    }
});

// Add auth token to requests if available
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('access_token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Handle errors globally
api.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error('API Error:', error.response?.data || error.message);

        // If 401, could redirect to login
        if (error.response?.status === 401) {
            localStorage.removeItem('access_token');
            localStorage.removeItem('user_id');
            localStorage.removeItem('twitch_id');
            // window.location.href = '/';
        }

        return Promise.reject(error);
    }
);

// API endpoints
export const configApi = {
    getGameConfig: () => api.get('/config')
};

export const authApi = {
    validate: () => api.get('/auth/validate')
};

export const userApi = {
    getUser: (id) => api.get(`/user/${id}`),
    getUserByTwitchId: (twitchId) => api.get(`/user/twitch/${twitchId}`), // For overlay - uses Twitch ID lookup
    getProgress: (userId) => api.get(`/progress/${userId}`),
    updateWave: (userId, wave) => api.post(`/progress/${userId}/wave`, { wave }),
    recordDefeat: (userId) => api.post(`/progress/${userId}/defeat`),
    saveMonsterState: (userId, monsterHp, lastModified = null) =>
        api.post(`/progress/${userId}/state`, { monsterHp, lastModified })
};

export const statsApi = {
    getStats: (userId) => api.get(`/stats/${userId}`),
    applyBuff: (userId, buff) => api.post(`/stats/${userId}/buff`, buff),
    resetMonthly: (userId) => api.post(`/stats/${userId}/reset-monthly`)
};

export const rankingsApi = {
    getGlobal: () => api.get('/rankings/global'),
    getUserRank: (userId) => api.get(`/rankings/rank/${userId}`)
};

export const streamApi = {
    getStatus: (twitchId) => api.get(`/stream/status/${twitchId}`)
};

export const userConfigApi = {
    get: (twitchId) => api.get(`/userconfig/${twitchId}`),
    save: (twitchId, config) => api.post(`/userconfig/${twitchId}`, config)
};

export default api;
