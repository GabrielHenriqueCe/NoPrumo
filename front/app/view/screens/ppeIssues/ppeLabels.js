// The words live in the dictionaries (labels.ppeItemStatus.*).
const ITEM_STATUS_TONES = { issued: 'active', returned: 'neutral', lost: 'inactive' }

export function itemStatusBadge(status) {
  return { tone: ITEM_STATUS_TONES[status] ?? 'neutral', labelKey: `labels.ppeItemStatus.${status}` }
}

// signedAt is local time without zone ('2026-10-10T14:03:22'), so it is cut, not parsed.
export function formatSignedAt(signedAt) {
  if (!signedAt) return '—'
  const [year, month, day] = signedAt.slice(0, 10).split('-')
  return `${day}/${month}/${year} ${signedAt.slice(11, 16)}`
}
