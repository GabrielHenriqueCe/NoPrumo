/*
  Employees (funcionário).

  Money is a separate permission: `payRate` is only sent to and accepted from
  whoever holds `view_finance`. The API returns a different DTO per profile —
  a DTO with the field set to null is not enough, the field must not exist.
  The document (CPF) goes in raw; masking, hashing and encrypting are the API's job.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /employees?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /employees/{id}
      200 item

    POST   /employees                { registrationNumber, name, jobRoleId, employmentRegimeId,
                                   payRate*, additionalPercentage, hireDate, phone,
                                   document }   * only with view_finance
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /employees/{id}           { registrationNumber, name, jobRoleId, employmentRegimeId,
                                   payRate*, additionalPercentage, hireDate, phone,
                                   document }   * only with view_finance
      200 item

    PATCH  /employees/{id}/activate
    PATCH  /employees/{id}/deactivate
      204 no content
*/

export function createEmployeeGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/employees', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/employees/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/employees', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/employees/${id}`, item, { signal })
    },

    /** Switched off, never deleted: the history has to stay. */
    setActive(id, active, { signal } = {}) {
      return http.patch(`/employees/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },
  }
}
