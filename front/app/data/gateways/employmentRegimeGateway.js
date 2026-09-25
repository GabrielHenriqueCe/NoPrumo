/*
  Employment regimes (regime de contratação): CLT, hourly, daily.
  The defaults come from the seed; the screen only adds more.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /employmentregimes?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /employmentregimes/{id}
      200 item

    POST   /employmentregimes                { label, unit, monthlyHours, description }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /employmentregimes/{id}           { label, unit, monthlyHours, description }
      200 item
*/

export function createEmploymentRegimeGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/employmentregimes', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/employmentregimes/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/employmentregimes', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/employmentregimes/${id}`, item, { signal })
    },
  }
}
