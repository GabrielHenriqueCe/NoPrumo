import { Fragment, useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { formatDate, todayIso } from './format'

/*
  The members tab: who is on the team now, and who was before.

  Nobody is removed. Ending a stint fills its end date and moves the row to
  the history, which is what answers "who was on this team in March" months
  later. Someone who comes back gets a new stint, starting after the last one
  ended.

  Every add and every end is saved the moment it is confirmed — this tab has
  no "save" of its own.
*/

// The employee list comes in pages of up to 100; the cap only stops a
// runaway loop if the API ever answered nonsense.
const EMPLOYEE_PAGE_SIZE = 100
const MAX_EMPLOYEE_PAGES = 50

export function TeamMembersPanel({ team, onChanged, onDone }) {
  const { teams, employees } = useContainer()

  const [members, setMembers] = useState([])
  const [loaded, setLoaded] = useState(false)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)
  const [employeeList, setEmployeeList] = useState([])
  const [employeesError, setEmployeesError] = useState(null)
  const [endingId, setEndingId] = useState(null)
  const endButtons = useRef({})

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  /*
    Only the first load shows the skeleton. After an add or an end the lists
    stay on screen while the new answer arrives, instead of blinking out
    under the person's cursor.
  */
  useEffect(() => {
    const controller = new AbortController()
    setLoadError(null)

    teams
      .listMembers(team.id, { signal: controller.signal })
      .then((result) => {
        setMembers(result)
        setLoaded(true)
      })
      .catch((error) => {
        // An aborted request is this effect being replaced, not a failure.
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load the members.')
      })

    return () => controller.abort()
  }, [teams, team.id, reloadToken])

  // Employees come from slice 2. Until someone registers one the picker is
  // empty — which is the right answer, not an error.
  useEffect(() => {
    const controller = new AbortController()

    listAllEmployees(employees, controller.signal)
      .then(setEmployeeList)
      .catch((error) => {
        if (!controller.signal.aborted) setEmployeesError(error.message ?? 'Could not load the employees.')
      })

    return () => controller.abort()
  }, [employees])

  const current = useMemo(() => members.filter((member) => member.endDate == null), [members])
  const past = useMemo(() => members.filter((member) => member.endDate != null), [members])

  // Active employees who are not on the team right now. The API refuses the
  // rest anyway; this only keeps them out of the list.
  const candidates = useMemo(() => {
    const onTeam = new Set(current.map((member) => member.employeeId))

    return employeeList
      .filter((employee) => employee.active && !onTeam.has(employee.id))
      .sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'))
  }, [employeeList, current])

  const changed = () => {
    setEndingId(null)
    reload()
    onChanged?.()
  }

  // The row's form goes away; the focus goes back to the button that opened it.
  const cancelEnding = (employeeId) => {
    setEndingId(null)
    setTimeout(() => endButtons.current[employeeId]?.focus(), 0)
  }

  return (
    <div className="flex flex-col gap-6 p-6">
      {team.active ? (
        <AddMemberForm
          teamId={team.id}
          candidates={candidates}
          employeesError={employeesError}
          onAdded={changed}
        />
      ) : (
        <p className="border border-line bg-cream-soft px-4 py-3 text-[13px] text-muted">
          This team is inactive. Activate it to add members.
        </p>
      )}

      {!loaded && !loadError && <ListSkeleton />}

      {loadError && (
        <div className="text-center">
          <p className="text-sm text-danger">{loadError}</p>
          <Button variant="outline" onClick={reload} className="mt-3">
            Try again
          </Button>
        </div>
      )}

      {loaded && !loadError && (
        <>
          <section>
            <h3 className="label mb-2">Current members · {current.length}</h3>

            {current.length === 0 ? (
              <p className="text-[13px] text-muted">Nobody on this team right now.</p>
            ) : (
              <table className="w-full border-collapse text-sm">
                <caption className="sr-only">Current members</caption>
                <thead>
                  <tr className="border-b border-line text-left">
                    <Th>Employee</Th>
                    <Th>Since</Th>
                    <th className="py-2" />
                  </tr>
                </thead>

                <tbody>
                  {current.map((member) => {
                    const ending = endingId === member.employeeId

                    return (
                      <Fragment key={`${member.employeeId}-${member.startDate}`}>
                        <tr className={ending ? 'bg-cream-soft' : 'border-b border-line-soft'}>
                          <EmployeeCell member={member} />
                          <td className="py-2.5 pr-3 whitespace-nowrap">{formatDate(member.startDate)}</td>

                          <td className="py-2.5 text-right">
                            {!ending && (
                              <Button
                                ref={(node) => {
                                  endButtons.current[member.employeeId] = node
                                }}
                                variant="outline"
                                className="px-3 py-1.5"
                                aria-label={`End the stint of ${member.employeeName}`}
                                onClick={() => setEndingId(member.employeeId)}
                              >
                                End stint
                              </Button>
                            )}
                          </td>
                        </tr>

                        {ending && (
                          <tr className="border-b border-line-soft bg-cream-soft">
                            <td colSpan={3} className="px-3 pt-1 pb-3">
                              <EndStintForm
                                teamId={team.id}
                                member={member}
                                onEnded={changed}
                                onCancel={() => cancelEnding(member.employeeId)}
                              />
                            </td>
                          </tr>
                        )}
                      </Fragment>
                    )
                  })}
                </tbody>
              </table>
            )}
          </section>

          <section>
            <h3 className="label mb-2">Past members · {past.length}</h3>

            {past.length === 0 ? (
              <p className="text-[13px] text-muted">Nobody has left this team yet.</p>
            ) : (
              <table className="w-full border-collapse text-sm">
                <caption className="sr-only">Past members</caption>
                <thead>
                  <tr className="border-b border-line text-left">
                    <Th>Employee</Th>
                    <Th>Period</Th>
                  </tr>
                </thead>

                <tbody>
                  {past.map((member) => (
                    <tr
                      key={`${member.employeeId}-${member.startDate}`}
                      className="border-b border-line-soft text-muted last:border-0"
                    >
                      <EmployeeCell member={member} />
                      <td className="py-2.5 whitespace-nowrap">
                        {formatDate(member.startDate)} – {formatDate(member.endDate)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </section>
        </>
      )}

      <div className="flex items-center gap-2.5">
        <Button type="button" variant="outline" onClick={onDone}>
          Done
        </Button>
      </div>
    </div>
  )
}

function AddMemberForm({ teamId, candidates, employeesError, onAdded }) {
  const { teams } = useContainer()

  const [form, setForm] = useState(() => ({ employeeId: '', startDate: todayIso() }))
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
      await teams.addMember(teamId, {
        employeeId: form.employeeId ? Number(form.employeeId) : null,
        startDate: form.startDate || null,
      })

      // The date stays: a crew usually starts together, on the same day.
      setForm((current) => ({ ...current, employeeId: '' }))
      onAdded()
    } catch (error) {
      // The API is the one that validates. The form only shows where it hurt.
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not add the member.')
    } finally {
      setBusy(false)
    }
  }

  const problem = formError ?? employeesError

  return (
    <form
      onSubmit={submit}
      noValidate
      aria-label="Add a member"
      className="border border-line-soft bg-cream-soft p-4"
    >
      <p className="label mb-3">Add a member</p>

      <div className="grid grid-cols-[minmax(0,1fr)_10.5rem] items-start gap-3">
        <SelectField
          label="Employee"
          name="employeeId"
          required
          value={form.employeeId}
          onChange={update('employeeId')}
          error={fieldErrors.employeeId}
          placeholder={candidates.length > 0 ? 'Select an employee…' : 'No active employee to add'}
          options={candidates.map((employee) => ({
            value: employee.id,
            label: employee.registrationNumber
              ? `${employee.name} · ${employee.registrationNumber}`
              : employee.name,
          }))}
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
      </div>

      {problem && (
        <p role="alert" className="mt-3 border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
          {problem}
        </p>
      )}

      <Button type="submit" busy={busy} busyLabel="Adding…" className="mt-3">
        Add member
      </Button>
    </form>
  )
}

function EndStintForm({ teamId, member, onEnded, onCancel }) {
  const { teams } = useContainer()

  // Today — unless the stint only starts later than that.
  const [endDate, setEndDate] = useState(() => {
    const today = todayIso()
    return member.startDate > today ? member.startDate : today
  })
  const [error, setError] = useState(null)
  const [busy, setBusy] = useState(false)
  const inputRef = useRef(null)

  useEffect(() => {
    inputRef.current?.focus()
  }, [])

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setError(null)

    try {
      await teams.endMember(teamId, member.employeeId, endDate || null)
      // On success this form goes away with the row it belongs to.
      onEnded()
    } catch (failure) {
      setError(failure.fieldErrors?.endDate ?? failure.message ?? 'Could not end the stint.')
      setBusy(false)
    }
  }

  return (
    <form
      onSubmit={submit}
      noValidate
      onKeyDown={(event) => {
        // Escape cancels this row only. Left alone it would reach the dialog
        // and close the whole thing.
        if (event.key === 'Escape') {
          event.stopPropagation()
          onCancel()
        }
      }}
      className="flex flex-wrap items-start gap-2.5"
    >
      <TextField
        ref={inputRef}
        label="End date"
        name="endDate"
        type="date"
        required
        min={member.startDate}
        value={endDate}
        onChange={(event) => {
          setEndDate(event.target.value)
          setError(null)
        }}
        error={error}
        hint={`On the team since ${formatDate(member.startDate)}.`}
        className="w-48"
      />

      <div className="flex gap-2 pt-5">
        <Button type="submit" busy={busy} busyLabel="Saving…" className="px-3 py-2">
          End stint
        </Button>
        <Button type="button" variant="outline" onClick={onCancel} disabled={busy} className="px-3 py-2">
          Cancel
        </Button>
      </div>
    </form>
  )
}

function EmployeeCell({ member }) {
  return (
    <td className="py-2.5 pr-3">
      <div className="font-medium text-graphite">{member.employeeName}</div>
      {member.registrationNumber && (
        <div className="text-[12px] text-muted">{member.registrationNumber}</div>
      )}
    </td>
  )
}

function Th({ children }) {
  return <th className="label py-2 pr-3 font-normal">{children}</th>
}

function ListSkeleton() {
  return (
    <div role="status" aria-label="Loading members">
      {Array.from({ length: 3 }).map((_, index) => (
        <div key={index} className="mb-2 h-10 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}

/*
  Every employee, page by page: the picker needs the whole list, and the API
  serves it in pages.
*/
async function listAllEmployees(employees, signal) {
  const all = []

  for (let page = 1; page <= MAX_EMPLOYEE_PAGES; page += 1) {
    const result = await employees.list({ page, size: EMPLOYEE_PAGE_SIZE }, { signal })
    all.push(...result.items)
    if (page >= result.totalPages || result.items.length === 0) break
  }

  return all
}
