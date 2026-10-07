import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { EmployeeTrainingFormDialog } from './EmployeeTrainingFormDialog'
import { formatDate } from '../../format/dates'
import { modalityLabel, statusBadge } from './trainingLabels'

// Expiry date and status come from the API; the screen never works them out.

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

// The API serves at most 100 per page; the page cap only stops a runaway loop.
const EMPLOYEE_PAGE_SIZE = 100
const MAX_EMPLOYEE_PAGES = 50

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function EmployeeTrainingsScreen() {
  const { employeeTrainings, employees, trainingTypes } = useContainer()

  const [data, setData] = useState(EMPTY_PAGE)
  const [employeeList, setEmployeeList] = useState([])
  const [typeList, setTypeList] = useState([])
  const [lookupError, setLookupError] = useState(null)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [typeFilter, setTypeFilter] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingTraining, setEditingTraining] = useState(null)

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

    employeeTrainings
      .list(
        { page, size: PAGE_SIZE, search: appliedSearch, trainingTypeId: typeFilter || null },
        { signal: controller.signal },
      )
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load trainings.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [employeeTrainings, page, appliedSearch, typeFilter, reloadToken])

  // Loaded separately so one failing does not empty the other.
  useEffect(() => {
    const controller = new AbortController()
    const fail = (error) => {
      if (!controller.signal.aborted) setLookupError(error.message ?? 'Could not load the form lists.')
    }

    listAllEmployees(employees, controller.signal).then(setEmployeeList).catch(fail)

    trainingTypes
      .list({ page: 1, size: 100 }, { signal: controller.signal })
      .then((result) => setTypeList(result.items))
      .catch(fail)

    return () => controller.abort()
  }, [employees, trainingTypes])

  // Stable on purpose: the Dialog refocuses its first field whenever onClose changes.
  const closeForm = useCallback(() => {
    setFormOpen(false)
    setEditingTraining(null)
  }, [])

  const openForm = (training) => {
    setEditingTraining(training)
    setFormOpen(true)
  }

  const saveTraining = async (values) => {
    if (editingTraining) {
      await employeeTrainings.update(editingTraining.id, values)
    } else {
      await employeeTrainings.create(values)
    }

    closeForm()
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Trainings</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="training-search">
            Search trainings
          </label>
          <input
            id="training-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by employee or training"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <div className="w-56">
            <SelectField
              label="Training type"
              name="training-type-filter"
              value={typeFilter}
              onChange={(event) => {
                setTypeFilter(event.target.value)
                setPage(1)
              }}
              placeholder="All training types"
              options={typeList.map((type) => ({ value: type.id, label: `${type.code} — ${type.name}` }))}
            />
          </div>

          <Button onClick={() => openForm(null)}>New training</Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Each row is one certificate. The expiry date is set by the system from the issue date and the
          validity of the training type, and the status compares it with today — “Expires soon” means
          30 days or less. Certificates are history: they are corrected, never deleted.
        </p>

        {lookupError && (
          <p role="alert" className="mb-4 border border-danger-soft bg-[#faefea] px-4 py-2.5 text-[13px] text-danger">
            {lookupError}
          </p>
        )}

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
                {appliedSearch ? `No training matches “${appliedSearch}”.` : 'No training recorded yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Recorded trainings</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>Employee</HeaderCell>
                  <HeaderCell>Training</HeaderCell>
                  <HeaderCell>Issued</HeaderCell>
                  <HeaderCell>Expires</HeaderCell>
                  <HeaderCell>Workload</HeaderCell>
                  <HeaderCell>Modality</HeaderCell>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((training) => {
                  const badge = statusBadge(training.status)

                  return (
                    <tr key={training.id} className="border-b border-line-soft last:border-0">
                      <td className="px-4 py-3">
                        <div className="font-medium">{training.employeeName}</div>
                        {training.registrationNumber && (
                          <div className="text-[12.5px] text-muted">{training.registrationNumber}</div>
                        )}
                      </td>

                      <td className="px-4 py-3">
                        <div className="font-mono text-[13px]">{training.trainingTypeCode}</div>
                        <div className="text-[12.5px] text-muted">{training.trainingTypeName}</div>
                      </td>

                      <td className="px-4 py-3 whitespace-nowrap">{formatDate(training.issueDate)}</td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center gap-2">
                          {training.expiryDate && (
                            <span className="whitespace-nowrap">{formatDate(training.expiryDate)}</span>
                          )}
                          <Badge tone={badge.tone}>{badge.label}</Badge>
                        </div>
                      </td>

                      <td className="px-4 py-3 whitespace-nowrap">
                        {training.workloadHours != null ? `${training.workloadHours} h` : '—'}
                      </td>

                      <td className="px-4 py-3">{modalityLabel(training.modality)}</td>

                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            aria-label={`Edit ${training.trainingTypeCode} of ${training.employeeName}`}
                            onClick={() => openForm(training)}
                          >
                            Edit
                          </Button>
                        </div>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          )}
        </div>

        {!loading && !loadError && (
          <Pagination page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
        )}
      </div>

      <EmployeeTrainingFormDialog
        open={formOpen}
        training={editingTraining}
        employees={employeeList}
        trainingTypes={typeList}
        onClose={closeForm}
        onSubmit={saveTraining}
      />
    </>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading trainings">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}

async function listAllEmployees(employees, signal) {
  const all = []

  for (let page = 1; page <= MAX_EMPLOYEE_PAGES; page += 1) {
    const result = await employees.list({ page, size: EMPLOYEE_PAGE_SIZE }, { signal })
    all.push(...result.items)
    if (page >= result.totalPages || result.items.length === 0) break
  }

  return all
}
