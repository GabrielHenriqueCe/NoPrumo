import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { usePreferences } from '../../preferences/preferencesContext'
import { useSession } from '../../providers/sessionContext'
import { Brand } from '../../shell/Brand'
import { Button } from '../../ui/Button'
import { SegmentedControl } from '../../ui/SegmentedControl'
import { TextField } from '../../ui/TextField'

/*
  Sign-in screen.

  There is no sign-up link and there will not be one: in this system an
  administrator creates the account and hands over a one-time password. For the
  same reason "forgot your password" does not send an email — it tells the
  person who can reset it.

  Nothing here decides whether the credentials are good — it sends them and
  shows what the API answered. The failure text stays vague on purpose, since
  naming which half was wrong tells a stranger that the other half is right.

  The language switch is here because whoever has never signed in has no other
  place to pick it.
*/

export function LoginScreen() {
  const { signIn, isAuthenticated, isRestoring } = useSession()
  const { t, language, setLanguage, resolvedTheme } = usePreferences()
  const navigate = useNavigate()
  const location = useLocation()

  const [form, setForm] = useState({ username: '', password: '' })
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)
  const [forgotOpen, setForgotOpen] = useState(false)

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
      else setFormError(error.status === 401 ? t('auth.invalid') : (error.message ?? t('auth.invalid')))
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="relative flex min-h-screen items-center justify-center bg-page p-6 text-ink">
      <div className="absolute top-5 right-6">
        <SegmentedControl
          name="login-language"
          label={t('auth.language')}
          size="sm"
          value={language}
          onChange={setLanguage}
          options={[
            { value: 'en', label: 'EN' },
            { value: 'pt-BR', label: 'PT' },
          ]}
        />
      </div>

      <form
        onSubmit={submit}
        noValidate
        className="w-full max-w-[420px] rounded-3xl border border-line bg-surface p-10 shadow-[0_12px_40px_rgb(0_0_0/0.06)]"
      >
        <Brand tone={resolvedTheme === 'dark' ? 'dark' : 'light'} size={64} />

        <h1 className="display mt-8 mb-6 text-[40px]">{t('auth.signIn')}</h1>

        <TextField
          label={t('auth.username')}
          name="username"
          placeholder="j.silva"
          autoComplete="username"
          autoFocus
          required
          value={form.username}
          onChange={update('username')}
          error={fieldErrors.username}
          className="mb-4"
        />

        <TextField
          label={t('auth.password')}
          name="password"
          type="password"
          autoComplete="current-password"
          required
          value={form.password}
          onChange={update('password')}
          error={fieldErrors.password}
          className="mb-4"
        />

        {formError && (
          <p role="alert" className="mb-4 rounded-2xl bg-danger-wash px-4 py-2.5 text-[13.5px] text-danger">
            {formError}
          </p>
        )}

        <Button type="submit" busy={busy} busyLabel={t('auth.signingIn')} className="w-full">
          {t('auth.signIn')}
        </Button>

        <button
          type="button"
          aria-expanded={forgotOpen}
          onClick={() => setForgotOpen((value) => !value)}
          className="mt-5 cursor-pointer text-sm font-semibold text-accent hover:underline"
        >
          {t('auth.forgot')}
        </button>

        <div aria-live="polite">
          {forgotOpen && (
            <div className="mt-3 rounded-2xl border border-line bg-sunken px-4 py-3">
              <p className="font-semibold">{t('auth.forgotTitle')}</p>
              <p className="mt-1 text-[13.5px] leading-relaxed text-muted">{t('auth.forgotText')}</p>
            </div>
          )}
        </div>
      </form>
    </div>
  )
}
