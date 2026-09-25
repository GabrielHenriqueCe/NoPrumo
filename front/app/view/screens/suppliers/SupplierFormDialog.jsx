import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'

const EMPTY = {
  name: '',
  documentMasked: '',
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
    // CPF: 000.000.000-00
    return digits
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2')
  } else {
    // CNPJ: 00.000.000/0001-00
    const limited = digits.slice(0, 14)
    return limited
      .replace(/^(\d{2})(\d)/, '$1.$2')
      .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
      .replace(/\.(\d{3})(\d)/, '.$1/$2')
      .replace(/(\d{4})(\d{1,2})$/, '$1-$2')
  }
}

export function SupplierFormDialog({ open, supplier, onClose, onSubmit }) {
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
            documentMasked: supplier.documentMasked ?? '',
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
    if (field === 'documentMasked') {
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

    try {
      await onSubmit({
        name: form.name.trim(),
        documentMasked: form.documentMasked.trim(),
        contactName: form.contactName.trim(),
        phone: form.phone.trim(),
        email: form.email.trim(),
        city: form.city.trim(),
        state: form.state.trim().toUpperCase(),
        notes: form.notes.trim(),
      })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the supplier.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit supplier' : 'New supplier'}
      description="Register material and service suppliers."
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6 max-h-[80vh] overflow-y-auto">
        <TextField
          label="Supplier name"
          name="name"
          placeholder="e.g. BuildMaterials Inc"
          required
          value={form.name}
          onChange={update('name')}
          error={fieldErrors.name}
        />

        <TextField
          label="Document (CPF / CNPJ)"
          name="documentMasked"
          placeholder="00.000.000/0001-00"
          value={form.documentMasked}
          onChange={update('documentMasked')}
          error={fieldErrors.documentMasked}
        />

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Contact name"
            name="contactName"
            placeholder="e.g. Carlos Santos"
            value={form.contactName}
            onChange={update('contactName')}
            error={fieldErrors.contactName}
          />

          <TextField
            label="Phone"
            name="phone"
            placeholder="(00) 0000-0000"
            value={form.phone}
            onChange={update('phone')}
            error={fieldErrors.phone}
          />
        </div>

        <TextField
          label="Email"
          name="email"
          type="email"
          placeholder="sales@supplier.com"
          value={form.email}
          onChange={update('email')}
          error={fieldErrors.email}
        />

        <div className="grid grid-cols-2 gap-4">
          <TextField
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
            value={form.state}
            onChange={update('state')}
            error={fieldErrors.state}
          />
        </div>

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…">
            {editing ? 'Save changes' : 'Create supplier'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
