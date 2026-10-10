import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { DecidePurchaseDialog } from './DecidePurchaseDialog'
import { PurchaseRequestFormDialog } from './PurchaseRequestFormDialog'
import { purchaseStatusLabel, purchaseStatusTone } from './purchaseStatus'
import { RegisterPurchaseDialog } from './RegisterPurchaseDialog'

const PAGE_SIZE = 10
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

function formatDate(value) {
  if (!value) return '—'
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

export function PurchasesScreen() {
  const { purchaseRequests, projects } = useContainer()

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [selectedStatus, setSelectedStatus] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [projectOptions, setProjectOptions] = useState([])
  const [formOpen, setFormOpen] = useState(false)
  const [decidingRequest, setDecidingRequest] = useState(null)
  const [purchasingRequest, setPurchasingRequest] = useState(null)

  const reload = useCallback(() => setReloadToken((v) => v + 1), [])

  useEffect(() => {
    const controller = new AbortController()
    projects
      .list({ size: 100 }, { signal: controller.signal })
      .then((res) => setProjectOptions(res.items || []))
      .catch(() => {})

    return () => controller.abort()
  }, [projects])

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(null)

    purchaseRequests
      .list(
        { page, size: PAGE_SIZE, status: selectedStatus || undefined },
        { signal: controller.signal },
      )
      .then((res) => setData(res))
      .catch((err) => {
        if (!controller.signal.aborted) {
          setError(err.message || 'Erro ao carregar solicitações de compra.')
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [purchaseRequests, page, selectedStatus, reloadToken])

  const handleDecide = async (decision) => {
    if (!decidingRequest) return
    await purchaseRequests.decide(decidingRequest.id, decision)
    reload()
  }

  const handleRegisterPurchase = async (purchaseData) => {
    if (!purchasingRequest) return
    await purchaseRequests.registerPurchase(purchasingRequest.id, purchaseData)
    reload()
  }

  return (
    <div className="mx-auto max-w-7xl px-6 py-8">
      {/* Page Header */}
      <header className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <div className="label">Materiais e Suprimentos</div>
          <h1 className="text-2xl font-bold tracking-tight text-graphite">Compras</h1>
        </div>

        <Button variant="primary" onClick={() => setFormOpen(true)}>
          + Nova Solicitação
        </Button>
      </header>

      {/* Filter bar */}
      <div className="mb-6 flex flex-wrap items-center gap-4 rounded border border-line bg-cream-soft p-4">
        <div className="w-48">
          <SelectField
            label="Status"
            value={selectedStatus}
            onChange={(e) => {
              setSelectedStatus(e.target.value)
              setPage(1)
            }}
          >
            <option value="">Todos os status</option>
            <option value="pending">Pendente</option>
            <option value="approved">Aprovado</option>
            <option value="rejected">Recusado</option>
            <option value="purchased">Comprado</option>
          </SelectField>
        </div>
      </div>

      {error && (
        <div className="mb-6 border border-danger-soft bg-[#faefea] p-4 text-sm text-danger">
          {error}
        </div>
      )}

      {/* Table */}
      <div className="overflow-x-auto border border-line bg-white">
        {loading ? (
          <p className="py-12 text-center text-xs text-muted">Carregando solicitações...</p>
        ) : data.items.length === 0 ? (
          <div className="p-8 text-center text-sm text-muted">
            Nenhuma solicitação de compra encontrada.
          </div>
        ) : (
          <table className="w-full border-collapse text-sm">
            <thead>
              <tr className="border-b border-line bg-cream-soft text-left">
                <th className="label px-4 py-3 font-normal">Obra</th>
                <th className="label px-4 py-3 font-normal">Data</th>
                <th className="label px-4 py-3 font-normal">Itens / Quantidades</th>
                <th className="label px-4 py-3 font-normal">Precisa Até</th>
                <th className="label px-4 py-3 font-normal">Status</th>
                <th className="label px-4 py-3 font-normal text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((req) => (
                <tr key={req.id} className="border-b border-line-soft last:border-0">
                  <td className="px-4 py-3">
                    <div className="font-medium text-graphite">{req.projectName}</div>
                    <div className="text-xs text-muted">Pedido #{req.id}</div>
                  </td>
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
                  <td className="px-4 py-3 text-right">
                    <div className="flex flex-wrap items-center justify-end gap-2">
                      {req.status?.toLowerCase() === 'pending' && (
                        <Button
                          variant="outline"
                          className="px-2.5 py-1 text-xs"
                          onClick={() => setDecidingRequest(req)}
                        >
                          Decidir
                        </Button>
                      )}

                      {req.status?.toLowerCase() === 'approved' && (
                        <Button
                          variant="primary"
                          className="px-2.5 py-1 text-xs"
                          onClick={() => setPurchasingRequest(req)}
                        >
                          Registrar Compra
                        </Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {!loading && !error && (
        <div className="mt-4">
          <Pagination
            page={data.page}
            totalPages={data.totalPages}
            total={data.total}
            onChange={setPage}
          />
        </div>
      )}

      {/* Dialogs */}
      <PurchaseRequestFormDialog
        open={formOpen}
        projectOptions={projectOptions}
        onClose={() => setFormOpen(false)}
        onSubmitted={reload}
      />

      <DecidePurchaseDialog
        open={!!decidingRequest}
        request={decidingRequest}
        onClose={() => setDecidingRequest(null)}
        onDecided={handleDecide}
      />

      <RegisterPurchaseDialog
        open={!!purchasingRequest}
        request={purchasingRequest}
        onClose={() => setPurchasingRequest(null)}
        onPurchased={handleRegisterPurchase}
      />
    </div>
  )
}
