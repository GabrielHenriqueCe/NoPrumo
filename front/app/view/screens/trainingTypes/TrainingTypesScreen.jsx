import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { TrainingTypeFormDialog } from './TrainingTypeFormDialog'
import { useT } from '../../preferences/preferencesContext'

// No deactivate: the table has no active/deletedAt column.

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function TrainingTypesScreen() {
  const t = useT()
  const { trainingTypes } = useContainer()

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingType, setEditingType] = useState(null)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  useEffect(() => {
    const timer = setTimeout(() => {
      setAppliedSearch(search.trim())
      setPage(1)
    }, SEARCH_DEBOUNCE_MS)

    return () => clearTimeout(timer)
  }, [search])

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    trainingTypes
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('trainingTypesScreen.couldNotLoadTrainingTypes'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [trainingTypes, page, appliedSearch, reloadToken, t])

  // Stable on purpose: the Dialog refocuses its first field whenever onClose changes.
  const closeForm = useCallback(() => {
    setFormOpen(false)
    setEditingType(null)
  }, [])

  const openForm = (type) => {
    setEditingType(type)
    setFormOpen(true)
  }

  const saveType = async (values) => {
    if (editingType) {
      await trainingTypes.update(editingType.id, values)
    } else {
      await trainingTypes.create(values)
    }

    closeForm()
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
        <div className="min-w-0">
          <p className="label">{t('trainingTypesScreen.masterData')}</p>
          <h1 className="display mt-1 text-[38px] leading-none">{t('trainingTypesScreen.trainingTypes')}</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="training-type-search">
            {t('trainingTypesScreen.searchTrainingTypes')}
          </label>
          <input
            id="training-type-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t('trainingTypesScreen.searchByCodeOrName')}
            className="w-72 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
          />

          <Button onClick={() => openForm(null)}>{t('trainingTypesScreen.newTrainingType')}</Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('trainingTypesScreen.nrCoursesTheAsoAnd')}
        </p>

        <div className="overflow-hidden rounded-3xl border border-line bg-surface">
          {loading && <TableSkeleton />}

          {!loading && loadError && (
            <div className="px-6 py-10 text-center">
              <p className="text-sm text-danger">{loadError}</p>
              <Button variant="outline" onClick={reload} className="mt-4">
                {t('common.tryAgain')}
              </Button>
            </div>
          )}

          {!loading && !loadError && data.items.length === 0 && (
            <div className="px-6 py-12 text-center">
              <p className="text-sm text-muted">
                {appliedSearch
                  ? t('trainingTypesScreen.noTrainingTypeMatchesAppliedsearch', { appliedSearch: appliedSearch })
                  : t('trainingTypesScreen.noTrainingTypeRegisteredYet')}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('trainingTypesScreen.registeredTrainingTypes')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>{t('trainingTypesScreen.code')}</HeaderCell>
                  <HeaderCell>{t('common.name')}</HeaderCell>
                  <HeaderCell>{t('trainingTypesScreen.validity')}</HeaderCell>
                  <HeaderCell>{t('trainingTypesScreen.minWorkload')}</HeaderCell>
                  <HeaderCell>{t('trainingTypesScreen.inPerson')}</HeaderCell>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((type) => (
                  <tr key={type.id} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3 font-mono text-[13px]">{type.code}</td>
                    <td className="px-4 py-3 font-medium">{type.name}</td>
                    <td className="px-4 py-3">{describeValidity(type.validityMonths)}</td>
                    <td className="px-4 py-3">
                      {type.minWorkloadHours != null ? `${type.minWorkloadHours} h` : '—'}
                    </td>

                    <td className="px-4 py-3">
                      {type.requiresInPerson ? (
                        <Badge tone="warning">{t('trainingTypesScreen.required')}</Badge>
                      ) : (
                        <span className="text-muted">{t('trainingTypesScreen.notRequired')}</span>
                      )}
                    </td>

                    <td className="px-4 py-3">
                      <div className="flex items-center justify-end gap-2">
                        <Button
                          variant="outline"
                          className="px-3 py-1.5"
                          aria-label={`Edit ${type.code}`}
                          onClick={() => openForm(type)}
                        >
                          {t('common.edit')}
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {!loading && !loadError && (
          <Pagination noun="training type" page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
        )}
      </div>

      <TrainingTypeFormDialog
        open={formOpen}
        trainingType={editingType}
        onClose={closeForm}
        onSubmit={saveType}
      />
    </>
  )
}

function describeValidity(months) {
  if (months == null) return 'No expiry'
  return months === 1 ? '1 month' : `${months} months`
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('trainingTypesScreen.loadingTrainingTypes')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
