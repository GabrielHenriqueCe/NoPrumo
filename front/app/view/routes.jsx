import { Navigate, Route, Routes } from 'react-router-dom'
import { RequireSession } from './guards/RequireSession'
import { ChangePasswordScreen } from './screens/auth/ChangePasswordScreen'
import { LoginScreen } from './screens/auth/LoginScreen'
import { UsersScreen } from './screens/users/UsersScreen'
import { AppShell } from './shell/AppShell'

/*
  Every screen in the app, in one readable list.

  The internal routes hang off a layout route, so the sidebar and header are
  mounted once and the screen swaps underneath. As screens are added, they
  become siblings of /users — the frame does not change.

  /change-password is deliberately outside that layout: whoever lands there
  has not finished signing in, and showing them a menu they cannot use would
  be a lie.
*/

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginScreen />} />

      <Route
        path="/change-password"
        element={
          <RequireSession>
            <ChangePasswordScreen />
          </RequireSession>
        }
      />

      <Route
        element={
          <RequireSession>
            <AppShell />
          </RequireSession>
        }
      >
        <Route
          path="/users"
          element={
            <RequireSession permission="manage_users">
              <UsersScreen />
            </RequireSession>
          }
        />
      </Route>

      {/* Anything unknown lands on the only internal screen there is. */}
      <Route path="*" element={<Navigate to="/users" replace />} />
    </Routes>
  )
}
