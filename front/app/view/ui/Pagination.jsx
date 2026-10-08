import { Button } from './Button'

// `noun` and `pluralNoun` name what is being counted: "3 projects", not "3 records".
export function Pagination({ page, totalPages, total, onChange, busy = false, noun = 'record', pluralNoun = `${noun}s` }) {
  const counted = `${total} ${total === 1 ? noun : pluralNoun}`

  if (totalPages <= 1) {
    return (
      <p className="mt-4 text-[12.5px] text-muted">
        {counted}
      </p>
    )
  }

  return (
    <nav className="mt-4 flex items-center gap-3" aria-label="Pagination">
      <Button
        variant="outline"
        onClick={() => onChange(page - 1)}
        disabled={busy || page <= 1}
        className="px-3 py-1.5"
      >
        Previous
      </Button>

      <span aria-live="polite" className="text-[12.5px] text-muted">
        Page {page} of {totalPages} · {counted}
      </span>

      <Button
        variant="outline"
        onClick={() => onChange(page + 1)}
        disabled={busy || page >= totalPages}
        className="px-3 py-1.5"
      >
        Next
      </Button>
    </nav>
  )
}
