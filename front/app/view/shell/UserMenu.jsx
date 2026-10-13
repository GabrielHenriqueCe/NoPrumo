import { useEffect, useId, useRef, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { usePreferences } from '../preferences/preferencesContext'
import { useSession } from '../providers/sessionContext'
import { SegmentedControl } from '../ui/SegmentedControl'

/*
  The circle with the initials at the foot of the rail. Theme and language sit
  one click away because they get switched mid-demo; everything else about the
  account lives on the My account screen.
*/

function initialsOf(name) {
  const words = (name ?? '').trim().split(/\s+/).filter(Boolean)
  if (words.length === 0) return '?'
  const first = words[0][0]
  const last = words.length > 1 ? words[words.length - 1][0] : ''
  return (first + last).toUpperCase()
}

export function UserMenu() {
  const { user, signOut } = useSession()
  const { t, resolvedTheme, setTheme, language, setLanguage } = usePreferences()
  const navigate = useNavigate()
  const panelId = useId()
  const [open, setOpen] = useState(false)
  const wrapper = useRef(null)

  useEffect(() => {
    if (!open) return undefined

    const closeOnOutside = (event) => {
      if (!wrapper.current?.contains(event.target)) setOpen(false)
    }
    const closeOnEscape = (event) => {
      if (event.key === 'Escape') setOpen(false)
    }

    document.addEventListener('mousedown', closeOnOutside)
    document.addEventListener('keydown', closeOnEscape)
    return () => {
      document.removeEventListener('mousedown', closeOnOutside)
      document.removeEventListener('keydown', closeOnEscape)
    }
  }, [open])

  const leave = () => {
    signOut()
    navigate('/login', { replace: true })
  }

  return (
    <div ref={wrapper} className="relative">
      <button
        type="button"
        aria-haspopup="true"
        aria-expanded={open}
        aria-controls={panelId}
        aria-label={t('shell.accountMenu', { name: user?.name ?? '' })}
        onClick={() => setOpen((value) => !value)}
        className="flex h-12 w-12 cursor-pointer items-center justify-center rounded-full bg-rail-active font-display text-lg font-bold tracking-wide text-accent-bright hover:ring-2 hover:ring-accent-bright/50"
      >
        {initialsOf(user?.name)}
      </button>

      {open && (
        <div
          id={panelId}
          className="absolute bottom-0 left-[calc(100%+14px)] z-40 w-72 rounded-3xl border border-line bg-surface p-2 text-ink shadow-[0_12px_40px_rgb(0_0_0/0.25)]"
        >
          <div className="px-4 pt-3 pb-3">
            <p className="font-display text-xl leading-tight font-bold uppercase">{user?.name ?? '—'}</p>
            <p className="text-[13px] text-muted">{user?.roleLabel}</p>
          </div>

          <div className="flex flex-col gap-3 border-y border-line-soft px-4 py-4">
            <PreferenceRow label={t('account.theme')}>
              <SegmentedControl
                name="menu-theme"
                label={t('account.theme')}
                size="sm"
                value={resolvedTheme}
                onChange={setTheme}
                options={[
                  { value: 'light', label: t('account.light') },
                  { value: 'dark', label: t('account.dark') },
                ]}
              />
            </PreferenceRow>

            <PreferenceRow label={t('account.language')}>
              <SegmentedControl
                name="menu-language"
                label={t('account.language')}
                size="sm"
                value={language}
                onChange={setLanguage}
                options={[
                  { value: 'en', label: 'EN' },
                  { value: 'pt-BR', label: 'PT' },
                ]}
              />
            </PreferenceRow>
          </div>

          <div className="flex flex-col py-1">
            <Link
              to="/account"
              onClick={() => setOpen(false)}
              className="flex items-center justify-between rounded-2xl px-4 py-3 font-semibold hover:bg-sunken"
            >
              {t('account.title')} <span aria-hidden="true">→</span>
            </Link>
            <button
              type="button"
              onClick={leave}
              className="flex cursor-pointer items-center justify-between rounded-2xl px-4 py-3 text-left font-semibold hover:bg-sunken"
            >
              {t('shell.signOut')} <span aria-hidden="true">→</span>
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

function PreferenceRow({ label, children }) {
  return (
    <div className="flex items-center justify-between gap-3">
      <span className="text-sm text-muted">{label}</span>
      {children}
    </div>
  )
}
