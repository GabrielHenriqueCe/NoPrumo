import { useEffect, useRef, useState } from 'react'
import { formatDate, todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'
import { formatSignedAt, itemStatusBadge } from './ppeLabels'
import { useT } from '../../preferences/preferencesContext'

// The receipt: items, their status and the employee's signature.
export function PpeIssueDialog({ open, issue, canManage, onClose, onChanged }) {
  const t = useT()
  const [returningItemId, setReturningItemId] = useState(null)
  const [actionError, setActionError] = useState(null)

  useEffect(() => {
    if (!open) return
    setReturningItemId(null)
    setActionError(null)
  }, [open, issue?.id])

  if (!issue) return null

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title={t('ppeIssueDialog.ppeIssueEmployeename', { employeeName: issue.employeeName })}
      description={`${formatDate(issue.date)}${issue.projectName ? ` · ${issue.projectName}` : ''}${
        issue.issuedByName ? t('ppeIssueDialog.issuedByIssuedbyname', { issuedByName: issue.issuedByName }) : ''
      }`}
    >
      <div className="flex flex-col gap-5 p-6">
        {issue.notes && <p className="text-sm text-muted">{issue.notes}</p>}

        <table className="w-full border-collapse text-sm">
          <caption className="sr-only">{t('ppeIssueDialog.itemsOfThisPpeIssue')}</caption>
          <thead>
            <tr className="border-b border-line text-left">
              <HeaderCell>{t('ppeIssueDialog.item')}</HeaderCell>
              <HeaderCell>{t('ppeIssueDialog.quantity')}</HeaderCell>
              <HeaderCell>{t('common.status')}</HeaderCell>
              {canManage && <th className="py-2" />}
            </tr>
          </thead>
          <tbody>
            {issue.items.map((item) => {
              const badge = itemStatusBadge(item.status)
              const returning = returningItemId === item.id

              return (
                <ItemRows
                  key={item.id}
                  issue={issue}
                  item={item}
                  badge={badge}
                  canManage={canManage}
                  returning={returning}
                  onStartReturning={() => setReturningItemId(item.id)}
                  onStopReturning={() => setReturningItemId(null)}
                  onChanged={(updated) => {
                    setReturningItemId(null)
                    setActionError(null)
                    onChanged(updated)
                  }}
                  onError={setActionError}
                />
              )
            })}
          </tbody>
        </table>

        {actionError && (
          <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
            {actionError}
          </p>
        )}

        <section className="border-t border-line-soft pt-4">
          <h3 className="label mb-2">{t('ppeIssueDialog.signature')}</h3>

          {issue.signed ? (
            <p className="text-sm">
              <Badge tone="active">{t('ppeIssueDialog.signed')}</Badge>{' '}
              <span className="text-muted">{t('ppeIssueDialog.on')} {formatSignedAt(issue.signedAt)}</span>
            </p>
          ) : canManage ? (
            <SignForm issue={issue} onSigned={onChanged} />
          ) : (
            <p className="text-sm">
              <Badge tone="warning">{t('ppeIssueDialog.pendingSignature')}</Badge>
            </p>
          )}
        </section>

        <div className="flex items-center gap-2.5">
          <Button type="button" variant="outline" onClick={onClose}>
            {t('ppeIssueDialog.done')}
          </Button>
        </div>
      </div>
    </Dialog>
  )
}

function ItemRows({ issue, item, badge, canManage, returning, onStartReturning, onStopReturning, onChanged, onError }) {
  const t = useT()
  const { ppeIssues } = useContainer()
  const [busy, setBusy] = useState(false)

  const markLost = async () => {
    setBusy(true)
    onError(null)

    try {
      onChanged(await ppeIssues.markItemLost(issue.id, item.id))
    } catch (error) {
      onError(error.message ?? t('ppeIssueDialog.couldNotMarkTheItem'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <>
      <tr className={returning ? 'bg-cream-soft' : 'border-b border-line-soft'}>
        <td className="py-2.5 pr-3">
          <div className="font-medium">{item.itemName}</div>
          <div className="text-[12px] text-muted">{item.ca ? `CA ${item.ca}` : t('ppeIssueDialog.noCa')}</div>
        </td>
        <td className="py-2.5 pr-3 whitespace-nowrap">
          {item.quantity} {item.unit}
        </td>
        <td className="py-2.5 pr-3">
          <Badge tone={badge.tone}>{t(badge.labelKey)}</Badge>
          {item.returnDate && <div className="text-[12px] text-muted">{formatDate(item.returnDate)}</div>}
        </td>
        {canManage && (
          <td className="py-2.5 text-right">
            {item.status === 'issued' && !returning && (
              <div className="flex justify-end gap-2">
                <Button variant="outline" className="px-3 py-1.5" disabled={busy} onClick={onStartReturning}>
                  {t('ppeIssueDialog.returned')}
                </Button>
                <Button variant="danger" className="px-3 py-1.5" busy={busy} busyLabel={t('common.saving')} onClick={markLost}>
                  {t('ppeIssueDialog.lost')}
                </Button>
              </div>
            )}
          </td>
        )}
      </tr>

      {returning && (
        <tr className="border-b border-line-soft bg-cream-soft">
          <td colSpan={4} className="px-3 pt-1 pb-3">
            <ReturnForm issue={issue} item={item} onReturned={onChanged} onCancel={onStopReturning} />
          </td>
        </tr>
      )}
    </>
  )
}

function ReturnForm({ issue, item, onReturned, onCancel }) {
  const t = useT()
  const { ppeIssues } = useContainer()
  const [returnDate, setReturnDate] = useState(todayIso)
  const [error, setError] = useState(null)
  const [busy, setBusy] = useState(false)
  const input = useRef(null)

  useEffect(() => {
    input.current?.focus()
  }, [])

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setError(null)

    try {
      // No setBusy(false) on success: this form unmounts with its row.
      onReturned(await ppeIssues.returnItem(issue.id, item.id, returnDate || null))
    } catch (failure) {
      setError(failure.fieldErrors?.returnDate ?? failure.message ?? t('ppeIssueDialog.couldNotRegisterTheReturn'))
      setBusy(false)
    }
  }

  return (
    <form
      onSubmit={submit}
      noValidate
      onKeyDown={(event) => {
        // Without stopPropagation, Escape would also close the whole dialog.
        if (event.key === 'Escape') {
          event.stopPropagation()
          onCancel()
        }
      }}
      className="flex flex-wrap items-start gap-2.5"
    >
      <TextField
        ref={input}
        label={t('ppeIssueDialog.returnDate')}
        name="returnDate"
        type="date"
        required
        min={item.issueDate ?? undefined}
        value={returnDate}
        onChange={(event) => {
          setReturnDate(event.target.value)
          setError(null)
        }}
        error={error}
        className="w-48"
      />

      <div className="flex gap-2 pt-5">
        <Button type="submit" busy={busy} busyLabel={t('common.saving')} className="px-3 py-2">
          {t('ppeIssueDialog.confirmReturn')}
        </Button>
        <Button type="button" variant="outline" onClick={onCancel} disabled={busy} className="px-3 py-2">
          {t('common.cancel')}
        </Button>
      </div>
    </form>
  )
}

function SignForm({ issue, onSigned }) {
  const t = useT()
  const { ppeIssues } = useContainer()
  const [typedName, setTypedName] = useState('')
  const [error, setError] = useState(null)
  const [busy, setBusy] = useState(false)

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setError(null)

    try {
      onSigned(await ppeIssues.sign(issue.id, typedName))
      setTypedName('')
    } catch (failure) {
      setError(failure.fieldErrors?.typedName ?? failure.message ?? t('ppeIssueDialog.couldNotSignTheReceipt'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <form onSubmit={submit} noValidate className="flex flex-wrap items-start gap-2.5">
      <TextField
        label={t('ppeIssueDialog.employeeSFullName')}
        name="typedName"
        autoComplete="off"
        required
        value={typedName}
        onChange={(event) => {
          setTypedName(event.target.value)
          setError(null)
        }}
        error={error}
        hint={t('ppeIssueDialog.employeenameTypesTheirNameTo', { employeeName: issue.employeeName })}
        className="min-w-0 flex-1"
      />

      <div className="pt-5">
        <Button type="submit" busy={busy} busyLabel={t('ppeIssueDialog.signing')} className="px-3 py-2">
          {t('ppeIssueDialog.sign')}
        </Button>
      </div>
    </form>
  )
}

function HeaderCell({ children }) {
  return <th className="label py-2 pr-3 font-normal">{children}</th>
}
