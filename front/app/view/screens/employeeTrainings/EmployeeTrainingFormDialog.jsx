import { useEffect, useMemo, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { formatDate, todayIso } from '../../format/dates'
import { MODALITIES } from './trainingLabels'

/*
  Record or correct a training.

  There is no expiry field. The API sets the expiry from the issue date and
  the training type's validity, and sends it back; the form only shows what
  came. The hints under the fields repeat the chosen type's own rules —
  validity, minimum workload, in person — so they are seen before the API
  has to refuse anything.
*/

const EMPTY = {
  employeeId: '',
  trainingTypeId: '',
  issueDate: '',
  workloadHours: '',
  modality: '',
  instructor: '',
}

export function EmployeeTrainingFormDialog({ open, training, employees, trainingTypes, onClose, onSubmit }) {
  const editing = Boolean(training)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(
      training
        ? {
            employeeId: String(training.employeeId ?? ''),
            trainingTypeId: String(training.trainingTypeId ?? ''),
            issueDate: training.issueDate ?? '',
            workloadHours: training.workloadHours != null ? String(training.workloadHours) : '',
            modality: training.modality ?? '',
            instructor: training.instructor ?? '',
          }
        : EMPTY,
    )
    setFieldErrors({})
    setFormError(null)
  }, [open, training])

  // Active employees — plus whoever this certificate already belongs to: the
  // record of someone who has since left still has to open, and be fixed.
  const employeeOptions = useMemo(
    () =>
      employees
        .filter((employee) => employee.active || employee.id === training?.employeeId)
        .sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'))
        .map((employee) => ({ value: employee.id, label: describeEmployee(employee) })),
    [employees, training],
  )

  const typeOptions = trainingTypes.map((type) => ({ value: type.id, label: `${type.code} — ${type.name}` }))
  const selectedType = trainingTypes.find((type) => String(type.id) === form.trainingTypeId)

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()

    // "1.5" would not even reach the validation: it fails to convert on the
    // way in and comes back as a technical message.
    if (!isWholeOrBlank(form.workloadHours)) {
      setFieldErrors({ workloadHours: 'Use a whole number.' })
      return
    }

    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    try {
      await onSubmit({
        employeeId: form.employeeId ? Number(form.employeeId) : null,
        trainingTypeId: form.trainingTypeId ? Number(form.trainingTypeId) : null,
        issueDate: form.issueDate || null,
        workloadHours: form.workloadHours.trim() === '' ? null : Number(form.workloadHours),
        modality: form.modality || null,
        instructor: form.instructor.trim() || null,
      })
    } catch (error) {
      // The API is the one that validates. The form only shows where it hurt.
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the training.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit training' : 'New training'}
      description={describeExpiry(training)}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <SelectField
          label="Employee"
          name="employeeId"
          required
          value={form.employeeId}
          onChange={update('employeeId')}
          error={fieldErrors.employeeId}
          placeholder={employeeOptions.length > 0 ? 'Select an employee…' : 'No active employee registered yet'}
          options={employeeOptions}
        />

        <div>
          <SelectField
            label="Training type"
            name="trainingTypeId"
            required
            value={form.trainingTypeId}
            onChange={update('trainingTypeId')}
            error={fieldErrors.trainingTypeId}
            placeholder={typeOptions.length > 0 ? 'Select a training type…' : 'No training type registered yet'}
            options={typeOptions}
          />
          {selectedType && !fieldErrors.trainingTypeId && (
            <FieldHint>{describeValidity(selectedType)}</FieldHint>
          )}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Issue date"
            name="issueDate"
            type="date"
            required
            max={todayIso()}
            value={form.issueDate}
            onChange={update('issueDate')}
            error={fieldErrors.issueDate}
          />

          <TextField
            label="Workload (hours)"
            name="workloadHours"
            type="number"
            inputMode="numeric"
            min="1"
            step="1"
            value={form.workloadHours}
            onChange={update('workloadHours')}
            error={fieldErrors.workloadHours}
            hint={
              selectedType?.minWorkloadHours != null
                ? `At least ${selectedType.minWorkloadHours} h for ${selectedType.code}.`
                : 'Optional.'
            }
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <SelectField
              label="Modality"
              name="modality"
              value={form.modality}
              onChange={update('modality')}
              error={fieldErrors.modality}
              placeholder="Not informed"
              options={MODALITIES}
            />
            {selectedType?.requiresInPerson && !fieldErrors.modality && (
              <FieldHint>{selectedType.code} must be taken in person.</FieldHint>
            )}
          </div>

          <TextField
            label="Instructor"
            name="instructor"
            placeholder="Who gave the training"
            value={form.instructor}
            onChange={update('instructor')}
            error={fieldErrors.instructor}
          />
        </div>

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…">
            {editing ? 'Save changes' : 'Record training'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}

/*
  SelectField has no hint of its own; this sits under it with the same look
  as TextField's.
*/
function FieldHint({ children }) {
  return <p className="mt-1.5 text-[12px] text-muted">{children}</p>
}

function describeEmployee(employee) {
  const name = employee.registrationNumber
    ? `${employee.name} · ${employee.registrationNumber}`
    : employee.name

  return employee.active ? name : `${name} (inactive)`
}

// Repeats the type's own number; the date itself is the API's to work out.
function describeValidity(type) {
  if (type.validityMonths == null) return 'This training does not expire.'

  const months = type.validityMonths === 1 ? '1 month' : `${type.validityMonths} months`
  return `Valid for ${months} from the issue date.`
}

function describeExpiry(training) {
  if (!training) return 'The system sets the expiry date from the issue date and the training type.'

  if (!training.expiryDate) return 'Saved without an expiry date: its training type does not expire.'

  return `Saved expiry: ${formatDate(training.expiryDate)}. The system recalculates it if the issue date or the type changes.`
}

function isWholeOrBlank(value) {
  const text = value.trim()
  return text === '' || /^-?\d+$/.test(text)
}
