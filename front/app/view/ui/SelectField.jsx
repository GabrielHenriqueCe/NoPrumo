import { useId } from 'react'
import { useT } from '../preferences/preferencesContext'

/*
  Native <select> on purpose. It costs nothing, works with the keyboard, and on
  a phone it opens the OS picker — which beats any list we would draw. A custom
  dropdown only earns its place when it needs search or multi-select, and this
  one needs neither: six roles, fixed.
*/

export function SelectField({ label, error, options = [], placeholder, className = '', children, ...rest }) {
  const t = useT()
  const id = useId()
  const errorId = `${id}-error`

  return (
    <div className={className}>
      <label htmlFor={id} className="label mb-1.5 block">
        {label}
      </label>

      <div className="relative">
        <select
          {...rest}
          id={id}
          aria-invalid={error ? true : undefined}
          aria-describedby={error ? errorId : undefined}
          className={[
            'min-h-11 w-full appearance-none rounded-2xl border bg-surface pr-10 pl-4 text-[15px] text-ink outline-none transition-colors',
            error ? 'border-late' : 'border-line focus:border-accent',
          ].join(' ')}
        >
          {/* Children are for callers that bring their own <option>s, including their own empty one. */}
          {!children && <option value="">{placeholder ?? t('common.select')}</option>}
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
          {children}
        </select>
        <span aria-hidden="true" className="pointer-events-none absolute top-1/2 right-4 -translate-y-1/2 text-muted">
          ▾
        </span>
      </div>

      {error && (
        <p id={errorId} className="mt-1.5 text-[12.5px] text-danger">
          {error}
        </p>
      )}
    </div>
  )
}
