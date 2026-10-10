export function purchaseStatusLabel(status) {
  switch (status?.toLowerCase()) {
    case 'pending':
      return 'Pending'
    case 'approved':
      return 'Approved'
    case 'rejected':
      return 'Rejected'
    case 'purchased':
      return 'Purchased'
    default:
      return status || '—'
  }
}

export function purchaseStatusTone(status) {
  switch (status?.toLowerCase()) {
    case 'pending':
      return 'warning'
    case 'approved':
      return 'active'
    case 'rejected':
      return 'inactive'
    case 'purchased':
      return 'neutral'
    default:
      return 'neutral'
  }
}
