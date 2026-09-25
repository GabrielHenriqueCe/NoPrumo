/*
  Stock items.

  Money is a separate permission: `referencePrice` is only sent to and accepted
  from whoever holds `view_finance`. The foreman and the warehouse keeper
  manage stock by quantity and never see the price.

  Proposed contract — the controller does not exist yet. Whoever writes it
  may change this; if so, update this comment together.

    GET    /stockitems?page&size&search
      200 { items, page, size, total, totalPages }

    GET    /stockitems/{id}
      200 item

    POST   /stockitems                { stockGroupId, code, name, unit, minQuantity,
                                   referencePrice*, ca, caExpiryDate }
                                   * only with view_finance
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /stockitems/{id}           { stockGroupId, code, name, unit, minQuantity,
                                   referencePrice*, ca, caExpiryDate }
                                   * only with view_finance
      200 item

    PATCH  /stockitems/{id}/activate
    PATCH  /stockitems/{id}/deactivate
      204 no content
*/

export function createStockItemGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/stockitems', { params: { page, size, search }, signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/stockitems/${id}`, { signal })
    },

    create(item, { signal } = {}) {
      return http.post('/stockitems', item, { signal })
    },

    update(id, item, { signal } = {}) {
      return http.put(`/stockitems/${id}`, item, { signal })
    },

    /** Switched off, never deleted: the history has to stay. */
    setActive(id, active, { signal } = {}) {
      return http.patch(`/stockitems/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },
  }
}
