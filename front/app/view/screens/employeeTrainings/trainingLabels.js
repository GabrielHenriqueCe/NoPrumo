/*
  How the values the API sends are written on screen. The values themselves
  stay as stored: in English, in code form ('in_person', 'expired'); the words
  live in the dictionaries (labels.modality.*, labels.trainingStatus.*).
*/

export const MODALITIES = ['in_person', 'online', 'blended']

// The status is the API's; the screen only picks colour and words. 'expiring' = 30 days or less.
const STATUS_TONES = {
  valid: 'active',
  expiring: 'warning',
  expired: 'inactive',
  no_expiry: 'neutral',
}

export function statusBadge(status) {
  return { tone: STATUS_TONES[status] ?? 'neutral', labelKey: `labels.trainingStatus.${status}` }
}
