import { useEffect, useState } from 'react'
import { todayIso } from '../../format/dates'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { APPOINTMENT_TYPES } from './appointmentLabels'
import { useT } from '../../preferences/preferencesContext'

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
  const t = useT()
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
      else setFormError(error.message ?? t('appointmentFormDialog.couldNotSaveTheAppointment'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('appointmentFormDialog.editAppointment') : t('appointmentFormDialog.newAppointment')}
      description={t('appointmentFormDialog.trainingExpiriesAreNotEntered')}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <TextField
          label={t('appointmentFormDialog.title')}
          name="title"
          placeholder={t('appointmentFormDialog.eGConcreteDelivery')}
          required
          value={form.title}
          onChange={update('title')}
          error={fieldErrors.title}
        />

        <div className="grid grid-cols-3 gap-4">
          <TextField
            label={t('appointmentFormDialog.date')}
            name="date"
            type="date"
            required
            value={form.date}
            onChange={update('date')}
            error={fieldErrors.date}
          />

          <TextField
            label={t('appointmentFormDialog.time')}
            name="time"
            type="time"
            value={form.time}
            onChange={update('time')}
            error={fieldErrors.time}
            hint={t('appointmentFormDialog.optional')}
          />

          <SelectField
            label={t('appointmentFormDialog.type')}
            name="type"
            required
            value={form.type}
            onChange={update('type')}
            error={fieldErrors.type}
            placeholder={t('appointmentFormDialog.selectAType')}
            options={APPOINTMENT_TYPES.map((value) => ({ value, label: t(`labels.appointmentType.${value}`) }))}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label={t('appointmentFormDialog.project')}
            name="projectId"
            value={form.projectId}
            onChange={update('projectId')}
            error={fieldErrors.projectId}
            placeholder={t('appointmentFormDialog.noProject')}
            options={projectOptions}
          />

          <SelectField
            label={t('appointmentFormDialog.responsible')}
            name="assignedToId"
            value={form.assignedToId}
            onChange={update('assignedToId')}
            error={fieldErrors.assignedToId}
            placeholder={t('appointmentFormDialog.nobodyInParticular')}
            options={assigneeOptions}
          />
        </div>

        <TextField
          label={t('appointmentFormDialog.description')}
          name="description"
          value={form.description}
          onChange={update('description')}
          error={fieldErrors.description}
          hint={t('appointmentFormDialog.optional')}
        />

        {formError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('appointmentFormDialog.createAppointment')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
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
