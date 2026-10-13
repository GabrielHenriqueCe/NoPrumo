import { useNavigate } from 'react-router-dom'
import { usePreferences } from '../../preferences/preferencesContext'
import { useSession } from '../../providers/sessionContext'
import { Brand } from '../../shell/Brand'
import { PasswordForm } from '../account/PasswordForm'

/*
  The forced change on first sign-in: RequireSession keeps the account here
  until the provisional password is replaced. No menu, because the person has
  not finished signing in. Changing the password by choice is on My account.
*/

export function ChangePasswordScreen() {
  const { signOut, user, mustChangePassword } = useSession()
  const { t, resolvedTheme } = usePreferences()
  const navigate = useNavigate()

  const leave = () => {
    signOut()
    navigate('/login', { replace: true })
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-page p-6 text-ink">
      <div className="w-full max-w-[440px] rounded-3xl border border-line bg-surface p-10 shadow-[0_12px_40px_rgb(0_0_0/0.06)]">
        <Brand tone={resolvedTheme === 'dark' ? 'dark' : 'light'} size={56} />

        <p className="label mt-8">{user?.name}</p>
        <h1 className="display mt-1 text-[34px]">{t('auth.changeTitle')}</h1>
        <p className="mt-2 mb-6 leading-relaxed text-muted">{t('auth.changeIntro')}</p>

        <PasswordForm
          autoFocus
          onChanged={() => navigate(mustChangePassword ? '/' : '/account', { replace: true })}
        />

        <button
          type="button"
          onClick={mustChangePassword ? leave : () => navigate('/account')}
          className="mt-6 cursor-pointer text-sm font-semibold text-muted hover:text-ink"
        >
          {mustChangePassword ? t('shell.signOut') : t('common.cancel')}
        </button>
      </div>
    </div>
  )
}
