/*
  Stock groups. Each one belongs to a stock category.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /stockgroups?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /stockgroups/{id}
      200 item

    POST   /stockgroups                { name, stockCategoryId }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /stockgroups/{id}           { name, stockCategoryId }
      200 item
*/

export function createStockGroupGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/stockgroups', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/stockgroups/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/stockgroups', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/stockgroups/${id}`, item, { signal })
    },
  }
}
