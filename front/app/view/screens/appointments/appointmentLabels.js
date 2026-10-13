// The words for these values live in the dictionaries (labels.appointmentType.*).
export const APPOINTMENT_TYPES = ['project', 'visit', 'delivery', 'inspection', 'training', 'meeting']

// First and last day of a month, in the API's 'YYYY-MM-DD' format.
export function monthRange(year, monthIndex) {
  const lastDay = new Date(year, monthIndex + 1, 0).getDate()
  const month = String(monthIndex + 1).padStart(2, '0')
  return { from: `${year}-${month}-01`, to: `${year}-${month}-${String(lastDay).padStart(2, '0')}` }
}

export function monthLabel(year, monthIndex, locale) {
  const name = new Intl.DateTimeFormat(locale, { month: 'long' }).format(new Date(year, monthIndex, 1))
  return `${name.charAt(0).toUpperCase()}${name.slice(1)} ${year}`
}

// Built from the parts, never `new Date('2026-10-02')`, which is UTC midnight.
export function weekdayOf(iso, locale) {
  const [year, month, day] = iso.split('-').map(Number)
  return new Intl.DateTimeFormat(locale, { weekday: 'short' }).format(new Date(year, month - 1, day))
}
