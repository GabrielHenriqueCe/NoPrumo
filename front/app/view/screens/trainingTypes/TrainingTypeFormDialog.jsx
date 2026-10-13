import { useEffect, useId, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

// Validity and workload are optional (a CNH has no workload): blank goes as null.
// The only local check is "whole number": "1.5" fails JSON conversion with a technical message.

const EMPTY = {
  code: '',
  name: '',
  validityMonths: '',
  minWorkloadHours: '',
  requiresInPerson: false,
}

const WHOLE_NUMBER_FIELDS = ['validityMonths', 'minWorkloadHours']

export function TrainingTypeFormDialog({ open, trainingType, onClose, onSubmit }) {
  const t = useT()
  const editing = Boolean(trainingType)
  const inPersonId = useId()

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(
      trainingType
        ? {
            code: trainingType.code ?? '',
            name: trainingType.name ?? '',
            validityMonths: toText(trainingType.validityMonths),
            minWorkloadHours: toText(trainingType.minWorkloadHours),
            requiresInPerson: Boolean(trainingType.requiresInPerson),
          }
        : EMPTY,
    )
    setFieldErrors({})
    setFormError(null)
  }, [open, trainingType])

  const update = (field) => (event) => {
    const { type, checked, value } = event.target
    setForm((current) => ({ ...current, [field]: type === 'checkbox' ? checked : value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()

    const notWhole = WHOLE_NUMBER_FIELDS.filter((field) => !isWholeOrBlank(form[field]))
    if (notWhole.length > 0) {
      setFieldErrors(Object.fromEntries(notWhole.map((field) => [field, 'Use a whole number.'])))
      return
    }

    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    try {
      await onSubmit({
        code: form.code.trim(),
        name: form.name.trim(),
        validityMonths: toNumberOrNull(form.validityMonths),
        minWorkloadHours: toNumberOrNull(form.minWorkloadHours),
        requiresInPerson: form.requiresInPerson,
      })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('trainingTypeFormDialog.couldNotSaveTheTraining'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('trainingTypeFormDialog.editTrainingType') : t('trainingTypeFormDialog.newTrainingType')}
      description={
        editing
          ? t('trainingTypeFormDialog.changingTheValidityDoesNot')
          : t('trainingTypeFormDialog.anNrCourseTheAso')
      }
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <div className="grid grid-cols-3 gap-4">
          <TextField
            label={t('trainingTypeFormDialog.code')}
            name="code"
            placeholder={t('trainingTypeFormDialog.eGNr35')}
            required
            value={form.code}
            onChange={update('code')}
            error={fieldErrors.code}
            hint={t('trainingTypeFormDialog.savedInCapitals')}
          />

          <TextField
            label={t('common.name')}
            name="name"
            placeholder={t('trainingTypeFormDialog.eGWorkAtHeight')}
            required
            className="col-span-2"
            value={form.name}
            onChange={update('name')}
            error={fieldErrors.name}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('trainingTypeFormDialog.validityMonths')}
            name="validityMonths"
            type="number"
            inputMode="numeric"
            min="1"
            step="1"
            value={form.validityMonths}
            onChange={update('validityMonths')}
            error={fieldErrors.validityMonths}
            hint={t('trainingTypeFormDialog.leaveBlankIfItNever')}
          />

          <TextField
            label={t('trainingTypeFormDialog.minimumWorkloadHours')}
            name="minWorkloadHours"
            type="number"
            inputMode="numeric"
            min="1"
            step="1"
            value={form.minWorkloadHours}
            onChange={update('minWorkloadHours')}
            error={fieldErrors.minWorkloadHours}
            hint={t('trainingTypeFormDialog.leaveBlankIfThereIs')}
          />
        </div>

        <div className="flex items-start gap-2.5 border-t border-line-soft pt-4">
          <input
            id={inPersonId}
            type="checkbox"
            name="requiresInPerson"
            checked={form.requiresInPerson}
            onChange={update('requiresInPerson')}
            aria-describedby={`${inPersonId}-hint`}
            className="mt-0.5 h-4 w-4 cursor-pointer accent-[var(--ink)]"
          />

          <div>
            <label htmlFor={inPersonId} className="cursor-pointer text-sm font-medium">
              {t('trainingTypeFormDialog.mustBeTakenInPerson')}
            </label>
            <p id={`${inPersonId}-hint`} className="mt-0.5 text-[12px] text-muted">
              {t('trainingTypeFormDialog.onlineAndBlendedTrainingsOf')}
            </p>
            {fieldErrors.requiresInPerson && (
              <p className="mt-1.5 text-[12.5px] text-danger">{fieldErrors.requiresInPerson}</p>
            )}
          </div>
        </div>

        {formError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('trainingTypeFormDialog.createTrainingType')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}

function toText(value) {
  return value == null ? '' : String(value)
}

function isWholeOrBlank(value) {
  const text = value.trim()
  return text === '' || /^-?\d+$/.test(text)
}

function toNumberOrNull(value) {
  return value.trim() === '' ? null : Number(value)
}
