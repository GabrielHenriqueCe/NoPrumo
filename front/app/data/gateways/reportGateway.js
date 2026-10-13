/*
  Home dashboard and the reports. Every number is calculated by the API; the
  screens only draw them.

  Contract (confirmed against DashboardController.cs and ReportsController.cs):

    GET    /dashboard                       any signed-in user
      200 { activeProjects, lateProjects, lateStages, stockItemsBelowMinimum,
            trainingsNeedingAttention, unsignedPpeIssues }
      a number the profile cannot see comes as null: the card is not drawn

    GET    /reports/project-closing         policy view_finance
      200 [ { projectId, code, name, status, late, revisedContractAmount,
              materialCost, laborCost, totalCost, margin, spentPercentage,
              progressPercentage, costAheadOfProgress } ]
*/

export function createReportGateway(http) {
  return {
    dashboard({ signal } = {}) {
      return http.get('/dashboard', { signal })
    },

    projectClosing({ signal } = {}) {
      return http.get('/reports/project-closing', { signal })
    },
  }
}
