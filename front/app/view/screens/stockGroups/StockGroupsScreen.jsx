import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { SelectField } from '../../ui/SelectField'
import { StockGroupFormDialog } from './StockGroupFormDialog'
import { useT } from '../../preferences/preferencesContext'

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
    const t = useT()
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
                if (!controller.signal.aborted) setLoadError(error.message ?? t('stockGroupsScreen.couldNotLoadStockGroups'))
            })
            .finally(() => {
                if (!controller.signal.aborted) setLoading(false)
            })

        return () => controller.abort()
    }, [stockGroups, page, appliedSearch, categoryFilter, reloadToken, t])

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
            <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
                <div className="min-w-0">
                    <p className="label">{t('stockGroupsScreen.masterData')}</p>
                    <h1 className="display mt-1 text-[38px] leading-none">{t('stockGroupsScreen.stockGroups')}</h1>
                </div>

                <div className="ml-auto flex items-center gap-3">
                    <label className="sr-only" htmlFor="stock-group-search">
                        {t('stockGroupsScreen.searchStockGroups')}
                    </label>
                    <input
                        id="stock-group-search"
                        type="search"
                        value={search}
                        onChange={(event) => setSearch(event.target.value)}
                        placeholder={t('stockGroupsScreen.searchByName')}
                        className="w-64 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
                    />

                    <div className="w-56">
                        <SelectField
                            label={t('stockGroupsScreen.category')}
                            name="category-filter"
                            value={categoryFilter}
                            onChange={(event) => {
                                setCategoryFilter(event.target.value)
                                setPage(1)
                            }}
                            placeholder={t('stockGroupsScreen.allCategories')}
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
                            {t('stockGroupsScreen.newGroup')}
                        </Button>
                    )}
                </div>
            </header>

            <div className="px-10 pt-6 pb-10">
                <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
                    {t('stockGroupsScreen.aGroupBelongsToOne')}
                </p>

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
                                {appliedSearch ? t('stockGroupsScreen.noGroupMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('stockGroupsScreen.noStockGroupRegisteredYet')}
                            </p>
                        </div>
                    )}

                    {!loading && !loadError && data.items.length > 0 && (
                        <table className="w-full border-collapse text-sm">
                            <caption className="sr-only">{t('stockGroupsScreen.registeredStockGroups')}</caption>
                            <thead>
                                <tr className="border-b border-line bg-cream-soft text-left">
                                    <Th>{t('common.name')}</Th>
                                    <Th>{t('stockGroupsScreen.category')}</Th>
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
                                                        {t('common.edit')}
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
    const t = useT()
    return (
        <div className="p-4" role="status" aria-label={t('stockGroupsScreen.loadingStockGroups')}>
            {Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
            ))}
        </div>
    )
}