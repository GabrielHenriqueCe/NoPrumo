/*
  PPE issues (ficha de EPI). The receipt keeps the item's name and CA of the
  day it was issued; each issued item is a stock exit for the employee.
  Nothing is deleted: an item is returned or marked lost.

  Contract (confirmed against PpeIssuesController.cs):

    read  — policy view_ppe: manage_safety or view_stock (foreman, warehouse keeper)
    write — manage_safety

    GET    /ppe-issues?page&size&search&projectId&employeeId
      200 { items: [issue], page, size, total, totalPages }

    issue  { id, employeeId, employeeName, registrationNumber, projectId,
             projectName, date, notes, issuedByName, signed, signedAt,
             items: [ { id, stockItemId, itemName, ca, quantity, unit,
                        status, issueDate, returnDate } ] }
    status issued · returned · lost

    GET    /ppe-issues/options                (manage_safety)
      200 { employees: [ { id, name, registrationNumber } ],
            items: [ { id, name, unit, ca, caExpiryDate, caExpired } ] }   PPE only

    GET    /ppe-issues/{id}
    POST   /ppe-issues          { employeeId, projectId, date, notes,
                                  items: [ { stockItemId, quantity } ] }
      201 issue
      400 ProblemDetails — item problems come together under `items`
          (not PPE, CA expired, duplicated, quantity)

    POST   /ppe-issues/{id}/sign                       { typedName }
    PATCH  /ppe-issues/{id}/items/{itemId}/return      { returnDate }
    PATCH  /ppe-issues/{id}/items/{itemId}/lost
      200 issue · 400 ProblemDetails
*/

export function createPpeIssueGateway(http) {
  return {
    list({ page = 1, size = 10, search = '', projectId = null, employeeId = null } = {}, { signal } = {}) {
      return http.get('/ppe-issues', {
        params: {
          page,
          size,
          search,
          projectId: projectId ?? undefined,
          employeeId: employeeId ?? undefined,
        },
        signal,
      })
    },

    options({ signal } = {}) {
      return http.get('/ppe-issues/options', { signal })
    },

    getById(id, { signal } = {}) {
      return http.get(`/ppe-issues/${id}`, { signal })
    },

    create(issue, { signal } = {}) {
      return http.post('/ppe-issues', issue, { signal })
    },

    sign(id, typedName, { signal } = {}) {
      return http.post(`/ppe-issues/${id}/sign`, { typedName }, { signal })
    },

    returnItem(id, itemId, returnDate, { signal } = {}) {
      return http.patch(`/ppe-issues/${id}/items/${itemId}/return`, { returnDate }, { signal })
    },

    markItemLost(id, itemId, { signal } = {}) {
      return http.patch(`/ppe-issues/${id}/items/${itemId}/lost`, null, { signal })
    },
  }
}
