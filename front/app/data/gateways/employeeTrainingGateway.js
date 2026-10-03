/*
  Employee trainings — the training record of each employee (NR courses,
  ASO...). Nothing here is deleted or switched off: the table has no `active`
  nor `deleted_at` column, and a certificate is history.

  `expiryDate` and `status` only ever come from the API. The expiry is
  `issueDate` plus the training type's `validityMonths`, set by the back when
  it saves; the front never sends it. `status` compares that date with today:
  'valid' · 'expiring' (30 days or less left) · 'expired' · 'no_expiry'.

  Contract (confirmed against EmployeeTrainingsController.cs):

    GET    /employeetrainings?page&size&search&employeeId&trainingTypeId
      200 { items, page, size, total, totalPages }
      search matches employee name or registration, training code or name,
      and instructor; newest issue date first

    GET    /employeetrainings/{id}
      200 item

    POST   /employeetrainings      { employeeId, trainingTypeId, issueDate,
                                     workloadHours, modality, instructor }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /employeetrainings/{id} same body as POST
      200 item

    item = { id, employeeId, employeeName, registrationNumber,
             trainingTypeId, trainingTypeCode, trainingTypeName,
             issueDate, expiryDate, status, workloadHours, modality, instructor }
    modality: 'in_person' · 'online' · 'blended' · null
*/

export function createEmployeeTrainingGateway(http) {
  return {
    list(
      { page = 1, size = 10, search = '', employeeId = null, trainingTypeId = null } = {},
      { signal } = {},
    ) {
      return http.get('/employeetrainings', {
        params: {
          page,
          size,
          search,
          employeeId: employeeId ?? undefined,
          trainingTypeId: trainingTypeId ?? undefined,
        },
        signal,
      })
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
