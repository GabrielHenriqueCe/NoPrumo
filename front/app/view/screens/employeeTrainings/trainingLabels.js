/*
  How the values the API sends are written on screen. The values themselves
  stay as stored: in English, in code form ('in_person', 'expired').
*/

export const MODALITIES = [
  { value: 'in_person', label: 'In person' },
  { value: 'online', label: 'Online' },
  { value: 'blended', label: 'Blended' },
]

export function modalityLabel(value) {
  if (!value) return '—'
  return MODALITIES.find((modality) => modality.value === value)?.label ?? value
}

// The status is the API's; the screen only picks colour and words. 'expiring' = 30 days or less.
const STATUS_BADGES = {
  valid: { tone: 'active', label: 'Valid' },
  expiring: { tone: 'warning', label: 'Expires soon' },
  expired: { tone: 'inactive', label: 'Expired' },
  no_expiry: { tone: 'neutral', label: 'No expiry' },
}

export function statusBadge(status) {
  return STATUS_BADGES[status] ?? { tone: 'neutral', label: status ?? '—' }
}
