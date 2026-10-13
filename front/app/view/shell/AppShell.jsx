import { NavLink, Outlet, useLocation } from 'react-router-dom'
import { useT } from '../preferences/preferencesContext'
import { useSession } from '../providers/sessionContext'
import { PillTabs } from '../ui/PillTabs'
import { PlumbMark } from './Brand'
import { MENU, SETTINGS_GROUP } from './menu'
import { RailIcon } from './RailIcon'
import { SETTINGS_ITEMS } from './settings'
import { UserMenu } from './UserMenu'

/*
  The frame every internal screen sits in: a dark icon rail on the left, the
  working area on the right.

  It is a layout route — the screen renders through <Outlet /> — so the rail
  exists once instead of being rebuilt by each page. A group with more than
  one screen shows them as tabs above the screen. What goes in each group lives
  in menu.js.
*/

const startsWith = (pathname, to) => pathname === to || pathname.startsWith(`${to}/`)

export function AppShell() {
  const { can } = useSession()
  const t = useT()
  const { pathname } = useLocation()

  const reachable = (group) => ({
    ...group,
    items: group.items.filter((item) => !item.permission || can(item.permission)),
  })

  const groups = MENU.map(reachable).filter((group) => group.items.length > 0)
  const settings = reachable(SETTINGS_GROUP)

  // The registers inside Settings are screens of their own, but they belong to the Settings icon.
  const inSettings = SETTINGS_ITEMS.some((item) => startsWith(pathname, item.to)) || startsWith(pathname, '/settings')
  const activeGroup = inSettings
    ? settings
    : groups.find((group) => group.items.some((item) => startsWith(pathname, item.to)))

  return (
    <div className="flex min-h-screen bg-page text-ink">
      <aside className="sticky top-0 flex h-screen w-[88px] flex-none flex-col items-center gap-1.5 bg-rail py-[18px] text-rail-text">
        <PlumbMark size={38} tone="dark" className="mb-4" />

        <nav aria-label={t('shell.main')} className="flex flex-col items-center gap-1.5">
          {groups.map((group) => (
            <RailLink key={group.key} group={group} active={group === activeGroup} label={t(`menu.groups.${group.key}`)} />
          ))}
        </nav>

        <div className="mt-auto flex flex-col items-center gap-3">
          {settings.items.length > 0 && (
            <RailLink group={settings} active={settings === activeGroup} label={t('menu.groups.settings')} />
          )}
          <UserMenu />
        </div>
      </aside>

      <main className="flex min-w-0 flex-1 flex-col">
        {activeGroup && activeGroup.items.length > 1 && (
          <div className="px-10 pt-6">
            <PillTabs
              label={t(`menu.groups.${activeGroup.key}`)}
              tabs={activeGroup.items.map((item) => ({ to: item.to, label: t(item.labelKey), end: false }))}
            />
          </div>
        )}
        <Outlet />
      </main>
    </div>
  )
}

function RailLink({ group, active, label }) {
  return (
    <NavLink
      to={group.items[0].to}
      aria-current={active ? 'page' : undefined}
      className={[
        'flex min-h-[60px] w-[68px] flex-col items-center justify-center gap-1 rounded-xl text-[11.5px] transition-colors',
        active ? 'bg-rail-active text-[#f3f1ec]' : 'hover:bg-rail-active/60 hover:text-[#f3f1ec]',
      ].join(' ')}
    >
      <RailIcon name={group.icon} active={active} />
      {label}
    </NavLink>
  )
}
