// The H progress bar: a rounded track, bronze fill, the late colour when behind.
export function ProgressTrack({ value, late, label, height = 'h-2.5' }) {
  const width = Math.min(Math.max(value ?? 0, 0), 100)
  return (
    <div
      role="progressbar"
      aria-label={label}
      aria-valuenow={Math.round(width)}
      aria-valuemin="0"
      aria-valuemax="100"
      className={`${height} overflow-hidden rounded-full bg-track`}
    >
      <div className={`h-full rounded-full ${late ? 'bg-late' : 'bg-accent'}`} style={{ width: `${width}%` }} />
    </div>
  )
}
