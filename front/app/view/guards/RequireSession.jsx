import { Navigate, useLocation } from 'react-router-dom'
import { useSession } from '../providers/sessionContext'

/*
  Gate for everything behind the login.

  `isRestoring` is handled before `isAuthenticated` on purpose: while the app
  is still asking the API who owns the stored token, nobody is "not signed in"
  yet, and bouncing them to the login screen for that half second would throw
  away the page they asked for.

  This hides screens. It does not protect data — the API is what refuses.
*/

const CHANGE_PASSWORD_PATH = '/change-password'

export function RequireSession({ permission, children }) {
  const { isRestoring, isAuthenticated, can, mustChangePassword } = useSession()
  const location = useLocation()

  if (isRestoring) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-cream">
        <div
          role="status"
          aria-label="Loading"
          className="h-10 w-10 animate-spin rounded-full border-4 border-gold border-t-transparent"
        />
      </div>
    )
  }

  if (!isAuthenticated) {
    // Remembers where they were headed, so signing in finishes the trip.
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  /*
    A provisional password is one an administrator read out loud over the
    phone — it is shared by construction until the person replaces it. So the
    account gets exactly one screen until that happens.
  */
  if (mustChangePassword && location.pathname !== CHANGE_PASSWORD_PATH) {
    return <Navigate to={CHANGE_PASSWORD_PATH} replace />
  }

  if (permission && !can(permission)) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-cream p-8">
        <div className="max-w-md border border-line bg-white p-8">
          <p className="label">No access</p>
          <h1 className="mt-2 text-xl font-semibold tracking-tight">
            This account has no screen available yet
          </h1>
          <p className="mt-3 text-sm leading-relaxed text-muted">
            Ask an administrator to review your role.
          </p>
        </div>
      </div>
    )
  }

  return children
}
