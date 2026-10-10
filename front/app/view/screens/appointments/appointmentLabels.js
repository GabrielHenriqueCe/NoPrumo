export const APPOINTMENT_TYPES = [
  { value: 'project', label: 'Project' },
  { value: 'visit', label: 'Visit' },
  { value: 'delivery', label: 'Delivery' },
  { value: 'inspection', label: 'Inspection' },
  { value: 'training', label: 'Training' },
  { value: 'meeting', label: 'Meeting' },
]

export function appointmentTypeLabel(value) {
  return APPOINTMENT_TYPES.find((type) => type.value === value)?.label ?? value
}

const MONTHS = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
]

const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

// First and last day of a month, in the API's 'YYYY-MM-DD' format.
export function monthRange(year, monthIndex) {
  const lastDay = new Date(year, monthIndex + 1, 0).getDate()
  const month = String(monthIndex + 1).padStart(2, '0')
  return { from: `${year}-${month}-01`, to: `${year}-${month}-${String(lastDay).padStart(2, '0')}` }
}

export function monthLabel(year, monthIndex) {
  return `${MONTHS[monthIndex]} ${year}`
}

// Built from the parts, never `new Date('2026-10-02')`, which is UTC midnight.
export function weekdayOf(iso) {
  const [year, month, day] = iso.split('-').map(Number)
  return WEEKDAYS[new Date(year, month - 1, day).getDay()]
}
