export function purchaseStatusLabel(status) {
  switch (status?.toLowerCase()) {
    case 'pending':
      return 'Pendente'
    case 'approved':
      return 'Aprovado'
    case 'rejected':
      return 'Recusado'
    case 'purchased':
      return 'Comprado'
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
