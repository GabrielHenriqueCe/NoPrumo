/*
  Training types (NR-35, NR-10, ASO...). The defaults are meant to come from
  the seed; the screen only adds more.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  `validityMonths` is what the API uses to set the expiry date of every
  training recorded from then on; changing it does not move the expiry of
  trainings already saved. Null means the certificate never expires.

  Contract (confirmed against TrainingTypesController.cs):

    GET    /trainingtypes?page&size&search
      200 { items, page, size, total, totalPages }
      item = { id, code, name, validityMonths, minWorkloadHours, requiresInPerson }
      search matches code or name; ordered by code

    GET    /trainingtypes/{id}
      200 item

    POST   /trainingtypes                { code, name, validityMonths, minWorkloadHours, requiresInPerson }
      201 item — the code comes back trimmed and upper-cased
      400 ProblemDetails with `errors` per field

    PUT    /trainingtypes/{id}           { code, name, validityMonths, minWorkloadHours, requiresInPerson }
      200 item
*/

export function createTrainingTypeGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/trainingtypes', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/trainingtypes/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/trainingtypes', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/trainingtypes/${id}`, item, { signal })
    },
  }
}
