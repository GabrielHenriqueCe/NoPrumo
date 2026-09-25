/*
  Job roles (função). Each one belongs to a department.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /jobroles?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /jobroles/{id}
      200 item

    POST   /jobroles                { name, departmentId }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /jobroles/{id}           { name, departmentId }
      200 item
*/

export function createJobRoleGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/jobroles', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/jobroles/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/jobroles', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/jobroles/${id}`, item, { signal })
    },
  }
}
