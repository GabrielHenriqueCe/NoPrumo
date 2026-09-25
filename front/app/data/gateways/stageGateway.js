/*
  Stages (etapa da obra). Always belong to a project.
  "Late" is calculated from `plannedDate`, never stored in `status`.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /stages?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /stages/{id}
      200 item

    POST   /stages                { projectId, name, sortOrder, teamId, supervisorId,
                                   plannedDate, percentage, status }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /stages/{id}           { projectId, name, sortOrder, teamId, supervisorId,
                                   plannedDate, percentage, status }
      200 item
*/

export function createStageGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/stages', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/stages/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/stages', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/stages/${id}`, item, { signal })
    },
  }
}
