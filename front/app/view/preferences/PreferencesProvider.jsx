import { useEffect, useMemo, useState } from 'react'
import { createFormatter } from '../i18n/format'
import { createTranslator } from '../i18n/translate'
import { PreferencesContext } from './preferencesContext'

/*
  Theme and language belong to this browser, not to the account: they live in
  localStorage under the same keys the script in index.html reads before the
  first paint. Storage can be blocked (private window), so every access is
  guarded and the app falls back to the defaults.
*/

const THEME_KEY = 'noprumo.theme'
const LANGUAGE_KEY = 'noprumo.language'

const THEMES = ['light', 'dark', 'system']
const LANGUAGES = ['en', 'pt-BR']

function read(key, allowed, fallback) {
  try {
    const value = localStorage.getItem(key)
    return allowed.includes(value) ? value : fallback
  } catch {
    return fallback
  }
}

function write(key, value) {
  try {
    localStorage.setItem(key, value)
  } catch {
    // Not saved: the choice still holds until the tab closes.
  }
}

const darkQuery = () => window.matchMedia('(prefers-color-scheme: dark)')

export function PreferencesProvider({ children }) {
  const [theme, setThemeState] = useState(() => read(THEME_KEY, THEMES, 'system'))
  const [language, setLanguageState] = useState(() => read(LANGUAGE_KEY, LANGUAGES, 'en'))
  const [systemDark, setSystemDark] = useState(() => darkQuery().matches)

  useEffect(() => {
    const query = darkQuery()
    const follow = (event) => setSystemDark(event.matches)
    query.addEventListener('change', follow)
    return () => query.removeEventListener('change', follow)
  }, [])

  const resolvedTheme = theme === 'system' ? (systemDark ? 'dark' : 'light') : theme

  useEffect(() => {
    document.documentElement.dataset.theme = resolvedTheme
  }, [resolvedTheme])

  useEffect(() => {
    document.documentElement.lang = language
  }, [language])

  const value = useMemo(
    () => ({
      theme,
      resolvedTheme,
      setTheme: (next) => {
        setThemeState(next)
        write(THEME_KEY, next)
      },
      language,
      setLanguage: (next) => {
        setLanguageState(next)
        write(LANGUAGE_KEY, next)
      },
      t: createTranslator(language),
      format: createFormatter(language),
    }),
    [theme, resolvedTheme, language],
  )

  return <PreferencesContext.Provider value={value}>{children}</PreferencesContext.Provider>
}
