/*
  Stock groups. Each one belongs to a stock category.
  No activate/deactivate: the table has no `active` nor `deleted_at` column.

  Contract (confirmed against StockGroupsController.cs):

    GET    /stockgroups?page&size&search&stockCategoryId
      200 { items, page, size, total, totalPages }

    POST   /stockgroups                 { name, stockCategoryId }
      201 item
      400 ProblemDetails with `errors` per field

    PUT    /stockgroups/{id}            { name, stockCategoryId }
      200 item

  There is no GET /{id}: the controller does not expose it, same as users.
*/

export function createStockGroupGateway(http) {
    return {
        list({ page = 1, size = 10, search = '', stockCategoryId = null } = {}, { signal } = {}) {
            return http.get('/stockgroups', {
                params: { page, size, search, stockCategoryId: stockCategoryId ?? undefined },
                signal,
            })
        },

        create(item, { signal } = {}) {
            return http.post('/stockgroups', item, { signal })
        },

        update(id, item, { signal } = {}) {
            return http.put(`/stockgroups/${id}`, item, { signal })
        },
    }
}