import { useT } from '../preferences/preferencesContext'
import { Button } from './Button'

/*
  `noun` names what is being counted: "3 projects", not "3 records". It is
  also the key under `nouns` in the dictionaries, so the count reads right in
  both languages; a noun with no entry there falls back to the English plural.
*/
export function Pagination({ page, totalPages, total, onChange, busy = false, noun = 'record', pluralNoun = `${noun}s` }) {
  const t = useT()
  const lastPage = Math.max(totalPages, 1)

  const translated = t(`nouns.${noun}`, { count: total })
  const counted = translated === `nouns.${noun}` ? `${total} ${total === 1 ? noun : pluralNoun}` : translated

  return (
    <nav className="mt-4 flex flex-wrap items-center gap-3" aria-label={t('common.pagination')}>
      <Button variant="outline" onClick={() => onChange(page - 1)} disabled={busy || page <= 1} className="px-4 py-1.5">
        {t('common.previous')}
      </Button>

      <span aria-live="polite" className="text-[13px] text-muted">
        {t('common.pageOf', { page, totalPages: lastPage })} · {counted}
      </span>

      <Button
        variant="outline"
        onClick={() => onChange(page + 1)}
        disabled={busy || page >= lastPage}
        className="px-4 py-1.5"
      >
        {t('common.next')}
      </Button>
    </nav>
  )
}
