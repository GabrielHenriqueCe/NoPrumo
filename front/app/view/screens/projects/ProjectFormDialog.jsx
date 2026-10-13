import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { PROJECT_STATUSES } from './projectStatus'
import { useT } from '../../preferences/preferencesContext'

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
  const t = useT()
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

    // Left out, not null, without view_finance: the API keeps the current amount either way.
    if (canSeeAmount) {
      values.contractAmount = form.contractAmount === '' ? null : Number(form.contractAmount)
    }

    try {
      await onSubmit(values)
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('projectFormDialog.couldNotSaveTheProject'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('projectFormDialog.editProject') : t('projectFormDialog.newProject')}
      description={t('projectFormDialog.aConstructionSiteItsClient')}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <div className="grid grid-cols-3 gap-4">
          <TextField
            label={t('projectFormDialog.code')}
            name="code"
            placeholder={t('projectFormDialog.ob202601')}
            required
            value={form.code}
            onChange={update('code')}
            error={fieldErrors.code}
          />

          <TextField
            className="col-span-2"
            label={t('common.name')}
            name="name"
            placeholder={t('projectFormDialog.eGAuroraResidential')}
            required
            value={form.name}
            onChange={update('name')}
            error={fieldErrors.name}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label={t('projectFormDialog.client')}
            name="clientId"
            value={form.clientId}
            onChange={update('clientId')}
            error={fieldErrors.clientId}
            placeholder={clientOptions.length ? t('projectFormDialog.selectAClient') : t('projectFormDialog.noClientRegistered')}
            options={clientOptions}
          />

          <SelectField
            label={t('common.status')}
            name="status"
            required
            value={form.status}
            onChange={update('status')}
            error={fieldErrors.status}
            options={PROJECT_STATUSES.map((value) => ({ value, label: t(`projectStatus.${value}`) }))}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label={t('projectFormDialog.supervisor')}
            name="supervisorId"
            value={form.supervisorId}
            onChange={update('supervisorId')}
            error={fieldErrors.supervisorId}
            placeholder={supervisorOptions.length ? t('projectFormDialog.selectAnEmployee') : t('projectFormDialog.noEmployeeRegistered')}
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
            label={t('projectFormDialog.technicalManager')}
            name="technicalManager"
            placeholder={t('projectFormDialog.eGEngAnaSouza')}
            value={form.technicalManager}
            onChange={update('technicalManager')}
            error={fieldErrors.technicalManager}
          />

          <TextField
            label={t('projectFormDialog.creaRt')}
            name="creaRt"
            placeholder={t('projectFormDialog.sc000000D')}
            value={form.creaRt}
            onChange={update('creaRt')}
            error={fieldErrors.creaRt}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('projectFormDialog.startDate')}
            name="startDate"
            type="date"
            value={form.startDate}
            onChange={update('startDate')}
            error={fieldErrors.startDate}
          />

          <TextField
            label={t('projectFormDialog.forecastDate')}
            name="forecastDate"
            type="date"
            value={form.forecastDate}
            onChange={update('forecastDate')}
            error={fieldErrors.forecastDate}
            hint={t('projectFormDialog.theProjectShowsAsLate')}
          />
        </div>

        {canSeeAmount && (
          <TextField
            label={t('projectFormDialog.contractAmountR')}
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
          <legend className="label pr-2">{t('projectFormDialog.siteAddress')}</legend>

          <div className="grid grid-cols-3 gap-4">
            <TextField
              className="col-span-2"
              label={t('projectFormDialog.street')}
              name="street"
              placeholder={t('projectFormDialog.eGRuaXvDe')}
              value={form.street}
              onChange={update('street')}
              error={fieldErrors.street}
            />

            <TextField
              label={t('projectFormDialog.number')}
              name="number"
              placeholder="100"
              value={form.number}
              onChange={update('number')}
              error={fieldErrors.number}
            />
          </div>

          <div className="grid grid-cols-3 gap-4">
            <TextField
              label={t('projectFormDialog.complement')}
              name="complement"
              placeholder={t('projectFormDialog.lot4')}
              value={form.complement}
              onChange={update('complement')}
              error={fieldErrors.complement}
            />

            <TextField
              label={t('projectFormDialog.district')}
              name="district"
              placeholder={t('projectFormDialog.centro')}
              value={form.district}
              onChange={update('district')}
              error={fieldErrors.district}
            />

            <TextField
              label={t('projectFormDialog.postalCode')}
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
              label={t('projectFormDialog.city')}
              name="city"
              placeholder={t('projectFormDialog.blumenau')}
              value={form.city}
              onChange={update('city')}
              error={fieldErrors.city}
            />

            <TextField
              label={t('projectFormDialog.stateUf')}
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
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('projectFormDialog.createProject')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
