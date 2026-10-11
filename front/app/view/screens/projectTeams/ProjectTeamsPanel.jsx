import { useCallback, useEffect, useRef, useState } from 'react'
import { formatDate, todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

// The "Team" tab of a project. The project page (Gabriel's frame) passes projectId.
export function ProjectTeamsPanel({ projectId }) {
  const { projectTeams } = useContainer()
  const { can } = useSession()
  const canManage = can('manage_projects')

  const [allocations, setAllocations] = useState([])
  const [loaded, setLoaded] = useState(false)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)
  const [teamOptions, setTeamOptions] = useState([])
  const [endingTeamId, setEndingTeamId] = useState(null)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  // Only the first load shows the skeleton, so the lists don't blink on every change.
  useEffect(() => {
    const controller = new AbortController()
    setLoadError(null)

    projectTeams
      .list(projectId, { signal: controller.signal })
      .then((result) => {
        setAllocations(result)
        setLoaded(true)
      })
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load the teams.')
      })

    return () => controller.abort()
  }, [projectTeams, projectId, reloadToken])

  useEffect(() => {
    if (!canManage) return undefined
    const controller = new AbortController()

    projectTeams
      .listTeamOptions(projectId, { signal: controller.signal })
      .then(setTeamOptions)
      .catch(() => {
        if (!controller.signal.aborted) setTeamOptions([])
      })

    return () => controller.abort()
  }, [projectTeams, projectId, canManage])

  const current = allocations.filter((allocation) => allocation.endDate == null)
  const past = allocations.filter((allocation) => allocation.endDate != null)
  const onProject = new Set(current.map((allocation) => allocation.teamId))
  const available = teamOptions.filter((team) => !onProject.has(team.id))

  const changed = () => {
    setEndingTeamId(null)
    reload()
  }

  return (
    <div className="flex flex-col gap-6">
      <p className="max-w-[54em] text-sm leading-relaxed text-muted">
        Teams working on this project and who is on them today. A team leaves the project with an end
        date: the period stays in the history.
      </p>

      {canManage && <AllocateTeamForm projectId={projectId} teams={available} onAllocated={changed} />}

      {!loaded && !loadError && <ListSkeleton />}

      {loadError && (
        <div className="border border-line bg-white px-6 py-10 text-center">
          <p className="text-sm text-danger">{loadError}</p>
          <Button variant="outline" onClick={reload} className="mt-4">
            Try again
          </Button>
        </div>
      )}

      {loaded && !loadError && (
        <>
          <section>
            <h3 className="label mb-2">On the project · {current.length}</h3>

            {current.length === 0 ? (
              <p className="border border-line bg-white px-6 py-8 text-center text-sm text-muted">
                No team on this project right now.
              </p>
            ) : (
              <div className="flex flex-col gap-4">
                {current.map((allocation) => (
                  <CurrentTeamCard
                    key={`${allocation.teamId}-${allocation.startDate}`}
                    projectId={projectId}
                    allocation={allocation}
                    canManage={canManage}
                    ending={endingTeamId === allocation.teamId}
                    onStartEnding={() => setEndingTeamId(allocation.teamId)}
                    onCancelEnding={() => setEndingTeamId(null)}
                    onEnded={changed}
                  />
                ))}
              </div>
            )}
          </section>

          <section>
            <h3 className="label mb-2">History · {past.length}</h3>

            {past.length === 0 ? (
              <p className="text-[13px] text-muted">No team has left this project yet.</p>
            ) : (
              <div className="border border-line bg-white">
                <table className="w-full border-collapse text-sm">
                  <caption className="sr-only">Teams that worked on this project</caption>
                  <thead>
                    <tr className="border-b border-line bg-cream-soft text-left">
                      <HeaderCell>Team</HeaderCell>
                      <HeaderCell>Department</HeaderCell>
                      <HeaderCell>Period</HeaderCell>
                    </tr>
                  </thead>
                  <tbody>
                    {past.map((allocation) => (
                      <tr key={`${allocation.teamId}-${allocation.startDate}`} className="border-b border-line-soft last:border-0">
                        <td className="px-4 py-3 font-medium">{allocation.teamName}</td>
                        <td className="px-4 py-3">{allocation.departmentName}</td>
                        <td className="px-4 py-3 whitespace-nowrap">
                          {formatDate(allocation.startDate)} – {formatDate(allocation.endDate)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </section>
        </>
      )}
    </div>
  )
}

function CurrentTeamCard({ projectId, allocation, canManage, ending, onStartEnding, onCancelEnding, onEnded }) {
  const endButton = useRef(null)

  const cancel = () => {
    onCancelEnding()
    setTimeout(() => endButton.current?.focus(), 0)
  }

  return (
    <article className="border border-line bg-white">
      <header className="flex flex-wrap items-center gap-3 border-b border-line-soft px-4 py-3">
        <div className="min-w-0">
          <h4 className="font-semibold">{allocation.teamName}</h4>
          <p className="text-[12.5px] text-muted">
            {allocation.departmentName} · on the project since {formatDate(allocation.startDate)}
          </p>
        </div>

        {canManage && !ending && (
          <Button
            ref={endButton}
            variant="outline"
            className="ml-auto px-3 py-1.5"
            aria-label={`End the period of ${allocation.teamName} on this project`}
            onClick={onStartEnding}
          >
            End period
          </Button>
        )}
      </header>

      {ending && (
        <div className="border-b border-line-soft bg-cream-soft px-4 py-3">
          <EndAllocationForm projectId={projectId} allocation={allocation} onEnded={onEnded} onCancel={cancel} />
        </div>
      )}

      {allocation.members.length === 0 ? (
        <p className="px-4 py-4 text-[13px] text-muted">Nobody on this team right now.</p>
      ) : (
        <table className="w-full border-collapse text-sm">
          <caption className="sr-only">Members of {allocation.teamName}</caption>
          <thead>
            <tr className="border-b border-line text-left">
              <HeaderCell>Employee</HeaderCell>
              <HeaderCell>Trainings needing attention</HeaderCell>
            </tr>
          </thead>
          <tbody>
            {allocation.members.map((member) => (
              <tr key={member.employeeId} className="border-b border-line-soft last:border-0">
                <td className="px-4 py-3">
                  <div className="font-medium">{member.employeeName}</div>
                  {member.registrationNumber && (
                    <div className="text-[12px] text-muted">{member.registrationNumber}</div>
                  )}
                </td>
                <td className="px-4 py-3">
                  {member.trainingAlerts.length === 0 ? (
                    <span className="text-muted">Up to date</span>
                  ) : (
                    <div className="flex flex-wrap gap-1.5">
                      {member.trainingAlerts.map((alert) => (
                        <Badge
                          key={alert.trainingTypeCode}
                          tone={alert.status === 'expired' ? 'inactive' : 'warning'}
                        >
                          {alert.trainingTypeCode} {alert.status === 'expired' ? 'expired' : 'expires'}{' '}
                          {formatDate(alert.expiryDate)}
                        </Badge>
                      ))}
                    </div>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </article>
  )
}

function AllocateTeamForm({ projectId, teams, onAllocated }) {
  const { projectTeams } = useContainer()

  const [form, setForm] = useState(() => ({ teamId: '', startDate: todayIso() }))
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    try {
      await projectTeams.allocate(projectId, {
        teamId: form.teamId ? Number(form.teamId) : null,
        startDate: form.startDate || null,
      })
      setForm((current) => ({ ...current, teamId: '' }))
      onAllocated()
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not allocate the team.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <form onSubmit={submit} noValidate aria-label="Allocate a team" className="border border-line bg-white p-4">
      <p className="label mb-3">Allocate a team</p>

      <div className="grid grid-cols-[minmax(0,1fr)_10.5rem_auto] items-start gap-3">
        <SelectField
          label="Team"
          name="teamId"
          required
          value={form.teamId}
          onChange={update('teamId')}
          error={fieldErrors.teamId}
          placeholder={teams.length > 0 ? 'Select a team…' : 'No team available'}
          options={teams.map((team) => ({ value: team.id, label: `${team.name} · ${team.departmentName}` }))}
        />

        <TextField
          label="Start date"
          name="startDate"
          type="date"
          required
          value={form.startDate}
          onChange={update('startDate')}
          error={fieldErrors.startDate}
        />

        <div className="pt-5">
          <Button type="submit" busy={busy} busyLabel="Allocating…">
            Allocate
          </Button>
        </div>
      </div>

      {formError && (
        <p role="alert" className="mt-3 border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
          {formError}
        </p>
      )}
    </form>
  )
}

function EndAllocationForm({ projectId, allocation, onEnded, onCancel }) {
  const { projectTeams } = useContainer()

  const [endDate, setEndDate] = useState(() => {
    const today = todayIso()
    return allocation.startDate > today ? allocation.startDate : today
  })
  const [error, setError] = useState(null)
  const [busy, setBusy] = useState(false)
  const input = useRef(null)

  useEffect(() => {
    input.current?.focus()
  }, [])

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setError(null)

    try {
      await projectTeams.end(projectId, allocation.teamId, endDate || null)
      // No setBusy(false) on success: this form unmounts with the card.
      onEnded()
    } catch (failure) {
      setError(failure.fieldErrors?.endDate ?? failure.message ?? 'Could not end the period.')
      setBusy(false)
    }
  }

  return (
    <form
      onSubmit={submit}
      noValidate
      onKeyDown={(event) => {
        if (event.key === 'Escape') onCancel()
      }}
      className="flex flex-wrap items-start gap-2.5"
    >
      <TextField
        ref={input}
        label="End date"
        name="endDate"
        type="date"
        required
        min={allocation.startDate}
        value={endDate}
        onChange={(event) => {
          setEndDate(event.target.value)
          setError(null)
        }}
        error={error}
        hint={`On the project since ${formatDate(allocation.startDate)}.`}
        className="w-48"
      />

      <div className="flex gap-2 pt-5">
        <Button type="submit" busy={busy} busyLabel="Saving…" className="px-3 py-2">
          End period
        </Button>
        <Button type="button" variant="outline" onClick={onCancel} disabled={busy} className="px-3 py-2">
          Cancel
        </Button>
      </div>
    </form>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function ListSkeleton() {
  return (
    <div className="border border-line bg-white p-4" role="status" aria-label="Loading teams">
      {Array.from({ length: 3 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
