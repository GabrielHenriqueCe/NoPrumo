import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { StockGroupFormDialog } from './StockGroupFormDialog'

/*
  Stock groups: the shelf inside a category's aisle. A group belongs to one
  category, and moving it once it has items is refused by the API.

  No activate/deactivate here: the table has no active/deletedAt column, and
  deleting would break every item that points to a group.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function StockGroupsScreen() {
    const { stockGroups, stockCategories } = useContainer()
    const { can } = useSession()
    const canManage = can('manage_stock')

    const [data, setData] = useState(EMPTY_PAGE)
    const [categories, setCategories] = useState([])
    const [page, setPage] = useState(1)
    const [search, setSearch] = useState('')
    const [appliedSearch, setAppliedSearch] = useState('')
    const [categoryFilter, setCategoryFilter] = useState('')
    const [loading, setLoading] = useState(true)
    const [loadError, setLoadError] = useState(null)
    const [reloadToken, setReloadToken] = useState(0)

    const [formOpen, setFormOpen] = useState(false)
    const [editingGroup, setEditingGroup] = useState(null)

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

        stockGroups
            .list(
                {
                    page,
                    size: PAGE_SIZE,
                    search: appliedSearch,
                    stockCategoryId: categoryFilter || null,
                },
                { signal: controller.signal },
            )
            .then((result) => setData(result))
            .catch((error) => {
                if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load stock groups.')
            })
            .finally(() => {
                if (!controller.signal.aborted) setLoading(false)
            })

        return () => controller.abort()
    }, [stockGroups, page, appliedSearch, categoryFilter, reloadToken])

    // Categories fill the filter and the form's dropdown; fetched once, not on
    // every list refresh. 100 is comfortably above the three seeded rows plus
    // whatever the team adds — the category screen paginates further if needed.
    useEffect(() => {
        const controller = new AbortController()

        stockCategories
            .list({ page: 1, size: 100 }, { signal: controller.signal })
            .then((result) => setCategories(result.items))
            .catch(() => {
                if (!controller.signal.aborted) setCategories([])
            })

        return () => controller.abort()
    }, [stockCategories])

    const saveGroup = async (values) => {
        if (editingGroup) {
            await stockGroups.update(editingGroup.id, values)
        } else {
            await stockGroups.create(values)
        }

        setFormOpen(false)
        setEditingGroup(null)
        reload()
    }

    return (
        <>
            <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
                <div className="min-w-0">
                    <p className="label">Master data</p>
                    <h1 className="mt-1 text-xl font-semibold tracking-tight">Stock groups</h1>
                </div>

                <div className="ml-auto flex items-center gap-3">
                    <label className="sr-only" htmlFor="stock-group-search">
                        Search stock groups
                    </label>
                    <input
                        id="stock-group-search"
                        type="search"
                        value={search}
                        onChange={(event) => setSearch(event.target.value)}
                        placeholder="Search by name"
                        className="w-64 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
                    />

                    <div className="w-56">
                        <SelectField
                            label="Category"
                            name="category-filter"
                            value={categoryFilter}
                            onChange={(event) => {
                                setCategoryFilter(event.target.value)
                                setPage(1)
                            }}
                            placeholder="All categories"
                            options={categories.map((category) => ({ value: category.id, label: category.name }))}
                        />
                    </div>

                    {canManage && (
                        <Button
                            onClick={() => {
                                setEditingGroup(null)
                                setFormOpen(true)
                            }}
                        >
                            New group
                        </Button>
                    )}
                </div>
            </header>

            <div className="p-8">
                <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
                    A group belongs to one category. Once it has items, the API refuses to move it to a
                    different category — that would change the flow of everything inside it.
                </p>

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
                                {appliedSearch ? `No group matches “${appliedSearch}”.` : 'No stock group registered yet.'}
                            </p>
                        </div>
                    )}

                    {!loading && !loadError && data.items.length > 0 && (
                        <table className="w-full border-collapse text-sm">
                            <caption className="sr-only">Registered stock groups</caption>
                            <thead>
                                <tr className="border-b border-line bg-cream-soft text-left">
                                    <Th>Name</Th>
                                    <Th>Category</Th>
                                    <th className="px-4 py-3" />
                                </tr>
                            </thead>

                            <tbody>
                                {data.items.map((group) => (
                                    <tr key={group.id} className="border-b border-line-soft last:border-0">
                                        <td className="px-4 py-3 font-medium">{group.name}</td>
                                        <td className="px-4 py-3">{group.stockCategoryName}</td>

                                        <td className="px-4 py-3">
                                            {canManage && (
                                                <div className="flex items-center justify-end gap-2">
                                                    <Button
                                                        variant="outline"
                                                        className="px-3 py-1.5"
                                                        onClick={() => {
                                                            setEditingGroup(group)
                                                            setFormOpen(true)
                                                        }}
                                                    >
                                                        Edit
                                                    </Button>
                                                </div>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    )}
                </div>

                {!loading && !loadError && (
                    <Pagination page={data.page} totalPages={data.totalPages} total={data.total} onChange={setPage} />
                )}
            </div>

            <StockGroupFormDialog
                open={formOpen}
                group={editingGroup}
                categories={categories}
                onClose={() => {
                    setFormOpen(false)
                    setEditingGroup(null)
                }}
                onSubmit={saveGroup}
            />
        </>
    )
}

function Th({ children }) {
    return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
    return (
        <div className="p-4" role="status" aria-label="Loading stock groups">
            {Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
            ))}
        </div>
    )
}