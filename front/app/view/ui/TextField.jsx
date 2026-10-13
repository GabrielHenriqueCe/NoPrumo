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

  `tone` is kept for old callers; both tones now follow the theme.
*/

// eslint-disable-next-line no-unused-vars
export function TextField({ label, error, hint, tone, className = '', ...rest }) {
  const id = useId()
  const errorId = `${id}-error`
  const hintId = `${id}-hint`

  const describedBy = [error ? errorId : null, hint ? hintId : null].filter(Boolean).join(' ')

  return (
    <div className={className}>
      <label htmlFor={id} className="label mb-1.5 block">
        {label}
      </label>

      <input
        {...rest}
        id={id}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy || undefined}
        className={[
          'min-h-11 w-full rounded-2xl border bg-surface px-4 text-[15px] text-ink outline-none transition-colors placeholder:text-muted',
          error ? 'border-late' : 'border-line focus:border-accent',
        ].join(' ')}
      />

      {hint && !error && (
        <p id={hintId} className="mt-1.5 text-[12.5px] text-muted">
          {hint}
        </p>
      )}

      {error && (
        <p id={errorId} className="mt-1.5 text-[12.5px] text-danger">
          {error}
        </p>
      )}
    </div>
  )
}
