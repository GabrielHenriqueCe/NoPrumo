import { useMemo } from 'react'
import { createContainer } from '../../container'
import { ContainerContext } from './containerContext'

/*
  Builds the wiring once and hands it down. Created inside React — instead of
  imported as a module singleton — so a test can mount the tree with different
  gateways without touching the real one.
*/
export function ContainerProvider({ container, children }) {
  const value = useMemo(() => container ?? createContainer(), [container])

  return <ContainerContext.Provider value={value}>{children}</ContainerContext.Provider>
}
