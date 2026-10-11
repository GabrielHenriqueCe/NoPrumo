import { useEffect, useState } from 'react'
import { Link, Navigate, NavLink, useParams } from 'react-router-dom'
import { formatDate } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { StagesScreen } from '../stages/StagesScreen'
import { ProjectOverview } from './ProjectOverview'
import { projectStatusLabel } from './projectStatus'

/*
  The project page: everything about one project, in tabs. Each tab is a
  component that receives the project; the tab list is the only place that
  knows which component goes where and who may see it.
*/

const PROJECT_TABS = [
  {
    key: 'overview',
    label: 'Overview',
    permission: 'manage_projects',
    render: (project) => <ProjectOverview project={project} />,
  },
  {
    key: 'stages',
    label: 'Stages',
    permission: 'manage_projects',
    render: (project) => <StagesScreen projectId={project.id} />,
  },
]

const FIRST_TAB = PROJECT_TABS[0].key

export function ProjectScreen() {
  const { projectId, tab } = useParams()
  const { projects } = useContainer()
  const { can } = useSession()

  const [project, setProject] = useState(null)
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

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

  if (loading) {
    return (
      <div className="p-8" role="status" aria-label="Loading project">
        <div className="h-16 animate-pulse bg-white" />
      </div>
    )
  }

  if (loadError) {
    return (
      <div className="p-8">
        <div className="border border-line bg-white px-6 py-10 text-center">
          <p className="text-sm text-danger">
            {loadError.status === 404 ? 'This project does not exist.' : (loadError.message ?? 'Could not load the project.')}
          </p>
          <div className="mt-4 flex justify-center gap-2.5">
            {loadError.status !== 404 && (
              <Button variant="outline" onClick={() => setReloadToken((value) => value + 1)}>
                Try again
              </Button>
            )}
            <Link to="/projects" className="px-4 py-2.5 text-sm font-semibold text-bronze hover:underline">
              Back to projects
            </Link>
          </div>
        </div>
      </div>
    )
  }

  return (
    <>
      <header className="border-b border-line bg-white px-8 pt-5">
        <Link to="/projects" className="label hover:text-bronze">
          ← Projects
        </Link>

        <div className="mt-1 flex flex-wrap items-center gap-3">
          <h1 className="text-xl font-semibold tracking-tight">{project.name}</h1>
          <div className="flex flex-wrap gap-1.5">
            {project.active ? (
              <Badge tone="neutral">{projectStatusLabel(project.status)}</Badge>
            ) : (
              <Badge tone="inactive">Inactive</Badge>
            )}
            {project.active && project.late && <Badge tone="warning">Late</Badge>}
          </div>
        </div>

        <p className="mt-1.5 flex flex-wrap gap-x-4 gap-y-1 text-[13px] text-muted">
          <span className="font-mono">{project.code}</span>
          {project.clientName && <span>{project.clientName}</span>}
          {project.city && <span>{project.state ? `${project.city} / ${project.state}` : project.city}</span>}
          {project.supervisorName && <span>Supervisor: {project.supervisorName}</span>}
          <span>Forecast: {formatDate(project.forecastDate)}</span>
        </p>

        <nav aria-label="Project sections" className="mt-4 -mb-px flex gap-1 overflow-x-auto">
          {tabs.map((projectTab) => (
            <NavLink
              key={projectTab.key}
              to={projectTab.key === FIRST_TAB ? basePath : `${basePath}/${projectTab.key}`}
              end
              className={({ isActive }) =>
                [
                  'border-b-2 px-3.5 py-2.5 text-sm whitespace-nowrap transition-colors',
                  isActive
                    ? 'border-gold font-semibold text-graphite'
                    : 'border-transparent text-muted hover:text-graphite',
                ].join(' ')
              }
            >
              {projectTab.label}
            </NavLink>
          ))}
        </nav>
      </header>

      {currentTab.render(project)}
    </>
  )
}
