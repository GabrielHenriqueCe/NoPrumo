export function createEmployeeGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/employees', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/employees/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/employees', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/employees/${id}`, item, { signal })
    },
    
    // Adicione o método remove
    remove(id, { signal } = {}) {
      return http.remove(`/employees/${id}`, { signal })
    }
  }
}