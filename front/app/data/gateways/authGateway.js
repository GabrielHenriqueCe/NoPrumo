/*
  Everything the app can ask about "who is signed in".

  This module defines the contract; `createAuthGateway` implements it against
  the real API and `fakeAuthGateway` implements the same shape in memory.
  Screens are handed one of them and cannot tell which — that is what lets the
  front be finished before the back has a single controller.

  Contract (mirrored in README.md for whoever writes the endpoints):

    POST /auth/login            { username, password }
      200 { token, user, mustChangePassword }
      401 wrong credentials · 403 inactive user

    GET  /auth/me
      200 user                  — rebuilds the session after a reload

    POST /auth/change-password  { currentPassword, newPassword }
      204 no content
*/

export function createAuthGateway(http) {
  return {
    /** Exchanges credentials for a token. Never sends or stores a raw password. */
    signIn({ username, password }, { signal } = {}) {
      return http.post('/auth/login', { username, password }, { signal })
    },

    /** Who owns the token we are carrying — the source of truth after a reload. */
    currentUser({ signal } = {}) {
      return http.get('/auth/me', { signal })
    },

    changePassword({ currentPassword, newPassword }, { signal } = {}) {
      return http.post('/auth/change-password', { currentPassword, newPassword }, { signal })
    },
  }
}
