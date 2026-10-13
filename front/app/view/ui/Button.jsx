/*
  One button, four looks. Everything that can be pressed goes through here so
  that size, focus ring and the busy state behave the same everywhere.

  `busy` disables the button and swaps the label, because the honest way to
  stop a double submit is to make the second click impossible — not to hope
  the user waits.

  The H look: a pill, with the label in the condensed uppercase face. A
  className carrying its own padding (px-3 py-1.5 on table rows) replaces the
  default size instead of fighting it.
*/

const VARIANTS = {
  primary: 'bg-primary text-on-primary hover:opacity-90 disabled:opacity-50',
  gold: 'bg-accent-bright text-[#10161c] hover:opacity-90 disabled:opacity-50',
  outline: 'border border-line bg-surface text-ink hover:border-accent disabled:text-muted',
  danger: 'border border-late bg-surface text-danger hover:bg-danger-wash disabled:text-muted',
}

const DEFAULT_SIZE = 'min-h-11 px-6 text-[16px]'

export function Button({
  variant = 'primary',
  busy = false,
  busyLabel = '…',
  disabled = false,
  className = '',
  children,
  ...rest
}) {
  const ownSize = /(^|\s)(p[xy]?-|min-h-)/.test(className)

  return (
    <button
      {...rest}
      disabled={disabled || busy}
      aria-busy={busy || undefined}
      className={[
        'inline-flex cursor-pointer items-center justify-center gap-2 rounded-full font-display font-bold tracking-[0.06em] uppercase transition',
        'disabled:cursor-not-allowed',
        ownSize ? 'text-[14px]' : DEFAULT_SIZE,
        VARIANTS[variant] ?? VARIANTS.primary,
        className,
      ].join(' ')}
    >
      {busy ? busyLabel : children}
    </button>
  )
}
