import { Fragment, useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { formatDate, todayIso } from '../../format/dates'
import { useT } from '../../preferences/preferencesContext'

// Nobody is removed: ending a stint fills its end date and the row becomes history.

// The API serves at most 100 per page; the page cap only stops a runaway loop.
const EMPLOYEE_PAGE_SIZE = 100
const MAX_EMPLOYEE_PAGES = 50

export function TeamMembersPanel({ team, onChanged, onDone }) {
  const t = useT()
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

  // Only the first load shows the skeleton, so the lists don't blink on every add or end.
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
        if (!controller.signal.aborted) setLoadError(error.message ?? t('teamMembersPanel.couldNotLoadTheMembers'))
      })

    return () => controller.abort()
  }, [teams, team.id, reloadToken, t])

  useEffect(() => {
    const controller = new AbortController()

    listAllEmployees(employees, controller.signal)
      .then(setEmployeeList)
      .catch((error) => {
        if (!controller.signal.aborted) setEmployeesError(error.message ?? t('teamMembersPanel.couldNotLoadTheEmployees'))
      })

    return () => controller.abort()
  }, [employees, t])

  const current = useMemo(() => members.filter((member) => member.endDate == null), [members])
  const past = useMemo(() => members.filter((member) => member.endDate != null), [members])

  const candidates = useMemo(() => {
    const onTeam = new Set(current.map((member) => member.employeeId))

    return employeeList
      .filter((employee) => employee.active && !onTeam.has(employee.id))
      .sort((first, second) => first.name.localeCompare(second.name, 'pt-BR'))
  }, [employeeList, current])

  const changed = () => {
    setEndingId(null)
    reload()
    onChanged?.()
  }

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
          {t('teamMembersPanel.thisTeamIsInactiveActivate')}
        </p>
      )}

      {!loaded && !loadError && <ListSkeleton />}

      {loadError && (
        <div className="text-center">
          <p className="text-sm text-danger">{loadError}</p>
          <Button variant="outline" onClick={reload} className="mt-3">
            {t('common.tryAgain')}
          </Button>
        </div>
      )}

      {loaded && !loadError && (
        <>
          <section>
            <h3 className="label mb-2">{t('teamMembersPanel.currentMembers')} {current.length}</h3>

            {current.length === 0 ? (
              <p className="text-[13px] text-muted">{t('teamMembersPanel.nobodyOnThisTeamRight')}</p>
            ) : (
              <table className="w-full border-collapse text-sm">
                <caption className="sr-only">{t('teamMembersPanel.currentMembers2')}</caption>
                <thead>
                  <tr className="border-b border-line text-left">
                    <HeaderCell>{t('teamMembersPanel.employee')}</HeaderCell>
                    <HeaderCell>{t('teamMembersPanel.since')}</HeaderCell>
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
                                aria-label={t('teamMembersPanel.endTheStintOfEmployeename', { employeeName: member.employeeName })}
                                onClick={() => setEndingId(member.employeeId)}
                              >
                                {t('teamMembersPanel.endStint')}
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
            <h3 className="label mb-2">{t('teamMembersPanel.pastMembers')} {past.length}</h3>

            {past.length === 0 ? (
              <p className="text-[13px] text-muted">{t('teamMembersPanel.nobodyHasLeftThisTeam')}</p>
            ) : (
              <table className="w-full border-collapse text-sm">
                <caption className="sr-only">{t('teamMembersPanel.pastMembers2')}</caption>
                <thead>
                  <tr className="border-b border-line text-left">
                    <HeaderCell>{t('teamMembersPanel.employee')}</HeaderCell>
                    <HeaderCell>{t('teamMembersPanel.period')}</HeaderCell>
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
          {t('teamMembersPanel.done')}
        </Button>
      </div>
    </div>
  )
}

function AddMemberForm({ teamId, candidates, employeesError, onAdded }) {
  const t = useT()
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
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('teamMembersPanel.couldNotAddTheMember'))
    } finally {
      setBusy(false)
    }
  }

  const problem = formError ?? employeesError

  return (
    <form
      onSubmit={submit}
      noValidate
      aria-label={t('teamMembersPanel.addAMember')}
      className="border border-line-soft bg-cream-soft p-4"
    >
      <p className="label mb-3">{t('teamMembersPanel.addAMember')}</p>

      <div className="grid grid-cols-[minmax(0,1fr)_10.5rem] items-start gap-3">
        <SelectField
          label={t('teamMembersPanel.employee')}
          name="employeeId"
          required
          value={form.employeeId}
          onChange={update('employeeId')}
          error={fieldErrors.employeeId}
          placeholder={candidates.length > 0 ? t('teamMembersPanel.selectAnEmployee') : t('teamMembersPanel.noActiveEmployeeToAdd')}
          options={candidates.map((employee) => ({
            value: employee.id,
            label: employee.registrationNumber
              ? `${employee.name} · ${employee.registrationNumber}`
              : employee.name,
          }))}
        />

        <TextField
          label={t('teamMembersPanel.startDate')}
          name="startDate"
          type="date"
          required
          value={form.startDate}
          onChange={update('startDate')}
          error={fieldErrors.startDate}
        />
      </div>

      {problem && (
        <p role="alert" className="mt-3 rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
          {problem}
        </p>
      )}

      <Button type="submit" busy={busy} busyLabel={t('teamMembersPanel.adding')} className="mt-3">
        {t('teamMembersPanel.addMember')}
      </Button>
    </form>
  )
}

function EndStintForm({ teamId, member, onEnded, onCancel }) {
  const t = useT()
  const { teams } = useContainer()

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
      // No setBusy(false) on success: this form unmounts with its row.
      onEnded()
    } catch (failure) {
      setError(failure.fieldErrors?.endDate ?? failure.message ?? t('teamMembersPanel.couldNotEndTheStint'))
      setBusy(false)
    }
  }

  return (
    <form
      onSubmit={submit}
      noValidate
      onKeyDown={(event) => {
        // Without stopPropagation, Escape would also close the whole dialog.
        if (event.key === 'Escape') {
          event.stopPropagation()
          onCancel()
        }
      }}
      className="flex flex-wrap items-start gap-2.5"
    >
      <TextField
        ref={inputRef}
        label={t('teamMembersPanel.endDate')}
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
        hint={t('teamMembersPanel.onTheTeamSinceValue', { value: formatDate(member.startDate) })}
        className="w-48"
      />

      <div className="flex gap-2 pt-5">
        <Button type="submit" busy={busy} busyLabel={t('common.saving')} className="px-3 py-2">
          {t('teamMembersPanel.endStint')}
        </Button>
        <Button type="button" variant="outline" onClick={onCancel} disabled={busy} className="px-3 py-2">
          {t('common.cancel')}
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

function HeaderCell({ children }) {
  return <th className="label py-2 pr-3 font-normal">{children}</th>
}

function ListSkeleton() {
  const t = useT()
  return (
    <div role="status" aria-label={t('teamMembersPanel.loadingMembers')}>
      {Array.from({ length: 3 }).map((_, index) => (
        <div key={index} className="mb-2 h-10 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}

async function listAllEmployees(employees, signal) {
  const all = []

  for (let page = 1; page <= MAX_EMPLOYEE_PAGES; page += 1) {
    const result = await employees.list({ page, size: EMPLOYEE_PAGE_SIZE }, { signal })
    all.push(...result.items)
    if (page >= result.totalPages || result.items.length === 0) break
  }

  return all
}
