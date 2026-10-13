/*
  Two or three mutually exclusive choices side by side (Light | Dark, EN | PT).
  It is a radio group underneath, so arrow keys and screen readers treat it as
  one question with one answer.
*/
export function SegmentedControl({ name, label, options, value, onChange, size = 'md' }) {
  const height = size === 'sm' ? 'min-h-8 px-3 text-[13px]' : 'min-h-10 px-4 text-sm'

  return (
    <div role="radiogroup" aria-label={label} className="inline-flex rounded-full border border-line bg-sunken p-0.5">
      {options.map((option) => {
        const checked = option.value === value
        return (
          <label
            key={option.value}
            className={[
              'inline-flex cursor-pointer items-center gap-1.5 rounded-full font-semibold transition-colors',
              height,
              checked ? 'bg-primary text-on-primary' : 'text-muted hover:text-ink',
            ].join(' ')}
          >
            <input
              type="radio"
              name={name}
              value={option.value}
              checked={checked}
              onChange={() => onChange(option.value)}
              className="sr-only"
            />
            {option.label}
          </label>
        )
      })}
    </div>
  )
}
