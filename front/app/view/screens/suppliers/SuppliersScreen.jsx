import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SupplierFormDialog } from './SupplierFormDialog'
import { useT } from '../../preferences/preferencesContext'

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function SuppliersScreen() {
  const t = useT()
  const { suppliers } = useContainer()

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [actionError, setActionError] = useState(null)
  const [pendingId, setPendingId] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingSupplier, setEditingSupplier] = useState(null)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  useEffect(() => {
    const timer = setTimeout(() => {
      setAppliedSearch(search.trim())
      setPage(1)
    }, SEARCH_DEBOUNCE_MS)

    return () => clearTimeout(timer)
  }, [search])

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setLoadError(null)

    suppliers
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('suppliersScreen.couldNotLoadSuppliers'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [suppliers, page, appliedSearch, reloadToken, t])

  const runAction = async (id, action) => {
    setPendingId(id)
    setActionError(null)

    try {
      await action()
      reload()
    } catch (error) {
      setActionError(error.message ?? t('suppliersScreen.theActionCouldNotBe'))
    } finally {
      setPendingId(null)
    }
  }

  const saveSupplier = async (values) => {
    if (editingSupplier) {
      await suppliers.update(editingSupplier.id, values)
    } else {
      await suppliers.create(values)
    }

    setFormOpen(false)
    setEditingSupplier(null)
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
        <div className="min-w-0">
          <p className="label">{t('suppliersScreen.masterData')}</p>
          <h1 className="display mt-1 text-[38px] leading-none">{t('suppliersScreen.suppliers')}</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="supplier-search">
            {t('suppliersScreen.searchSuppliers')}
          </label>
          <input
            id="supplier-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t('suppliersScreen.searchByNameDocumentOr')}
            className="w-72 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
          />

          <Button
            onClick={() => {
              setEditingSupplier(null)
              setFormOpen(true)
            }}
          >
            {t('suppliersScreen.newSupplier')}
          </Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('suppliersScreen.suppliersProvideConstructionMaterialsSubcontracts')}
        </p>

        {actionError && (
          <p role="alert" className="mb-4 rounded-2xl bg-danger-wash px-4 py-2.5 text-[13px] text-danger">
            {actionError}
          </p>
        )}

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
              <p className="text-sm text-muted">
                {appliedSearch ? t('suppliersScreen.noSupplierMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('suppliersScreen.noSupplierRegisteredYet')}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('suppliersScreen.registeredSuppliers')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <Th>{t('common.name')}</Th>
                  <Th>{t('suppliersScreen.document')}</Th>
                  <Th>{t('suppliersScreen.contact')}</Th>
                  <Th>{t('suppliersScreen.location')}</Th>
                  <Th>{t('common.status')}</Th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((supplier) => {
                  const busy = pendingId === supplier.id

                  return (
                    <tr key={supplier.id} className="border-b border-line-soft last:border-0">
                      <td className="px-4 py-3">
                        <div className="font-medium">{supplier.name}</div>
                        {supplier.email && <div className="text-[12.5px] text-muted">{supplier.email}</div>}
                      </td>

                      <td className="px-4 py-3 font-mono text-[13px]">
                        {supplier.documentMasked || '—'}
                      </td>

                      <td className="px-4 py-3">
                        <div>{supplier.contactName || '—'}</div>
                        {supplier.phone && <div className="text-[12.5px] text-muted">{supplier.phone}</div>}
                      </td>

                      <td className="px-4 py-3">
                        {supplier.city ? `${supplier.city}${supplier.state ? ` / ${supplier.state}` : ''}` : '—'}
                      </td>

                      <td className="px-4 py-3">
                        <Badge tone={supplier.active ? 'active' : 'inactive'}>
                          {supplier.active ? t('common.active') : t('common.inactive')}
                        </Badge>
                      </td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            disabled={busy}
                            onClick={() => {
                              setEditingSupplier(supplier)
                              setFormOpen(true)
                            }}
                          >
                            {t('common.edit')}
                          </Button>

                          <Button
                            variant={supplier.active ? 'danger' : 'outline'}
                            className="px-3 py-1.5"
                            busy={busy}
                            onClick={() =>
                              runAction(supplier.id, () => suppliers.setActive(supplier.id, !supplier.active))
                            }
                          >
                            {supplier.active ? t('common.deactivate') : t('common.activate')}
                          </Button>
                        </div>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          )}
        </div>

        {!loading && !loadError && (
          <Pagination
            page={data.page}
            totalPages={data.totalPages}
            total={data.total}
            onChange={setPage}
            busy={Boolean(pendingId)}
          />
        )}
      </div>

      <SupplierFormDialog
        open={formOpen}
        supplier={editingSupplier}
        onClose={() => {
          setFormOpen(false)
          setEditingSupplier(null)
        }}
        onSubmit={saveSupplier}
      />
    </>
  )
}

function Th({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('suppliersScreen.loadingSuppliers')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
