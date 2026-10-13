import { NavLink } from 'react-router-dom'

/*
  The H tab bar: a white pill holding one pill per section, the current one
  filled. Used for the sections of a project and for the screens of a menu
  group. Each tab is a link, so the address says where you are.
*/
export function PillTabs({ label, tabs }) {
  return (
    <nav
      aria-label={label}
      className="flex max-w-full gap-1 self-start overflow-x-auto rounded-full border border-line bg-surface p-1"
    >
      {tabs.map((tab) => (
        <NavLink
          key={tab.to}
          to={tab.to}
          end={tab.end ?? true}
          className={({ isActive }) =>
            [
              'inline-flex min-h-10 items-center rounded-full px-5 font-display text-base tracking-[0.06em] whitespace-nowrap uppercase transition-colors',
              isActive ? 'bg-primary font-bold text-on-primary' : 'font-semibold text-ink hover:bg-sunken',
            ].join(' ')
          }
        >
          {tab.label}
        </NavLink>
      ))}
    </nav>
  )
}
