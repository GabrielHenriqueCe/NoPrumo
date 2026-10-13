import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { STAGE_STATUSES } from './stageStatus'
import { useT } from '../../preferences/preferencesContext'

const EMPTY = {
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

// The project is the page the form opened from; it has no select.
export function StageFormDialog({ open, stage, projectId, teamOptions, supervisorOptions, onClose, onSubmit }) {
  const t = useT()
  const editing = Boolean(stage)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

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
      projectId: Number(projectId),
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
      if (error.isValidation) {
        setFieldErrors(error.fieldErrors)
        // There is no project field on screen, so its error (e.g. an inactive project) goes here.
        if (error.fieldErrors.projectId) setFormError(error.fieldErrors.projectId)
      } else {
        setFormError(error.message ?? t('stageFormDialog.couldNotSaveTheStage'))
      }
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('stageFormDialog.editStage') : t('stageFormDialog.newStage')}
      description={t('stageFormDialog.aStepOfAProject')}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <div className="grid grid-cols-3 gap-4">
          <TextField
            className="col-span-2"
            label={t('common.name')}
            name="name"
            placeholder={t('stageFormDialog.eGFoundation')}
            required
            value={form.name}
            onChange={update('name')}
            error={fieldErrors.name}
          />

          <TextField
            label={t('stageFormDialog.order')}
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
            label={t('common.status')}
            name="status"
            required
            value={form.status}
            onChange={update('status')}
            error={fieldErrors.status}
            options={STAGE_STATUSES.map((value) => ({ value, label: t(`stageStatus.${value}`) }))}
          />

          <TextField
            label={t('stageFormDialog.progress')}
            name="percentage"
            type="number"
            min="0"
            max="100"
            step="0.01"
            inputMode="decimal"
            value={form.percentage}
            onChange={update('percentage')}
            error={fieldErrors.percentage}
            hint={t('stageFormDialog.aCompletedStageMustBe')}
          />
        </div>

        <TextField
          label={t('stageFormDialog.plannedDate')}
          name="plannedDate"
          type="date"
          value={form.plannedDate}
          onChange={update('plannedDate')}
          error={fieldErrors.plannedDate}
          hint={t('stageFormDialog.theStageShowsAsLate')}
        />

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label={t('stageFormDialog.team')}
            name="teamId"
            value={form.teamId}
            onChange={update('teamId')}
            error={fieldErrors.teamId}
            placeholder={teamOptions.length ? t('stageFormDialog.selectATeam') : t('stageFormDialog.noTeamRegistered')}
            options={teamOptions}
          />

          <SelectField
            label={t('stageFormDialog.supervisor')}
            name="supervisorId"
            value={form.supervisorId}
            onChange={update('supervisorId')}
            error={fieldErrors.supervisorId}
            placeholder={supervisorOptions.length ? t('stageFormDialog.selectAnEmployee') : t('stageFormDialog.noEmployeeRegistered')}
            options={supervisorOptions}
          />
        </div>

        {formError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('stageFormDialog.createStage')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
