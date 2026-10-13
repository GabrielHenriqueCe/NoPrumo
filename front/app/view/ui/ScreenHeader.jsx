/*
  The H screen title: big condensed uppercase heading, an optional count pill
  and the screen's actions pushed to the right.
*/
export function ScreenHeader({ eyebrow, title, count, alert, children }) {
  return (
    <header className="flex flex-wrap items-center gap-3 px-10 pt-7">
      <div className="min-w-0">
        {eyebrow && <p className="label mb-1">{eyebrow}</p>}
        <h1 className="display text-[38px] leading-none">{title}</h1>
      </div>

      {count != null && (
        <span className="rounded-full border border-line bg-surface px-3 py-1 font-mono text-[13px] text-muted">
          {count}
        </span>
      )}
      {alert && (
        <span className="rounded-full bg-danger-wash px-3 py-1 font-mono text-[13px] text-danger">{alert}</span>
      )}

      {children && <div className="ml-auto flex flex-wrap items-center gap-2.5">{children}</div>}
    </header>
  )
}
