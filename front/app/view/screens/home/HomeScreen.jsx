import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { usePreferences } from '../../preferences/preferencesContext'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { ScreenHeader } from '../../ui/ScreenHeader'

/*
  The first screen after signing in. Each card is one number the API counted,
  and it only comes for the profile that deals with it: a null card is not
  drawn. A card that needs attention (anything above zero that is a problem)
  gets the H alarm colour; the card links to where the problem is handled.
*/

const CARDS = [
  { key: 'activeProjects', to: '/projects', alarm: false },
  { key: 'lateProjects', to: '/projects', alarm: true },
  { key: 'lateStages', to: '/projects', alarm: true },
  { key: 'stockItemsBelowMinimum', to: '/stock-items', alarm: true },
  { key: 'trainingsNeedingAttention', to: '/schedule', alarm: true },
  { key: 'unsignedPpeIssues', to: '/projects', alarm: true },
]

export function HomeScreen() {
  const { reports } = useContainer()
  const { user } = useSession()
  const { t } = usePreferences()

  const [dashboard, setDashboard] = useState(null)
  const [loadError, setLoadError] = useState(null)

  useEffect(() => {
    const controller = new AbortController()

    reports
      .dashboard({ signal: controller.signal })
      .then(setDashboard)
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('common.couldNotLoad'))
      })

    return () => controller.abort()
  }, [reports, t])

  const firstName = user?.name?.split(' ')[0] ?? ''
  const cards = dashboard ? CARDS.filter((card) => dashboard[card.key] != null) : []

  return (
    <>
      <ScreenHeader eyebrow={user?.roleLabel} title={t('home.greeting', { name: firstName })} />

      <div className="px-10 pt-6 pb-10">
        {loadError && <p className="rounded-3xl border border-line bg-surface px-6 py-10 text-center text-danger">{loadError}</p>}

        {!dashboard && !loadError && (
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3" role="status" aria-label={t('common.loading')}>
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="h-40 animate-pulse rounded-3xl bg-surface" />
            ))}
          </div>
        )}

        {dashboard && cards.length === 0 && (
          <p className="rounded-3xl border border-line bg-surface px-6 py-10 text-center text-muted">{t('home.empty')}</p>
        )}

        {cards.length > 0 && (
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {cards.map((card) => {
              const value = dashboard[card.key]
              const attention = card.alarm && value > 0
              return (
                <Link
                  key={card.key}
                  to={card.to}
                  className={[
                    'flex flex-col gap-2 rounded-3xl border bg-surface p-6 transition-colors hover:border-accent',
                    attention ? 'border-[1.5px] border-late' : 'border-line',
                  ].join(' ')}
                >
                  <span className={`display text-[56px] leading-none ${attention ? 'text-late' : 'text-accent'}`}>
                    {value}
                  </span>
                  <span className="display text-[20px]">{t(`home.cards.${card.key}.title`)}</span>
                  <span className="text-[14px] text-muted">{t(`home.cards.${card.key}.hint`)}</span>
                </Link>
              )
            })}
          </div>
        )}
      </div>
    </>
  )
}
