import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

/*
  Create or edit a user.

  No password field, in either mode. On create the API generates the temporary
  one and returns it; on edit the password is not the administrator's business
  — resetting is a separate, deliberate action. A form that can quietly set
  someone else's password is a form that will be used to do exactly that.

  The username cannot be changed after creation: it is what signs the audit
  trail, and renaming it would orphan the history.
*/

const EMPTY = { name: '', username: '', email: '', roleId: '' }

export function UserFormDialog({ open, user, roles, onClose, onSubmit }) {
  const editing = Boolean(user)

  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setForm(
      user
        ? {
            name: user.name ?? '',
            username: user.username ?? '',
            email: user.email ?? '',
            roleId: String(user.roleId ?? ''),
          }
        : EMPTY,
    )
    setFieldErrors({})
    setFormError(null)
  }, [open, user])

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
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
        username: form.username.trim(),
        email: form.email.trim(),
        roleId: Number(form.roleId),
      })
    } catch (error) {
      // The API is the one that validates. The form only shows where it hurt.
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the user.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? 'Edit user' : 'New user'}
      description={
        editing
          ? 'Changes take effect on the next sign-in.'
          : 'The system generates a temporary password to hand over.'
      }
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <TextField
          label="Full name"
          name="name"
          required
          value={form.name}
          onChange={update('name')}
          error={fieldErrors.name}
        />

        <TextField
          label="Username"
          name="username"
          required
          disabled={editing}
          value={form.username}
          onChange={update('username')}
          error={fieldErrors.username}
          hint={editing ? 'The username cannot be changed.' : 'How this person signs in.'}
        />

        <TextField
          label="Email"
          name="email"
          type="email"
          value={form.email}
          onChange={update('email')}
          error={fieldErrors.email}
        />

        <SelectField
          label="Role"
          name="roleId"
          required
          value={form.roleId}
          onChange={update('roleId')}
          error={fieldErrors.roleId}
          placeholder="Select a role…"
          options={roles.map((role) => ({ value: role.id, label: role.label }))}
        />

        {formError && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Saving…">
            {editing ? 'Save changes' : 'Create user'}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
