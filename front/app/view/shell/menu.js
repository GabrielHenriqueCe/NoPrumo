import { SETTINGS_PERMISSIONS } from './settings'

/*
  The sidebar, as data. AppShell draws it; the routes read it to decide where
  someone lands after signing in.

  The sidebar is the company's: projects first, then what goes around them
  (people, materials, commercial). Everything about one project lives inside
  the project page, in its tabs, not here. What is registered once and only
  picked from a list goes to Settings (settings.js).

  Every item declares the permission it needs. A group whose items are all out
  of reach does not render its heading either — an empty section would only
  advertise what the person cannot open.
*/

export const MENU = [
  {
    heading: null,
    items: [{ to: '/projects', label: 'Projects', permission: 'view_projects' }],
  },
  {
    heading: 'People',
    items: [
      { to: '/employees', label: 'Employees', permission: 'manage_employees' },
      { to: '/teams', label: 'Teams', permission: 'manage_employees' },
      { to: '/trainings', label: 'Trainings', permission: 'manage_safety' },
    ],
  },
  {
    heading: 'Materials',
    items: [{ to: '/stock-items', label: 'Stock items', permission: 'manage_stock' }],
  },
  {
    heading: 'Commercial',
    items: [
      { to: '/clients', label: 'Clients', permission: 'manage_projects' },
      { to: '/suppliers', label: 'Suppliers', permission: 'manage_purchases' },

    ],
  },
  {
    heading: 'Company',
    items: [
      { to: '/settings', label: 'Settings', permission: SETTINGS_PERMISSIONS },
    ],
  },
]
