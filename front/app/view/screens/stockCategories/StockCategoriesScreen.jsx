import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { StockCategoryFormDialog } from './StockCategoryFormDialog'

/*
  Stock categories: the three flows the whole system is built around
  (consumable material, PPE, tool). The three base rows come from the seed;
  this screen only lets more be added.

  No activate/deactivate here: the table has no active/deletedAt column, and
  deleting would break every group and item that points to a category.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function StockCategoriesScreen() {
  const { stockCategories } = useContainer()
  const { can } = useSession()
  const canManage = can('manage_stock')

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingCategory, setEditingCategory] = useState(null)

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

    stockCategories
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load stock categories.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [stockCategories, page, appliedSearch, reloadToken])

  const saveCategory = async (values) => {
    if (editingCategory) {
      await stockCategories.update(editingCategory.id, values)
    } else {
      await stockCategories.create(values)
    }

    setFormOpen(false)
    setEditingCategory(null)
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Stock categories</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="stock-category-search">
            Search stock categories
          </label>
          <input
            id="stock-category-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by name"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          {canManage && (
            <Button
              onClick={() => {
                setEditingCategory(null)
                setFormOpen(true)
              }}
            >
              New category
            </Button>
          )}
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Consumable material, PPE and tool come from the seed and define the three stock flows.
          There is no deactivate here: a category cannot be removed once groups depend on it.
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
                {appliedSearch ? `No category matches “${appliedSearch}”.` : 'No stock category registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered stock categories</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <Th>Name</Th>
                  <Th>Tracks project balance</Th>
                  <Th>Requires return</Th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((category) => (
                  <tr key={category.id} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3 font-medium">{category.name}</td>

                    <td className="px-4 py-3">
                      <Badge tone={category.tracksProjectBalance ? 'active' : 'inactive'}>
                        {category.tracksProjectBalance ? 'Yes' : 'No'}
                      </Badge>
                    </td>

                    <td className="px-4 py-3">
                      <Badge tone={category.requiresReturn ? 'active' : 'inactive'}>
                        {category.requiresReturn ? 'Yes' : 'No'}
                      </Badge>
                    </td>

                    <td className="px-4 py-3">
                      {canManage && (
                        <div className="flex items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            onClick={() => {
                              setEditingCategory(category)
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

      <StockCategoryFormDialog
        open={formOpen}
        category={editingCategory}
        onClose={() => {
          setFormOpen(false)
          setEditingCategory(null)
        }}
        onSubmit={saveCategory}
      />
    </>
  )
}

function Th({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading stock categories">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}