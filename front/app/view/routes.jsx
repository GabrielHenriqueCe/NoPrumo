import { Navigate, Route, Routes } from 'react-router-dom'
import { RequireSession } from './guards/RequireSession'
import { useSession } from './providers/sessionContext'
import { AccountScreen } from './screens/account/AccountScreen'
import { AppointmentsScreen } from './screens/appointments/AppointmentsScreen'
import { ChangePasswordScreen } from './screens/auth/ChangePasswordScreen'
import { LoginScreen } from './screens/auth/LoginScreen'
import { ClientsScreen } from './screens/clients/ClientsScreen'
import { DepartmentsScreen } from './screens/departments/DepartmentsScreen'
import { EmployeesScreen } from './screens/employees/EmployeesScreen'
import { EmployeeTrainingsScreen } from './screens/employeeTrainings/EmployeeTrainingsScreen'
import { EmploymentRegimesScreen } from './screens/employmentRegimes/EmploymentRegimesScreen'
import { HomeScreen } from './screens/home/HomeScreen'
import { JobRolesScreen } from './screens/jobRoles/JobRolesScreen'
import { PortalScreen } from './screens/portal/PortalScreen'
import { ProjectScreen } from './screens/projects/ProjectScreen'
import { ProjectsScreen } from './screens/projects/ProjectsScreen'
import { PurchasesScreen } from './screens/purchases/PurchasesScreen'
import { ReportsScreen } from './screens/reports/ReportsScreen'
import { SettingsScreen } from './screens/settings/SettingsScreen'
import { StockCategoriesScreen } from './screens/stockCategories/StockCategoriesScreen'
import { StockGroupsScreen } from './screens/stockGroups/StockGroupsScreen'
import { StockItemsScreen } from './screens/stockItems/StockItemsScreen'
import { SuppliersScreen } from './screens/suppliers/SuppliersScreen'
import { TeamsScreen } from './screens/teams/TeamsScreen'
import { TrainingTypesScreen } from './screens/trainingTypes/TrainingTypesScreen'
import { UsersScreen } from './screens/users/UsersScreen'
import { AppShell } from './shell/AppShell'
import { MENU, SCHEDULE_PERMISSIONS } from './shell/menu'
import { SETTINGS_PERMISSIONS } from './shell/settings'

/*
  Every screen in the app, in one readable list.

  The internal routes hang off a layout route, so the sidebar and header are
  mounted once and the screen swaps underneath.

  Each internal screen is one line below: path, the permission it needs and
  the component. The permission has to match the one in menu.js — the menu
  hides the link, this line refuses the page to whoever types the address.

  /change-password is deliberately outside that layout: whoever lands there
  has not finished signing in, and showing them a menu they cannot use would
  be a lie.
*/

const SCREENS = [
  { path: '/home', permission: null, element: <HomeScreen /> },
  { path: '/account', permission: null, element: <AccountScreen /> },
  { path: '/reports', permission: 'view_finance', element: <ReportsScreen /> },

  { path: '/settings', permission: SETTINGS_PERMISSIONS, element: <SettingsScreen /> },

  { path: '/users', permission: 'manage_users', element: <UsersScreen /> },

  { path: '/departments', permission: 'manage_employees', element: <DepartmentsScreen /> },
  { path: '/job-roles', permission: 'manage_employees', element: <JobRolesScreen /> },
  { path: '/employment-regimes', permission: 'manage_employees', element: <EmploymentRegimesScreen /> },
  { path: '/employees', permission: 'manage_employees', element: <EmployeesScreen /> },
  { path: '/teams', permission: 'manage_employees', element: <TeamsScreen /> },

  { path: '/clients', permission: 'manage_projects', element: <ClientsScreen /> },
  { path: '/suppliers', permission: 'manage_purchases', element: <SuppliersScreen /> },
  { path: '/purchases', permission: 'manage_purchases', element: <PurchasesScreen /> },

  { path: '/stock-categories', permission: 'manage_stock', element: <StockCategoriesScreen /> },
  { path: '/stock-groups', permission: 'manage_stock', element: <StockGroupsScreen /> },
  { path: '/stock-items', permission: 'manage_stock', element: <StockItemsScreen /> },

  { path: '/projects', permission: 'view_projects', element: <ProjectsScreen /> },
  { path: '/projects/:projectId/:tab?', permission: 'view_projects', element: <ProjectScreen /> },

  { path: '/schedule', permission: SCHEDULE_PERMISSIONS, element: <AppointmentsScreen /> },

  { path: '/training-types', permission: 'manage_safety', element: <TrainingTypesScreen /> },
  { path: '/trainings', permission: 'manage_safety', element: <EmployeeTrainingsScreen /> },
]

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginScreen />} />
      <Route path="/portal/:token" element={<PortalScreen />} />

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
        {SCREENS.map(({ path, permission, element }) => (
          <Route
            key={path}
            path={path}
            element={<RequireSession permission={permission}>{element}</RequireSession>}
          />
        ))}
      </Route>

      <Route
        path="*"
        element={
          <RequireSession>
            <FirstAllowedScreen />
          </RequireSession>
        }
      />
    </Routes>
  )
}

/*
  Anything unknown — including the bare "/" right after signing in — lands on
  the first menu item this account can open: Home, which everyone has.
*/
function FirstAllowedScreen() {
  const { can } = useSession()

  const first = MENU.flatMap((group) => group.items).find(
    (item) => !item.permission || can(item.permission),
  )

  return <Navigate to={first?.to ?? '/home'} replace />
}
