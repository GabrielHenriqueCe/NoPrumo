/*
  How the values the API sends are written on screen. The values themselves
  stay as they are stored — in English, in code form ('in_person', 'expired').

  Dates travel as 'YYYY-MM-DD' (C# DateOnly) and are shown as '02 Oct 2026',
  the same way the API writes dates in its messages, and the same in every
  browser (Intl's en-GB says "Sept"). Nothing here goes through
  `new Date('2026-10-02')`: that is UTC midnight, which in Brazil is still
  the 1st — the classic one-day-off on every date column. Today is built from
  local time for the same reason.
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

export const MODALITIES = [
  { value: 'in_person', label: 'In person' },
  { value: 'online', label: 'Online' },
  { value: 'blended', label: 'Blended' },
]

export function modalityLabel(value) {
  if (!value) return '—'
  return MODALITIES.find((modality) => modality.value === value)?.label ?? value
}

/*
  The status is the API's answer — the screen only picks the colour and the
  words. 'expiring' means 30 days or less left.
*/
const STATUS_BADGES = {
  valid: { tone: 'active', label: 'Valid' },
  expiring: { tone: 'warning', label: 'Expires soon' },
  expired: { tone: 'inactive', label: 'Expired' },
  no_expiry: { tone: 'neutral', label: 'No expiry' },
}

export function statusBadge(status) {
  return STATUS_BADGES[status] ?? { tone: 'neutral', label: status ?? '—' }
}
