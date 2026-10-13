import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { usePreferences } from '../../preferences/preferencesContext'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { ScreenHeader } from '../../ui/ScreenHeader'

/*
  Project closing: the Finance numbers of every active project side by side
  (view_finance only). Every value, including the alarm, comes ready from the
  API. The print button uses the browser's own print, which also saves as PDF.
*/

export function ReportsScreen() {
  const { reports } = useContainer()
  const { t, format } = usePreferences()

  const [rows, setRows] = useState(null)
  const [loadError, setLoadError] = useState(null)

  useEffect(() => {
    const controller = new AbortController()

    reports
      .projectClosing({ signal: controller.signal })
      .then(setRows)
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(error.message ?? t('common.couldNotLoad'))
      })

    return () => controller.abort()
  }, [reports, t])

  return (
    <>
      <ScreenHeader eyebrow={t('reports.eyebrow')} title={t('reports.title')} count={rows?.length}>
        <Button variant="outline" onClick={() => window.print()}>
          {t('reports.print')}
        </Button>
      </ScreenHeader>

      <div className="px-10 pt-6 pb-10">
        <p className="mb-5 max-w-[60em] leading-relaxed text-muted">{t('reports.intro')}</p>

        <div className="overflow-x-auto rounded-3xl border border-line bg-surface">
          {loadError && <p className="px-6 py-10 text-center text-danger">{loadError}</p>}

          {!rows && !loadError && (
            <div className="p-4" role="status" aria-label={t('common.loading')}>
              {Array.from({ length: 4 }).map((_, index) => (
                <div key={index} className="mb-2 h-11 animate-pulse rounded-xl bg-sunken last:mb-0" />
              ))}
            </div>
          )}

          {rows && rows.length === 0 && <p className="px-6 py-10 text-center text-muted">{t('reports.empty')}</p>}

          {rows && rows.length > 0 && (
            <table className="w-full border-collapse text-[14.5px]">
              <caption className="sr-only">{t('reports.title')}</caption>
              <thead>
                <tr className="border-b border-line bg-sunken text-left">
                  <HeaderCell>{t('reports.project')}</HeaderCell>
                  <HeaderCell align="right">{t('reports.contract')}</HeaderCell>
                  <HeaderCell align="right">{t('finance.material')}</HeaderCell>
                  <HeaderCell align="right">{t('finance.labor')}</HeaderCell>
                  <HeaderCell align="right">{t('finance.totalCost')}</HeaderCell>
                  <HeaderCell align="right">{t('finance.margin')}</HeaderCell>
                  <HeaderCell align="right">{t('finance.spentVsBuilt')}</HeaderCell>
                </tr>
              </thead>
              <tbody>
                {rows.map((row) => (
                  <tr key={row.projectId} className="border-b border-line-soft last:border-0">
                    <td className="px-4 py-3.5">
                      <Link to={`/projects/${row.projectId}/finance`} className="font-semibold hover:text-accent">
                        {row.name}
                      </Link>
                      <div className="mt-1 flex flex-wrap items-center gap-2">
                        <span className="font-mono text-[12.5px] text-muted">{row.code}</span>
                        <span className="text-[12.5px] text-muted">{t(`projectStatus.${row.status}`)}</span>
                        {row.late && <Badge tone="late">{t('common.late')}</Badge>}
                      </div>
                    </td>
                    <Money value={row.revisedContractAmount} format={format} />
                    <Money value={row.materialCost} format={format} />
                    <Money value={row.laborCost} format={format} />
                    <Money value={row.totalCost} format={format} />
                    <Money value={row.margin} format={format} />
                    <td
                      className={`px-4 py-3.5 text-right font-mono whitespace-nowrap ${row.costAheadOfProgress ? 'text-danger' : ''}`}
                    >
                      {format.percent(row.spentPercentage)} / {format.percent(row.progressPercentage)}
                      {row.costAheadOfProgress && <span className="sr-only"> · {t('finance.aheadWarning')}</span>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </>
  )
}

function HeaderCell({ children, align = 'left' }) {
  return (
    <th scope="col" className={`label px-4 py-3 font-normal ${align === 'right' ? 'text-right' : ''}`}>
      {children}
    </th>
  )
}

function Money({ value, format }) {
  return <td className="px-4 py-3.5 text-right font-mono whitespace-nowrap">{format.money(value)}</td>
}
