import { Link } from 'react-router-dom'
import { useSession } from '../../providers/sessionContext'
import { SETTINGS_ITEMS } from '../../shell/settings'

export function SettingsScreen() {
  const { can } = useSession()
  const items = SETTINGS_ITEMS.filter((item) => can(item.permission))

  return (
    <>
      <header className="border-b border-line bg-white px-8 py-5">
        <p className="label">Company</p>
        <h1 className="mt-1 text-xl font-semibold tracking-tight">Settings</h1>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          What is registered once and then only picked from a list. It rarely changes, so it lives here,
          out of the way of the daily work.
        </p>

        <ul className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {items.map((item) => (
            <li key={item.to}>
              <Link
                to={item.to}
                className="group flex h-full flex-col border border-line bg-white px-5 py-4 transition-colors hover:border-gold"
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
