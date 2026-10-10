export function createProjectLinkGateway(http) {
  return {
    listByProject(projectId, { signal } = {}) {
      return http.get(`/projects/${projectId}/links`, { signal })
    },

    create(projectId, { label, daysValid = 90 } = {}, { signal } = {}) {
      return http.post(`/projects/${projectId}/links`, { label, daysValid }, { signal })
    },

    revoke(id, { signal } = {}) {
      return http.patch(`/project-links/${id}/revoke`, null, { signal })
    },

    getPortalProject(token, { signal } = {}) {
      return http.get(`/portal/${token}`, { signal })
    },
  }
}
