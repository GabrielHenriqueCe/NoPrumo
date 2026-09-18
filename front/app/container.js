import { createAuthGateway } from './data/gateways/authGateway'
import { createUserGateway } from './data/gateways/userGateway'
import { createHttpClient } from './data/http/httpClient'
import { tokenStorage } from './data/storage/tokenStorage'

/*
  Composition root: the one file that knows how the app is wired.

  Screens never import from `data/` and never call fetch. They receive a
  gateway and use it, which keeps the API address, the token and the error
  translation in one place instead of spread across the screens.
*/

export function createContainer({
  baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5262/api',
} = {}) {
  /*
    A rejected token is a whole-app event, not a screen event: whichever call
    hits 401 first, the session has to end once. Listeners subscribe instead of
    the HTTP client reaching into React, which would invert the dependency.
  */
  const sessionExpiredListeners = new Set()
  const notifySessionExpired = (error) => {
    sessionExpiredListeners.forEach((listener) => listener(error))
  }

  const http = createHttpClient({
    baseUrl,
    getToken: () => tokenStorage.read(),
    onUnauthorized: notifySessionExpired,
  })

  return {
    auth: createAuthGateway(http),
    users: createUserGateway(http),
    tokenStorage,
    baseUrl,

    /** Returns the unsubscribe function, so React can clean up on unmount. */
    onSessionExpired(listener) {
      sessionExpiredListeners.add(listener)
      return () => sessionExpiredListeners.delete(listener)
    },
  }
}
