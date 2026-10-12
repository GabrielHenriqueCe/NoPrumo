/*
  What is registered once and then only picked from a list. It is not daily
  work, so it stays out of the sidebar and lives in the Settings screen. Each
  item is still its own screen and keeps its own route.
*/

export const SETTINGS_ITEMS = [
  { to: '/users', label: 'Users', permission: 'manage_users', description: 'Who signs in, and with which role.' },

  { to: '/departments', label: 'Departments', permission: 'manage_employees', description: 'Where each employee works.' },
  { to: '/job-roles', label: 'Job roles', permission: 'manage_employees', description: 'Mason, carpenter, helper…' },
  {
    to: '/employment-regimes',
    label: 'Employment regimes',
    permission: 'manage_employees',
    description: 'CLT, hourly, daily: how each one is paid.',
  },

  {
    to: '/stock-categories',
    label: 'Stock categories',
    permission: 'manage_stock',
    description: 'Consumable, PPE and tool: what each one does with the balance.',
  },
  { to: '/stock-groups', label: 'Stock groups', permission: 'manage_stock', description: 'Binders, steel, aggregates…' },

  {
    to: '/training-types',
    label: 'Training types',
    permission: 'manage_safety',
    description: 'NR courses and exams, and how long each one lasts.',
  },
]

// Settings opens for whoever can open at least one of its items.
export const SETTINGS_PERMISSIONS = [...new Set(SETTINGS_ITEMS.map((item) => item.permission))]
