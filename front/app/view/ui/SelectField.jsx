import { useId } from 'react'

/*
  Native <select> on purpose. It costs nothing, works with the keyboard, and on
  a phone it opens the OS picker — which beats any list we would draw. A custom
  dropdown only earns its place when it needs search or multi-select, and this
  one needs neither: six roles, fixed.
*/

export function SelectField({ label, error, options, placeholder = 'Select…', className = '', ...rest }) {
  const id = useId()
  const errorId = `${id}-error`

  return (
    <div className={className}>
      <label htmlFor={id} className="label mb-1.5 block">
        {label}
      </label>

      <select
        {...rest}
        id={id}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? errorId : undefined}
        className={[
          'w-full appearance-none bg-white px-3 py-2.5 text-sm text-graphite outline-none transition-colors',
          'border',
          error ? 'border-danger-soft' : 'border-line focus:border-gold',
        ].join(' ')}
      >
        <option value="">{placeholder}</option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

      {error && (
        <p id={errorId} className="mt-1.5 text-[12.5px] text-danger">
          {error}
        </p>
      )}
    </div>
  )
}
