import { useEffect, useState } from 'react'
import { usePreferences } from '../../preferences/preferencesContext'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'

const TRAINING_BADGES = {
  Valid: { tone: 'active', key: 'valid' },
  Expiring: { tone: 'warning', key: 'expiring' },
  Expired: { tone: 'inactive', key: 'expired' },
  NoExpiry: { tone: 'neutral', key: 'noExpiry' },
}

export function EmployeeProfile({ employeeId, onClose }) {
  const { employees } = useContainer()
  const { can } = useSession()
  const { t, format } = usePreferences()
  const canViewFinance = can('view_finance')

  const [profile, setProfile] = useState(null)
  const [error, setError] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let isMounted = true

    async function loadProfile() {
      try {
        const data = await employees.getById(employeeId)
        if (isMounted) {
          setProfile(data)
          setIsLoading(false)
        }
      } catch {
        if (isMounted) {
          setError(t('employeeProfile.couldNotLoad'))
          setIsLoading(false)
        }
      }
    }

    loadProfile()

    return () => {
      isMounted = false
    }
  }, [employeeId, employees, t])

  const title = t('employeeProfile.title')

  if (isLoading) {
    return (
      <Dialog open title={title} onClose={onClose}>
        <div className="p-6" role="status">
          {t('common.loading')}…
        </div>
      </Dialog>
    )
  }

  if (error) {
    return (
      <Dialog open title={title} onClose={onClose}>
        <div className="p-6 text-danger">{error}</div>
      </Dialog>
    )
  }

  if (!profile) return null

  const trainingBadge = (status) => {
    const badge = TRAINING_BADGES[status]
    return badge ? <Badge tone={badge.tone}>{t(`employeeProfile.${badge.key}`)}</Badge> : <Badge>{status}</Badge>
  }

  return (
    <Dialog open title={title} onClose={onClose}>
      <div className="grid grid-cols-1 gap-8 p-6 md:grid-cols-2">
        <div className="space-y-6">
          <Section title={t('employeeProfile.personal')}>
            <Row term={t('employeeProfile.name')}>{profile.name}</Row>
            <Row term={t('employeeProfile.registration')}>{profile.registrationNumber}</Row>
            <Row term={t('employeeProfile.document')}>{profile.documentMasked}</Row>
            <Row term={t('employeeProfile.phone')}>{profile.phone}</Row>
            <Row term={t('employeeProfile.status')}>{profile.active ? t('common.active') : t('common.inactive')}</Row>
          </Section>

          <Section title={t('employeeProfile.employment')}>
            <Row term={t('employeeProfile.jobRole')}>{profile.jobRoleName}</Row>
            <Row term={t('employeeProfile.regime')}>{profile.employmentRegimeLabel}</Row>
            <Row term={t('employeeProfile.hireDate')}>{profile.hireDate && format.date(profile.hireDate)}</Row>
            {canViewFinance && (
              <>
                <Row term={t('employeeProfile.payRate')}>{format.money(profile.payRate ?? 0)}</Row>
                <Row term={t('employeeProfile.additional')}>{format.percent(profile.additionalPercentage ?? 0)}</Row>
              </>
            )}
          </Section>
        </div>

        <div className="space-y-6">
          <Section title={t('employeeProfile.team')}>
            {profile.currentTeamName ? (
              <>
                <Row term={t('employeeProfile.teamName')}>{profile.currentTeamName}</Row>
                <Row term={t('employeeProfile.allocatedSince')}>
                  {profile.teamAllocationDate && format.date(profile.teamAllocationDate)}
                </Row>

                <div className="pt-2">
                  <span className="mb-1 block text-muted">{t('employeeProfile.activeProjects')}</span>
                  {profile.currentProjects?.length > 0 ? (
                    <ul className="ml-2 list-inside list-disc">
                      {profile.currentProjects.map((project) => (
                        <li key={project.id}>
                          {project.code} · {project.name}
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-muted italic">{t('employeeProfile.noProjects')}</p>
                  )}
                </div>
              </>
            ) : (
              <p className="text-muted italic">{t('employeeProfile.noTeam')}</p>
            )}
          </Section>

          <Section title={t('employeeProfile.trainings')}>
            {profile.trainings?.length > 0 ? (
              <div className="space-y-3">
                {profile.trainings.map((training) => (
                  <div key={training.id} className="rounded-2xl border border-line bg-sunken p-3">
                    <div className="mb-1 flex items-start justify-between gap-3">
                      <span className="font-medium">{training.typeName}</span>
                      {trainingBadge(training.status)}
                    </div>
                    <p className="text-[13px] text-muted">
                      {format.date(training.issueDate)}
                      {training.expiryDate && ` → ${format.date(training.expiryDate)}`}
                    </p>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-muted italic">{t('employeeProfile.noTrainings')}</p>
            )}
          </Section>
        </div>
      </div>

      <div className="flex justify-end border-t border-line-soft p-4">
        <Button variant="outline" onClick={onClose}>
          {t('common.close')}
        </Button>
      </div>
    </Dialog>
  )
}

function Section({ title, children }) {
  return (
    <section>
      <h2 className="display mb-3 border-b border-line-soft pb-2 text-[22px]">{title}</h2>
      <div className="space-y-2 text-sm">{children}</div>
    </section>
  )
}

function Row({ term, children }) {
  return (
    <p>
      <span className="inline-block w-36 text-muted">{term}</span> {children || '—'}
    </p>
  )
}
