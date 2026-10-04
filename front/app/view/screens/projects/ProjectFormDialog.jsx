import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { PROJECT_STATUSES } from './projectStatus'

/*
  Create or edit a project.

  contractAmount only appears for accounts with view_finance. The API is the
  real gate: it ignores the field for everyone else.

  No date checks here (forecast before start, and so on). The database has no
  CHECK constraints, so the API validates — and its answer lands on the field
  through ProblemDetails `errors`.
*/

const EMPTY = {
  code: '',
  name: '',
  clientId: '',
  status: 'planning',
  supervisorId: '',
  cno: '',
  technicalManager: '',
  creaRt: '',
  startDate: '',
  forecastDate: '',
  contractAmount: '',
  street: '',
  number: '',
  complement: '',
  district: '',
  city: '',
  state: '',
  postalCode: '',
}

function fromProject(project) {
  return Object.fromEntries(
    Object.keys(EMPTY).map((field) => [field, project[field] != null ? String(project[field]) : EMPTY[field]]),
  )
}

const textOrNull = (value) => value.trim() || null
const idOrNull = (value) => (value ? Number(value) : null)

export function ProjectFormDialog({ open, project, clientOptions, supervisorOptions, canSeeAmount, onClose, onSubmit }) {
  const editing = Boolean(project)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(project ? fromProject(project) : EMPTY)
    setFieldErrors({})
    setFormError(null)
  }, [open, project])

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
      code: form.code.trim(),
      name: form.name.trim(),
      clientId: idOrNull(form.clientId),
      status: form.status,
      supervisorId: idOrNull(form.supervisorId),
      cno: textOrNull(form.cno),
      technicalManager: textOrNull(form.technicalManager),
      creaRt: textOrNull(form.creaRt),
      startDate: form.startDate || null,
      forecastDate: form.forecastDate || null,
      street: textOrNull(form.street),
      number: textOrNull(form.number),
      complement: textOrNull(form.complement),
      district: textOrNull(form.district),
      city: textOrNull(form.city),
      state: textOrNull(form.state.toUpperCase()),
      postalCode: textOrNull(form.postalCode),
    }

    // Not sent at all without view_finance, instead of sent as null: a null
    // would read as "set the amount to zero" if the API ever trusted it.
    if (canSeeAmount) {
      values.contractAmount = form.contractAmount === '' ? null : Number(form.contractAmount)
    }

    try {
      await onSubmit(values)
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the project.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit project' : 'New project'}
      description="A construction site, its client and who runs it."
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <div className="grid grid-cols-3 gap-4">
          <TextField
            label="Code"
            name="code"
            placeholder="OB-2026-01"
            required
            value={form.code}
            onChange={update('code')}
            error={fieldErrors.code}
          />

          <TextField
            className="col-span-2"
            label="Name"
            name="name"
            placeholder="e.g. Aurora Residential"
            required
            value={form.name}
            onChange={update('name')}
            error={fieldErrors.name}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label="Client"
            name="clientId"
            value={form.clientId}
            onChange={update('clientId')}
            error={fieldErrors.clientId}
            placeholder={clientOptions.length ? 'Select a client…' : 'No client registered'}
            options={clientOptions}
          />

          <SelectField
            label="Status"
            name="status"
            required
            value={form.status}
            onChange={update('status')}
            error={fieldErrors.status}
            options={PROJECT_STATUSES}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label="Supervisor"
            name="supervisorId"
            value={form.supervisorId}
            onChange={update('supervisorId')}
            error={fieldErrors.supervisorId}
            placeholder={supervisorOptions.length ? 'Select an employee…' : 'No employee registered'}
            options={supervisorOptions}
          />

          <TextField
            label="CNO"
            name="cno"
            placeholder="00.000.00000/00"
            value={form.cno}
            onChange={update('cno')}
            error={fieldErrors.cno}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Technical manager"
            name="technicalManager"
            placeholder="e.g. Eng. Ana Souza"
            value={form.technicalManager}
            onChange={update('technicalManager')}
            error={fieldErrors.technicalManager}
          />

          <TextField
            label="CREA / RT"
            name="creaRt"
            placeholder="SC-000000/D"
            value={form.creaRt}
            onChange={update('creaRt')}
            error={fieldErrors.creaRt}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Start date"
            name="startDate"
            type="date"
            value={form.startDate}
            onChange={update('startDate')}
            error={fieldErrors.startDate}
          />

          <TextField
            label="Forecast date"
            name="forecastDate"
            type="date"
            value={form.forecastDate}
            onChange={update('forecastDate')}
            error={fieldErrors.forecastDate}
            hint="The project shows as late once this date has passed."
          />
        </div>

        {canSeeAmount && (
          <TextField
            label="Contract amount (R$)"
            name="contractAmount"
            type="number"
            min="0"
            step="0.01"
            inputMode="decimal"
            placeholder="0.00"
            value={form.contractAmount}
            onChange={update('contractAmount')}
            error={fieldErrors.contractAmount}
          />
        )}

        <fieldset className="flex flex-col gap-4 border-t border-line-soft pt-4">
          <legend className="label pr-2">Site address</legend>

          <div className="grid grid-cols-3 gap-4">
            <TextField
              className="col-span-2"
              label="Street"
              name="street"
              placeholder="e.g. Rua XV de Novembro"
              value={form.street}
              onChange={update('street')}
              error={fieldErrors.street}
            />

            <TextField
              label="Number"
              name="number"
              placeholder="100"
              value={form.number}
              onChange={update('number')}
              error={fieldErrors.number}
            />
          </div>

          <div className="grid grid-cols-3 gap-4">
            <TextField
              label="Complement"
              name="complement"
              placeholder="Lot 4"
              value={form.complement}
              onChange={update('complement')}
              error={fieldErrors.complement}
            />

            <TextField
              label="District"
              name="district"
              placeholder="Centro"
              value={form.district}
              onChange={update('district')}
              error={fieldErrors.district}
            />

            <TextField
              label="Postal code"
              name="postalCode"
              placeholder="89000-000"
              value={form.postalCode}
              onChange={update('postalCode')}
              error={fieldErrors.postalCode}
            />
          </div>

          <div className="grid grid-cols-3 gap-4">
            <TextField
              className="col-span-2"
              label="City"
              name="city"
              placeholder="Blumenau"
              value={form.city}
              onChange={update('city')}
              error={fieldErrors.city}
            />

            <TextField
              label="State (UF)"
              name="state"
              placeholder="SC"
              maxLength={2}
              value={form.state}
              onChange={update('state')}
              error={fieldErrors.state}
            />
          </div>
        </fieldset>

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…">
            {editing ? 'Save changes' : 'Create project'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
