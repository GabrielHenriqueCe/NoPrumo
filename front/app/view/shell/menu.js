/*
  The sidebar, as data. AppShell draws it; the routes read it to decide where
  someone lands after signing in.

  The menu is grouped on purpose. The shape of the menu is what tells whoever
  builds the next screen where it belongs: everything registered once and then
  only picked from a list is master data, so it goes under the same heading as
  users instead of piling up loose at the top level. The groups for daily work
  (stock movement, time entries, purchases) come with those screens.

  Inside the group the order is by domain: whoever looks for "Job roles" finds
  it next to "Departments", not in the middle of "Clients".

  Every item declares the permission it needs. A group whose items are all out
  of reach does not render its heading either — an empty section would only
  advertise what the person cannot open.
*/

export const MENU = [
  {
    heading: 'Master data',
    items: [
      { to: '/users', label: 'Users', permission: 'manage_users' },

      { to: '/departments', label: 'Departments', permission: 'manage_employees' },
      { to: '/job-roles', label: 'Job roles', permission: 'manage_employees' },
      { to: '/employment-regimes', label: 'Employment regimes', permission: 'manage_employees' },
      { to: '/employees', label: 'Employees', permission: 'manage_employees' },
      { to: '/teams', label: 'Teams', permission: 'manage_employees' },

      { to: '/clients', label: 'Clients', permission: 'manage_projects' },
      { to: '/suppliers', label: 'Suppliers', permission: 'manage_purchases' },

      { to: '/stock-categories', label: 'Stock categories', permission: 'manage_stock' },
      { to: '/stock-groups', label: 'Stock groups', permission: 'manage_stock' },
      { to: '/stock-items', label: 'Stock items', permission: 'manage_stock' },

      { to: '/projects', label: 'Projects', permission: 'manage_projects' },
      { to: '/stages', label: 'Stages', permission: 'manage_projects' },

      { to: '/training-types', label: 'Training types', permission: 'manage_safety' },
      { to: '/trainings', label: 'Trainings', permission: 'manage_safety' },
    ],
  },
]
