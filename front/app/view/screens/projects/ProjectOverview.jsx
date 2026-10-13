import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { formatDate } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { useT } from '../../preferences/preferencesContext'

// 100 is the API's page cap; a project has a few dozen stages at most.
const STAGES_SIZE = 100

const percent = new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 0 })

export function ProjectOverview({ project }) {
  const t = useT()
  const { stages } = useContainer()

  const [stageList, setStageList] = useState([])
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    stages
      .list({ page: 1, size: STAGES_SIZE, projectId: project.id }, { signal: controller.signal })
      .then((result) => setStageList(result.items))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('projectOverview.couldNotLoadTheStages'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [stages, project.id, t])

  const address = [
    [project.street, project.number].filter(Boolean).join(', '),
    project.complement,
    project.district,
    project.postalCode,
  ]
    .filter(Boolean)
    .join(' · ')

  return (
    <div className="grid items-start gap-6 p-8 lg:grid-cols-[minmax(0,1fr)_minmax(0,1.4fr)]">
      <section className="overflow-hidden rounded-3xl border border-line bg-surface" aria-labelledby="project-details-heading">
        <h2 id="project-details-heading" className="border-b border-line-soft px-5 py-3.5 text-[15px] font-semibold">
          {t('projectOverview.details')}
        </h2>

        <dl className="grid grid-cols-[auto_minmax(0,1fr)] gap-x-6 gap-y-2.5 px-5 py-4 text-sm">
          <Detail term={t('projectOverview.client')}>{project.clientName}</Detail>
          <Detail term="CNO">{project.cno && <span className="font-mono">{project.cno}</span>}</Detail>
          <Detail term={t('projectOverview.address')}>{address}</Detail>
          <Detail term={t('projectOverview.supervisor')}>{project.supervisorName}</Detail>
          <Detail term={t('projectOverview.technicalManager')}>
            {project.technicalManager &&
              (project.creaRt ? t('projectOverview.technicalmanagerCreaCreart', { technicalManager: project.technicalManager, creaRt: project.creaRt }) : project.technicalManager)}
          </Detail>
          <Detail term={t('projectOverview.start')}>{project.startDate && formatDate(project.startDate)}</Detail>
          <Detail term={t('projectOverview.forecast')}>{project.forecastDate && formatDate(project.forecastDate)}</Detail>
          <Detail term={t('projectOverview.completion')}>{project.completionDate && formatDate(project.completionDate)}</Detail>
          <Detail term={t('projectOverview.notes')}>{project.notes}</Detail>
        </dl>
      </section>

      <section className="overflow-hidden rounded-3xl border border-line bg-surface" aria-labelledby="project-stages-heading">
        <div className="flex items-center gap-3 border-b border-line-soft px-5 py-3.5">
          <h2 id="project-stages-heading" className="text-[15px] font-semibold">
            {t('projectOverview.stages')}
          </h2>
          <Link to={`/projects/${project.id}/stages`} className="label ml-auto hover:text-bronze">
            {t('projectOverview.allStages')}
          </Link>
        </div>

        {loading && (
          <div className="p-4" role="status" aria-label={t('projectOverview.loadingStages')}>
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="mb-2 h-10 animate-pulse rounded-xl bg-sunken last:mb-0" />
            ))}
          </div>
        )}

        {!loading && loadError && <p className="px-5 py-8 text-center text-sm text-danger">{loadError}</p>}

        {!loading && !loadError && stageList.length === 0 && (
          <p className="px-5 py-8 text-center text-sm text-muted">{t('projectOverview.noStageRegisteredForThis')}</p>
        )}

        {!loading && !loadError && stageList.length > 0 && (
          <ol>
            {stageList.map((stage) => (
              <li
                key={stage.id}
                className="flex flex-wrap items-center gap-x-4 gap-y-2 border-b border-line-soft px-5 py-3 last:border-0"
              >
                <span className="w-6 font-mono text-[12.5px] text-muted">{stage.sortOrder}</span>

                <div className="min-w-0 flex-1">
                  <div className="font-medium">{stage.name}</div>
                  <div className="text-[12.5px] text-muted">
                    {stage.teamName || t('projectOverview.noTeam')} {t('projectOverview.planned')} {formatDate(stage.plannedDate)}
                  </div>
                </div>

                <div className="flex flex-wrap gap-1.5">
                  <Badge tone="neutral">{t(`stageStatus.${stage.status}`)}</Badge>
                  {stage.late && <Badge tone="warning">{t('common.late')}</Badge>}
                </div>

                <ProgressBar value={stage.percentage} late={stage.late} />
              </li>
            ))}
          </ol>
        )}
      </section>
    </div>
  )
}

function Detail({ term, children }) {
  return (
    <>
      <dt className="text-muted">{term}</dt>
      <dd className="min-w-0 break-words">{children || '—'}</dd>
    </>
  )
}

function ProgressBar({ value, late }) {
  const t = useT()
  return (
    <div className="flex w-36 items-center gap-2.5">
      <div
        role="progressbar"
        aria-label={t('projectOverview.progress')}
        aria-valuenow={value}
        aria-valuemin={0}
        aria-valuemax={100}
        className="h-1.5 flex-1 bg-line-soft"
      >
        <div className={`h-full ${late ? 'bg-danger' : 'bg-gold'}`} style={{ width: `${value}%` }} />
      </div>
      <span className="w-9 text-right text-[12.5px] text-muted tabular-nums">{percent.format(value)}%</span>
    </div>
  )
}
