import { createContext, useContext } from 'react'

export const PreferencesContext = createContext(null)

/** { theme, setTheme, language, setLanguage, t, format } — see PreferencesProvider. */
export function usePreferences() {
  const preferences = useContext(PreferencesContext)
  if (!preferences) throw new Error('usePreferences must be used inside PreferencesProvider')
  return preferences
}

/** The translator alone, for screens that only need the words. */
export function useT() {
  return usePreferences().t
}
