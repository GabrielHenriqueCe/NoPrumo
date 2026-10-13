import { useEffect, useState } from 'react'
import { usePreferences } from '../../preferences/preferencesContext'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { CostChart } from './CostChart'

/*
  The "Finance" tab (view_finance only). Every number comes ready from the API,
  including whether the cost is running ahead of the work: the screen compares
  nothing, it only shows.

  Spent against built, and not the margin, leads the page: halfway through a
  project the margin is almost the whole contract and says nothing.
*/

export function ProjectFinancePanel({ projectId }) {
  const { projects } = useContainer()
  const { t, format } = usePreferences()

  const [financials, setFinancials] = useState(null)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    setLoadError(null)

    projects
      .finance(projectId, { signal: controller.signal })
      .then(setFinancials)
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('common.couldNotLoad'))
      })

    return () => controller.abort()
  }, [projects, projectId, reloadToken, t])

  if (loadError) {
    return (
      <div className="px-10 py-8">
        <div className="rounded-3xl border border-line bg-surface px-6 py-10 text-center">
          <p className="text-danger">{loadError}</p>
          <Button variant="outline" className="mt-4" onClick={() => setReloadToken((value) => value + 1)}>
            {t('common.tryAgain')}
          </Button>
        </div>
      </div>
    )
  }

  if (!financials) {
    return (
      <div className="grid gap-4 px-10 py-6 md:grid-cols-4" role="status" aria-label={t('common.loading')}>
        {Array.from({ length: 4 }).map((_, index) => (
          <div key={index} className="h-32 animate-pulse rounded-3xl bg-surface" />
        ))}
      </div>
    )
  }

  const ahead = financials.costAheadOfProgress

  return (
    <div className="flex flex-col gap-5 px-10 pt-5 pb-10">
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <Figure label={t('finance.revisedContract')} value={format.money(financials.revisedContractAmount)}>
          {financials.amendmentsAmount !== 0 &&
            t('finance.withAmendments', {
              contract: format.money(financials.contractAmount),
              amendments: format.money(financials.amendmentsAmount),
            })}
        </Figure>
        <Figure label={t('finance.totalCost')} value={format.money(financials.totalCost)}>
          {t('finance.split', {
            material: format.money(financials.materialCost),
            labor: format.money(financials.laborCost),
          })}
        </Figure>
        <Figure label={t('finance.margin')} value={format.money(financials.margin)}>
          {t('finance.marginHint')}
        </Figure>
        <Figure
          label={t('finance.spentVsBuilt')}
          value={`${format.percent(financials.spentPercentage)} / ${format.percent(financials.progressPercentage)}`}
          tone={ahead ? 'late' : 'ok'}
        >
          {ahead ? t('finance.aheadWarning') : t('finance.onTrack')}
        </Figure>
      </div>

      <section className="rounded-3xl border border-line bg-surface p-6" aria-labelledby="pace-heading">
        <h2 id="pace-heading" className="display text-[22px]">
          {t('finance.paceTitle')}
        </h2>
        <p className="mt-1 mb-5 text-[14px] text-muted">{t('finance.paceHint')}</p>
        <Bar label={t('finance.spent')} value={financials.spentPercentage} late={ahead} format={format} />
        <Bar label={t('finance.built')} value={financials.progressPercentage} format={format} />
      </section>

      <section className="rounded-3xl border border-line bg-surface p-6" aria-labelledby="cost-heading">
        <h2 id="cost-heading" className="display text-[22px]">
          {t('finance.chartTitle')}
        </h2>
        <p className="mt-1 mb-4 text-[14px] text-muted">{t('finance.chartHint')}</p>
        {financials.costByDay.length === 0 ? (
          <p className="py-10 text-center text-muted">{t('finance.noCost')}</p>
        ) : (
          <CostChart days={financials.costByDay} />
        )}
      </section>
    </div>
  )
}

function Figure({ label, value, tone, children }) {
  const border = tone === 'late' ? 'border-late border-[1.5px]' : 'border-line'
  return (
    <div className={`rounded-3xl border bg-surface p-5 ${border}`}>
      <p className="label">{label}</p>
      <p className={`display mt-2 text-[34px] ${tone === 'late' ? 'text-late' : ''}`}>{value}</p>
      {children && <p className="mt-1.5 text-[13px] leading-snug text-muted">{children}</p>}
    </div>
  )
}

function Bar({ label, value, late, format }) {
  const width = Math.min(Math.max(value, 0), 100)
  return (
    <div className="mb-4 last:mb-0">
      <div className="mb-1.5 flex items-baseline justify-between">
        <span className="text-[13px] font-semibold tracking-[0.06em] text-muted uppercase">{label}</span>
        <span className={`display text-[24px] ${late ? 'text-late' : 'text-accent'}`}>{format.percent(value)}</span>
      </div>
      <div
        role="progressbar"
        aria-label={label}
        aria-valuenow={Math.round(value)}
        aria-valuemin="0"
        aria-valuemax="100"
        className="h-2.5 overflow-hidden rounded-full bg-track"
      >
        <div className={`h-full rounded-full ${late ? 'bg-late' : 'bg-accent'}`} style={{ width: `${width}%` }} />
      </div>
    </div>
  )
}
