/*
  Schedule (appointment). Training expiries come in the same answer, calculated
  by the API from the certificates — they are never stored as appointments.

  Contract (confirmed against AppointmentsController.cs,
  policy manage_schedule = manage_projects or manage_safety):

    GET    /appointments?from&to&projectId
      200 { from, to, appointments: [appointment], trainingExpiries: [expiry] }
      400 ProblemDetails — from/to required, to >= from, at most 92 days

    appointment { id, date, time, title, description, projectId, projectName,
                  assignedToId, assignedToName, type, completed, completedAt }
    type        project · visit · delivery · inspection · training · meeting
    time        'HH:mm:ss' or null
    expiry      { trainingId, employeeId, employeeName, trainingTypeCode,
                  trainingTypeName, expiryDate, status }
                only the latest certificate of each type; with projectId, the
                people on the teams allocated to that project today

    GET    /appointments/options
      200 { projects: [ { id, name } ], assignees: [ { id, name } ] }

    GET    /appointments/{id}
    POST   /appointments           { date, time, title, description, projectId, assignedToId, type }
      201 appointment · 400 ProblemDetails with `errors` per field
    PUT    /appointments/{id}      same body
      200 appointment
    PATCH  /appointments/{id}/complete
    PATCH  /appointments/{id}/reopen
      200 appointment
*/

export function createAppointmentGateway(http) {
  return {
    agenda({ from, to, projectId = null }, { signal } = {}) {
      return http.get('/appointments', { params: { from, to, projectId: projectId ?? undefined }, signal })
    },

    options({ signal } = {}) {
      return http.get('/appointments/options', { signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/appointments/${id}`, { signal })
    },

    create(appointment, { signal } = {}) {
      return http.post('/appointments', appointment, { signal })
    },

    update(id, appointment, { signal } = {}) {
      return http.put(`/appointments/${id}`, appointment, { signal })
    },

    setCompleted(id, completed, { signal } = {}) {
      return http.patch(`/appointments/${id}/${completed ? 'complete' : 'reopen'}`, null, { signal })
    },
  }
}
