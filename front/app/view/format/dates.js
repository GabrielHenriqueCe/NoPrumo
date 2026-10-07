/*
  Dates travel as 'YYYY-MM-DD' (C# DateOnly). Never through
  `new Date('2026-10-02')`: that is UTC midnight, which in Brazil is still the
  1st. Today is built from local time for the same reason.
*/

/** '2026-10-02' → '02/10/2026'. */
export function formatDate(iso) {
  if (!iso) return '—'
  const [year, month, day] = iso.split('-')
  return `${day}/${month}/${year}`
}

/** Today on this machine, in the API's format. */
export function todayIso() {
  const now = new Date()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${now.getFullYear()}-${month}-${day}`
}
