import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useSession } from '../../providers/sessionContext'
import { Brand } from '../../shell/Brand'
import { Button } from '../../ui/Button'
import { TextField } from '../../ui/TextField'

/*
  Sign-in screen.

  There is no sign-up link and there will not be one: in this system an
  administrator creates the account and hands over a one-time password. The
  screen says so out loud, because the first thing a new user looks for is the
  button that does not exist.

  Nothing here decides whether the credentials are good — it sends them and
  shows what the API answered. The failure text stays vague on purpose
  ("Invalid username or password"), since naming which half was wrong tells a
  stranger that the other half is right.
*/

export function LoginScreen() {
  const { signIn, isAuthenticated, isRestoring } = useSession()
  const navigate = useNavigate()
  const location = useLocation()

  const [form, setForm] = useState({ username: '', password: '' })
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  if (isAuthenticated && !isRestoring) {
    return <Navigate to={location.state?.from ?? '/'} replace />
  }

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    // Clearing as they type means the error never outlives the mistake.
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setFormError(null)
    setFieldErrors({})

    try {
      await signIn({ username: form.username.trim(), password: form.password })
      navigate(location.state?.from ?? '/', { replace: true })
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not sign in.')
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

        <p className="label mt-8 mb-5 text-muted-dark">Sign in</p>

        <TextField
          tone="dark"
          label="Username"
          name="username"
          placeholder="e.g. j.silva"
          autoComplete="username"
          autoFocus
          required
          value={form.username}
          onChange={update('username')}
          error={fieldErrors.username}
          className="mb-4"
        />

        <TextField
          tone="dark"
          label="Password"
          name="password"
          type="password"
          placeholder="Your password"
          autoComplete="current-password"
          required
          value={form.password}
          onChange={update('password')}
          error={fieldErrors.password}
          className="mb-4"
        />

        {formError && (
          <p role="alert" className="mb-4 border border-danger-soft/40 bg-danger/10 px-3 py-2 text-[12.5px] text-danger-soft">
            {formError}
          </p>
        )}

        <Button type="submit" variant="gold" busy={busy} busyLabel="Signing in…" className="w-full">
          Sign in
        </Button>

        <p className="mt-6 text-[12px] leading-relaxed text-muted-dark">
          No self sign-up: accounts are created by an administrator, who hands over a temporary
          password.
        </p>
      </form>
    </div>
  )
}
