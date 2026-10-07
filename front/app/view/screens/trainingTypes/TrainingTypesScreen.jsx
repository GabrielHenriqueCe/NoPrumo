import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { TrainingTypeFormDialog } from './TrainingTypeFormDialog'

/*
  Training types: the NR courses, the ASO and any other certificate an
  employee may need, each with how long it stays valid.

  No activate/deactivate here: the table has no active/deletedAt column, and
  deleting would break every training already recorded against the type.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function TrainingTypesScreen() {
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

  // Typing should not fire a request per keystroke; it waits for a pause.
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
        // An aborted request is this effect being replaced, not a failure.
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load training types.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [trainingTypes, page, appliedSearch, reloadToken])

  // Kept stable on purpose: the dialog moves the focus back to its first
  // field whenever onClose changes, and this screen re-renders on every load.
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
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Training types</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="training-type-search">
            Search training types
          </label>
          <input
            id="training-type-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by code or name"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <Button onClick={() => openForm(null)}>New training type</Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          NR courses, the ASO and every other certificate an employee may need. The validity sets the
          expiry date of each training recorded from now on — changing it does not move the trainings
          already saved. There is no deactivate here: those trainings keep pointing to their type.
        </p>

        <div className="border border-line bg-white">
          {loading && <TableSkeleton />}

          {!loading && loadError && (
            <div className="px-6 py-10 text-center">
              <p className="text-sm text-danger">{loadError}</p>
              <Button variant="outline" onClick={reload} className="mt-4">
                Try again
              </Button>
            </div>
          )}

          {!loading && !loadError && data.items.length === 0 && (
            <div className="px-6 py-12 text-center">
              <p className="text-sm text-muted">
                {appliedSearch
                  ? `No training type matches “${appliedSearch}”.`
                  : 'No training type registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered training types</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>Code</HeaderCell>
                  <HeaderCell>Name</HeaderCell>
                  <HeaderCell>Validity</HeaderCell>
                  <HeaderCell>Min. workload</HeaderCell>
                  <HeaderCell>In person</HeaderCell>
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
                        <Badge tone="warning">Required</Badge>
                      ) : (
                        <span className="text-muted">Not required</span>
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
                          Edit
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
          <Pagination page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
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

/*
  Placeholder rows instead of a spinner: the table keeps its shape while it
  loads, so the page does not jump when the data lands.
*/
function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading training types">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
