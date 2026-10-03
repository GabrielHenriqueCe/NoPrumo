import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { STAGE_STATUSES } from './stageStatus'

const EMPTY = {
  projectId: '',
  name: '',
  sortOrder: '',
  status: 'planned',
  percentage: '0',
  plannedDate: '',
  teamId: '',
  supervisorId: '',
}

function fromStage(stage) {
  return Object.fromEntries(
    Object.keys(EMPTY).map((field) => [field, stage[field] != null ? String(stage[field]) : EMPTY[field]]),
  )
}

const idOrNull = (value) => (value ? Number(value) : null)

export function StageFormDialog({ open, stage, projectOptions, teamOptions, supervisorOptions, onClose, onSubmit }) {
  const editing = Boolean(stage)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  const projectChosen = Boolean(form.projectId)

  useEffect(() => {
    if (!open) return
    setForm(stage ? fromStage(stage) : EMPTY)
    setFieldErrors({})
    setFormError(null)
  }, [open, stage])

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    const values = {
      projectId: Number(form.projectId),
      name: form.name.trim(),
      sortOrder: form.sortOrder === '' ? 0 : Number(form.sortOrder),
      status: form.status,
      percentage: form.percentage === '' ? 0 : Number(form.percentage),
      plannedDate: form.plannedDate || null,
      teamId: idOrNull(form.teamId),
      supervisorId: idOrNull(form.supervisorId),
    }

    try {
      await onSubmit(values)
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the stage.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit stage' : 'New stage'}
      description="A step of a project's schedule, in the order it is built."
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <SelectField
          label="Project"
          name="projectId"
          required
          value={form.projectId}
          onChange={update('projectId')}
          error={fieldErrors.projectId}
          placeholder={projectOptions.length ? 'Select a project…' : 'No active project registered'}
          options={projectOptions}
        />

        <fieldset disabled={!projectChosen} className="flex flex-col gap-4 disabled:opacity-50">
          {!projectChosen && (
            <legend className="mb-4 text-[12.5px] text-muted">Pick the project first.</legend>
          )}

          <div className="grid grid-cols-3 gap-4">
            <TextField
              className="col-span-2"
              label="Name"
              name="name"
              placeholder="e.g. Foundation"
              required
              value={form.name}
              onChange={update('name')}
              error={fieldErrors.name}
            />

            <TextField
              label="Order"
              name="sortOrder"
              type="number"
              min="0"
              step="1"
              inputMode="numeric"
              placeholder="1"
              value={form.sortOrder}
              onChange={update('sortOrder')}
              error={fieldErrors.sortOrder}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <SelectField
              label="Status"
              name="status"
              required
              value={form.status}
              onChange={update('status')}
              error={fieldErrors.status}
              options={STAGE_STATUSES}
            />

            <TextField
              label="Progress (%)"
              name="percentage"
              type="number"
              min="0"
              max="100"
              step="0.01"
              inputMode="decimal"
              value={form.percentage}
              onChange={update('percentage')}
              error={fieldErrors.percentage}
              hint="A completed stage must be at 100%."
            />
          </div>

          <TextField
            label="Planned date"
            name="plannedDate"
            type="date"
            value={form.plannedDate}
            onChange={update('plannedDate')}
            error={fieldErrors.plannedDate}
            hint="The stage shows as late once this date has passed."
          />

          <div className="grid grid-cols-2 gap-4">
            <SelectField
              label="Team"
              name="teamId"
              value={form.teamId}
              onChange={update('teamId')}
              error={fieldErrors.teamId}
              placeholder={teamOptions.length ? 'Select a team…' : 'No team registered'}
              options={teamOptions}
            />

            <SelectField
              label="Supervisor"
              name="supervisorId"
              value={form.supervisorId}
              onChange={update('supervisorId')}
              error={fieldErrors.supervisorId}
              placeholder={supervisorOptions.length ? 'Select an employee…' : 'No employee registered'}
              options={supervisorOptions}
            />
          </div>
        </fieldset>

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…" disabled={!projectChosen}>
            {editing ? 'Save changes' : 'Create stage'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
