import { createContext, useContext } from 'react'

/*
  Context and hook live apart from the provider component so the file that
  exports the component exports nothing else — that is what keeps Vite's fast
  refresh working while you edit.
*/

export const ContainerContext = createContext(null)

/** Gives a screen the gateways it needs, already wired by container.js. */
export function useContainer() {
  const container = useContext(ContainerContext)

  if (!container) {
    throw new Error('useContainer must be used inside <ContainerProvider>')
  }

  return container
}
