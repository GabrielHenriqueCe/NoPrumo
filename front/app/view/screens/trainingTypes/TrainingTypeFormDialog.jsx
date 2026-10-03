import { useEffect, useId, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'

/*
  Create or edit a training type.

  Validity and minimum workload are optional on purpose: a driving licence
  has no workload, a CIPA mandate has no fixed validity. Blank goes to the
  API as null, which is how it reads "does not apply".

  The API is the one that validates. The only check made here is that both
  numbers are whole: "1.5" would not even reach the validation — it fails to
  convert on the way in and comes back as a technical message.
*/

const EMPTY = {
  code: '',
  name: '',
  validityMonths: '',
  minWorkloadHours: '',
  requiresInPerson: false,
}

const WHOLE_NUMBER_FIELDS = ['validityMonths', 'minWorkloadHours']

export function TrainingTypeFormDialog({ open, trainingType, onClose, onSubmit }) {
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
      // The API is the one that validates. The form only shows where it hurt.
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the training type.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit training type' : 'New training type'}
      description={
        editing
          ? 'Changing the validity does not move the expiry of trainings already saved.'
          : 'An NR course, the ASO or any other certificate an employee may need.'
      }
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <div className="grid grid-cols-3 gap-4">
          <TextField
            label="Code"
            name="code"
            placeholder="e.g. NR-35"
            required
            value={form.code}
            onChange={update('code')}
            error={fieldErrors.code}
            hint="Saved in capitals."
          />

          <TextField
            label="Name"
            name="name"
            placeholder="e.g. Work at height"
            required
            className="col-span-2"
            value={form.name}
            onChange={update('name')}
            error={fieldErrors.name}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Validity (months)"
            name="validityMonths"
            type="number"
            inputMode="numeric"
            min="1"
            step="1"
            value={form.validityMonths}
            onChange={update('validityMonths')}
            error={fieldErrors.validityMonths}
            hint="Leave blank if it never expires."
          />

          <TextField
            label="Minimum workload (hours)"
            name="minWorkloadHours"
            type="number"
            inputMode="numeric"
            min="1"
            step="1"
            value={form.minWorkloadHours}
            onChange={update('minWorkloadHours')}
            error={fieldErrors.minWorkloadHours}
            hint="Leave blank if there is none."
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
            className="mt-0.5 h-4 w-4 cursor-pointer accent-graphite"
          />

          <div>
            <label htmlFor={inPersonId} className="cursor-pointer text-sm font-medium">
              Must be taken in person
            </label>
            <p id={`${inPersonId}-hint`} className="mt-0.5 text-[12px] text-muted">
              Online and blended trainings of this type are refused.
            </p>
            {fieldErrors.requiresInPerson && (
              <p className="mt-1.5 text-[12.5px] text-danger">{fieldErrors.requiresInPerson}</p>
            )}
          </div>
        </div>

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…">
            {editing ? 'Save changes' : 'Create training type'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
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
