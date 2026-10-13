import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { StageFormDialog } from './StageFormDialog'
import { useT } from '../../preferences/preferencesContext'

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

// The "Stages" tab of a project.
export function ProjectStagesPanel({ projectId }) {
  const t = useT()
  const { stages, teams, employees } = useContainer()
  const { can } = useSession()
  const canManage = can('manage_projects')

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

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
      .list({ page, size: PAGE_SIZE, search: appliedSearch, projectId }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('projectStagesPanel.couldNotLoadStages'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [stages, page, appliedSearch, projectId, reloadToken, t])

  useEffect(() => {
    // The lookups only feed the form, and their endpoints refuse whoever cannot open it.
    if (!canManage) return undefined

    const controller = new AbortController()
    const options = { signal: controller.signal }
    const activeOptions = (items, toLabel) =>
      items.filter((item) => item.active).map((item) => ({ value: item.id, label: toLabel(item) }))

    teams
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setTeamOptions(activeOptions(result.items, (team) => team.name)))
      .catch(() => {})

    employees
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setSupervisorOptions(activeOptions(result.items, (employee) => employee.name)))
      .catch(() => {})

    return () => controller.abort()
  }, [teams, employees, canManage])

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
      <div className="px-10 pt-6 pb-10">
        <div className="mb-6 flex flex-wrap items-center gap-4">
          <p className="max-w-[54em] text-sm leading-relaxed text-muted">
            {t('projectStagesPanel.inBuildingOrderAStage')}
          </p>

          <div className="ml-auto flex items-center gap-3">
            <label className="sr-only" htmlFor="stage-search">
              {t('projectStagesPanel.searchStages')}
            </label>
            <input
              id="stage-search"
              type="search"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder={t('projectStagesPanel.searchByStage')}
              className="w-72 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
            />

            {canManage && <Button onClick={() => openForm(null)}>{t('projectStagesPanel.newStage')}</Button>}
          </div>
        </div>

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
                {appliedSearch ? t('projectStagesPanel.noStageMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('projectStagesPanel.noStageRegisteredYet')}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('projectStagesPanel.stagesOfThisProject')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>{t('projectStagesPanel.stage')}</HeaderCell>
                  <HeaderCell>{t('projectStagesPanel.team')}</HeaderCell>
                  <HeaderCell>{t('projectStagesPanel.supervisor')}</HeaderCell>
                  <HeaderCell>{t('projectStagesPanel.planned')}</HeaderCell>
                  <HeaderCell>{t('projectStagesPanel.progress')}</HeaderCell>
                  <HeaderCell>{t('common.status')}</HeaderCell>
                  {canManage && <th className="px-4 py-3" />}
                </tr>
              </thead>

              <tbody>
                {data.items.map((stage) => (
                  <tr key={stage.id} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3">
                      <div className="font-medium">{stage.name}</div>
                      <div className="font-mono text-[12.5px] text-muted">#{stage.sortOrder}</div>
                    </td>

                    <td className="px-4 py-3">{stage.teamName || '—'}</td>

                    <td className="px-4 py-3">{stage.supervisorName || '—'}</td>

                    <td className="px-4 py-3 whitespace-nowrap">{formatDate(stage.plannedDate)}</td>

                    <td className="px-4 py-3 whitespace-nowrap tabular-nums">{percent.format(stage.percentage)}%</td>

                    <td className="px-4 py-3">
                      <div className="flex flex-wrap gap-1.5">
                        <Badge tone="neutral">{t(`stageStatus.${stage.status}`)}</Badge>
                        {stage.late && <Badge tone="warning">{t('common.late')}</Badge>}
                      </div>
                    </td>

                    {canManage && (
                      <td className="px-4 py-3">
                        <div className="flex justify-end">
                          <Button variant="outline" className="px-3 py-1.5" onClick={() => openForm(stage)}>
                            {t('common.edit')}
                          </Button>
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {!loading && !loadError && (
          <Pagination noun="stage" page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
        )}
      </div>

      <StageFormDialog
        open={formOpen}
        stage={editingStage}
        projectId={projectId}
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
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('projectStagesPanel.loadingStages')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
