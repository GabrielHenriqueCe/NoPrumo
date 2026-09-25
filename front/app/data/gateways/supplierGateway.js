export function createSupplierGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/suppliers', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/suppliers/${id}`, { signal })
    },

    create(supplier, { signal } = {}) {
      return http.post('/suppliers', supplier, { signal })
    },

    update(id, supplier, { signal } = {}) {
      return http.put(`/suppliers/${id}`, supplier, { signal })
    },

    setActive(id, active, { signal } = {}) {
      return http.patch(`/suppliers/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },
  }
}
