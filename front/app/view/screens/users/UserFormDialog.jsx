import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

/*
  Create or edit a user.

  No password field, in either mode. On create the API generates the temporary
  one and returns it; on edit the password is not the administrator's business
  — resetting is a separate, deliberate action. A form that can quietly set
  someone else's password is a form that will be used to do exactly that.

  The username cannot be changed after creation: it is what signs the audit
  trail, and renaming it would orphan the history.
*/

const EMPTY = { name: '', username: '', email: '', roleId: '', employeeId: '' }

export function UserFormDialog({ open, user, roles, employeeOptions, onClose, onSubmit }) {
  const t = useT()
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
            employeeId: user.employeeId != null ? String(user.employeeId) : '',
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
        employeeId: form.employeeId ? Number(form.employeeId) : null,
      })
    } catch (error) {
      // The API is the one that validates. The form only shows where it hurt.
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('userFormDialog.couldNotSaveTheUser'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title={editing ? t('userFormDialog.editUser') : t('userFormDialog.newUser')}
      description={
        editing
          ? t('userFormDialog.changesTakeEffectOnThe')
          : t('userFormDialog.theSystemGeneratesATemporary')
      }
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <TextField
          label={t('userFormDialog.fullName')}
          name="name"
          placeholder={t('userFormDialog.eGJoODa')}
          required
          value={form.name}
          onChange={update('name')}
          error={fieldErrors.name}
        />

        <TextField
          label={t('userFormDialog.username')}
          name="username"
          placeholder={t('userFormDialog.eGJSilva')}
          required
          disabled={editing}
          value={form.username}
          onChange={update('username')}
          error={fieldErrors.username}
          hint={editing ? t('userFormDialog.theUsernameCannotBeChanged') : t('userFormDialog.howThisPersonSignsIn')}
        />

        <TextField
          label={t('userFormDialog.email')}
          name="email"
          type="email"
          placeholder={t('userFormDialog.nameCompanyCom')}
          value={form.email}
          onChange={update('email')}
          error={fieldErrors.email}
        />

        <SelectField
          label={t('userFormDialog.role')}
          name="roleId"
          required
          value={form.roleId}
          onChange={update('roleId')}
          error={fieldErrors.roleId}
          placeholder={t('userFormDialog.selectARole')}
          options={roles.map((role) => ({ value: role.id, label: role.label }))}
        />

        <SelectField
          label={t('userFormDialog.employee')}
          name="employeeId"
          value={form.employeeId}
          onChange={update('employeeId')}
          error={fieldErrors.employeeId}
          placeholder={employeeOptions.length ? t('userFormDialog.notLinkedToAnEmployee') : t('userFormDialog.noActiveEmployeeRegistered')}
          options={employeeOptions}
        />
        <p className="-mt-2 text-[12.5px] text-muted">
          {t('userFormDialog.aForemanSeesOnlyThe')}
        </p>

        {formError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {formError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
            {editing ? t('common.saveChanges') : t('userFormDialog.createUser')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
