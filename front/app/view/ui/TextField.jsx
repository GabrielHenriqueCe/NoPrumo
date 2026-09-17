import { useId } from 'react'

/*
  A labelled input.

  Three things it does that a bare <input> does not, and that are the reason
  this component exists at all:

  - a real <label> tied to the field, instead of a placeholder standing in for
    one. A placeholder disappears the moment someone types, which leaves anyone
    who was interrupted mid-form guessing what the box was for;
  - the error sits next to the field it belongs to, and `aria-describedby`
    links them, so a screen reader announces the reason and not just "invalid";
  - `aria-invalid` marks the field itself, which is what assistive tech reads.
*/

export function TextField({
  label,
  error,
  hint,
  tone = 'light',
  className = '',
  ...rest
}) {
  const id = useId()
  const errorId = `${id}-error`
  const hintId = `${id}-hint`
  const dark = tone === 'dark'

  const describedBy = [error ? errorId : null, hint ? hintId : null].filter(Boolean).join(' ')

  return (
    <div className={className}>
      <label htmlFor={id} className={`label mb-1.5 block ${dark ? 'text-muted-dark' : ''}`}>
        {label}
      </label>

      <input
        {...rest}
        id={id}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy || undefined}
        className={[
          'w-full px-3 py-2.5 text-sm outline-none transition-colors',
          dark
            ? 'border bg-[#0d1319] text-cream placeholder:text-[#4d5865]'
            : 'border bg-white text-graphite placeholder:text-muted',
          error
            ? 'border-danger-soft'
            : dark
              ? 'border-[#2a3540] focus:border-gold-bright'
              : 'border-line focus:border-gold',
        ].join(' ')}
      />

      {hint && !error && (
        <p id={hintId} className={`mt-1.5 text-[12px] ${dark ? 'text-muted-dark' : 'text-muted'}`}>
          {hint}
        </p>
      )}

      {error && (
        <p id={errorId} className={`mt-1.5 text-[12.5px] ${dark ? 'text-danger-soft' : 'text-danger'}`}>
          {error}
        </p>
      )}
    </div>
  )
}
