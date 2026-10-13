import { useCallback, useEffect, useState } from 'react'
import { Link, Navigate, useParams } from 'react-router-dom'
import { usePreferences } from '../../preferences/preferencesContext'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { PillTabs } from '../../ui/PillTabs'
import { ProjectPpePanel } from '../ppeIssues/ProjectPpePanel'
import { ProjectTeamsPanel } from '../projectTeams/ProjectTeamsPanel'
import { ProjectStagesPanel } from '../stages/ProjectStagesPanel'
import { ProjectFinancePanel } from './ProjectFinancePanel'
import { ProjectFormDialog } from './ProjectFormDialog'
import { ProjectLinksDialog } from './ProjectLinksDialog'
import { ProjectMaterialPanel } from './ProjectMaterialPanel'
import { ProjectOverview } from './ProjectOverview'
import { useProjectLookups } from './useProjectLookups'

/*
  The project page: everything about one project, in tabs. Each tab is a
  component that receives the project; the tab list is the only place that
  knows which component goes where and who may see it.

  The header also holds what is done to the project as a whole: edit it, share
  the client portal, switch it off.
*/

const PROJECT_TABS = [
  {
    key: 'overview',
    permission: 'view_projects',
    render: (project) => <ProjectOverview project={project} />,
  },
  {
    key: 'stages',
    permission: 'view_projects',
    render: (project) => <ProjectStagesPanel projectId={project.id} />,
  },
  {
    key: 'material',
    permission: 'view_stock',
    render: (project) => <ProjectMaterialPanel projectId={project.id} />,
  },
  {
    key: 'team',
    permission: 'manage_projects',
    render: (project) => (
      <div className="px-10 pt-5 pb-10">
        <ProjectTeamsPanel projectId={project.id} />
      </div>
    ),
  },
  {
    // The API's view_ppe: whoever issues PPE, and whoever only reads it on site.
    key: 'ppe',
    permission: ['manage_safety', 'view_stock'],
    render: (project) => (
      <div className="px-10 pt-5 pb-10">
        <ProjectPpePanel projectId={project.id} />
      </div>
    ),
  },
  {
    key: 'finance',
    permission: 'view_finance',
    render: (project) => <ProjectFinancePanel projectId={project.id} />,
  },
]

const FIRST_TAB = PROJECT_TABS[0].key

export function ProjectScreen() {
  const { projectId, tab } = useParams()
  const { projects } = useContainer()
  const { can } = useSession()
  const { t, format } = usePreferences()
  const canManage = can('manage_projects')
  const { clientOptions, supervisorOptions } = useProjectLookups(canManage)

  const [project, setProject] = useState(null)
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)
  const [formOpen, setFormOpen] = useState(false)
  const [linksOpen, setLinksOpen] = useState(false)
  const [busy, setBusy] = useState(false)
  const [actionError, setActionError] = useState(null)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    projects
      .getById(projectId, { signal: controller.signal })
      .then((result) => setProject(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error)
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [projects, projectId, reloadToken])

  const tabs = PROJECT_TABS.filter((projectTab) => can(projectTab.permission))
  const currentTab = tabs.find((projectTab) => projectTab.key === (tab ?? FIRST_TAB))
  const basePath = `/projects/${projectId}`

  // The first tab lives at the bare address, so /overview and unknown tabs fold back into it.
  if (!currentTab || tab === FIRST_TAB) return <Navigate to={basePath} replace />

  if (loading && !project) {
    return (
      <div className="px-10 pt-7" role="status" aria-label={t('common.loading')}>
        <div className="h-12 w-80 animate-pulse rounded-2xl bg-surface" />
        <div className="mt-4 h-12 w-[520px] max-w-full animate-pulse rounded-full bg-surface" />
      </div>
    )
  }

  if (loadError) {
    return (
      <div className="px-10 py-8">
        <div className="rounded-3xl border border-line bg-surface px-6 py-10 text-center">
          <p className="text-danger">
            {loadError.status === 404 ? t('project.notFound') : (loadError.message ?? t('common.couldNotLoad'))}
          </p>
          <div className="mt-4 flex justify-center gap-2.5">
            {loadError.status !== 404 && (
              <Button variant="outline" onClick={reload}>
                {t('common.tryAgain')}
              </Button>
            )}
            <Link to="/projects" className="px-4 py-2.5 font-semibold text-accent hover:underline">
              {t('project.backToList')}
            </Link>
          </div>
        </div>
      </div>
    )
  }

  const toggleActive = async () => {
    setBusy(true)
    setActionError(null)
    try {
      await projects.setActive(project.id, !project.active)
      reload()
    } catch (error) {
      setActionError(error.message ?? t('common.couldNotSave'))
    } finally {
      setBusy(false)
    }
  }

  const saveProject = async (values) => {
    await projects.update(project.id, values)
    setFormOpen(false)
    reload()
  }

  const place = [project.city, project.state].filter(Boolean).join(' / ')
  const facts = [
    project.code,
    project.clientName,
    place,
    project.supervisorName && t('project.supervisor', { name: project.supervisorName }),
    project.forecastDate && t('project.forecastOn', { date: format.date(project.forecastDate) }),
  ].filter(Boolean)

  return (
    <>
      <header className="flex flex-col px-10 pt-6">
        <Link
          to="/projects"
          className="inline-flex min-h-9 items-center gap-1.5 self-start rounded-full border border-line bg-surface px-3.5 text-[13.5px] hover:border-accent"
        >
          ← {t('menu.projects')}
        </Link>

        <div className="mt-2.5 flex flex-wrap items-center gap-3">
          <h1 className="display text-[42px] leading-none">{project.name}</h1>
          {project.active ? (
            <Badge tone="neutral">{t(`projectStatus.${project.status}`)}</Badge>
          ) : (
            <Badge tone="inactive">{t('common.inactive')}</Badge>
          )}
          {project.active && project.late && <Badge tone="late">{t('common.late')}</Badge>}

          {canManage && (
            <div className="ml-auto flex flex-wrap gap-2">
              <Button variant="outline" className="px-4 py-2" onClick={() => setFormOpen(true)}>
                {t('common.edit')}
              </Button>
              <Button variant="outline" className="px-4 py-2" onClick={() => setLinksOpen(true)}>
                {t('project.portal')}
              </Button>
              <Button variant={project.active ? 'danger' : 'outline'} className="px-4 py-2" busy={busy} onClick={toggleActive}>
                {project.active ? t('common.deactivate') : t('common.activate')}
              </Button>
            </div>
          )}
        </div>

        <p className="mt-1.5 font-mono text-[13px] text-muted">{facts.join(' · ')}</p>

        {actionError && (
          <p role="alert" className="mt-3 rounded-2xl bg-danger-wash px-4 py-2.5 text-[13.5px] text-danger">
            {actionError}
          </p>
        )}

        <div className="mt-4">
          <PillTabs
            label={t('project.sections')}
            tabs={tabs.map((projectTab) => ({
              to: projectTab.key === FIRST_TAB ? basePath : `${basePath}/${projectTab.key}`,
              label: t(`project.tabs.${projectTab.key}`),
            }))}
          />
        </div>
      </header>

      {currentTab.render(project)}

      {canManage && (
        <>
          <ProjectFormDialog
            open={formOpen}
            project={project}
            clientOptions={clientOptions}
            supervisorOptions={supervisorOptions}
            canSeeAmount={can('view_finance')}
            onClose={() => setFormOpen(false)}
            onSubmit={saveProject}
          />
          <ProjectLinksDialog
            open={linksOpen}
            projectId={project.id}
            projectName={project.name}
            onClose={() => setLinksOpen(false)}
          />
        </>
      )}
    </>
  )
}
