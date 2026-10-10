import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { PurchaseRequestFormDialog } from './PurchaseRequestFormDialog'
import { purchaseStatusLabel, purchaseStatusTone } from './purchaseStatus'

function formatDate(value) {
  if (!value) return '—'
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

export function ProjectMaterialPurchasesPanel({ projectId }) {
  const { purchaseRequests } = useContainer()

  const [requests, setRequests] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [formOpen, setFormOpen] = useState(false)

  const loadRequests = useCallback(() => {
    if (!projectId) return
    const controller = new AbortController()
    setLoading(true)
    setError(null)

    purchaseRequests
      .list({ projectId, size: 50 }, { signal: controller.signal })
      .then((data) => setRequests(data.items || []))
      .catch((err) => {
        if (!controller.signal.aborted) {
          setError(err.message || 'Erro ao carregar solicitações de compra.')
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [projectId, purchaseRequests])

  useEffect(() => {
    return loadRequests()
  }, [loadRequests])

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <div className="label">Suprimentos da Obra</div>
          <h2 className="text-lg font-semibold text-graphite">Solicitações de Compra</h2>
        </div>

        <Button variant="primary" onClick={() => setFormOpen(true)}>
          + Solicitar Materiais
        </Button>
      </div>

      {error && (
        <div className="border border-danger-soft bg-[#faefea] p-3 text-sm text-danger">
          {error}
        </div>
      )}

      {loading ? (
        <p className="py-8 text-center text-xs text-muted">Carregando solicitações...</p>
      ) : requests.length === 0 ? (
        <div className="border border-line bg-cream-soft p-8 text-center">
          <p className="text-sm text-muted">Nenhuma solicitação de compra cadastrada para esta obra.</p>
        </div>
      ) : (
        <div className="overflow-x-auto border border-line bg-white">
          <table className="w-full border-collapse text-sm">
            <thead>
              <tr className="border-b border-line bg-cream-soft text-left">
                <th className="label px-4 py-3 font-normal">Data</th>
                <th className="label px-4 py-3 font-normal">Itens / Quantidades</th>
                <th className="label px-4 py-3 font-normal">Precisa Até</th>
                <th className="label px-4 py-3 font-normal">Solicitado Por</th>
                <th className="label px-4 py-3 font-normal">Status</th>
                <th className="label px-4 py-3 font-normal">Observações</th>
              </tr>
            </thead>
            <tbody>
              {requests.map((req) => (
                <tr key={req.id} className="border-b border-line-soft last:border-0">
                  <td className="px-4 py-3 whitespace-nowrap">{formatDate(req.date)}</td>
                  <td className="px-4 py-3">
                    <div className="space-y-1">
                      {req.items?.map((item) => (
                        <div key={item.id} className="text-xs">
                          <span className="font-medium text-graphite">{item.stockItemName}:</span>{' '}
                          <span className="tabular-nums">
                            {item.requestedQuantity} {item.unit}
                          </span>
                          {item.fulfilledQuantity > 0 && (
                            <span className="ml-1 text-success">
                              (Atendido: {item.fulfilledQuantity} {item.unit})
                            </span>
                          )}
                        </div>
                      ))}
                    </div>
                  </td>
                  <td className="px-4 py-3 whitespace-nowrap">{formatDate(req.neededByDate)}</td>
                  <td className="px-4 py-3 text-xs text-muted">{req.requesterName || '—'}</td>
                  <td className="px-4 py-3">
                    <Badge tone={purchaseStatusTone(req.status)}>
                      {purchaseStatusLabel(req.status)}
                    </Badge>
                    {req.rejectionReason && (
                      <div className="mt-1 text-[11px] text-danger">
                        Motivo: {req.rejectionReason}
                      </div>
                    )}
                  </td>
                  <td className="px-4 py-3 text-xs text-muted">{req.notes || '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <PurchaseRequestFormDialog
        open={formOpen}
        projectId={projectId}
        onClose={() => setFormOpen(false)}
        onSubmitted={loadRequests}
      />
    </div>
  )
}
