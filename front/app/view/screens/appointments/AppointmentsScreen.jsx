import { useCallback, useEffect, useState } from 'react'
import { formatDate, todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { SelectField } from '../../ui/SelectField'
import { AppointmentFormDialog } from './AppointmentFormDialog'
import { monthLabel, monthRange, weekdayOf } from './appointmentLabels'
import { usePreferences } from '../../preferences/preferencesContext'

// Training expiries come calculated by the API, mixed in by date: they are not appointments.
const EMPTY_AGENDA = { appointments: [], trainingExpiries: [] }
const EMPTY_OPTIONS = { projects: [], assignees: [] }

export function AppointmentsScreen() {
  const { t, format } = usePreferences()
  const { appointments } = useContainer()

  const [month, setMonth] = useState(() => {
    const now = new Date()
    return { year: now.getFullYear(), index: now.getMonth() }
  })
  const [projectFilter, setProjectFilter] = useState('')
  const [agenda, setAgenda] = useState(EMPTY_AGENDA)
  const [options, setOptions] = useState(EMPTY_OPTIONS)
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [actionError, setActionError] = useState(null)
  const [pendingId, setPendingId] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingAppointment, setEditingAppointment] = useState(null)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    appointments
      .agenda({ ...monthRange(month.year, month.index), projectId: projectFilter || null }, { signal: controller.signal })
      .then(setAgenda)
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('appointmentsScreen.couldNotLoadTheSchedule'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [appointments, month, projectFilter, reloadToken, t])

  useEffect(() => {
    const controller = new AbortController()

    appointments
      .options({ signal: controller.signal })
      .then(setOptions)
      .catch(() => {
        if (!controller.signal.aborted) setOptions(EMPTY_OPTIONS)
      })

    return () => controller.abort()
  }, [appointments])

  const moveMonth = (step) =>
    setMonth((current) => {
      const date = new Date(current.year, current.index + step, 1)
      return { year: date.getFullYear(), index: date.getMonth() }
    })

  // Stable on purpose: the Dialog refocuses its first field whenever onClose changes.
  const closeForm = useCallback(() => {
    setFormOpen(false)
    setEditingAppointment(null)
  }, [])

  const openForm = (appointment) => {
    setEditingAppointment(appointment)
    setFormOpen(true)
  }

  const saveAppointment = async (values) => {
    if (editingAppointment) {
      await appointments.update(editingAppointment.id, values)
    } else {
      await appointments.create(values)
    }

    closeForm()
    reload()
  }

  const toggleCompleted = async (appointment) => {
    setPendingId(appointment.id)
    setActionError(null)

    try {
      await appointments.setCompleted(appointment.id, !appointment.completed)
      reload()
    } catch (error) {
      setActionError(error.message ?? t('appointmentsScreen.theActionCouldNotBe'))
    } finally {
      setPendingId(null)
    }
  }

  const days = groupByDay(agenda)
  const today = todayIso()

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
        <div className="min-w-0">
          <p className="label">{t('appointmentsScreen.planning')}</p>
          <h1 className="display mt-1 text-[38px] leading-none">{t('appointmentsScreen.schedule')}</h1>
        </div>

        <div className="ml-auto flex flex-wrap items-center gap-3">
          <nav className="flex items-center gap-2" aria-label={t('appointmentsScreen.month')}>
            <Button variant="outline" className="px-3 py-1.5" onClick={() => moveMonth(-1)}>
              {t('common.previous')}
            </Button>
            <span aria-live="polite" className="w-36 text-center text-sm font-medium">
              {monthLabel(month.year, month.index, format.locale)}
            </span>
            <Button variant="outline" className="px-3 py-1.5" onClick={() => moveMonth(1)}>
              {t('common.next')}
            </Button>
          </nav>

          <div className="w-56">
            <SelectField
              label={t('appointmentsScreen.project')}
              name="project-filter"
              value={projectFilter}
              onChange={(event) => setProjectFilter(event.target.value)}
              placeholder={t('appointmentsScreen.allProjects')}
              options={options.projects.map((project) => ({ value: project.id, label: project.name }))}
            />
          </div>

          <Button onClick={() => openForm(null)}>{t('appointmentsScreen.newAppointment')}</Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('appointmentsScreen.appointmentsOfTheMonthTogether')}
        </p>

        {actionError && (
          <p role="alert" className="mb-4 rounded-2xl bg-danger-wash px-4 py-2.5 text-[13px] text-danger">
            {actionError}
          </p>
        )}

        {loading && <ListSkeleton />}

        {!loading && loadError && (
          <div className="border border-line bg-surface px-6 py-10 text-center">
            <p className="text-sm text-danger">{loadError}</p>
            <Button variant="outline" onClick={reload} className="mt-4">
              {t('common.tryAgain')}
            </Button>
          </div>
        )}

        {!loading && !loadError && days.length === 0 && (
          <div className="border border-line bg-surface px-6 py-12 text-center">
            <p className="text-sm text-muted">{t('appointmentsScreen.nothingScheduledIn')} {monthLabel(month.year, month.index, format.locale)}.</p>
          </div>
        )}

        {!loading && !loadError && days.length > 0 && (
          <div className="flex flex-col gap-4">
            {days.map((day) => (
              <section key={day.date} className="overflow-hidden rounded-3xl border border-line bg-surface" aria-labelledby={`day-${day.date}`}>
                <h2
                  id={`day-${day.date}`}
                  className="flex items-center gap-2 border-b border-line-soft bg-cream-soft px-4 py-2 text-sm font-semibold"
                >
                  {weekdayOf(day.date, format.locale)} {formatDate(day.date)}
                  {day.date === today && <Badge tone="warning">{t('appointmentsScreen.today')}</Badge>}
                </h2>

                <ul>
                  {day.appointments.map((appointment) => (
                    <li
                      key={`appointment-${appointment.id}`}
                      className="flex flex-wrap items-center gap-3 border-b border-line-soft px-4 py-3 last:border-0"
                    >
                      <span className="w-12 font-mono text-[13px] text-muted">
                        {appointment.time ? appointment.time.slice(0, 5) : '—'}
                      </span>

                      <div className="min-w-0 flex-1">
                        <div className={`font-medium ${appointment.completed ? 'text-muted line-through' : ''}`}>
                          {appointment.title}
                        </div>
                        <div className="text-[12.5px] text-muted">
                          {[t(`labels.appointmentType.${appointment.type}`), appointment.projectName, appointment.assignedToName]
                            .filter(Boolean)
                            .join(' · ')}
                        </div>
                      </div>

                      {appointment.completed && <Badge tone="active">{t('appointmentsScreen.completed')}</Badge>}

                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          className="px-3 py-1.5"
                          aria-label={`Edit ${appointment.title}`}
                          disabled={pendingId === appointment.id}
                          onClick={() => openForm(appointment)}
                        >
                          {t('common.edit')}
                        </Button>
                        <Button
                          variant="outline"
                          className="px-3 py-1.5"
                          aria-label={`${appointment.completed ? 'Reopen' : 'Complete'} ${appointment.title}`}
                          busy={pendingId === appointment.id}
                          busyLabel={t('common.saving')}
                          onClick={() => toggleCompleted(appointment)}
                        >
                          {appointment.completed ? t('appointmentsScreen.reopen') : t('appointmentsScreen.complete')}
                        </Button>
                      </div>
                    </li>
                  ))}

                  {day.expiries.map((expiry) => (
                    <li
                      key={`expiry-${expiry.trainingId}`}
                      className="flex flex-wrap items-center gap-3 border-b border-line-soft px-4 py-3 last:border-0"
                    >
                      <span className="w-12 font-mono text-[13px] text-muted">—</span>

                      <div className="min-w-0 flex-1">
                        <div className="font-medium">
                          {expiry.trainingTypeCode} {t('appointmentsScreen.of')} {expiry.employeeName} {t('appointmentsScreen.expires')}
                        </div>
                        <div className="text-[12.5px] text-muted">{expiry.trainingTypeName} {t('appointmentsScreen.trainingCertificate')}</div>
                      </div>

                      <Badge tone={expiry.status === 'expired' ? 'inactive' : 'warning'}>
                        {expiry.status === 'expired' ? t('appointmentsScreen.expired') : t('appointmentsScreen.expiresSoon')}
                      </Badge>
                    </li>
                  ))}
                </ul>
              </section>
            ))}
          </div>
        )}
      </div>

      <AppointmentFormDialog
        open={formOpen}
        appointment={editingAppointment}
        options={options}
        defaultProjectId={projectFilter}
        onClose={closeForm}
        onSubmit={saveAppointment}
      />
    </>
  )
}

function groupByDay({ appointments, trainingExpiries }) {
  const days = new Map()
  const dayOf = (date) => {
    if (!days.has(date)) days.set(date, { date, appointments: [], expiries: [] })
    return days.get(date)
  }

  appointments.forEach((appointment) => dayOf(appointment.date).appointments.push(appointment))
  trainingExpiries.forEach((expiry) => dayOf(expiry.expiryDate).expiries.push(expiry))

  return [...days.values()].sort((first, second) => first.date.localeCompare(second.date))
}

function ListSkeleton() {
  const { t } = usePreferences()
  return (
    <div className="border border-line bg-surface p-4" role="status" aria-label={t('appointmentsScreen.loadingTheSchedule')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
