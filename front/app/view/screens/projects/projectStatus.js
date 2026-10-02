/*
  Values stored in project.status. "Late" is not one of them: it comes from the
  API as `late`, calculated from forecastDate — never stored, never derived here.
*/

export const PROJECT_STATUSES = [
  { value: 'planning', label: 'Planning' },
  { value: 'in_progress', label: 'In progress' },
  { value: 'on_hold', label: 'On hold' },
  { value: 'completed', label: 'Completed' },
  { value: 'cancelled', label: 'Cancelled' },
]

export function projectStatusLabel(value) {
  return PROJECT_STATUSES.find((status) => status.value === value)?.label ?? value
}
