/*
  Projects (obra).

  `contractAmount` is only sent to and accepted from whoever holds
  `view_finance` — separate DTO per profile. "Late" is not a status: it is
  calculated by comparing `forecastDate` with today, never stored.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /projects?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /projects/{id}
      200 item

    POST   /projects                { code, clientId, name, cno, address..., contractAmount*,
                                   status, supervisorId, technicalManager, creaRt,
                                   startDate, forecastDate }   * only with view_finance
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /projects/{id}           { code, clientId, name, cno, address..., contractAmount*,
                                   status, supervisorId, technicalManager, creaRt,
                                   startDate, forecastDate }   * only with view_finance
      200 item

    PATCH  /projects/{id}/activate
    PATCH  /projects/{id}/deactivate
      204 no content
*/

export function createProjectGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/projects', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/projects/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/projects', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/projects/${id}`, item, { signal })
    },

    /** Switched off, never deleted: the history has to stay. */
    setActive(id, active, { signal } = {}) {
      return http.patch(`/projects/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },
  }
}
