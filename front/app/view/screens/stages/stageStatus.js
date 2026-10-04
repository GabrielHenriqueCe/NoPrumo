export const STAGE_STATUSES = [
  { value: 'planned', label: 'Planned' },
  { value: 'in_progress', label: 'In progress' },
  { value: 'completed', label: 'Completed' },
]

export function stageStatusLabel(value) {
  return STAGE_STATUSES.find((status) => status.value === value)?.label ?? value
}
