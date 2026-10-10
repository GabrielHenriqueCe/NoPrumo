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
  const [loadError, setLoadError] = useState(null)
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
    setLoadError(null)

    purchaseRequests
      .list(
        { page, size: PAGE_SIZE, status: selectedStatus || undefined },
        { signal: controller.signal },
      )
      .then((res) => setData(res))
      .catch((err) => {
        if (!controller.signal.aborted) {
          setLoadError(err.message || 'Could not load purchase requests.')
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
    <>
      <header className="flex flex-wrap items-end justify-between gap-4 border-b border-line bg-cream-soft px-8 py-6">
        <div>
          <span className="label">Materials & Supplies</span>
          <h1 className="mt-1 text-2xl font-bold tracking-tight">Purchases</h1>
        </div>

        <div className="flex items-center gap-3">
          <Button onClick={() => setFormOpen(true)}>
            New request
          </Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Manage material purchase requests submitted by foremen across projects. Purchasing reviews,
          approves or rejects requests and records supplier purchases directly to stock.
        </p>

        <div className="mb-6 flex flex-wrap items-center gap-4 border border-line bg-cream-soft p-4">
          <div className="w-56">
            <SelectField
              label="Filter by status"
              value={selectedStatus}
              onChange={(e) => {
                setSelectedStatus(e.target.value)
                setPage(1)
              }}
            >
              <option value="">All statuses</option>
              <option value="pending">Pending</option>
              <option value="approved">Approved</option>
              <option value="rejected">Rejected</option>
              <option value="purchased">Purchased</option>
            </SelectField>
          </div>
        </div>

        <div className="border border-line bg-white">
          {loading && <TableSkeleton />}

          {!loading && loadError && (
            <div className="px-6 py-10 text-center">
              <p className="text-sm text-danger">{loadError}</p>
              <Button variant="outline" onClick={reload} className="mt-4">
                Try again
              </Button>
            </div>
          )}

          {!loading && !loadError && data.items.length === 0 && (
            <div className="px-6 py-12 text-center">
              <p className="text-sm text-muted">No purchase request registered yet.</p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Purchase requests</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>Project</HeaderCell>
                  <HeaderCell>Date</HeaderCell>
                  <HeaderCell>Items / Quantities</HeaderCell>
                  <HeaderCell>Needed by</HeaderCell>
                  <HeaderCell>Status</HeaderCell>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody>
                {data.items.map((req) => (
                  <tr key={req.id} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3">
                      <div className="font-medium text-graphite">{req.projectName}</div>
                      <div className="font-mono text-[12.5px] text-muted">Request #{req.id}</div>
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
                                (Fulfilled: {item.fulfilledQuantity} {item.unit})
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
                          Reason: {req.rejectionReason}
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
                            Decide
                          </Button>
                        )}

                        {req.status?.toLowerCase() === 'approved' && (
                          <Button
                            variant="primary"
                            className="px-2.5 py-1 text-xs"
                            onClick={() => setPurchasingRequest(req)}
                          >
                            Register purchase
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

        {!loading && !loadError && (
          <div className="mt-4">
            <Pagination
              page={data.page}
              totalPages={data.totalPages}
              total={data.total}
              noun="purchase requests"
              onChange={setPage}
            />
          </div>
        )}
      </div>

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
    </>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading purchase requests">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
