import { useCallback, useEffect, useState } from 'react'
import { formatDate } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { useSession } from '../../providers/sessionContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Pagination } from '../../ui/Pagination'
import { PpeIssueDialog } from './PpeIssueDialog'
import { PpeIssueFormDialog } from './PpeIssueFormDialog'
import { useT } from '../../preferences/preferencesContext'

// The "PPE" tab of a project. Foreman and warehouse keeper only read;
// issuing, signing and closing items is manage_safety.
const PAGE_SIZE = 10
const SEARCH_DEBOUNCE_MS = 350
const EMPTY_PAGE = { items: [], page: 1, totalPages: 1, total: 0 }

export function ProjectPpePanel({ projectId }) {
  const t = useT()
  const { ppeIssues } = useContainer()
  const { can } = useSession()
  const canManage = can('manage_safety')

  const [data, setData] = useState(EMPTY_PAGE)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState(null)
  const [reloadToken, setReloadToken] = useState(0)

  const [formOpen, setFormOpen] = useState(false)
  const [openIssue, setOpenIssue] = useState(null)

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

    ppeIssues
      .list({ page, size: PAGE_SIZE, search: appliedSearch, projectId }, { signal: controller.signal })
      .then((result) => setData(result))
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('projectPpePanel.couldNotLoadThePpe'))
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [ppeIssues, projectId, page, appliedSearch, reloadToken, t])

  // Stable on purpose: the Dialog refocuses its first field whenever onClose changes.
  const closeForm = useCallback(() => setFormOpen(false), [])
  const closeIssue = useCallback(() => setOpenIssue(null), [])

  const issued = (issue) => {
    setFormOpen(false)
    reload()
    // Straight to the receipt: the employee signs it right after receiving.
    setOpenIssue(issue)
  }

  const issueChanged = (issue) => {
    setOpenIssue(issue)
    reload()
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-wrap items-center gap-3">
        <p className="max-w-[44em] text-sm leading-relaxed text-muted">
          {t('projectPpePanel.ppeHandedOutOnThis')}
        </p>

        <div className="ml-auto flex items-center gap-3">
          <label className="sr-only" htmlFor={`ppe-search-${projectId}`}>
            {t('projectPpePanel.searchPpeIssues')}
          </label>
          <input
            id={`ppe-search-${projectId}`}
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t('projectPpePanel.searchByEmployeeOrItem')}
            className="w-64 h-11 rounded-full border border-line bg-surface px-5 text-sm outline-none focus:border-accent"
          />

          {canManage && <Button onClick={() => setFormOpen(true)}>{t('projectPpePanel.newPpeIssue')}</Button>}
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
            <p className="text-sm text-muted">
              {appliedSearch ? t('projectPpePanel.noPpeIssueMatchesAppliedsearch', { appliedSearch: appliedSearch }) : t('projectPpePanel.noPpeIssuedOnThis')}
            </p>
          </div>
        )}

        {!loading && !loadError && data.items.length > 0 && (
          <table className="w-full border-collapse text-sm">
            <caption className="sr-only">{t('projectPpePanel.ppeIssuedOnThisProject')}</caption>
            <thead>
              <tr className="border-b border-line bg-cream-soft text-left">
                <HeaderCell>{t('projectPpePanel.date')}</HeaderCell>
                <HeaderCell>{t('projectPpePanel.employee')}</HeaderCell>
                <HeaderCell>{t('projectPpePanel.items')}</HeaderCell>
                <HeaderCell>{t('projectPpePanel.signature')}</HeaderCell>
                <th className="px-4 py-3" />
              </tr>
            </thead>

            <tbody>
              {data.items.map((issue) => (
                <tr key={issue.id} className="border-b border-line-soft last:border-0">
                  <td className="px-4 py-3 whitespace-nowrap">{formatDate(issue.date)}</td>

                  <td className="px-4 py-3">
                    <div className="font-medium">{issue.employeeName}</div>
                    {issue.registrationNumber && (
                      <div className="text-[12.5px] text-muted">{issue.registrationNumber}</div>
                    )}
                  </td>

                  <td className="px-4 py-3">
                    {issue.items.map((item) => (
                      <div key={item.id}>
                        {item.itemName} × {item.quantity} {item.unit}
                      </div>
                    ))}
                  </td>

                  <td className="px-4 py-3">
                    <Badge tone={issue.signed ? 'active' : 'warning'}>
                      {issue.signed ? t('projectPpePanel.signed') : t('projectPpePanel.pendingSignature')}
                    </Badge>
                  </td>

                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end">
                      <Button
                        variant="outline"
                        className="px-3 py-1.5"
                        aria-label={t('projectPpePanel.openThePpeIssueOf', { employeeName: issue.employeeName, value: formatDate(issue.date) })}
                        onClick={() => setOpenIssue(issue)}
                      >
                        {t('projectPpePanel.open')}
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {!loading && !loadError && (
        <Pagination
          noun="PPE issue"
          page={data.page}
          totalPages={data.totalPages}
          total={data.total}
          onChange={setPage}
        />
      )}

      {canManage && (
        <PpeIssueFormDialog open={formOpen} projectId={projectId} onClose={closeForm} onIssued={issued} />
      )}

      <PpeIssueDialog
        open={Boolean(openIssue)}
        issue={openIssue}
        canManage={canManage}
        onClose={closeIssue}
        onChanged={issueChanged}
      />
    </div>
  )
}

function HeaderCell({ children }) {
  return <th className="label px-4 py-3 font-normal">{children}</th>
}

function TableSkeleton() {
  const t = useT()
  return (
    <div className="p-4" role="status" aria-label={t('projectPpePanel.loadingPpeIssues')}>
      {Array.from({ length: 4 }).map((_, index) => (
        <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
      ))}
    </div>
  )
}
