import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

const EMPTY = {
  name: '',
  personType: 'company',
  documentMasked: '',
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
  if (personType === 'individual' || personType === 'pf') {
    // CPF: 000.000.000-00
    const limited = digits.slice(0, 11)
    return limited
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

export function ClientFormDialog({ open, client, onClose, onSubmit }) {
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
            documentMasked: client.documentMasked ?? '',
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
    if (field === 'documentMasked') {
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
      documentMasked: formatDocument(current.documentMasked, newType),
    }))
    setFieldErrors((current) => ({ ...current, personType: undefined, documentMasked: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    try {
      await onSubmit({
        name: form.name.trim(),
        personType: form.personType,
        documentMasked: form.documentMasked.trim(),
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
      else setFormError(error.message ?? 'Could not save the client.')
    } finally {
      setBusy(false)
    }
  }

  const isPf = form.personType === 'individual' || form.personType === 'pf'

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit client' : 'New client'}
      description="Register partner clients for projects."
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6 max-h-[80vh] overflow-y-auto">
        <TextField
          label="Client name"
          name="name"
          placeholder="e.g. Acme Construction Ltd"
          required
          value={form.name}
          onChange={update('name')}
          error={fieldErrors.name}
        />

        <div className="grid grid-cols-2 gap-4">
          <SelectField
            label="Person type"
            name="personType"
            required
            value={form.personType}
            onChange={handlePersonTypeChange}
            error={fieldErrors.personType}
            options={[
              { value: 'company', label: 'Company (PJ)' },
              { value: 'individual', label: 'Individual (PF)' },
            ]}
          />

          <TextField
            label={isPf ? 'CPF' : 'CNPJ'}
            name="documentMasked"
            placeholder={isPf ? '000.000.000-00' : '00.000.000/0001-00'}
            value={form.documentMasked}
            onChange={update('documentMasked')}
            error={fieldErrors.documentMasked}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Email"
            name="email"
            type="email"
            placeholder="contact@client.com"
            value={form.email}
            onChange={update('email')}
            error={fieldErrors.email}
          />

          <TextField
            label="Contact name"
            name="contactName"
            placeholder="e.g. Maria Silva"
            value={form.contactName}
            onChange={update('contactName')}
            error={fieldErrors.contactName}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <TextField
            label="Phone"
            name="phone"
            placeholder="(00) 0000-0000"
            value={form.phone}
            onChange={update('phone')}
            error={fieldErrors.phone}
          />

          <TextField
            label="Mobile"
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
              label="Street"
              name="street"
              placeholder="e.g. Av. Brasil"
              value={form.street}
              onChange={update('street')}
              error={fieldErrors.street}
            />
          </div>

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
            placeholder="Suite 201"
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
            label="Postal Code"
            name="postalCode"
            placeholder="89000-000"
            value={form.postalCode}
            onChange={update('postalCode')}
            error={fieldErrors.postalCode}
          />
        </div>

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
            {editing ? 'Save changes' : 'Create client'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
