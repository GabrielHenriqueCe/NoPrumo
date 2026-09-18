import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { TemporaryPasswordDialog } from './TemporaryPasswordDialog'
import { UserFormDialog } from './UserFormDialog'

/*
  User administration: the list, plus create, edit, switch on/off and reset.

  Accounts are never deleted. A user signed time entries, issued PPE and
  approved purchases; deleting the row would orphan all of it. Switching the
  account off ends the access and keeps the history, which is also what the
  labour paperwork expects.
*/

const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350

const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function UsersScreen() {
  const { users } = useContainer()
  const { user: signedInUser } = useSession()

  const [data, setData] = useState(EMPTY_PAGE)
  const [roles, setRoles] = useState([])
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [actionError, setActionError] = useState(null)
  const [pendingId, setPendingId] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [editingUser, setEditingUser] = useState(null)
  const [issued, setIssued] = useState(null)

  const reload = useCallback(() => setReloadToken((value) => value + 1), [])

  // Typing should not fire a request per keystroke; it waits for a pause.
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

    users
      .list({ page, size: PAGE_SIZE, search: appliedSearch }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        // An aborted request is this effect being replaced, not a failure.
        if (!controller.signal.aborted) setLoadError(error.message ?? 'Could not load users.')
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [users, page, appliedSearch, reloadToken])

  // Roles fill a fixed dropdown; fetched once, not on every list refresh.
  useEffect(() => {
    const controller = new AbortController()

    users
      .listRoles({ signal: controller.signal })
      .then(setRoles)
      .catch(() => {
        if (!controller.signal.aborted) setRoles([])
      })

    return () => controller.abort()
  }, [users])

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

  const saveUser = async (values) => {
    const result = editingUser
      ? await users.update(editingUser.id, values)
      : await users.create(values)

    setFormOpen(false)
    setEditingUser(null)
    reload()

    // Only a brand new account comes back with a password to hand over.
    if (result?.temporaryPassword) {
      setIssued({ user: result.user, password: result.temporaryPassword })
    }
  }

  const resetPassword = (user) =>
    runAction(user.id, async () => {
      const { temporaryPassword } = await users.resetPassword(user.id)
      setIssued({ user, password: temporaryPassword })
    })

  return (
    <>
      <header className="flex flex-wrap items-center gap-4 border-b border-line bg-white px-8 py-5">
        <div className="min-w-0">
          <p className="label">Administration</p>
          <h1 className="mt-1 text-xl font-semibold tracking-tight">Users</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="user-search">
            Search users
          </label>
          <input
            id="user-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search by name, username or email"
            className="w-72 border border-line bg-white px-3 py-2 text-sm outline-none focus:border-gold"
          />

          <Button
            onClick={() => {
              setEditingUser(null)
              setFormOpen(true)
            }}
          >
            New user
          </Button>
        </div>
      </header>

      <div className="p-8">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          Accounts are created here — there is no public sign-up. An account is switched off, never
          deleted: the time entries, PPE records and purchases it signed have to keep pointing
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
                {appliedSearch ? `No user matches “${appliedSearch}”.` : 'No user registered yet.'}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">Registered users</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <Th>Name</Th>
                  <Th>Username</Th>
                  <Th>Role</Th>
                  <Th>Status</Th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>

              <tbody>
                {data.items.map((user) => {
                  const busy = pendingId === user.id
                  const isSelf = user.id === signedInUser?.id

                  return (
                    <tr key={user.id} className="border-b border-line-soft last:border-0">
                      <td className="px-4 py-3">
                        <div className="font-medium">{user.name}</div>
                        {user.email && <div className="text-[12.5px] text-muted">{user.email}</div>}
                      </td>

                      <td className="px-4 py-3 font-mono text-[13px]">{user.username}</td>
                      <td className="px-4 py-3">{user.roleLabel}</td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap gap-1.5">
                          <Badge tone={user.active ? 'active' : 'inactive'}>
                            {user.active ? 'Active' : 'Inactive'}
                          </Badge>
                          {user.mustChangePassword && <Badge tone="warning">Temporary password</Badge>}
                        </div>
                      </td>

                      <td className="px-4 py-3">
                        <div className="flex flex-wrap items-center justify-end gap-2">
                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            disabled={busy}
                            onClick={() => {
                              setEditingUser(user)
                              setFormOpen(true)
                            }}
                          >
                            Edit
                          </Button>

                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            disabled={busy}
                            onClick={() => resetPassword(user)}
                          >
                            Reset password
                          </Button>

                          <Button
                            variant={user.active ? 'danger' : 'outline'}
                            className="px-3 py-1.5"
                            busy={busy}
                            /*
                              Switching off your own account locks you out on
                              the next reload, so the control is not offered.
                              The API refuses it as well — this only spares the
                              round trip.
                            */
                            disabled={busy || (user.active && isSelf)}
                            title={user.active && isSelf ? 'You cannot deactivate your own account' : undefined}
                            onClick={() =>
                              runAction(user.id, () => users.setActive(user.id, !user.active))
                            }
                          >
                            {user.active ? 'Deactivate' : 'Activate'}
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

      <UserFormDialog
        open={formOpen}
        user={editingUser}
        roles={roles}
        onClose={() => {
          setFormOpen(false)
          setEditingUser(null)
        }}
        onSubmit={saveUser}
      />

      <TemporaryPasswordDialog
        open={Boolean(issued)}
        user={issued?.user}
        password={issued?.password ?? ''}
        onClose={() => setIssued(null)}
      />
    </>
  )
}

function Th({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

/*
  Placeholder rows instead of a spinner: the table keeps its shape while it
  loads, so the page does not jump when the data lands.
*/
function TableSkeleton() {
  return (
    <div className="p-4" role="status" aria-label="Loading users">
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse bg-cream last:mb-0" />
      ))}
    </div>
  )
}
