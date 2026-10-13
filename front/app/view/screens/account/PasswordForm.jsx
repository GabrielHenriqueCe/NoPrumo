import { useState } from 'react'
import { useT } from '../../preferences/preferencesContext'
import { useSession } from '../../providers/sessionContext'
import { Button } from '../../ui/Button'
import { TextField } from '../../ui/TextField'

/*
  Current password, new password, confirmation. Used by My account and by the
  forced change on first sign-in.

  The confirmation never reaches the API: it exists to catch a typo in a value
  nobody can see while typing. The minimum length is the API's too; checking
  it here only saves a round trip.
*/

const MIN_LENGTH = 8

export function PasswordForm({ submitLabel, onChanged, autoFocus = false }) {
  const { changePassword } = useSession()
  const t = useT()

  const [form, setForm] = useState({ currentPassword: '', newPassword: '', confirmation: '' })
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)
  const [done, setDone] = useState(false)

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
    setDone(false)
  }

  const submit = async (event) => {
    event.preventDefault()
    setFormError(null)

    if (form.newPassword.length < MIN_LENGTH) {
      setFieldErrors({ newPassword: t('account.passwordHint') })
      return
    }

    if (form.newPassword !== form.confirmation) {
      setFieldErrors({ confirmation: t('auth.mismatch') })
      return
    }

    setBusy(true)
    setFieldErrors({})

    try {
      await changePassword(form.currentPassword, form.newPassword)
      setForm({ currentPassword: '', newPassword: '', confirmation: '' })
      setDone(true)
      onChanged?.()
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('common.couldNotSave'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <form onSubmit={submit} noValidate className="flex flex-col gap-4">
      <TextField
        label={t('auth.currentPassword')}
        name="currentPassword"
        type="password"
        autoComplete="current-password"
        autoFocus={autoFocus}
        required
        value={form.currentPassword}
        onChange={update('currentPassword')}
        error={fieldErrors.currentPassword}
      />

      <TextField
        label={t('auth.newPassword')}
        name="newPassword"
        type="password"
        autoComplete="new-password"
        required
        value={form.newPassword}
        onChange={update('newPassword')}
        error={fieldErrors.newPassword}
        hint={t('account.passwordHint')}
      />

      <TextField
        label={t('auth.confirmPassword')}
        name="confirmation"
        type="password"
        autoComplete="new-password"
        required
        value={form.confirmation}
        onChange={update('confirmation')}
        error={fieldErrors.confirmation}
      />

      <div aria-live="polite">
        {formError && <p className="rounded-2xl bg-danger-wash px-4 py-2.5 text-[13.5px] text-danger">{formError}</p>}
        {done && <p className="rounded-2xl bg-success-wash px-4 py-2.5 text-[13.5px] text-success">{t('auth.changed')}</p>}
      </div>

      <Button type="submit" busy={busy} busyLabel={t('common.saving')} className="self-start">
        {submitLabel ?? t('auth.changeSubmit')}
      </Button>
    </form>
  )
}
