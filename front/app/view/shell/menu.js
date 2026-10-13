import { SETTINGS_PERMISSIONS } from './settings'

/*
  The rail, as data. AppShell draws it; the routes read it to decide where
  someone lands after signing in.

  Each group is one icon on the rail. Clicking it opens the first item this
  person can reach, and the group's items show as tabs above the screen. The
  rail is the company's: one project lives inside its own page, in its tabs,
  not here. What is registered once and only picked from a list goes to
  Settings (settings.js), pinned to the bottom of the rail.

  Every item declares the permission it needs; null means anyone signed in. A
  group whose items are all out of reach does not show at all.
*/

export const SCHEDULE_PERMISSIONS = ['manage_projects', 'manage_safety']

export const MENU = [
  {
    key: 'home',
    icon: 'home',
    items: [{ to: '/home', labelKey: 'menu.home', permission: null }],
  },
  {
    key: 'projects',
    icon: 'projects',
    items: [{ to: '/projects', labelKey: 'menu.projects', permission: 'view_projects' }],
  },
  {
    key: 'people',
    icon: 'people',
    items: [
      { to: '/employees', labelKey: 'menu.employees', permission: 'manage_employees' },
      { to: '/teams', labelKey: 'menu.teams', permission: 'manage_employees' },
      { to: '/trainings', labelKey: 'menu.trainings', permission: 'manage_safety' },
    ],
  },
  {
    key: 'planning',
    icon: 'planning',
    items: [{ to: '/schedule', labelKey: 'menu.schedule', permission: SCHEDULE_PERMISSIONS }],
  },
  {
    key: 'materials',
    icon: 'materials',
    items: [{ to: '/stock-items', labelKey: 'menu.stockItems', permission: 'manage_stock' }],
  },
  {
    key: 'commercial',
    icon: 'commercial',
    items: [
      { to: '/clients', labelKey: 'menu.clients', permission: 'manage_projects' },
      { to: '/suppliers', labelKey: 'menu.suppliers', permission: 'manage_purchases' },
      { to: '/purchases', labelKey: 'menu.purchases', permission: 'manage_purchases' },
    ],
  },
  {
    key: 'reports',
    icon: 'reports',
    items: [{ to: '/reports', labelKey: 'menu.reports', permission: 'view_finance' }],
  },
]

export const SETTINGS_GROUP = {
  key: 'settings',
  icon: 'settings',
  items: [{ to: '/settings', labelKey: 'menu.settings', permission: SETTINGS_PERMISSIONS }],
}
