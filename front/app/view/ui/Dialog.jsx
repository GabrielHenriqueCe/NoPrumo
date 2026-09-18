import { useEffect, useId, useRef } from 'react'

/*
  Modal dialog.

  A box drawn on top of the page is the easy part; the behaviour is what makes
  it usable:

  - Escape closes it, because that is where everyone's hand goes first;
  - focus moves into the dialog when it opens, so a keyboard user is not left
    tabbing through the page behind it;
  - the page behind stops scrolling, otherwise the background drifts while the
    dialog stays put;
  - clicking the backdrop closes, clicking inside does not.

  `aria-modal` plus a labelled heading is what tells a screen reader that the
  rest of the page is, for now, out of play.
*/

export function Dialog({ open, title, description, onClose, children }) {
  const titleId = useId()
  const panelRef = useRef(null)

  useEffect(() => {
    if (!open) return undefined

    const onKeyDown = (event) => {
      if (event.key === 'Escape') onClose?.()
    }

    document.addEventListener('keydown', onKeyDown)

    const previousOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'

    // Waits for the panel to exist before handing it the focus.
    const focusTimer = setTimeout(() => {
      const firstField = panelRef.current?.querySelector(
        'input, select, textarea, button:not([data-dialog-dismiss])',
      )
      ;(firstField ?? panelRef.current)?.focus()
    }, 0)

    return () => {
      document.removeEventListener('keydown', onKeyDown)
      document.body.style.overflow = previousOverflow
      clearTimeout(focusTimer)
    }
  }, [open, onClose])

  if (!open) return null

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-graphite/55 p-4"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget) onClose?.()
      }}
    >
      <div
        ref={panelRef}
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        tabIndex={-1}
        className="max-h-[88vh] w-[540px] max-w-full overflow-y-auto border border-line bg-white outline-none"
      >
        <header className="flex items-start gap-4 border-b border-line-soft px-6 py-5">
          <div className="min-w-0">
            <h2 id={titleId} className="text-lg font-semibold tracking-tight">
              {title}
            </h2>
            {description && <p className="mt-1 text-[13px] text-muted">{description}</p>}
          </div>

          <button
            type="button"
            data-dialog-dismiss
            onClick={onClose}
            aria-label="Close"
            className="ml-auto cursor-pointer text-2xl leading-none text-muted hover:text-graphite"
          >
            ×
          </button>
        </header>

        {children}
      </div>
    </div>
  )
}
