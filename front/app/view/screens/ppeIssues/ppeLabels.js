const ITEM_STATUS_BADGES = {
  issued: { tone: 'active', label: 'With employee' },
  returned: { tone: 'neutral', label: 'Returned' },
  lost: { tone: 'inactive', label: 'Lost' },
}

export function itemStatusBadge(status) {
  return ITEM_STATUS_BADGES[status] ?? { tone: 'neutral', label: status ?? '—' }
}

// signedAt is local time without zone ('2026-10-10T14:03:22'), so it is cut, not parsed.
export function formatSignedAt(signedAt) {
  if (!signedAt) return '—'
  const [year, month, day] = signedAt.slice(0, 10).split('-')
  return `${day}/${month}/${year} ${signedAt.slice(11, 16)}`
}
