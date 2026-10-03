/*
  Dates travel as 'YYYY-MM-DD' (C# DateOnly).

  Shown as '02 Oct 2026' — the same way the API writes dates in its
  messages, and the same in every browser (Intl's en-GB says "Sept").
  Nothing here goes through `new Date('2026-10-02')`: that is UTC midnight,
  which in Brazil is still the 1st — the classic one-day-off on every date
  column. Today is built from local time for the same reason.
*/

const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']

/** '2026-10-02' → '02 Oct 2026'. Empty stays empty. */
export function formatDate(iso) {
  if (!iso) return ''
  const [year, month, day] = iso.split('-')
  return `${day} ${MONTHS[Number(month) - 1]} ${year}`
}

/** Today on this machine, in the API's format. */
export function todayIso() {
  const now = new Date()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${now.getFullYear()}-${month}-${day}`
}
