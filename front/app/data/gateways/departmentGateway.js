/*
  Departments (setor). Master data for job roles and teams.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /departments?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /departments/{id}
      200 item

    POST   /departments                { name }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /departments/{id}           { name }
      200 item
*/

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
  }
}
