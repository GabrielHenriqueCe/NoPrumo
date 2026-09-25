/*
  Training types (NR-35, NR-10, ASO...). The defaults come from the seed;
  the screen only adds more.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /trainingtypes?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /trainingtypes/{id}
      200 item

    POST   /trainingtypes                { code, name, validityMonths, minWorkloadHours, requiresInPerson }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /trainingtypes/{id}           { code, name, validityMonths, minWorkloadHours, requiresInPerson }
      200 item
*/

export function createTrainingTypeGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/trainingtypes', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/trainingtypes/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/trainingtypes', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/trainingtypes/${id}`, item, { signal })
    },
  }
}
