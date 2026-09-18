/*
  Where the session token lives between page loads.

  localStorage keeps the user logged in across reloads and tabs, which is what
  people expect from an internal system they use all day. It also means the
  token is readable by scripts running on the page — an acceptable trade for a
  tool that runs on the company network, and one the back can shorten by
  keeping the token short-lived.

  Every access is guarded: localStorage throws in private windows and when the
  browser blocks site data, and a crash here would take the whole app down.
*/

const TOKEN_KEY = 'noprumo.token'

export const tokenStorage = {
  read() {
    try {
      return localStorage.getItem(TOKEN_KEY)
    } catch {
      return null
    }
  },

  write(token) {
    try {
      localStorage.setItem(TOKEN_KEY, token)
    } catch {
      // Session still works for this tab; it just will not survive a reload.
    }
  },

  clear() {
    try {
      localStorage.removeItem(TOKEN_KEY)
    } catch {
      // Nothing stored means nothing to clear.
    }
  },
}
