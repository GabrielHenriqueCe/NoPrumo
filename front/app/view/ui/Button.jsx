/*
  One button, four looks. Everything that can be pressed goes through here so
  that size, focus ring and the busy state behave the same everywhere.

  `busy` disables the button and swaps the label, because the honest way to
  stop a double submit is to make the second click impossible — not to hope
  the user waits.
*/

const VARIANTS = {
  primary: 'bg-graphite text-cream hover:bg-graphite-soft disabled:bg-steel-500',
  gold: 'bg-gold-bright text-graphite hover:bg-gold disabled:bg-line',
  outline: 'border border-line bg-white text-graphite hover:border-gold disabled:text-muted',
  danger: 'border border-danger-soft bg-white text-danger hover:bg-[#faefea] disabled:text-muted',
}

export function Button({
  variant = 'primary',
  busy = false,
  busyLabel = 'Working…',
  disabled = false,
  className = '',
  children,
  ...rest
}) {
  return (
    <button
      {...rest}
      disabled={disabled || busy}
      aria-busy={busy || undefined}
      className={[
        'px-4 py-2.5 text-sm font-semibold transition-colors',
        'disabled:cursor-not-allowed',
        VARIANTS[variant] ?? VARIANTS.primary,
        className,
      ].join(' ')}
    >
      {busy ? busyLabel : children}
    </button>
  )
}
