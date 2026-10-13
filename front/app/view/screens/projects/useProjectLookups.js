import { useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'

// 100 is the API's page cap.
const LOOKUP_SIZE = 100

/*
  The clients and supervisors the project form picks from. Loaded only for
  whoever can open the form: those endpoints refuse everyone else. A failed
  lookup leaves its select empty; the screen still works.
*/
export function useProjectLookups(enabled) {
  const { clients, employees } = useContainer()
  const [clientOptions, setClientOptions] = useState([])
  const [supervisorOptions, setSupervisorOptions] = useState([])

  useEffect(() => {
    if (!enabled) return undefined

    const controller = new AbortController()
    const options = { signal: controller.signal }
    const activeOptions = (items) =>
      items.filter((item) => item.active).map((item) => ({ value: item.id, label: item.name }))

    clients
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setClientOptions(activeOptions(result.items)))
      .catch(() => {})

    employees
      .list({ page: 1, size: LOOKUP_SIZE }, options)
      .then((result) => setSupervisorOptions(activeOptions(result.items)))
      .catch(() => {})

    return () => controller.abort()
  }, [clients, employees, enabled])

  return { clientOptions, supervisorOptions }
}
