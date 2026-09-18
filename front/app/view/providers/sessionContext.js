import { createContext, useContext } from 'react'

export const SessionContext = createContext(null)

/** Who is signed in, and the two actions that change that. */
export function useSession() {
  const session = useContext(SessionContext)

  if (!session) {
    throw new Error('useSession must be used inside <SessionProvider>')
  }

  return session
}
