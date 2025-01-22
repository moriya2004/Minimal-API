import axios from 'axios';

// Set the default API base URL using Config Defaults
axios.defaults.baseURL = "http://localhost:5030";

// Add an interceptor to catch errors in the response and log them
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error("API Error:", error.response?.data || error.message);
    return Promise.reject(error);
  }
);

const apiService = {
  getTasks: async () => {
    try {
      const result = await axios.get('/items');
      return result.data;
    } catch (error) {
      console.error("Error in getTasks:", error);
      throw error;
    }
  },

  addTask: async (name) => {
    try {
      const result = await axios.post('/items', { 
        name, 
        isComplete: false // Set isComplete to false by default
      });
      return result.data;
    } catch (error) {
      console.error("Error in addTask:", error);
      throw error;
    }
  },

  setCompleted: async (id, isComplete) => {
    try {
      const result = await axios.put(`/items/${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error("Error in setCompleted:", error);
      throw error;
    }
  },

  deleteTask: async (id) => {
    try {
      const result = await axios.delete(`/items/${id}`);
      return result.data;
    } catch (error) {
      console.error("Error in deleteTask:", error);
      throw error;
    }
  },
};

export default apiService;
