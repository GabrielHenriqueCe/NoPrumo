export function createEmploymentRegimeGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/employment-regimes', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/employment-regimes/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/employment-regimes', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/employment-regimes/${id}`, item, { signal })
    },
    
    // Adicione o método remove
    remove(id, { signal } = {}) {
      return http.remove(`/employment-regimes/${id}`, { signal })
    }
  }
}