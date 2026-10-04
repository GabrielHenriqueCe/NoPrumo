/*
  Stages (etapa da obra). Always belong to a project.
  "Late" is calculated from `plannedDate`, never stored in `status`.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Contract (confirmed against StagesController.cs):

    GET    /stages?page&size&search&projectId
      200 { items, page, size, total, totalPages }

    item   { id, projectId, projectName, name, sortOrder, teamId, teamName,
             supervisorId, supervisorName, plannedDate, startDate,
             completionDate, percentage, status, notes, late }

    status planned · in_progress · completed

    GET    /stages/{id}
      200 item

    POST   /stages                { projectId, name, sortOrder, teamId, supervisorId,
                                   plannedDate, percentage, status }
      201 item
      400 ProblemDetails with `errors` per field
          (projectId must be an active project; completed requires 100%)

    PUT    /stages/{id}           same body as POST
      200 item
*/

export function createStageGateway(http) {
  return {
    list({ page = 1, size = 10, search = '', projectId } = {}, { signal } = {}) {
      const params = projectId ? { page, size, search, projectId } : { page, size, search }
      return http.get('/stages', { params, signal })
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
