// The words live in the dictionaries (labels.purchaseStatus.*).
export function purchaseStatusKey(status) {
  return `labels.purchaseStatus.${String(status ?? '').toLowerCase()}`
}

export function purchaseStatusTone(status) {
  switch (status?.toLowerCase()) {
    case 'pending':
      return 'warning'
    case 'approved':
      return 'active'
    case 'rejected':
      return 'inactive'
    default:
      return 'neutral'
  }
}
