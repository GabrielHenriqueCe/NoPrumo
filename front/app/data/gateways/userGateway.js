/*
  User administration. There is no public sign-up in this system: an admin
  creates the account and the API answers with a one-time password to hand
  over. The password is never chosen here and never travels back.

  Contract (mirrored in README.md for whoever writes the endpoints):

    GET    /users?page&size&search
      200 { items, page, size, total, totalPages }

    POST   /users                     { name, username, email, roleId }
      201 { user, temporaryPassword }
      400 ProblemDetails with `errors` per field

    PUT    /users/{id}                { name, email, roleId }
      200 user

    PATCH  /users/{id}/activate
    PATCH  /users/{id}/deactivate
      204 no content

    POST   /users/{id}/reset-password
      200 { temporaryPassword }

    GET    /roles
      200 [ { id, name, label } ]
*/

export function createUserGateway(http) {
  return {
    list({ page = 1, size = 10, search = '' } = {}, { signal } = {}) {
      return http.get('/users', { params: { page, size, search }, signal })
    },

    create(user, { signal } = {}) {
      return http.post('/users', user, { signal })
    },

    update(id, user, { signal } = {}) {
      return http.put(`/users/${id}`, user, { signal })
    },

    /** Accounts are switched off, never deleted: their history has to stay. */
    setActive(id, active, { signal } = {}) {
      return http.patch(`/users/${id}/${active ? 'activate' : 'deactivate'}`, null, { signal })
    },

    resetPassword(id, { signal } = {}) {
      return http.post(`/users/${id}/reset-password`, null, { signal })
    },

    listRoles({ signal } = {}) {
      return http.get('/roles', { signal })
    },
  }
}
