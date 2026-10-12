/*
  Stock movements: the read side the project page needs.

  Contract (confirmed against StockMovementsController.cs):

    GET    /stockmovements/project-balance?projectId=1        policy view_stock
      200 [ { stockItemId, name, unit, minQuantity, balance } ]
      active consumable items; the balance is the sum of the movements on the project
      404 unknown project

    GET    /stockmovements/material-cost?projectId=1&from=&to=   policy view_finance
      200 { projectId, total, items: [ { stockItemId, name, unit, quantity, total } ] }
      only entries on the project, at the unit cost recorded on each entry
      404 unknown project
*/

export function createStockMovementGateway(http) {
  return {
    projectBalance(projectId, { signal } = {}) {
      return http.get(`/stockmovements/project-balance?projectId=${projectId}`, { signal })
    },

    materialCost(projectId, { signal } = {}) {
      return http.get(`/stockmovements/material-cost?projectId=${projectId}`, { signal })
    },
  }
}
