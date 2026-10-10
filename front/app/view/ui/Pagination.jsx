import { Button } from './Button'

// `noun` and `pluralNoun` name what is being counted: "3 projects", not "3 records".
export function Pagination({ page, totalPages, total, onChange, busy = false, noun = 'record', pluralNoun = `${noun}s` }) {
    const counted = `${total} ${total === 1 ? noun : pluralNoun}`

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
                {/* Usamos Math.max para não mostrar "Page 1 of 0" quando não houver registos */}
                Page {page} of {Math.max(totalPages, 1)} · {counted}
            </span>

            <Button
                variant="outline"
                onClick={() => onChange(page + 1)}
                disabled={busy || page >= Math.max(totalPages, 1)}
                className="px-3 py-1.5"
            >
                Next
            </Button>
        </nav>
    )
}