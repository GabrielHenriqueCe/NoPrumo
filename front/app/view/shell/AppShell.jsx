import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useSession } from '../providers/sessionContext'
import { Brand } from './Brand'

/*
  The frame every internal screen sits in: dark sidebar, light working area.

  It is a layout route — the screen renders through <Outlet /> — so the menu
  and the header exist once instead of being rebuilt by each page.

  Menu items are filtered by permission. Only one item exists today; the array
  is where the rest go as the screens are built.
*/

const MENU = [{ to: '/users', label: 'Users', permission: 'manage_users' }]

export function AppShell() {
  const { user, signOut, can } = useSession()
  const navigate = useNavigate()

  const leave = () => {
    signOut()
    navigate('/login', { replace: true })
  }

  const items = MENU.filter((item) => !item.permission || can(item.permission))

  return (
    <div className="grid min-h-screen" style={{ gridTemplateColumns: '236px minmax(0,1fr)' }}>
      <aside className="flex flex-col bg-graphite">
        <div className="px-5 py-6">
          <Brand tone="dark" size={38} />
        </div>

        <nav className="flex flex-col">
          {items.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                [
                  'border-l-2 px-[18px] py-2.5 text-sm transition-colors',
                  isActive
                    ? 'border-gold bg-white/8 text-cream'
                    : 'border-transparent text-[#c6cdd4] hover:bg-white/5 hover:text-cream',
                ].join(' ')
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="mt-auto border-t border-line-dark px-5 py-5">
          <p className="label mb-1.5 text-muted-dark">Signed in</p>
          <p className="text-[13.5px] text-[#c6cdd4]">{user?.name ?? '—'}</p>
          <p className="text-[11.5px] text-muted-dark">{user?.roleLabel ?? ''}</p>

          <button
            type="button"
            onClick={leave}
            className="label mt-3 cursor-pointer text-muted-dark hover:text-cream"
          >
            Sign out →
          </button>
        </div>
      </aside>

      <main className="min-w-0">
        <Outlet />
      </main>
    </div>
  )
}
