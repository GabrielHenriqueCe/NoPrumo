import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { ProjectFormDialog } from './ProjectFormDialog'
import { projectStatusLabel } from './projectStatus'

/*
  Projects (obra).

  contractAmount: whoever lacks view_finance gets a different DTO from the API,
  without the field. The API is the real gate; the screen only mirrors it, so
  it does not draw an empty column nor a field that would be ignored.

  "Late" comes ready from the API as `late`. The screen never compares dates.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }
// Clients and employees feed the selects. 100 is the API's page cap.
const LOOKUP_SIZE = 100

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

function formatDate(value) {
  if (!value) return '—'
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

export function ProjectsScreen() {
  const { projects, clients, employees } = useContainer()
  const { can } = useSession()
  const canSeeAmount = can('view_finance')

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [actionError, setActionError] = useState(null)
  const [pendingId, setPendingId] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [clientOptions, setClientOptions] = useState([])
  const [supervisorOptions, setSupervisorOptions] = useState([])

  const [formOpen, setFormOpen] = useState(false)
  const [editingProject, setEditingProject] = useState(null)

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

    projects
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load projects.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [projects, page, appliedSearch, reloadToken])

  // Lookups for the dialog. If either fails the select just comes empty —
  // the list of projects still works.
  useEffect(() => {
    const controller = new AbortController()
    const options = { signal: controller.signal }

    clients
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) =>
        setClientOptions(
          result.items.filter((client) => client.active).map((client) => ({ value: client.id, label: client.name })),
        ),
      )
      .catch(() => {})

    employees
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) =>
        setSupervisorOptions(
          result.items
            .filter((employee) => employee.active)
            .map((employee) => ({ value: employee.id, label: employee.name })),
        ),
      )
      .catch(() => {})

    return () => controller.abort()
  }, [clients, employees])

  const runAction = async (id, action) => {
    setPendingId(id)
    setActionError(null)

    try {
      await action()
      reload()
    } catch (error) {
      setActionError(error.message ?? 'The action could not be completed.')
    } finally {
      setPendingId(null)
    }
  }

  const saveProject = async (values) => {
    if (editingProject) {
      await projects.update(editingProject.id, values)
    } else {
      await projects.create(values)
    }

    setFormOpen(false)
    setEditingProject(null)
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Projects</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="project-search">
            Search projects
          </label>
          <input
            id="project-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by code, name or city"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <Button
            onClick={() => {
              setEditingProject(null)
              setFormOpen(true)
            }}
          >
            New project
          </Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Each project is a construction site. A project is late when its forecast date has passed and
          it is not completed. Projects are deactivated, never deleted: stock, time entries and costs
          stay attached to them.
        </p>

        {actionError && (
          <p role="alert" className="mb-4 border border-danger-soft bg-[#faefea] px-4 py-2.5 text-[13px] text-danger">
            {actionError}
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
                {appliedSearch ? `No project matches “${appliedSearch}”.` : 'No project registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered projects</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>Project</HeaderCell>
                  <HeaderCell>Client</HeaderCell>
                  <HeaderCell>Supervisor</HeaderCell>
                  <HeaderCell>Forecast</HeaderCell>
                  {canSeeAmount && <HeaderCell>Contract</HeaderCell>}
                  <HeaderCell>Status</HeaderCell>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((project) => {
                  const busy = pendingId === project.id

                  return (
                    <tr key={project.id} className="border-b border-line-soft last:border-0">
                      <td className="px-4 py-3">
                        <div className="font-medium">{project.name}</div>
                        <div className="font-mono text-[12.5px] text-muted">
                          {project.code}
                          {project.city ? ` · ${project.city}${project.state ? ` / ${project.state}` : ''}` : ''}
                        </div>
                      </td>

                      <td className="px-4 py-3">{project.clientName || '—'}</td>

                      <td className="px-4 py-3">{project.supervisorName || '—'}</td>

                      <td className="px-4 py-3 whitespace-nowrap">{formatDate(project.forecastDate)}</td>

                      {canSeeAmount && (
                        <td className="px-4 py-3 whitespace-nowrap tabular-nums">
                          {project.contractAmount != null ? money.format(project.contractAmount) : '—'}
                        </td>
                      )}

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap gap-1.5">
                          {project.active ? (
                            <Badge tone="neutral">{projectStatusLabel(project.status)}</Badge>
                          ) : (
                            <Badge tone="inactive">Inactive</Badge>
                          )}
                          {project.active && project.late && <Badge tone="warning">Late</Badge>}
                        </div>
                      </td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            disabled={busy}
                            onClick={() => {
                              setEditingProject(project)
                              setFormOpen(true)
                            }}
                          >
                            Edit
                          </Button>

                          <Button
                            variant={project.active ? 'danger' : 'outline'}
                            className="px-3 py-1.5"
                            busy={busy}
                            onClick={() =>
                              runAction(project.id, () => projects.setActive(project.id, !project.active))
                            }
                          >
                            {project.active ? 'Deactivate' : 'Activate'}
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
          <Pagination
            page={data.page}
            totalPages={data.totalPages}
            total={data.total}
            onChange={setPage}
            busy={Boolean(pendingId)}
          />
        )}
      </div>

      <ProjectFormDialog
        open={formOpen}
        project={editingProject}
        clientOptions={clientOptions}
        supervisorOptions={supervisorOptions}
        canSeeAmount={canSeeAmount}
        onClose={() => {
          setFormOpen(false)
          setEditingProject(null)
        }}
        onSubmit={saveProject}
      />
    </>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading projects">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
