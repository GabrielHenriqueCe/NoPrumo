/*
  Employee trainings. `expiryDate` follows from `issueDate` plus the
  training type's `validityMonths`.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /employeetrainings?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /employeetrainings/{id}
      200 item

    POST   /employeetrainings                { employeeId, trainingTypeId, issueDate, expiryDate,
                                   workloadHours, modality, instructor }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /employeetrainings/{id}           { employeeId, trainingTypeId, issueDate, expiryDate,
                                   workloadHours, modality, instructor }
      200 item
*/

export function createEmployeeTrainingGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/employeetrainings', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/employeetrainings/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/employeetrainings', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/employeetrainings/${id}`, item, { signal })
    },
  }
}
