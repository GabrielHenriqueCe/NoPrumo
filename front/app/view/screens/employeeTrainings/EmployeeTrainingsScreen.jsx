import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { EmployeeTrainingFormDialog } from './EmployeeTrainingFormDialog'
import { formatDate } from '../../format/dates'
import { statusBadge } from './trainingLabels'
import { useT } from '../../preferences/preferencesContext'

// Expiry date and status come from the API; the screen never works them out.

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

// The API serves at most 100 per page; the page cap only stops a runaway loop.
const EMPLOYEE_PAGE_SIZE = 100
const MAX_EMPLOYEE_PAGES = 50

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function EmployeeTrainingsScreen() {
  const t = useT()
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
        if (!controller.signal.aborted) setLoadError(error.message ?? t('employeeTrainingsScreen.couldNotLoadTrainings'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [employeeTrainings, page, appliedSearch, typeFilter, reloadToken, t])

  // Loaded separately so one failing does not empty the other.
  useEffect(() => {
    const controller = new AbortController()
    const fail = (error) => {
      if (!controller.signal.aborted) setLookupError(error.message ?? t('employeeTrainingsScreen.couldNotLoadTheForm'))
    }

    listAllEmployees(employees, controller.signal).then(setEmployeeList).catch(fail)

    trainingTypes
      .list({ page: 1, size: 100 }, { signal: controller.signal })
      .then((result) => setTypeList(result.items))
      .catch(fail)

    return () => controller.abort()
  }, [employees, trainingTypes, t])

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
      <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
        <div className="min-w-0">
          <p className="label">{t('employeeTrainingsScreen.masterData')}</p>
          <h1 className="display mt-1 text-[38px] leading-none">{t('employeeTrainingsScreen.trainings')}</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="training-search">
            {t('employeeTrainingsScreen.searchTrainings')}
          </label>
          <input
            id="training-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t('employeeTrainingsScreen.searchByEmployeeOrTraining')}
            className="w-72 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
          />

          <div className="w-56">
            <SelectField
              label={t('employeeTrainingsScreen.trainingType')}
              name="training-type-filter"
              value={typeFilter}
              onChange={(event) => {
                setTypeFilter(event.target.value)
                setPage(1)
              }}
              placeholder={t('employeeTrainingsScreen.allTrainingTypes')}
              options={typeList.map((type) => ({ value: type.id, label: `${type.code} — ${type.name}` }))}
            />
          </div>

          <Button onClick={() => openForm(null)}>{t('employeeTrainingsScreen.newTraining')}</Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('employeeTrainingsScreen.eachRowIsOneCertificate')}
        </p>

        {lookupError && (
          <p role="alert" className="mb-4 rounded-2xl bg-danger-wash px-4 py-2.5 text-[13px] text-danger">
            {lookupError}
          </p>
        )}

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
                {appliedSearch ? t('employeeTrainingsScreen.noTrainingMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('employeeTrainingsScreen.noTrainingRecordedYet')}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('employeeTrainingsScreen.recordedTrainings')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>{t('employeeTrainingsScreen.employee')}</HeaderCell>
                  <HeaderCell>{t('employeeTrainingsScreen.training')}</HeaderCell>
                  <HeaderCell>{t('employeeTrainingsScreen.issued')}</HeaderCell>
                  <HeaderCell>{t('employeeTrainingsScreen.expires')}</HeaderCell>
                  <HeaderCell>{t('employeeTrainingsScreen.workload')}</HeaderCell>
                  <HeaderCell>{t('employeeTrainingsScreen.modality')}</HeaderCell>
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
                          <Badge tone={badge.tone}>{t(badge.labelKey)}</Badge>
                        </div>
                      </td>

                      <td className="px-4 py-3 whitespace-nowrap">
                        {training.workloadHours != null ? `${training.workloadHours} h` : '—'}
                      </td>

                      <td className="px-4 py-3">{training.modality ? t(`labels.modality.${training.modality}`) : '—'}</td>

                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            aria-label={t('employeeTrainingsScreen.editTrainingtypecodeOfEmployeename', { trainingTypeCode: training.trainingTypeCode, employeeName: training.employeeName })}
                            onClick={() => openForm(training)}
                          >
                            {t('common.edit')}
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
          <Pagination noun="training" page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
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
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('employeeTrainingsScreen.loadingTrainings')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
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
