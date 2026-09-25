import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { ClientFormDialog } from './ClientFormDialog'

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function ClientsScreen() {
  const { clients } = useContainer()

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
  const [editingClient, setEditingClient] = useState(null)

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

    clients
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load clients.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [clients, page, appliedSearch, reloadToken])

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

  const saveClient = async (values) => {
    if (editingClient) {
      await clients.update(editingClient.id, values)
    } else {
      await clients.create(values)
    }

    setFormOpen(false)
    setEditingClient(null)
    reload()
  }

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Master data</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Clients</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="client-search">
            Search clients
          </label>
          <input
            id="client-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by name, document or email"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <Button
            onClick={() => {
              setEditingClient(null)
              setFormOpen(true)
            }}
          >
            New client
          </Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Clients represent external project owners and partners. Records are deactivated, never
          deleted, to maintain project association history.
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
                {appliedSearch ? `No client matches “${appliedSearch}”.` : 'No client registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered clients</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <Th>Name</Th>
                  <Th>Document</Th>
                  <Th>Contact</Th>
                  <Th>Location</Th>
                  <Th>Status</Th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((client) => {
                  const busy = pendingId === client.id

                  return (
                    <tr key={client.id} className="border-b border-line-soft last:border-0">
                      <td className="px-4 py-3">
                        <div className="font-medium">{client.name}</div>
                        {client.email && <div className="text-[12.5px] text-muted">{client.email}</div>}
                      </td>

                      <td className="px-4 py-3 font-mono text-[13px]">
                        {client.documentMasked || '—'}
                      </td>

                      <td className="px-4 py-3">
                        <div>{client.contactName || '—'}</div>
                        {client.phone && <div className="text-[12.5px] text-muted">{client.phone}</div>}
                      </td>

                      <td className="px-4 py-3">
                        {client.city ? `${client.city}${client.state ? ` / ${client.state}` : ''}` : '—'}
                      </td>

                      <td className="px-4 py-3">
                        <Badge tone={client.active ? 'active' : 'inactive'}>
                          {client.active ? 'Active' : 'Inactive'}
                        </Badge>
                      </td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            disabled={busy}
                            onClick={() => {
                              setEditingClient(client)
                              setFormOpen(true)
                            }}
                          >
                            Edit
                          </Button>

                          <Button
                            variant={client.active ? 'danger' : 'outline'}
                            className="px-3 py-1.5"
                            busy={busy}
                            onClick={() =>
                              runAction(client.id, () => clients.setActive(client.id, !client.active))
                            }
                          >
                            {client.active ? 'Deactivate' : 'Activate'}
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

      <ClientFormDialog
        open={formOpen}
        client={editingClient}
        onClose={() => {
          setFormOpen(false)
          setEditingClient(null)
        }}
        onSubmit={saveClient}
      />
    </>
  )
}

function Th({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading clients">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
