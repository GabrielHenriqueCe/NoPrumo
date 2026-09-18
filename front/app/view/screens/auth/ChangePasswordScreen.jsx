import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useSession } from '../../providers/sessionContext'
import { Brand } from '../../shell/Brand'
import { Button } from '../../ui/Button'
import { TextField } from '../../ui/TextField'

/*
  Forced password change on first sign-in.

  An account created by an administrator starts with a password that was read
  out loud over the phone or written on a slip of paper — it is shared by
  construction. RequireSession keeps the account on this screen until it is
  replaced, so the provisional one never survives the first session.

  The confirmation field never reaches the API: it exists to catch a typo in a
  value nobody can see while typing.
*/

const MIN_LENGTH = 8

export function ChangePasswordScreen() {
  const { changePassword, signOut, user } = useSession()
  const navigate = useNavigate()

  const [form, setForm] = useState({ currentPassword: '', newPassword: '', confirmation: '' })
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const leave = () => {
    signOut()
    navigate('/login', { replace: true })
  }

  const submit = async (event) => {
    event.preventDefault()
    setFormError(null)

    if (form.newPassword.length < MIN_LENGTH) {
      setFieldErrors({ newPassword: `Use at least ${MIN_LENGTH} characters.` })
      return
    }

    if (form.newPassword !== form.confirmation) {
      setFieldErrors({ confirmation: 'The two passwords do not match.' })
      return
    }

    setBusy(true)
    setFieldErrors({})

    try {
      await changePassword(form.currentPassword, form.newPassword)
      navigate('/users', { replace: true })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not change the password.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-graphite p-6">
      <form
        onSubmit={submit}
        noValidate
        className="w-full max-w-[390px] border border-line-dark bg-graphite-soft p-10"
      >
        <Brand tone="dark" size={68} />

        <p className="label mt-8 text-muted-dark">Choose a new password</p>

        <p className="mt-2 mb-5 text-[12.5px] leading-relaxed text-muted-dark">
          {user?.name ? `${user.name}, your` : 'Your'} account still uses the password an
          administrator handed over. Replace it to continue.
        </p>

        <TextField
          tone="dark"
          label="Current password"
          name="currentPassword"
          type="password"
          placeholder="The password you just signed in with"
          autoComplete="current-password"
          autoFocus
          required
          value={form.currentPassword}
          onChange={update('currentPassword')}
          error={fieldErrors.currentPassword}
          className="mb-4"
        />

        <TextField
          tone="dark"
          label="New password"
          name="newPassword"
          type="password"
          placeholder={`At least ${MIN_LENGTH} characters`}
          autoComplete="new-password"
          required
          value={form.newPassword}
          onChange={update('newPassword')}
          error={fieldErrors.newPassword}
          hint={`At least ${MIN_LENGTH} characters.`}
          className="mb-4"
        />

        <TextField
          tone="dark"
          label="Confirm the new password"
          name="confirmation"
          type="password"
          placeholder="Type the new password again"
          autoComplete="new-password"
          required
          value={form.confirmation}
          onChange={update('confirmation')}
          error={fieldErrors.confirmation}
          className="mb-4"
        />

        {formError && (
          <p
            role="alert"
            className="mb-4 border border-danger-soft/40 bg-danger/10 px-3 py-2 text-[12.5px] text-danger-soft"
          >
            {formError}
          </p>
        )}

        <Button type="submit" variant="gold" busy={busy} busyLabel="Saving…" className="w-full">
          Save and continue
        </Button>

        <button
          type="button"
          onClick={leave}
          className="label mt-5 block w-full cursor-pointer text-center text-muted-dark hover:text-cream"
        >
          Sign out
        </button>
      </form>
    </div>
  )
}
