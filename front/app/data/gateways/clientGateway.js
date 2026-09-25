export function createClientGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/clients', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/clients/${id}`, { signal })
    },

    create(client, { signal } = {}) {
      return http.post('/clients', client, { signal })
    },

    update(id, client, { signal } = {}) {
      return http.put(`/clients/${id}`, client, { signal })
    },

    setActive(id, active, { signal } = {}) {
      return http.patch(`/clients/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },
  }
}
