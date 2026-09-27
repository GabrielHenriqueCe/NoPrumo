export function createJobRoleGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/job-roles', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/job-roles/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/job-roles', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/job-roles/${id}`, item, { signal })
    },
    
    // Adicione o método de remoção (soft delete)
    remove(id, { signal } = {}) {
      return http.remove(`/job-roles/${id}`, { signal })
    }
  }
}