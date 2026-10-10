import { useEffect, useState } from 'react'
import { todayIso } from '../../format/dates'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { APPOINTMENT_TYPES } from './appointmentLabels'

const emptyForm = (projectId) => ({
  date: todayIso(),
  time: '',
  title: '',
  type: 'project',
  projectId: projectId ?? '',
  assignedToId: '',
  description: '',
})

export function AppointmentFormDialog({ open, appointment, options, defaultProjectId, onClose, onSubmit }) {
  const editing = Boolean(appointment)

  const [form, setForm] = useState(() => emptyForm(defaultProjectId))
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(
      appointment
        ? {
            date: appointment.date,
            time: appointment.time ? appointment.time.slice(0, 5) : '',
            title: appointment.title,
            type: appointment.type,
            projectId: appointment.projectId != null ? String(appointment.projectId) : '',
            assignedToId: appointment.assignedToId != null ? String(appointment.assignedToId) : '',
            description: appointment.description ?? '',
          }
        : emptyForm(defaultProjectId),
    )
    setFieldErrors({})
    setFormError(null)
  }, [open, appointment, defaultProjectId])

  // A project or person switched off later still shows on the old appointment.
  const projectOptions = withCurrent(options.projects, appointment?.projectId, appointment?.projectName)
  const assigneeOptions = withCurrent(options.assignees, appointment?.assignedToId, appointment?.assignedToName)

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
      await onSubmit({
        date: form.date || null,
        // <input type="time"> gives 'HH:mm'; the API reads TimeOnly as 'HH:mm:ss'.
        time: form.time ? `${form.time}:00` : null,
        title: form.title.trim(),
        type: form.type,
        projectId: form.projectId ? Number(form.projectId) : null,
        assignedToId: form.assignedToId ? Number(form.assignedToId) : null,
        description: form.description.trim() || null,
      })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the appointment.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit appointment' : 'New appointment'}
      description="Training expiries are not entered here: they come from the training records."
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <TextField
          label="Title"
          name="title"
          placeholder="e.g. Concrete delivery"
          required
          value={form.title}
          onChange={update('title')}
          error={fieldErrors.title}
        />

        <div className="grid grid-cols-3 gap-4">
          <TextField
            label="Date"
            name="date"
            type="date"
            required
            value={form.date}
            onChange={update('date')}
            error={fieldErrors.date}
          />

          <TextField
            label="Time"
            name="time"
            type="time"
            value={form.time}
            onChange={update('time')}
            error={fieldErrors.time}
            hint="Optional."
          />

          <SelectField
            label="Type"
            name="type"
            required
            value={form.type}
            onChange={update('type')}
            error={fieldErrors.type}
            placeholder="Select a type…"
            options={APPOINTMENT_TYPES}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label="Project"
            name="projectId"
            value={form.projectId}
            onChange={update('projectId')}
            error={fieldErrors.projectId}
            placeholder="No project"
            options={projectOptions}
          />

          <SelectField
            label="Responsible"
            name="assignedToId"
            value={form.assignedToId}
            onChange={update('assignedToId')}
            error={fieldErrors.assignedToId}
            placeholder="Nobody in particular"
            options={assigneeOptions}
          />
        </div>

        <TextField
          label="Description"
          name="description"
          value={form.description}
          onChange={update('description')}
          error={fieldErrors.description}
          hint="Optional."
        />

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…">
            {editing ? 'Save changes' : 'Create appointment'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}

function withCurrent(list, currentId, currentName) {
  const options = list.map((option) => ({ value: option.id, label: option.name }))
  if (currentId != null && !list.some((option) => option.id === currentId)) {
    options.push({ value: currentId, label: `${currentName ?? `#${currentId}`} (inactive)` })
  }
  return options
}
