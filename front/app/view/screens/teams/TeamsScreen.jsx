import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { TeamFormDialog } from './TeamFormDialog'
import { useT } from '../../preferences/preferencesContext'


const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function TeamsScreen() {
  const t = useT()
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
        if (!controller.signal.aborted) setLoadError(error.message ?? t('teamsScreen.couldNotLoadTeams'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [teams, page, appliedSearch, departmentFilter, reloadToken, t])

  // The departments API returns them unordered.
  useEffect(() => {
    const controller = new AbortController()

    departments
      .list({ page: 1, size: 100 }, { signal: controller.signal })
      .then((result) =>
        setDepartmentList([...result.items].sort((first, second) => first.name.localeCompare(second.name, 'pt-BR'))),
      )
      .catch(() => {
        if (!controller.signal.aborted) setDepartmentList([])
      })

    return () => controller.abort()
  }, [departments])

  // Stable on purpose: the Dialog refocuses its first control whenever onClose changes.
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
      setActionError(error.message ?? t('teamsScreen.theActionCouldNotBe'))
    } finally {
      setPendingId(null)
    }
  }

  const saveTeam = async (values) => {
    if (editingTeam) {
      await teams.update(editingTeam.id, values)
      closeForm()
    } else {
      // A new team is empty: the dialog stays open on the members tab.
      const created = await teams.create(values)
      setEditingTeam(created)
      setDialogTab('members')
    }

    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
        <div className="min-w-0">
          <p className="label">{t('teamsScreen.masterData')}</p>
          <h1 className="display mt-1 text-[38px] leading-none">{t('teamsScreen.teams')}</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="team-search">
            {t('teamsScreen.searchTeams')}
          </label>
          <input
            id="team-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t('teamsScreen.searchByTeamOrDepartment')}
            className="w-64 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
          />

          <div className="w-56">
            <SelectField
              label={t('teamsScreen.department')}
              name="department-filter"
              value={departmentFilter}
              onChange={(event) => {
                setDepartmentFilter(event.target.value)
                setPage(1)
              }}
              placeholder={t('teamsScreen.allDepartments')}
              options={departmentList.map((department) => ({ value: department.id, label: department.name }))}
            />
          </div>

          <Button onClick={() => openForm(null)}>{t('teamsScreen.newTeam')}</Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('teamsScreen.aTeamBelongsToA')}
        </p>

        {actionError && (
          <p role="alert" className="mb-4 rounded-2xl bg-danger-wash px-4 py-2.5 text-[13px] text-danger">
            {actionError}
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
                {appliedSearch ? t('teamsScreen.noTeamMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('teamsScreen.noTeamRegisteredYet')}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('teamsScreen.registeredTeams')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>{t('common.name')}</HeaderCell>
                  <HeaderCell>{t('teamsScreen.department')}</HeaderCell>
                  <HeaderCell>{t('teamsScreen.members')}</HeaderCell>
                  <HeaderCell>{t('common.status')}</HeaderCell>
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
                          {team.active ? t('common.active') : t('common.inactive')}
                        </Badge>
                      </td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            aria-label={t('teamsScreen.membersOfName', { name: team.name })}
                            disabled={busy}
                            onClick={() => openForm(team, 'members')}
                          >
                            {t('teamsScreen.members')}
                          </Button>

                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            aria-label={`Edit ${team.name}`}
                            disabled={busy}
                            onClick={() => openForm(team)}
                          >
                            {t('common.edit')}
                          </Button>

                          <Button
                            variant={team.active ? 'danger' : 'outline'}
                            className="px-3 py-1.5"
                            aria-label={`${team.active ? 'Deactivate' : 'Activate'} ${team.name}`}
                            busy={busy}
                            disabled={busy}
                            onClick={() => runAction(team.id, () => teams.setActive(team.id, !team.active))}
                          >
                            {team.active ? t('common.deactivate') : t('common.activate')}
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
            noun="team"
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

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('teamsScreen.loadingTeams')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
