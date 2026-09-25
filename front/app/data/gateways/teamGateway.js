/*
  Teams (equipe). Members are managed inside the team screen, not on a
  screen of their own.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /teams?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /teams/{id}
      200 item

    POST   /teams                { name, departmentId }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /teams/{id}           { name, departmentId }
      200 item

    PATCH  /teams/{id}/activate
    PATCH  /teams/{id}/deactivate
      204 no content

    GET    /teams/{id}/members
      200 [ { id, employeeId, employeeName, startDate, endDate } ]

    POST   /teams/{id}/members        { employeeId, startDate }
      201 member

    PATCH  /teams/{id}/members/{memberId}/end   { endDate }
      204 no content
*/

export function createTeamGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/teams', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/teams/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/teams', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/teams/${id}`, item, { signal })
    },

    /** Switched off, never deleted: the history has to stay. */
    setActive(id, active, { signal } = {}) {
      return http.patch(`/teams/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },

    listMembers(teamId, { signal } = {}) {
      return http.get(`/teams/${teamId}/members`, { signal })
    },

    addMember(teamId, member, { signal } = {}) {
      return http.post(`/teams/${teamId}/members`, member, { signal })
    },

    endMember(teamId, memberId, endDate, { signal } = {}) {
      return http.patch(`/teams/${teamId}/members/${memberId}/end`, { endDate }, { signal })
    },
  }
}
