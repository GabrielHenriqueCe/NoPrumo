/*
  Time entries (ponto) for a construction project.
  The API supplies cost data only to accounts with view_finance.
*/
export function createTimeEntryGateway(http) {
  return {
    list({ projectId, from, to, page = 1, size = 10 } = {}, { signal } = {}) {
      return http.get('/time-entries', {
        params: { projectId, from, to, page, size },
        signal,
      })
    },

    create(entry, { signal } = {}) {
      return http.post('/time-entries', entry, { signal })
    },
  }
}
