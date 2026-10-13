import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { TemporaryPasswordDialog } from './TemporaryPasswordDialog'
import { UserFormDialog } from './UserFormDialog'
import { useT } from '../../preferences/preferencesContext'

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
// 100 is the API's page cap.
const EMPLOYEE_LOOKUP_SIZE = 100

export function UsersScreen() {
  const t = useT()
  const { users, employees } = useContainer()
  const { user: signedInUser } = useSession()

  const [data, setData] = useState(EMPTY_PAGE)
  const [roles, setRoles] = useState([])
  const [employeeOptions, setEmployeeOptions] = useState([])
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
        if (!controller.signal.aborted) setLoadError(error.message ?? t('usersScreen.couldNotLoadUsers'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [users, page, appliedSearch, reloadToken, t])

  // Roles and employees fill fixed dropdowns; fetched once, not on every list refresh.
  useEffect(() => {
    const controller = new AbortController()

    users
      .listRoles({ signal: controller.signal })
      .then(setRoles)
      .catch(() => {
        if (!controller.signal.aborted) setRoles([])
      })

    employees
      .list({ page: 1, size: EMPLOYEE_LOOKUP_SIZE }, { signal: controller.signal })
      .then((result) =>
        setEmployeeOptions(
          result.items
            .filter((employee) => employee.active)
            .map((employee) => ({ value: employee.id, label: employee.name })),
        ),
      )
      .catch(() => {})

    return () => controller.abort()
  }, [users, employees])

  const runAction = async (id, action) => {
    setPendingId(id)
    setActionError(null)

    try {
      await action()
      reload()
    } catch (error) {
      setActionError(error.message ?? t('usersScreen.theActionCouldNotBe'))
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
      <header className="flex flex-wrap items-center gap-4 px-10 pt-7">
        <div className="min-w-0">
          <p className="label">{t('usersScreen.administration')}</p>
          <h1 className="display mt-1 text-[38px] leading-none">{t('usersScreen.users')}</h1>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor="user-search">
            {t('usersScreen.searchUsers')}
          </label>
          <input
            id="user-search"
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t('usersScreen.searchByNameUsernameOr')}
            className="w-72 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
          />

          <Button
            onClick={() => {
              setEditingUser(null)
              setFormOpen(true)
            }}
          >
            {t('usersScreen.newUser')}
          </Button>
        </div>
      </header>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-6 max-w-[54em] text-sm leading-relaxed text-muted">
          {t('usersScreen.accountsAreCreatedHereThere')}
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
                {appliedSearch ? t('usersScreen.noUserMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('usersScreen.noUserRegisteredYet')}
              </p>
            </div>
          )}

          {!loading && !loadError && data.items.length > 0 && (
            <table className="w-full border-collapse text-sm">
              <caption className="sr-only">{t('usersScreen.registeredUsers')}</caption>
              <thead>
                <tr className="border-b border-line bg-cream-soft text-left">
                  <Th>{t('common.name')}</Th>
                  <Th>{t('usersScreen.username')}</Th>
                  <Th>{t('usersScreen.role')}</Th>
                  <Th>{t('common.status')}</Th>
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
                            {user.active ? t('common.active') : t('common.inactive')}
                          </Badge>
                          {user.mustChangePassword && <Badge tone="warning">{t('usersScreen.temporaryPassword')}</Badge>}
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
                            {t('common.edit')}
                          </Button>

                          <Button
                            variant="outline"
                            className="px-3 py-1.5"
                            disabled={busy}
                            onClick={() => resetPassword(user)}
                          >
                            {t('usersScreen.resetPassword')}
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
                            title={user.active && isSelf ? t('usersScreen.youCannotDeactivateYourOwn') : undefined}
                            onClick={() =>
                              runAction(user.id, () => users.setActive(user.id, !user.active))
                            }
                          >
                            {user.active ? t('common.deactivate') : t('common.activate')}
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
            noun="user"
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
        employeeOptions={employeeOptions}
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
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('usersScreen.loadingUsers')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
