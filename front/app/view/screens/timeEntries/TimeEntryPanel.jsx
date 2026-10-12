import { useCallback, useEffect, useState } from 'react'
import { formatDate, todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

const PAGE_SIZE = 10
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }
const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

function monthStart() {
  return `${todayIso().slice(0, 7)}-01`
}

function formatTime(value) {
  return value ? String(value).slice(0, 5) : '—'
}

function formatHours(value) {
  if (value == null || value === '') return '—'
  return `${Number(value).toLocaleString('pt-BR', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })} h`
}

export function TimeEntryPanel({ projectId, employees = [] }) {
  const { timeEntries } = useContainer()
  const { can } = useSession()
  const canManage = can(['manage_projects', 'manage_safety', 'manage_schedule'])
  const canSeeCost = can('view_finance')

  const [data, setData] = useState(EMPTY_PAGE)
  const [from, setFrom] = useState(monthStart)
  const [to, setTo] = useState(todayIso)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [form, setForm] = useState(() => ({
    date: todayIso(),
    employeeId: '',
    clockIn: '08:00',
    clockOut: '17:00',
  }))
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [success, setSuccess] = useState(null)
  const [saving, setSaving] = useState(false)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  useEffect(() => {
    if (!canManage) {
      setLoading(false)
      return undefined
    }

    if (!from || !to || from > to) {
      setData(EMPTY_PAGE)
      setLoadError(!from || !to
        ? 'Choose both dates to filter time entries.'
        : 'The start date cannot be after the end date.')
      setLoading(false)
      return undefined
    }

    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    timeEntries
      .list({ projectId, from, to, page, size: PAGE_SIZE }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message || 'Could not load time entries.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [timeEntries, projectId, from, to, page, reloadToken, canManage])

  const updateForm = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
    setFormError(null)
    setSuccess(null)
  }

  const submit = async (event) => {
    event.preventDefault()
    setFieldErrors({})
    setFormError(null)
    setSuccess(null)

    const errors = {}
    if (!form.date) errors.date = 'Choose the date.'
    if (!form.employeeId) errors.employeeId = 'Choose an employee.'
    if (!form.clockIn) errors.clockIn = 'Enter the clock-in time.'
    if (!form.clockOut) errors.clockOut = 'Enter the clock-out time.'
    if (form.clockIn && form.clockOut && form.clockOut < form.clockIn) {
      errors.clockOut = 'Clock-out cannot be earlier than clock-in.'
    }

    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors)
      return
    }

    setSaving(true)
    try {
      await timeEntries.create({
        projectId: Number(projectId),
        employeeId: Number(form.employeeId),
        date: form.date,
        clockIn: form.clockIn,
        clockOut: form.clockOut,
      })
      setForm((current) => ({ ...current, employeeId: '' }))
      setSuccess('Time entry registered successfully.')
      setPage(1)
      reload()
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message || 'Could not register the time entry.')
    } finally {
      setSaving(false)
    }
  }

  const employeeOptions = employees.map((employee) => ({
    value: employee.id,
    label: employee.registrationNumber
      ? `${employee.name} · ${employee.registrationNumber}`
      : employee.name,
  }))

  if (!canManage) {
    return (
      <section className="border border-line bg-white p-4">
        <p className="label">Time entry</p>
        <p className="mt-2 text-sm text-muted">You do not have permission to view time entries on this project.</p>
      </section>
    )
  }

  return (
    <section className="flex flex-col gap-4">
      <div>
        <p className="label">Project team</p>
        <h3 className="mt-1 text-lg font-semibold text-graphite">Time entry</h3>
        <p className="mt-1 max-w-[54em] text-sm leading-relaxed text-muted">
          Record the hours worked by employees currently assigned to this project.
        </p>
      </div>

      <div className="border border-line bg-white p-4">
        <p className="label">Register hours</p>
        <form onSubmit={submit} noValidate className="mt-3 grid grid-cols-1 items-start gap-3 sm:grid-cols-2 xl:grid-cols-5">
          <TextField
            label="Date"
            name="date"
            type="date"
            required
            value={form.date}
            onChange={updateForm('date')}
            error={fieldErrors.date}
          />

          <SelectField
            label="Employee"
            name="employeeId"
            required
            value={form.employeeId}
            onChange={updateForm('employeeId')}
            error={fieldErrors.employeeId}
            placeholder={employeeOptions.length ? 'Select an employee…' : 'No current team members'}
            options={employeeOptions}
          />

          <TextField
            label="Clock-in"
            name="clockIn"
            type="time"
            required
            value={form.clockIn}
            onChange={updateForm('clockIn')}
            error={fieldErrors.clockIn}
          />

          <TextField
            label="Clock-out"
            name="clockOut"
            type="time"
            required
            value={form.clockOut}
            onChange={updateForm('clockOut')}
            error={fieldErrors.clockOut}
          />

          <div className="pt-5">
            <Button type="submit" busy={saving} busyLabel="Saving…" disabled={employeeOptions.length === 0}>
              Register
            </Button>
          </div>
        </form>

        {!employeeOptions.length && (
          <p className="mt-3 text-[12.5px] text-muted">
            Allocate a team to the project and add current members before registering time.
          </p>
        )}
        {formError && (
          <p role="alert" className="mt-3 border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}
        {success && (
          <p role="status" className="mt-3 border border-line bg-cream-soft px-3 py-2 text-[12.5px] text-graphite">
            {success}
          </p>
        )}
      </div>

      <div className="flex flex-wrap items-end gap-3">
        <div>
          <p className="label">History</p>
          <h4 className="mt-1 font-semibold text-graphite">Recorded hours</h4>
        </div>
        <div className="ml-auto grid grid-cols-1 gap-3 sm:grid-cols-2">
          <TextField
            label="From"
            name="from"
            type="date"
            required
            value={from}
            onChange={(event) => {
              setFrom(event.target.value)
              setPage(1)
            }}
            className="w-44"
          />
          <TextField
            label="To"
            name="to"
            type="date"
            required
            value={to}
            onChange={(event) => {
              setTo(event.target.value)
              setPage(1)
            }}
            className="w-44"
          />
        </div>
      </div>

      <div className="overflow-x-auto border border-line bg-white">
        {loading ? (
          <div className="p-4" role="status" aria-label="Loading time entries">
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="mb-2 h-10 animate-pulse bg-cream last:mb-0" />
            ))}
          </div>
        ) : loadError ? (
          <div className="px-6 py-10 text-center">
            <p role="alert" className="text-sm text-danger">{loadError}</p>
            <Button variant="outline" onClick={reload} className="mt-4">Try again</Button>
          </div>
        ) : data.items.length === 0 ? (
          <div className="px-6 py-10 text-center text-sm text-muted">
            No time entries for the selected period.
          </div>
        ) : (
          <table className="w-full border-collapse text-sm">
            <caption className="sr-only">Time entries for this project</caption>
            <thead>
              <tr className="border-b border-line bg-cream-soft text-left">
                <HeaderCell>Date</HeaderCell>
                <HeaderCell>Employee</HeaderCell>
                <HeaderCell>Clock-in</HeaderCell>
                <HeaderCell>Clock-out</HeaderCell>
                <HeaderCell>Hours</HeaderCell>
                {canSeeCost && <HeaderCell>Cost generated</HeaderCell>}
              </tr>
            </thead>
            <tbody>
              {data.items.map((entry) => (
                <tr key={entry.id} className="border-b border-line-soft last:border-0">
                  <td className="px-4 py-3 whitespace-nowrap">{formatDate(entry.date)}</td>
                  <td className="px-4 py-3 font-medium">{entry.employeeName || '—'}</td>
                  <td className="px-4 py-3 whitespace-nowrap tabular-nums">{formatTime(entry.clockIn)}</td>
                  <td className="px-4 py-3 whitespace-nowrap tabular-nums">{formatTime(entry.clockOut)}</td>
                  <td className="px-4 py-3 whitespace-nowrap tabular-nums">{formatHours(entry.hours)}</td>
                  {canSeeCost && (
                    <td className="px-4 py-3 whitespace-nowrap tabular-nums">
                      {entry.cost == null ? '—' : money.format(Number(entry.cost))}
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {!loading && !loadError && (
        <Pagination
          noun="time entry"
          page={data.page}
          totalPages={data.totalPages}
          total={data.total}
          onChange={setPage}
        />
      )}
    </section>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}
