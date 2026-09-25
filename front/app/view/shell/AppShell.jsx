import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useSession } from '../providers/sessionContext'
import { Brand } from './Brand'
import { MENU } from './menu'

/*
  The frame every internal screen sits in: dark sidebar, light working area.

  It is a layout route — the screen renders through <Outlet /> — so the menu
  and the header exist once instead of being rebuilt by each page. What goes
  in the menu, and why it is grouped the way it is, lives in menu.js.
*/

export function AppShell() {
  const { user, signOut, can } = useSession()
  const navigate = useNavigate()

  const leave = () => {
    signOut()
    navigate('/login', { replace: true })
  }

  const groups = MENU.map((group) => ({
    ...group,
    items: group.items.filter((item) => !item.permission || can(item.permission)),
  })).filter((group) => group.items.length > 0)

  return (
    <div className="grid min-h-screen" style={{ gridTemplateColumns: '236px minmax(0,1fr)' }}>
      <aside className="flex flex-col bg-graphite">
        <div className="px-5 py-6">
          <Brand tone="dark" size={38} />
        </div>

        <nav className="flex flex-col">
          {groups.map((group) => (
            <div key={group.heading} className="mb-1">
              <p className="label px-[18px] pt-3 pb-1.5 text-muted-dark">{group.heading}</p>

              {group.items.map((item) => (
                <NavLink
                  key={item.to}
                  to={item.to}
                  className={({ isActive }) =>
                    [
                      'flex items-center gap-2 border-l-2 py-2.5 pr-[18px] pl-[18px] text-sm transition-colors',
                      isActive
                        ? 'border-gold bg-white/8 text-cream'
                        : 'border-transparent text-[#c6cdd4] hover:bg-white/5 hover:text-cream',
                    ].join(' ')
                  }
                >
                  <span aria-hidden="true" className="text-[11px] opacity-40">
                    └
                  </span>
                  {item.label}
                </NavLink>
              ))}
            </div>
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
