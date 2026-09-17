import { useCallback, useEffect, useMemo, useState } from 'react'
import { useContainer } from './containerContext'
import { SessionContext } from './sessionContext'

/*
  Holds the answer to "who is using the app right now".

  The token survives a reload but the user does not, so on start-up — and only
  when there is a token to try — the app asks the API who owns it. Until that
  answer arrives the state is `restoring`, which is different from `anonymous`:
  showing the login screen to someone who is already signed in, for the half
  second the check takes, is a bug people notice.
*/

const RESTORING = 'restoring'
const ANONYMOUS = 'anonymous'
const AUTHENTICATED = 'authenticated'

export function SessionProvider({ children }) {
  const { auth, tokenStorage, onSessionExpired } = useContainer()

  const [user, setUser] = useState(null)
  const [status, setStatus] = useState(() => (tokenStorage.read() ? RESTORING : ANONYMOUS))

  const endSession = useCallback(() => {
    tokenStorage.clear()
    setUser(null)
    setStatus(ANONYMOUS)
  }, [tokenStorage])

  const signIn = useCallback(
    async (credentials) => {
      const result = await auth.signIn(credentials)
      tokenStorage.write(result.token)
      setUser(result.user)
      setStatus(AUTHENTICATED)
      return result
    },
    [auth, tokenStorage],
  )

  // A token the API refuses is dead everywhere, not just on the screen that
  // happened to use it. The HTTP client reports it here, once.
  useEffect(() => onSessionExpired(endSession), [onSessionExpired, endSession])

  useEffect(() => {
    if (status !== RESTORING) return undefined

    const controller = new AbortController()

    auth
      .currentUser({ signal: controller.signal })
      .then((restored) => {
        setUser(restored)
        setStatus(AUTHENTICATED)
      })
      .catch(() => {
        // Expired, revoked, or the API is down: either way there is no session
        // to offer, and the login screen is the honest answer.
        if (!controller.signal.aborted) endSession()
      })

    return () => controller.abort()
  }, [status, auth, endSession])

  const value = useMemo(
    () => ({
      user,
      isRestoring: status === RESTORING,
      isAuthenticated: status === AUTHENTICATED,
      signIn,
      signOut: endSession,
      /*
        Permission, never role name. The screen asks what someone may do, so
        changing who may do it is a back-end decision — not a hunt through the
        UI for `role === 'admin'`. This only hides controls; the API is what
        actually refuses.
      */
      can: (permission) => Boolean(user?.permissions?.includes(permission)),
    }),
    [user, status, signIn, endSession],
  )

  return <SessionContext.Provider value={value}>{children}</SessionContext.Provider>
}
