export function createDepartmentGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/departments', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/departments/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/departments', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/departments/${id}`, item, { signal })
    },

    remove(id, { signal } = {}) {
      return http.remove(`/departments/${id}`, { signal })
    }
  }
}