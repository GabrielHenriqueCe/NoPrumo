import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

const EMPTY = {
  name: '',
  document: '',
  contactName: '',
  phone: '',
  email: '',
  city: '',
  state: '',
  notes: '',
}

function formatDocument(value) {
  const digits = value.replace(/\D/g, '')
  if (digits.length <= 11) {
    return digits
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

export function SupplierFormDialog({ open, supplier, onClose, onSubmit }) {
  const t = useT()
  const editing = Boolean(supplier)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(
      supplier
        ? {
            name: supplier.name ?? '',
            document: '', // Empty on edit: user leaves blank to keep existing document
            contactName: supplier.contactName ?? '',
            phone: supplier.phone ?? '',
            email: supplier.email ?? '',
            city: supplier.city ?? '',
            state: supplier.state ?? '',
            notes: supplier.notes ?? '',
          }
        : EMPTY,
    )
    setFieldErrors({})
    setFormError(null)
  }, [open, supplier])

  const update = (field) => (event) => {
    let value = event.target.value
    if (field === 'document') {
      value = formatDocument(value)
    }
    setForm((current) => ({ ...current, [field]: value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
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
        document: documentToSend,
        contactName: form.contactName.trim(),
        phone: form.phone.trim(),
        email: form.email.trim(),
        city: form.city.trim(),
        state: form.state.trim().toUpperCase(),
        notes: form.notes.trim(),
      })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('supplierFormDialog.couldNotSaveTheSupplier'))
    } finally {
      setBusy(false)
    }
  }

  const docPlaceholder = editing && supplier?.documentMasked
    ? t('supplierFormDialog.documentmaskedLeaveBlankToKeep', { documentMasked: supplier.documentMasked })
    : '00.000.000/0001-00'

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('supplierFormDialog.editSupplier') : t('supplierFormDialog.newSupplier')}
      description={t('supplierFormDialog.registerMaterialAndServiceSuppliers')}
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6 max-h-[80vh] overflow-y-auto">
        <TextField
          label={t('supplierFormDialog.supplierName')}
          name="name"
          placeholder={t('supplierFormDialog.eGBuildmaterialsInc')}
          required
          value={form.name}
          onChange={update('name')}
          error={fieldErrors.name}
        />

        <TextField
          label={t('supplierFormDialog.documentCpfCnpj')}
          name="document"
          placeholder={docPlaceholder}
          value={form.document}
          onChange={update('document')}
          error={fieldErrors.document || fieldErrors.documentMasked}
          hint={editing ? t('supplierFormDialog.leaveBlankToKeepThe') : undefined}
        />

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('supplierFormDialog.contactName')}
            name="contactName"
            placeholder={t('supplierFormDialog.eGCarlosSantos')}
            value={form.contactName}
            onChange={update('contactName')}
            error={fieldErrors.contactName}
          />

          <TextField
            label={t('supplierFormDialog.phone')}
            name="phone"
            placeholder="(00) 0000-0000"
            value={form.phone}
            onChange={update('phone')}
            error={fieldErrors.phone}
          />
        </div>

        <TextField
          label={t('supplierFormDialog.email')}
          name="email"
          type="email"
          placeholder={t('supplierFormDialog.salesSupplierCom')}
          value={form.email}
          onChange={update('email')}
          error={fieldErrors.email}
        />

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label={t('supplierFormDialog.city')}
            name="city"
            placeholder={t('supplierFormDialog.blumenau')}
            value={form.city}
            onChange={update('city')}
            error={fieldErrors.city}
          />

          <TextField
            label={t('supplierFormDialog.stateUf')}
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
            {editing ? t('common.saveChanges') : t('supplierFormDialog.createSupplier')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
