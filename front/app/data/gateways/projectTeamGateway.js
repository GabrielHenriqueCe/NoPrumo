/*
  Teams on a project (team_project). A team leaves a project with an end date;
  the period stays as history.

  Contract (confirmed against ProjectTeamsController.cs, policy manage_projects):

    GET    /projects/{projectId}/teams
      200 [ { teamId, teamName, departmentName, startDate, endDate,
              members: [ { employeeId, employeeName, registrationNumber,
                           trainingAlerts: [ { trainingTypeCode, trainingTypeName,
                                               expiryDate, status } ] } ] } ]
      open periods first; members only on the open period (who is on the team today);
      trainingAlerts: the latest certificate of each type that is 'expired' or 'expiring'
      404 unknown project

    GET    /projects/{projectId}/teams/options
      200 [ { id, name, departmentName } ]   active teams

    POST   /projects/{projectId}/teams        { teamId, startDate }
      201 period
      400 ProblemDetails with `errors` per field

    PATCH  /projects/{projectId}/teams/{teamId}/end   { endDate }
      200 period
      404 the team is not on the project right now
*/

export function createProjectTeamGateway(http) {
  return {
    list(projectId, { signal } = {}) {
      return http.get(`/projects/${projectId}/teams`, { signal })
    },

    listTeamOptions(projectId, { signal } = {}) {
      return http.get(`/projects/${projectId}/teams/options`, { signal })
    },

    allocate(projectId, allocation, { signal } = {}) {
      return http.post(`/projects/${projectId}/teams`, allocation, { signal })
    },

    end(projectId, teamId, endDate, { signal } = {}) {
      return http.patch(`/projects/${projectId}/teams/${teamId}/end`, { endDate }, { signal })
    },
  }
}
