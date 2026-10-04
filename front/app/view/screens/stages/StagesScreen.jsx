import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { StageFormDialog } from './StageFormDialog'
import { stageStatusLabel } from './stageStatus'

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }
// 100 is the API's page cap.
const LOOKUP_SIZE = 100

const percent = new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 2 })

function formatDate(value) {
  if (!value) return '—'
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

export function StagesScreen() {
  const { stages, projects, teams, employees } = useContainer()

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [projectOptions, setProjectOptions] = useState([])
  const [teamOptions, setTeamOptions] = useState([])
  const [supervisorOptions, setSupervisorOptions] = useState([])

  const [formOpen, setFormOpen] = useState(false)
  const [editingStage, setEditingStage] = useState(null)

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

    stages
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load stages.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [stages, page, appliedSearch, reloadToken])

  useEffect(() => {
    const controller = new AbortController()
    const options = { signal: controller.signal }
    const activeOptions = (items, toLabel) =>
      items.filter((item) => item.active).map((item) => ({ value: item.id, label: toLabel(item) }))

    projects
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setProjectOptions(activeOptions(result.items, (project) => `${project.code} · ${project.name}`)))
      .catch(() => {})

    teams
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setTeamOptions(activeOptions(result.items, (team) => team.name)))
      .catch(() => {})

    employees
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setSupervisorOptions(activeOptions(result.items, (employee) => employee.name)))
      .catch(() => {})

    return () => controller.abort()
  }, [projects, teams, employees])

  const openForm = (stage) => {
    setEditingStage(stage)
    setFormOpen(true)
  }

  const closeForm = () => {
    setFormOpen(false)
    setEditingStage(null)
  }

  const saveStage = async (values) => {
    if (editingStage) {
      await stages.update(editingStage.id, values)
    } else {
      await stages.create(values)
    }

    closeForm()
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Stages</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="stage-search">
            Search stages
          </label>
          <input
            id="stage-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by stage or project"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <Button onClick={() => openForm(null)}>New stage</Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Each stage is a step of a project&apos;s schedule, listed project by project in building order.
          A stage is late when its planned date has passed and it is not completed.
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
                {appliedSearch ? `No stage matches “${appliedSearch}”.` : 'No stage registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered stages</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>Stage</HeaderCell>
                  <HeaderCell>Project</HeaderCell>
                  <HeaderCell>Team</HeaderCell>
                  <HeaderCell>Supervisor</HeaderCell>
                  <HeaderCell>Planned</HeaderCell>
                  <HeaderCell>Progress</HeaderCell>
                  <HeaderCell>Status</HeaderCell>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((stage) => (
                  <tr key={stage.id} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3">
                      <div className="font-medium">{stage.name}</div>
                      <div className="font-mono text-[12.5px] text-muted">#{stage.sortOrder}</div>
                    </td>

                    <td className="px-4 py-3">{stage.projectName}</td>

                    <td className="px-4 py-3">{stage.teamName || '—'}</td>

                    <td className="px-4 py-3">{stage.supervisorName || '—'}</td>

                    <td className="px-4 py-3 whitespace-nowrap">{formatDate(stage.plannedDate)}</td>

                    <td className="px-4 py-3 whitespace-nowrap tabular-nums">{percent.format(stage.percentage)}%</td>

                    <td className="px-4 py-3">
                      <div className="flex flex-wrap gap-1.5">
                        <Badge tone="neutral">{stageStatusLabel(stage.status)}</Badge>
                        {stage.late && <Badge tone="warning">Late</Badge>}
                      </div>
                    </td>

                    <td className="px-4 py-3">
                      <div className="flex justify-end">
                        <Button variant="outline" className="px-3 py-1.5" onClick={() => openForm(stage)}>
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

      <StageFormDialog
        open={formOpen}
        stage={editingStage}
        projectOptions={projectOptions}
        teamOptions={teamOptions}
        supervisorOptions={supervisorOptions}
        onClose={closeForm}
        onSubmit={saveStage}
      />
    </>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading stages">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
