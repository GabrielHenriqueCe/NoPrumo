export function createPurchaseRequestGateway(http) {
  return {
    list({ page = 1, size = 10, projectId, status } = {}, { signal } = {}) {
      return http.get('/purchase-requests', {
        params: { page, size, projectId, status },
        signal,
      })
    },

    getById(id, { signal } = {}) {
      return http.get(`/purchase-requests/${id}`, { signal })
    },

    create(data, { signal } = {}) {
      return http.post('/purchase-requests', data, { signal })
    },

    decide(id, { approved, rejectionReason }, { signal } = {}) {
      return http.post(`/purchase-requests/${id}/decide`, { approved, rejectionReason }, { signal })
    },

    registerPurchase(id, data, { signal } = {}) {
      return http.post(`/purchase-requests/${id}/purchase`, data, { signal })
    },
  }
}
