import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

const EMPTY = {
  name: '',
  personType: 'company',
  document: '',
  email: '',
  contactName: '',
  phone: '',
  mobile: '',
  street: '',
  number: '',
  complement: '',
  district: '',
  city: '',
  state: '',
  postalCode: '',
  notes: '',
}

function formatDocument(value, personType) {
  const digits = value.replace(/\D/g, '')
  if (personType === 'individual') {
    const limited = digits.slice(0, 11)
    return limited
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2')
  } else {
    const limited = digits.slice(0, 14)
    return limited
      .replace(/^(\d{2})(\d)/, '$1.$2')
      .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
      .replace(/\.(\d{3})(\d)/, '.$1/$2')
      .replace(/(\d{4})(\d{1,2})$/, '$1-$2')
  }
}

export function ClientFormDialog({ open, client, onClose, onSubmit }) {
  const t = useT()
  const editing = Boolean(client)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(
      client
        ? {
            name: client.name ?? '',
            personType: client.personType ?? 'company',
            document: '', // Empty on edit: user leaves blank to keep existing document
            email: client.email ?? '',
            contactName: client.contactName ?? '',
            phone: client.phone ?? '',
            mobile: client.mobile ?? '',
            street: client.street ?? '',
            number: client.number ?? '',
            complement: client.complement ?? '',
            district: client.district ?? '',
            city: client.city ?? '',
            state: client.state ?? '',
            postalCode: client.postalCode ?? '',
            notes: client.notes ?? '',
          }
        : EMPTY,
    )
    setFieldErrors({})
    setFormError(null)
  }, [open, client])

  const update = (field) => (event) => {
    let value = event.target.value
    if (field === 'document') {
      value = formatDocument(value, form.personType)
    }
    setForm((current) => ({ ...current, [field]: value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const handlePersonTypeChange = (event) => {
    const newType = event.target.value
    setForm((current) => ({
      ...current,
      personType: newType,
      document: formatDocument(current.document, newType),
    }))
    setFieldErrors((current) => ({ ...current, personType: undefined, document: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    const documentToSend = form.document.trim() ? form.document.trim() : null

    try {
      await onSubmit({
        name: form.name.trim(),
        personType: form.personType,
        document: documentToSend,
        email: form.email.trim(),
        contactName: form.contactName.trim(),
        phone: form.phone.trim(),
        mobile: form.mobile.trim(),
        street: form.street.trim(),
        number: form.number.trim(),
        complement: form.complement.trim(),
        district: form.district.trim(),
        city: form.city.trim(),
        state: form.state.trim().toUpperCase(),
        postalCode: form.postalCode.trim(),
        notes: form.notes.trim(),
      })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('clientFormDialog.couldNotSaveTheClient'))
    } finally {
      setBusy(false)
    }
  }

  const isIndividual = form.personType === 'individual'

  const docPlaceholder = editing && client?.documentMasked
    ? t('clientFormDialog.documentmaskedLeaveBlankToKeep', { documentMasked: client.documentMasked })
    : isIndividual
    ? '000.000.000-00'
    : '00.000.000/0001-00'

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('clientFormDialog.editClient') : t('clientFormDialog.newClient')}
      description={t('clientFormDialog.registerPartnerClientsForProjects')}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6 max-h-[80vh] overflow-y-auto">
        <TextField
          label={t('clientFormDialog.clientName')}
          name="name"
          placeholder={t('clientFormDialog.eGAcmeConstructionLtd')}
          required
          value={form.name}
          onChange={update('name')}
          error={fieldErrors.name}
        />

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label={t('clientFormDialog.personType')}
            name="personType"
            required
            value={form.personType}
            onChange={handlePersonTypeChange}
            error={fieldErrors.personType}
            options={[
              { value: 'company', label: t('clientFormDialog.companyPj') },
              { value: 'individual', label: t('clientFormDialog.individualPf') },
            ]}
          />

          <TextField
            label={isIndividual ? 'CPF' : 'CNPJ'}
            name="document"
            placeholder={docPlaceholder}
            value={form.document}
            onChange={update('document')}
            error={fieldErrors.document || fieldErrors.documentMasked}
            hint={editing ? t('clientFormDialog.leaveBlankToKeepThe') : undefined}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('clientFormDialog.email')}
            name="email"
            type="email"
            placeholder={t('clientFormDialog.contactClientCom')}
            value={form.email}
            onChange={update('email')}
            error={fieldErrors.email}
          />

          <TextField
            label={t('clientFormDialog.contactName')}
            name="contactName"
            placeholder={t('clientFormDialog.eGMariaSilva')}
            value={form.contactName}
            onChange={update('contactName')}
            error={fieldErrors.contactName}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('clientFormDialog.phone')}
            name="phone"
            placeholder="(00) 0000-0000"
            value={form.phone}
            onChange={update('phone')}
            error={fieldErrors.phone}
          />

          <TextField
            label={t('clientFormDialog.mobile')}
            name="mobile"
            placeholder="(00) 90000-0000"
            value={form.mobile}
            onChange={update('mobile')}
            error={fieldErrors.mobile}
          />
        </div>

        <div className="grid grid-cols-3 gap-4">
          <div className="col-span-2">
            <TextField
              label={t('clientFormDialog.street')}
              name="street"
              placeholder={t('clientFormDialog.eGAvBrasil')}
              value={form.street}
              onChange={update('street')}
              error={fieldErrors.street}
            />
          </div>

          <TextField
            label={t('clientFormDialog.number')}
            name="number"
            placeholder="100"
            value={form.number}
            onChange={update('number')}
            error={fieldErrors.number}
          />
        </div>

        <div className="grid grid-cols-3 gap-4">
          <TextField
            label={t('clientFormDialog.complement')}
            name="complement"
            placeholder={t('clientFormDialog.suite201')}
            value={form.complement}
            onChange={update('complement')}
            error={fieldErrors.complement}
          />

          <TextField
            label={t('clientFormDialog.district')}
            name="district"
            placeholder={t('clientFormDialog.centro')}
            value={form.district}
            onChange={update('district')}
            error={fieldErrors.district}
          />

          <TextField
            label={t('clientFormDialog.postalCode')}
            name="postalCode"
            placeholder="89000-000"
            value={form.postalCode}
            onChange={update('postalCode')}
            error={fieldErrors.postalCode}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('clientFormDialog.city')}
            name="city"
            placeholder={t('clientFormDialog.blumenau')}
            value={form.city}
            onChange={update('city')}
            error={fieldErrors.city}
          />

          <TextField
            label={t('clientFormDialog.stateUf')}
            name="state"
            placeholder="SC"
            value={form.state}
            onChange={update('state')}
            error={fieldErrors.state}
          />
        </div>

        {formError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('clientFormDialog.createClient')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
