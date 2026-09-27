/*
  Stock items.

  Money is a separate permission: `referencePrice` is only sent to and accepted
  from whoever holds `view_finance`. The foreman and the warehouse keeper
  manage stock by quantity and never see the price.

  Contract (confirmed against StockItemsController.cs):

    GET    /stockitems?page&size&search&stockGroupId
      200 { items, page, size, total, totalPages }
      Each item includes `referencePrice` only for accounts with view_finance.

    POST   /stockitems                { stockGroupId, code, name, unit, minQuantity,
                                   referencePrice*, ca, caExpiryDate }
                                   * only applied for accounts with view_finance;
                                     ignored otherwise, even if sent
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /stockitems/{id}           same body as POST
      200 item

    PATCH  /stockitems/{id}/activate
    PATCH  /stockitems/{id}/deactivate
      204 no content

  There is no GET /{id}: the controller does not expose it, same as the other
  stock screens and users.
*/

export function createStockItemGateway(http) {
    return {
        list({ page = 1, size = 10, search = '', stockGroupId = null } = {}, { signal } = {}) {
            return http.get('/stockitems', {
                params: { page, size, search, stockGroupId: stockGroupId ?? undefined },
                signal,
            })
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