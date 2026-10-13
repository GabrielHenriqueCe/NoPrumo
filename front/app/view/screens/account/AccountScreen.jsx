import { usePreferences } from '../../preferences/preferencesContext'
import { useSession } from '../../providers/sessionContext'
import { ScreenHeader } from '../../ui/ScreenHeader'
import { SegmentedControl } from '../../ui/SegmentedControl'
import { PasswordForm } from './PasswordForm'

/*
  My account: who is signed in, the password and the preferences of this
  browser. Name, role and the linked employee are read-only here — changing
  them is an administrator's job, on the Users screen.
*/

export function AccountScreen() {
  const { user } = useSession()
  const { t, theme, setTheme, language, setLanguage } = usePreferences()

  return (
    <>
      <ScreenHeader eyebrow={t('account.eyebrow')} title={t('account.title')} />

      <div className="grid items-start gap-5 px-10 pt-6 pb-10 xl:grid-cols-2">
        <Card title={t('account.profile')} hint={t('account.profileHint')}>
          <dl className="grid grid-cols-[auto_minmax(0,1fr)] gap-x-8 gap-y-3">
            <Detail term={t('common.name')}>{user?.name}</Detail>
            <Detail term={t('account.username')}>
              <span className="font-mono text-[14px]">{user?.username}</span>
            </Detail>
            <Detail term={t('account.email')}>{user?.email}</Detail>
            <Detail term={t('account.role')}>{user?.roleLabel}</Detail>
            <Detail term={t('account.employee')}>
              {user?.employeeId ? t('account.linked') : t('account.notLinked')}
            </Detail>
          </dl>
        </Card>

        <Card title={t('account.preferences')} hint={t('account.preferencesHint')}>
          <div className="flex flex-col gap-6">
            <Preference label={t('account.theme')} hint={t('account.themeHint')}>
              <SegmentedControl
                name="account-theme"
                label={t('account.theme')}
                value={theme}
                onChange={setTheme}
                options={[
                  { value: 'light', label: t('account.light') },
                  { value: 'dark', label: t('account.dark') },
                  { value: 'system', label: t('account.system') },
                ]}
              />
            </Preference>

            <Preference label={t('account.language')} hint={t('account.languageHint')}>
              <SegmentedControl
                name="account-language"
                label={t('account.language')}
                value={language}
                onChange={setLanguage}
                options={[
                  { value: 'en', label: t('account.english') },
                  { value: 'pt-BR', label: t('account.portuguese') },
                ]}
              />
            </Preference>
          </div>
        </Card>

        <Card title={t('account.password')} hint={t('account.passwordHint')}>
          <PasswordForm />
        </Card>
      </div>
    </>
  )
}

function Card({ title, hint, children }) {
  return (
    <section className="rounded-3xl border border-line bg-surface p-7">
      <h2 className="display text-[24px]">{title}</h2>
      {hint && <p className="mt-1 mb-5 text-[14px] text-muted">{hint}</p>}
      {children}
    </section>
  )
}

function Detail({ term, children }) {
  return (
    <>
      <dt className="text-muted">{term}</dt>
      <dd className="min-w-0 font-medium break-words">{children || '—'}</dd>
    </>
  )
}

function Preference({ label, hint, children }) {
  return (
    <div>
      <p className="mb-2 font-semibold">{label}</p>
      {children}
      <p className="mt-2 text-[13px] text-muted">{hint}</p>
    </div>
  )
}
