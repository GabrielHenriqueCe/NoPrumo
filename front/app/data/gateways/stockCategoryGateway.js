/*
  Stock categories. `tracksProjectBalance` and `requiresReturn` define the
  three stock flows: consumables (balance per project), PPE (warehouse →
  employee) and tools (go out and come back). Those three come from the seed;
  the screen only adds more.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /stockcategories?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /stockcategories/{id}
      200 item

    POST   /stockcategories                { name, tracksProjectBalance, requiresReturn }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /stockcategories/{id}           { name, tracksProjectBalance, requiresReturn }
      200 item
*/

export function createStockCategoryGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/stockcategories', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/stockcategories/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/stockcategories', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/stockcategories/${id}`, item, { signal })
    },
  }
}
