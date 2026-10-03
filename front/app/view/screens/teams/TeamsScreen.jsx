import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { TeamFormDialog } from './TeamFormDialog'

/*
  Teams: who works together, inside a department. The team dialog carries a
  second tab with the members, because a member is not something anyone
  registers on its own — it only exists inside a team.

  A team is switched off, never deleted, and the API refuses to switch off
  one that still has people in it: each member leaves with their own date,
  and only whoever runs the team knows which.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function TeamsScreen() {
  const { teams, departments } = useContainer()

  const [data, setData] = useState(EMPTY_PAGE)
  const [departmentList, setDepartmentList] = useState([])
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [departmentFilter, setDepartmentFilter] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [actionError, setActionError] = useState(null)
  const [pendingId, setPendingId] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingTeam, setEditingTeam] = useState(null)
  const [dialogTab, setDialogTab] = useState('details')

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

    teams
      .list(
        { page, size: PAGE_SIZE, search: appliedSearch, departmentId: departmentFilter || null },
        { signal: controller.signal },
      )
      .then((result) => setData(result))
      .catch((error) => {
        // An aborted request is this effect being replaced, not a failure.
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load teams.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [teams, page, appliedSearch, departmentFilter, reloadToken])

  /*
    Departments feed the filter and the form's dropdown; fetched once, not on
    every list refresh. They come from slice 2: until someone registers one,
    this list is empty and so is the dropdown — which is the right answer.
    That API returns them in no particular order, so they are sorted here.
  */
  useEffect(() => {
    const controller = new AbortController()

    departments
      .list({ page: 1, size: 100 }, { signal: controller.signal })
      .then((result) =>
        setDepartmentList([...result.items].sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'))),
      )
      .catch(() => {
        if (!controller.signal.aborted) setDepartmentList([])
      })

    return () => controller.abort()
  }, [departments])

  // Kept stable on purpose: the dialog moves the focus back to its first
  // control whenever onClose changes, and the list reloads behind it while
  // members are added.
  const closeForm = useCallback(() => {
    setFormOpen(false)
    setEditingTeam(null)
  }, [])

  const openForm = (team, tab = 'details') => {
    setEditingTeam(team)
    setDialogTab(tab)
    setFormOpen(true)
  }

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

  const saveTeam = async (values) => {
    if (editingTeam) {
      await teams.update(editingTeam.id, values)
      closeForm()
    } else {
      // A new team is empty. The dialog stays open on the members tab, which
      // is the next thing anyone does with it.
      const created = await teams.create(values)
      setEditingTeam(created)
      setDialogTab('members')
    }

    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Teams</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="team-search">
            Search teams
          </label>
          <input
            id="team-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by team or department"
            className="w-64 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <div className="w-56">
            <SelectField
              label="Department"
              name="department-filter"
              value={departmentFilter}
              onChange={(event) => {
                setDepartmentFilter(event.target.value)
                setPage(1)
              }}
              placeholder="All departments"
              options={departmentList.map((department) => ({ value: department.id, label: department.name }))}
            />
          </div>

          <Button onClick={() => openForm(null)}>New team</Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          A team belongs to a department. Members join and leave with a date: ending a stint keeps the
          record of who was on the team, and when. A team is switched off, never deleted — and only
          once nobody is left in it.
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
                {appliedSearch ? `No team matches “${appliedSearch}”.` : 'No team registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered teams</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <Th>Name</Th>
                  <Th>Department</Th>
                  <Th>Members</Th>
                  <Th>Status</Th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((team) => {
                  const busy = pendingId === team.id

                  return (
                    <tr key={team.id} className="border-b border-line-soft last:border-0">
                      <td className="px-4 py-3 font-medium">{team.name}</td>
                      <td className="px-4 py-3">{team.departmentName}</td>
                      <td className="px-4 py-3">{team.memberCount}</td>

                      <td className="px-4 py-3">
                        <Badge tone={team.active ? 'active' : 'inactive'}>
                          {team.active ? 'Active' : 'Inactive'}
                        </Badge>
                      </td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            aria-label={`Members of ${team.name}`}
                            disabled={busy}
                            onClick={() => openForm(team, 'members')}
                          >
                            Members
                          </Button>

                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            aria-label={`Edit ${team.name}`}
                            disabled={busy}
                            onClick={() => openForm(team)}
                          >
                            Edit
                          </Button>

                          <Button
                            variant={team.active ? 'danger' : 'outline'}
                            className="px-3 py-1.5"
                            aria-label={`${team.active ? 'Deactivate' : 'Activate'} ${team.name}`}
                            busy={busy}
                            disabled={busy}
                            onClick={() => runAction(team.id, () => teams.setActive(team.id, !team.active))}
                          >
                            {team.active ? 'Deactivate' : 'Activate'}
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

      <TeamFormDialog
        open={formOpen}
        team={editingTeam}
        initialTab={dialogTab}
        departments={departmentList}
        onClose={closeForm}
        onSubmit={saveTeam}
        onMembersChanged={reload}
      />
    </>
  )
}

function Th({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

/*
  Placeholder rows instead of a spinner: the table keeps its shape while it
  loads, so the page does not jump when the data lands.
*/
function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading teams">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
