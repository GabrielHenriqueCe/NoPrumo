import { useEffect, useMemo, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { formatDate, todayIso } from '../../format/dates'
import { MODALITIES } from './trainingLabels'
import { useT } from '../../preferences/preferencesContext'

// No expiry field: the API calculates it. The hints only repeat the chosen type's rules.

const EMPTY = {
  employeeId: '',
  trainingTypeId: '',
  issueDate: '',
  workloadHours: '',
  modality: '',
  instructor: '',
}

export function EmployeeTrainingFormDialog({ open, training, employees, trainingTypes, onClose, onSubmit }) {
  const t = useT()
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

  // Plus the certificate's own employee, even if inactive: old records still have to open.
  const employeeOptions = useMemo(
    () =>
      employees
        .filter((employee) => employee.active || employee.id === training?.employeeId)
        .sort((first, second) => first.name.localeCompare(second.name, 'pt-BR'))
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

    // "1.5" fails JSON conversion and would come back as a technical message.
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
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('employeeTrainingFormDialog.couldNotSaveTheTraining'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('employeeTrainingFormDialog.editTraining') : t('employeeTrainingFormDialog.newTraining')}
      description={describeExpiry(training, t)}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <SelectField
          label={t('employeeTrainingFormDialog.employee')}
          name="employeeId"
          required
          value={form.employeeId}
          onChange={update('employeeId')}
          error={fieldErrors.employeeId}
          placeholder={employeeOptions.length > 0 ? t('employeeTrainingFormDialog.selectAnEmployee') : t('employeeTrainingFormDialog.noActiveEmployeeRegisteredYet')}
          options={employeeOptions}
        />

        <div>
          <SelectField
            label={t('employeeTrainingFormDialog.trainingType')}
            name="trainingTypeId"
            required
            value={form.trainingTypeId}
            onChange={update('trainingTypeId')}
            error={fieldErrors.trainingTypeId}
            placeholder={typeOptions.length > 0 ? t('employeeTrainingFormDialog.selectATrainingType') : t('employeeTrainingFormDialog.noTrainingTypeRegisteredYet')}
            options={typeOptions}
          />
          {selectedType && !fieldErrors.trainingTypeId && (
            <FieldHint>{describeValidity(selectedType, t)}</FieldHint>
          )}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('employeeTrainingFormDialog.issueDate')}
            name="issueDate"
            type="date"
            required
            max={todayIso()}
            value={form.issueDate}
            onChange={update('issueDate')}
            error={fieldErrors.issueDate}
          />

          <TextField
            label={t('employeeTrainingFormDialog.workloadHours')}
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
                ? t('employeeTrainingFormDialog.atLeastMinworkloadhoursHFor', { minWorkloadHours: selectedType.minWorkloadHours, code: selectedType.code })
                : t('employeeTrainingFormDialog.optional')
            }
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <SelectField
              label={t('employeeTrainingFormDialog.modality')}
              name="modality"
              value={form.modality}
              onChange={update('modality')}
              error={fieldErrors.modality}
              placeholder={t('employeeTrainingFormDialog.notInformed')}
              options={MODALITIES.map((value) => ({ value, label: t(`labels.modality.${value}`) }))}
            />
            {selectedType?.requiresInPerson && !fieldErrors.modality && (
              <FieldHint>{selectedType.code} {t('employeeTrainingFormDialog.mustBeTakenInPerson')}</FieldHint>
            )}
          </div>

          <TextField
            label={t('employeeTrainingFormDialog.instructor')}
            name="instructor"
            placeholder={t('employeeTrainingFormDialog.whoGaveTheTraining')}
            value={form.instructor}
            onChange={update('instructor')}
            error={fieldErrors.instructor}
          />
        </div>

        {formError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('employeeTrainingFormDialog.recordTraining')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}

// SelectField has no hint prop.
function FieldHint({ children }) {
  return <p className="mt-1.5 text-[12px] text-muted">{children}</p>
}

function describeEmployee(employee) {
  const name = employee.registrationNumber
    ? `${employee.name} · ${employee.registrationNumber}`
    : employee.name

  return employee.active ? name : `${name} (inactive)`
}

function describeValidity(type, t) {
  if (type.validityMonths == null) return t('extras.neverExpires')

  return t('extras.validFor', { count: type.validityMonths })
}

function describeExpiry(training, t) {
  if (!training) return t('extras.expirySetBySystem')

  if (!training.expiryDate) return t('extras.savedWithoutExpiry')

  return t('extras.savedExpiry', { value: formatDate(training.expiryDate) })
}

function isWholeOrBlank(value) {
  const text = value.trim()
  return text === '' || /^-?\d+$/.test(text)
}
