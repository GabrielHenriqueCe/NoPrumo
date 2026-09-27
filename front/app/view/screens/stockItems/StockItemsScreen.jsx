import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { StockItemFormDialog } from './StockItemFormDialog'

/*
  Stock items: the box on the shelf. The only one of the three stock screens
  that deactivates — StockItem is the only entity with Active/DeletedAt.

  ReferencePrice is shown only when the API sends it (accounts with
  view_finance). The API is the real gate; this just mirrors what it decided.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function StockItemsScreen() {
    const { stockItems, stockGroups } = useContainer()
    const { can } = useSession()
    const canManage = can('manage_stock')

    const [data, setData] = useState(EMPTY_PAGE)
    const [groups, setGroups] = useState([])
    const [page, setPage] = useState(1)
    const [search, setSearch] = useState('')
    const [appliedSearch, setAppliedSearch] = useState('')
    const [groupFilter, setGroupFilter] = useState('')
    const [loading, setLoading] = useState(true)
    const [loadError, setLoadError] = useState(null)
    const [actionError, setActionError] = useState(null)
    const [pendingId, setPendingId] = useState(null)
    const [reloadToken, setReloadToken] = useState(0)

    const [formOpen, setFormOpen] = useState(false)
    const [editingItem, setEditingItem] = useState(null)

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

        stockItems
            .list(
                { page, size: PAGE_SIZE, search: appliedSearch, stockGroupId: groupFilter || null },
                { signal: controller.signal },
            )
            .then((result) => setData(result))
            .catch((error) => {
                if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load stock items.')
            })
            .finally(() => {
                if (!controller.signal.aborted) setLoading(false)
            })

        return () => controller.abort()
    }, [stockItems, page, appliedSearch, groupFilter, reloadToken])

    // Groups feed the filter and the form's dropdown; fetched once.
    useEffect(() => {
        const controller = new AbortController()

        stockGroups
            .list({ page: 1, size: 100 }, { signal: controller.signal })
            .then((result) => setGroups(result.items))
            .catch(() => {
                if (!controller.signal.aborted) setGroups([])
            })

        return () => controller.abort()
    }, [stockGroups])

    // The API decides this by whether the field comes back at all — the front
    // never guesses the permission on its own.
    const canSeePrice = data.items.length > 0 && data.items[0].referencePrice !== undefined

    const runAction = async (id, action) => {
        setPendingId(id)
        setActionError(null)

        try {
            await action()
            reload()
        } catch (error) {
            setActionError(error.message ?? 'The action could not be completed.')
        } finally {
            setPendingId(null)
        }
    }

    const saveItem = async (values) => {
        if (editingItem) {
            await stockItems.update(editingItem.id, values)
        } else {
            await stockItems.create(values)
        }

        setFormOpen(false)
        setEditingItem(null)
        reload()
    }

    return (
        <>
            <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
                <div className="min-w-0">
                    <p className="label">Master data</p>
                    <h1 className="mt-1 text-xl font-semibold tracking-tight">Stock items</h1>
                </div>

                <div className="ml-auto flex items-center gap-3">
                    <label className="sr-only" htmlFor="stock-item-search">
                        Search stock items
                    </label>
                    <input
                        id="stock-item-search"
                        type="search"
                        value={search}
                        onChange={(event) => setSearch(event.target.value)}
                        placeholder="Search by name or code"
                        className="w-64 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
                    />

                    <div className="w-56">
                        <SelectField
                            label="Group"
                            name="group-filter"
                            value={groupFilter}
                            onChange={(event) => {
                                setGroupFilter(event.target.value)
                                setPage(1)
                            }}
                            placeholder="All groups"
                            options={groups.map((group) => ({ value: group.id, label: group.name }))}
                        />
                    </div>

                    {canManage && (
                        <Button
                            onClick={() => {
                                setEditingItem(null)
                                setFormOpen(true)
                            }}
                        >
                            New item
                        </Button>
                    )}
                </div>
            </header>

            <div className="p-8">
                <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
                    An item is switched off, never deleted: its movement history has to keep pointing
                    somewhere.
                </p>

                {actionError && (
                    <p role="alert" className="mb-4 border border-danger-soft bg-[#faefea] px-4 py-2.5 text-[13px] text-danger">
                        {actionError}
                    </p>
                )}

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
                            <p className="text-sm text-muted">
                                {appliedSearch ? `No item matches “${appliedSearch}”.` : 'No stock item registered yet.'}
                            </p>
                        </div>
                    )}

                    {!loading && !loadError && data.items.length > 0 && (
                        <table className="w-full border-collapse text-sm">
                            <caption className="sr-only">Registered stock items</caption>
                            <thead>
                                <tr className="border-b border-line bg-cream-soft text-left">
                                    <Th>Name</Th>
                                    <Th>Group</Th>
                                    <Th>Unit</Th>
                                    <Th>Min. quantity</Th>
                                    {canSeePrice && <Th>Reference price</Th>}
                                    <Th>Status</Th>
                                    <th className="px-4 py-3" />
                                </tr>
                            </thead>

                            <tbody>
                                {data.items.map((item) => {
                                    const busy = pendingId === item.id

                                    return (
                                        <tr key={item.id} className="border-b border-line-soft last:border-0">
                                            <td className="px-4 py-3">
                                                <div className="font-medium">{item.name}</div>
                                                {item.code && <div className="text-[12.5px] text-muted">{item.code}</div>}
                                            </td>

                                            <td className="px-4 py-3">{item.stockGroupName}</td>
                                            <td className="px-4 py-3">{item.unit}</td>
                                            <td className="px-4 py-3">{item.minQuantity}</td>

                                            {canSeePrice && (
                                                <td className="px-4 py-3">
                                                    {item.referencePrice != null
                                                        ? item.referencePrice.toLocaleString('en-US', { style: 'currency', currency: 'BRL' })
                                                        : '—'}
                                                </td>
                                            )}

                                            <td className="px-4 py-3">
                                                <Badge tone={item.active ? 'active' : 'inactive'}>
                                                    {item.active ? 'Active' : 'Inactive'}
                                                </Badge>
                                            </td>

                                            <td className="px-4 py-3">
                                                {canManage && (
                                                    <div className="flex items-center justify-end gap-2">
                                                        <Button
                                                            variant="outline"
                                                            className="px-3 py-1.5"
                                                            disabled={busy}
                                                            onClick={() => {
                                                                setEditingItem(item)
                                                                setFormOpen(true)
                                                            }}
                                                        >
                                                            Edit
                                                        </Button>

                                                        <Button
                                                            variant={item.active ? 'danger' : 'outline'}
                                                            className="px-3 py-1.5"
                                                            busy={busy}
                                                            disabled={busy}
                                                            onClick={() => runAction(item.id, () => stockItems.setActive(item.id, !item.active))}
                                                        >
                                                            {item.active ? 'Deactivate' : 'Activate'}
                                                        </Button>
                                                    </div>
                                                )}
                                            </td>
                                        </tr>
                                    )
                                })}
                            </tbody>
                        </table>
                    )}
                </div>

                {!loading && !loadError && (
                    <Pagination page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
                )}
            </div>

            <StockItemFormDialog
                open={formOpen}
                item={editingItem}
                groups={groups}
                canSeePrice={canSeePrice}
                onClose={() => {
                    setFormOpen(false)
                    setEditingItem(null)
                }}
                onSubmit={saveItem}
            />
        </>
    )
}

function Th({ children }) {
    return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
    return (
        <div className="p-4" role="status" aria-label="Loading stock items">
            {Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
            ))}
        </div>
    )
}