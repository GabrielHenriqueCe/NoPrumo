import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { usePreferences } from '../../preferences/preferencesContext'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { ProgressTrack } from '../../ui/ProgressTrack'
import { ScreenHeader } from '../../ui/ScreenHeader'
import { ProjectFormDialog } from './ProjectFormDialog'
import { useProjectLookups } from './useProjectLookups'

/*
  The projects, as H cards: name, code and city, the progress in big numbers
  and the late alarm. Progress and late both come calculated from the API.
  Editing, the client portal and switching a project off live on the project
  page; here a new project can be created.
*/

const PAGE_SIZE = 12
const SEARCH_DEBOUNCE_MS = 350
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function ProjectsScreen() {
  const { projects } = useContainer()
  const { can } = useSession()
  const { t, format } = usePreferences()
  const canSeeAmount = can('view_finance')
  const canManage = can('manage_projects')
  const { clientOptions, supervisorOptions } = useProjectLookups(canManage)

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)
  const [formOpen, setFormOpen] = useState(false)

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
        if (!controller.signal.aborted) setLoadError(error.message ?? t('common.couldNotLoad'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [projects, page, appliedSearch, reloadToken, t])

  const createProject = async (values) => {
    await projects.create(values)
    setFormOpen(false)
    reload()
  }

  const nothingYet = !loading && !loadError && data.total === 0 && !appliedSearch

  return (
    <>
      <ScreenHeader title={t('projects.title')} count={loading ? null : data.total}>
        <label className="sr-only" htmlFor="project-search">
          {t('projects.search')}
        </label>
        <input
          id="project-search"
          type="search"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder={t('projects.searchPlaceholder')}
          className="h-12 w-64 max-w-full rounded-full border border-line bg-surface px-5 text-ink outline-none focus:border-accent"
        />
        {canManage && <Button onClick={() => setFormOpen(true)}>+ {t('projects.new')}</Button>}
      </ScreenHeader>

      <div className="flex flex-1 flex-col px-10 pt-6 pb-10">
        {loading && (
          <div className="grid gap-[18px] [grid-template-columns:repeat(auto-fill,minmax(340px,1fr))]" role="status" aria-label={t('common.loading')}>
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="h-52 animate-pulse rounded-[22px] bg-surface" />
            ))}
          </div>
        )}

        {!loading && loadError && (
          <div className="rounded-3xl border border-line bg-surface px-6 py-10 text-center">
            <p className="text-danger">{loadError}</p>
            <Button variant="outline" onClick={reload} className="mt-4">
              {t('common.tryAgain')}
            </Button>
          </div>
        )}

        {nothingYet && <EmptyProjects canManage={canManage} onCreate={() => setFormOpen(true)} />}

        {!loading && !loadError && data.total === 0 && appliedSearch && (
          <p className="rounded-3xl border border-line bg-surface px-6 py-12 text-center text-muted">
            {t('projects.noMatch', { search: appliedSearch })}
          </p>
        )}

        {!loading && !loadError && data.items.length > 0 && (
          <>
            <div className="grid gap-[18px] [grid-template-columns:repeat(auto-fill,minmax(340px,1fr))]">
              {data.items.map((project) => (
                <ProjectCard key={project.id} project={project} t={t} format={format} />
              ))}
            </div>

            <Pagination noun="project" page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
          </>
        )}
      </div>

      <ProjectFormDialog
        open={formOpen}
        project={null}
        clientOptions={clientOptions}
        supervisorOptions={supervisorOptions}
        canSeeAmount={canSeeAmount}
        onClose={() => setFormOpen(false)}
        onSubmit={createProject}
      />
    </>
  )
}

function ProjectCard({ project, t, format }) {
  const late = project.active && project.late
  const place = [project.city, project.state].filter(Boolean).join(' / ')

  return (
    <Link
      to={`/projects/${project.id}`}
      className={[
        'flex flex-col gap-3.5 rounded-[22px] border-[1.5px] bg-surface px-6 py-[22px] transition-colors hover:border-accent',
        late ? 'border-late' : 'border-line',
        project.active ? '' : 'opacity-60',
      ].join(' ')}
    >
      <div className="flex items-start gap-3">
        <div className="min-w-0 flex-1">
          <div className="display text-[26px]">{project.name}</div>
          <div className="mt-1 font-mono text-[13px] text-muted">
            {project.code}
            {place && ` · ${place}`}
          </div>
        </div>
        {late && <Badge tone="late">{t('common.late')}</Badge>}
        {!project.active && <Badge tone="inactive">{t('common.inactive')}</Badge>}
      </div>

      <div className="flex items-baseline gap-3">
        <span className={`display text-[48px] leading-none ${late ? 'text-late' : 'text-accent'}`}>
          {format.percent(project.progress)}
        </span>
        <span className="font-medium text-muted">{t(`projectStatus.${project.status}`)}</span>
      </div>

      <ProgressTrack value={project.progress} late={late} label={t('projects.progress')} />

      <div className="flex flex-wrap gap-x-5 gap-y-1.5 text-[14.5px] text-muted">
        {project.clientName && <span>{project.clientName}</span>}
        <span>
          {t('projects.forecast')} <span className="font-mono text-[13px]">{format.date(project.forecastDate)}</span>
        </span>
      </div>
    </Link>
  )
}

function EmptyProjects({ canManage, onCreate }) {
  const { t } = usePreferences()

  return (
    <section
      aria-labelledby="empty-projects-title"
      className="flex flex-1 flex-col items-center justify-center rounded-3xl border border-line bg-surface px-6 py-10 text-center"
    >
      <div aria-hidden="true" className="h-24 w-px bg-line" />
      <svg width="24" height="32" viewBox="0 0 22 30" aria-hidden="true">
        <path d="M7 0 H15 V4 H7 Z" fill="#808080" />
        <path d="M7 4 L1 10 L11 30 L21 10 L15 4 Z" fill="#d9b451" />
      </svg>
      <p className="label mt-7">{t('nouns.project', { count: 0 })}</p>
      <h2 id="empty-projects-title" className="display mt-2.5 max-w-[560px] text-[42px]">
        {t('projects.emptyTitle')}
      </h2>
      <p className="mt-3 max-w-[440px] leading-relaxed text-muted">{t('projects.emptyText')}</p>
      {canManage && (
        <Button className="mt-6 min-h-14 px-8 text-[19px]" onClick={onCreate}>
          + {t('projects.new')}
        </Button>
      )}
    </section>
  )
}
