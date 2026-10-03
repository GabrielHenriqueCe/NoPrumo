/*
  Teams (equipe). Members are managed inside the team screen, not on a
  screen of their own.

  A team is switched off, never deleted. A member leaves with an end date
  instead of disappearing: the row is the record of who was where, and when.

  Contract (confirmed against TeamsController.cs):

    GET    /teams?page&size&search&departmentId
      200 { items, page, size, total, totalPages }
      team = { id, name, departmentId, departmentName, active, memberCount }
      search matches the team or the department name; memberCount counts
      only the open stints (no endDate)

    GET    /teams/{id}
      200 team

    POST   /teams                { name, departmentId }
      201 team
      400 ProblemDetails with `errors` per field

    PUT    /teams/{id}           { name, departmentId }
      200 team

    PATCH  /teams/{id}/activate
    PATCH  /teams/{id}/deactivate
      204 no content
      409 ProblemDetails — deactivating a team that still has current
          members, or activating one whose name an active team has taken

    GET    /teams/{id}/members
      200 [ { employeeId, employeeName, registrationNumber, startDate, endDate } ]
      every stint: the current ones first, then the most recent exits

    POST   /teams/{id}/members        { employeeId, startDate }
      201 member
      400 ProblemDetails with `errors` per field
      409 the team is inactive

    PATCH  /teams/{id}/members/{employeeId}/end   { endDate }
      200 member — closes that employee's open stint
      404 the employee has no open stint in this team

  There is no member id: the table's key is (employee, team, startDate), and
  an employee has at most one open stint per team, so the employee picks it.
*/

export function createTeamGateway(http) {
  return {
    list({ page = 1, size = 10, search = '', departmentId = null } = {}, { signal } = {}) {
      return http.get('/teams', {
        params: { page, size, search, departmentId: departmentId ?? undefined },
        signal,
      })
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

    /** Closes the stint with a date; the row stays as history. */
    endMember(teamId, employeeId, endDate, { signal } = {}) {
      return http.patch(`/teams/${teamId}/members/${employeeId}/end`, { endDate }, { signal })
    },
  }
}
