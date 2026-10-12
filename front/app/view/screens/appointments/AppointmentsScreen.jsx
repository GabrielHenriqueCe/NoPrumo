import { useCallback, useEffect, useState } from 'react'
import { formatDate, todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { SelectField } from '../../ui/SelectField'
import { AppointmentFormDialog } from './AppointmentFormDialog'
import { appointmentTypeLabel, monthLabel, monthRange, weekdayOf } from './appointmentLabels'

// Training expiries come calculated by the API, mixed in by date: they are not appointments.
const EMPTY_AGENDA = { appointments: [], trainingExpiries: [] }
const EMPTY_OPTIONS = { projects: [], assignees: [] }

export function AppointmentsScreen() {
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
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load the schedule.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [appointments, month, projectFilter, reloadToken])

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
      setActionError(error.message ?? 'The action could not be completed.')
    } finally {
      setPendingId(null)
    }
  }

  const days = groupByDay(agenda)
  const today = todayIso()

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Planning</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Schedule</h1>
        </div>

        <div className="ml-auto flex flex-wrap items-center gap-3">
          <nav className="flex items-center gap-2" aria-label="Month">
            <Button variant="outline" className="px-3 py-1.5" onClick={() => moveMonth(-1)}>
              Previous
            </Button>
            <span aria-live="polite" className="w-36 text-center text-sm font-medium">
              {monthLabel(month.year, month.index)}
            </span>
            <Button variant="outline" className="px-3 py-1.5" onClick={() => moveMonth(1)}>
              Next
            </Button>
          </nav>

          <div className="w-56">
            <SelectField
              label="Project"
              name="project-filter"
              value={projectFilter}
              onChange={(event) => setProjectFilter(event.target.value)}
              placeholder="All projects"
              options={options.projects.map((project) => ({ value: project.id, label: project.name }))}
            />
          </div>

          <Button onClick={() => openForm(null)}>New appointment</Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Appointments of the month, together with the training certificates that expire in it. With a
          project picked, the expiries are the ones of the people on its teams today.
        </p>

        {actionError && (
          <p role="alert" className="mb-4 border border-danger-soft bg-[#faefea] px-4 py-2.5 text-[13px] text-danger">
            {actionError}
          </p>
        )}

        {loading && <ListSkeleton />}

        {!loading && loadError && (
          <div className="border border-line bg-white px-6 py-10 text-center">
            <p className="text-sm text-danger">{loadError}</p>
            <Button variant="outline" onClick={reload} className="mt-4">
              Try again
            </Button>
          </div>
        )}

        {!loading && !loadError && days.length === 0 && (
          <div className="border border-line bg-white px-6 py-12 text-center">
            <p className="text-sm text-muted">Nothing scheduled in {monthLabel(month.year, month.index)}.</p>
          </div>
        )}

        {!loading && !loadError && days.length > 0 && (
          <div className="flex flex-col gap-4">
            {days.map((day) => (
              <section key={day.date} className="border border-line bg-white" aria-labelledby={`day-${day.date}`}>
                <h2
                  id={`day-${day.date}`}
                  className="flex items-center gap-2 border-b border-line-soft bg-cream-soft px-4 py-2 text-sm font-semibold"
                >
                  {weekdayOf(day.date)} {formatDate(day.date)}
                  {day.date === today && <Badge tone="warning">Today</Badge>}
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
                          {[appointmentTypeLabel(appointment.type), appointment.projectName, appointment.assignedToName]
                            .filter(Boolean)
                            .join(' · ')}
                        </div>
                      </div>

                      {appointment.completed && <Badge tone="active">Completed</Badge>}

                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          className="px-3 py-1.5"
                          aria-label={`Edit ${appointment.title}`}
                          disabled={pendingId === appointment.id}
                          onClick={() => openForm(appointment)}
                        >
                          Edit
                        </Button>
                        <Button
                          variant="outline"
                          className="px-3 py-1.5"
                          aria-label={`${appointment.completed ? 'Reopen' : 'Complete'} ${appointment.title}`}
                          busy={pendingId === appointment.id}
                          busyLabel="Saving…"
                          onClick={() => toggleCompleted(appointment)}
                        >
                          {appointment.completed ? 'Reopen' : 'Complete'}
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
                          {expiry.trainingTypeCode} of {expiry.employeeName} expires
                        </div>
                        <div className="text-[12.5px] text-muted">{expiry.trainingTypeName} · training certificate</div>
                      </div>

                      <Badge tone={expiry.status === 'expired' ? 'inactive' : 'warning'}>
                        {expiry.status === 'expired' ? 'Expired' : 'Expires soon'}
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
  return (
    <div className="border border-line bg-white p-4" role="status" aria-label="Loading the schedule">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
