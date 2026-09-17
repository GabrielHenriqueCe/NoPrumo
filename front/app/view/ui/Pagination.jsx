import { Button } from './Button'

/*
  Paging controls. The API decides how many pages exist; this only moves
  between them and says where you are, because "Page 2 of 7" is the part
  people actually read before clicking.
*/

export function Pagination({ page, totalPages, total, onChange, busy = false }) {
  if (totalPages <= 1) {
    return (
      <p className="mt-4 text-[12.5px] text-muted">
        {total} {total === 1 ? 'user' : 'users'}
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
        Page {page} of {totalPages} · {total} {total === 1 ? 'user' : 'users'}
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
