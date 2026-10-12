import { useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { ProjectMaterialPurchasesPanel } from '../purchases/ProjectMaterialPurchasesPanel'

const quantity = new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 3 })

// The "Material" tab: what is on site now (José's balance) and what was asked for (Everson's requests).
export function ProjectMaterialPanel({ projectId }) {
  const { stockMovements } = useContainer()

  const [balances, setBalances] = useState([])
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    stockMovements
      .projectBalance(projectId, { signal: controller.signal })
      .then((result) => setBalances(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load the material on site.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [stockMovements, projectId])

  const onSite = balances.filter((balance) => balance.balance !== 0)

  return (
    <div className="flex flex-col gap-6 p-8">
      <section className="border border-line bg-white" aria-labelledby="material-on-site-heading">
        <h2 id="material-on-site-heading" className="border-b border-line-soft px-5 py-3.5 text-[15px] font-semibold">
          On site
        </h2>

        {loading && (
          <div className="p-4" role="status" aria-label="Loading material on site">
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="mb-2 h-10 animate-pulse bg-cream last:mb-0" />
            ))}
          </div>
        )}

        {!loading && loadError && <p className="px-5 py-8 text-center text-sm text-danger">{loadError}</p>}

        {!loading && !loadError && onSite.length === 0 && (
          <p className="px-5 py-8 text-center text-sm text-muted">No material on this project yet.</p>
        )}

        {!loading && !loadError && onSite.length > 0 && (
          <table className="w-full border-collapse text-sm">
            <caption className="sr-only">Material on site</caption>
            <thead>
              <tr className="border-b border-line bg-cream-soft text-left">
                <HeaderCell>Item</HeaderCell>
                <HeaderCell>Unit</HeaderCell>
                <HeaderCell align="right">Balance</HeaderCell>
              </tr>
            </thead>
            <tbody>
              {onSite.map((balance) => (
                <tr key={balance.stockItemId} className="border-b border-line-soft last:border-0">
                  <td className="px-4 py-3">{balance.name}</td>
                  <td className="px-4 py-3 text-muted">{balance.unit}</td>
                  <td className="px-4 py-3 text-right font-mono">{quantity.format(balance.balance)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <ProjectMaterialPurchasesPanel projectId={projectId} />
    </div>
  )
}

function HeaderCell({ children, align = 'left' }) {
  return (
    <th scope="col" className={`label px-4 py-3 font-normal ${align === 'right' ? 'text-right' : ''}`}>
      {children}
    </th>
  )
}
