import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { DecidePurchaseDialog } from './DecidePurchaseDialog'
import { PurchaseRequestFormDialog } from './PurchaseRequestFormDialog'
import { purchaseStatusKey, purchaseStatusTone } from './purchaseStatus'
import { RegisterPurchaseDialog } from './RegisterPurchaseDialog'
import { useT } from '../../preferences/preferencesContext'

const PAGE_SIZE = 10
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

function formatDate(value) {
  if (!value) return '—'
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

export function PurchasesScreen() {
  const t = useT()
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
          setLoadError(err.message || t('purchasesScreen.couldNotLoadPurchaseRequests'))
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [purchaseRequests, page, selectedStatus, reloadToken, t])

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
          <span className="label">{t('purchasesScreen.materialsSupplies')}</span>
          <h1 className="mt-1 text-2xl font-bold tracking-tight">{t('purchasesScreen.purchases')}</h1>
        </div>

        <div className="flex items-center gap-3">
          <Button onClick={() => setFormOpen(true)}>
            {t('purchasesScreen.newRequest')}
          </Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('purchasesScreen.manageMaterialPurchaseRequestsSubmitted')}
        </p>

        <div className="mb-6 flex flex-wrap items-center gap-4 border border-line bg-cream-soft p-4">
          <div className="w-56">
            <SelectField
              label={t('purchasesScreen.filterByStatus')}
              value={selectedStatus}
              onChange={(e) => {
                setSelectedStatus(e.target.value)
                setPage(1)
              }}
            >
              <option value="">{t('purchasesScreen.allStatuses')}</option>
              <option value="pending">{t('purchasesScreen.pending')}</option>
              <option value="approved">{t('purchasesScreen.approved')}</option>
              <option value="rejected">{t('purchasesScreen.rejected')}</option>
              <option value="purchased">{t('purchasesScreen.purchased')}</option>
            </SelectField>
          </div>
        </div>

        <div className="overflow-hidden rounded-3xl border border-line bg-surface">
          {loading && <TableSkeleton />}

          {!loading && loadError && (
            <div className="px-6 py-10 text-center">
              <p className="text-sm text-danger">{loadError}</p>
              <Button variant="outline" onClick={reload} className="mt-4">
                {t('common.tryAgain')}
              </Button>
            </div>
          )}

          {!loading && !loadError && data.items.length === 0 && (
            <div className="px-6 py-12 text-center">
              <p className="text-sm text-muted">{t('purchasesScreen.noPurchaseRequestRegisteredYet')}</p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('purchasesScreen.purchaseRequests')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <HeaderCell>{t('purchasesScreen.project')}</HeaderCell>
                  <HeaderCell>{t('purchasesScreen.date')}</HeaderCell>
                  <HeaderCell>{t('purchasesScreen.itemsQuantities')}</HeaderCell>
                  <HeaderCell>{t('purchasesScreen.neededBy')}</HeaderCell>
                  <HeaderCell>{t('common.status')}</HeaderCell>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody>
                {data.items.map((req) => (
                  <tr key={req.id} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3">
                      <div className="font-medium text-graphite">{req.projectName}</div>
                      <div className="font-mono text-[12.5px] text-muted">{t('purchasesScreen.request')}{req.id}</div>
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
                                {t('purchasesScreen.fulfilled')} {item.fulfilledQuantity} {item.unit})
                              </span>
                            )}
                          </div>
                        ))}
                      </div>
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap">{formatDate(req.neededByDate)}</td>
                    <td className="px-4 py-3">
                      <Badge tone={purchaseStatusTone(req.status)}>
                        {t(purchaseStatusKey(req.status))}
                      </Badge>
                      {req.rejectionReason && (
                        <div className="mt-1 text-[11px] text-danger">
                          {t('purchasesScreen.reason')} {req.rejectionReason}
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
                            {t('purchasesScreen.decide')}
                          </Button>
                        )}

                        {req.status?.toLowerCase() === 'approved' && (
                          <Button
                            variant="primary"
                            className="px-2.5 py-1 text-xs"
                            onClick={() => setPurchasingRequest(req)}
                          >
                            {t('purchasesScreen.registerPurchase')}
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
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('purchasesScreen.loadingPurchaseRequests')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
