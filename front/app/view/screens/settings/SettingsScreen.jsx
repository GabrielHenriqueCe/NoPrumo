import { Link } from 'react-router-dom'
import { useSession } from '../../providers/sessionContext'
import { SETTINGS_ITEMS } from '../../shell/settings'
import { useT } from '../../preferences/preferencesContext'

export function SettingsScreen() {
  const t = useT()
  const { can } = useSession()
  const items = SETTINGS_ITEMS.filter((item) => can(item.permission))

  return (
    <>
      <header className="border-b border-line bg-surface px-8 py-5">
        <p className="label">{t('settingsScreen.company')}</p>
        <h1 className="display mt-1 text-[38px] leading-none">{t('settingsScreen.settings')}</h1>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('settingsScreen.whatIsRegisteredOnceAnd')}
        </p>

        <ul className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {items.map((item) => (
            <li key={item.to}>
              <Link
                to={item.to}
                className="group flex h-full flex-col border border-line bg-surface px-5 py-4 transition-colors hover:border-gold"
              >
                <span className="flex items-center justify-between gap-3 font-semibold">
                  {item.label}
                  <span aria-hidden="true" className="text-muted transition-colors group-hover:text-bronze">
                    →
                  </span>
                </span>
                <span className="mt-1 text-[13px] leading-relaxed text-muted">{item.description}</span>
              </Link>
            </li>
          ))}
        </ul>
      </div>
    </>
  )
}
