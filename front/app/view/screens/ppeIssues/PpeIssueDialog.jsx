import { useEffect, useRef, useState } from 'react'
import { formatDate, todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'
import { formatSignedAt, itemStatusBadge } from './ppeLabels'

// The receipt: items, their status and the employee's signature.
export function PpeIssueDialog({ open, issue, canManage, onClose, onChanged }) {
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
      title={`PPE issue · ${issue.employeeName}`}
      description={`${formatDate(issue.date)}${issue.projectName ? ` · ${issue.projectName}` : ''}${
        issue.issuedByName ? ` · issued by ${issue.issuedByName}` : ''
      }`}
    >
      <div className="flex flex-col gap-5 p-6">
        {issue.notes && <p className="text-sm text-muted">{issue.notes}</p>}

        <table className="w-full border-collapse text-sm">
          <caption className="sr-only">Items of this PPE issue</caption>
          <thead>
            <tr className="border-b border-line text-left">
              <HeaderCell>Item</HeaderCell>
              <HeaderCell>Quantity</HeaderCell>
              <HeaderCell>Status</HeaderCell>
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
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {actionError}
          </p>
        )}

        <section className="border-t border-line-soft pt-4">
          <h3 className="label mb-2">Signature</h3>

          {issue.signed ? (
            <p className="text-sm">
              <Badge tone="active">Signed</Badge>{' '}
              <span className="text-muted">on {formatSignedAt(issue.signedAt)}</span>
            </p>
          ) : canManage ? (
            <SignForm issue={issue} onSigned={onChanged} />
          ) : (
            <p className="text-sm">
              <Badge tone="warning">Pending signature</Badge>
            </p>
          )}
        </section>

        <div className="flex items-center gap-2.5">
          <Button type="button" variant="outline" onClick={onClose}>
            Done
          </Button>
        </div>
      </div>
    </Dialog>
  )
}

function ItemRows({ issue, item, badge, canManage, returning, onStartReturning, onStopReturning, onChanged, onError }) {
  const { ppeIssues } = useContainer()
  const [busy, setBusy] = useState(false)

  const markLost = async () => {
    setBusy(true)
    onError(null)

    try {
      onChanged(await ppeIssues.markItemLost(issue.id, item.id))
    } catch (error) {
      onError(error.message ?? 'Could not mark the item as lost.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <>
      <tr className={returning ? 'bg-cream-soft' : 'border-b border-line-soft'}>
        <td className="py-2.5 pr-3">
          <div className="font-medium">{item.itemName}</div>
          <div className="text-[12px] text-muted">{item.ca ? `CA ${item.ca}` : 'No CA'}</div>
        </td>
        <td className="py-2.5 pr-3 whitespace-nowrap">
          {item.quantity} {item.unit}
        </td>
        <td className="py-2.5 pr-3">
          <Badge tone={badge.tone}>{badge.label}</Badge>
          {item.returnDate && <div className="text-[12px] text-muted">{formatDate(item.returnDate)}</div>}
        </td>
        {canManage && (
          <td className="py-2.5 text-right">
            {item.status === 'issued' && !returning && (
              <div className="flex justify-end gap-2">
                <Button variant="outline" className="px-3 py-1.5" disabled={busy} onClick={onStartReturning}>
                  Returned
                </Button>
                <Button variant="danger" className="px-3 py-1.5" busy={busy} busyLabel="Saving…" onClick={markLost}>
                  Lost
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
      setError(failure.fieldErrors?.returnDate ?? failure.message ?? 'Could not register the return.')
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
        label="Return date"
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
        <Button type="submit" busy={busy} busyLabel="Saving…" className="px-3 py-2">
          Confirm return
        </Button>
        <Button type="button" variant="outline" onClick={onCancel} disabled={busy} className="px-3 py-2">
          Cancel
        </Button>
      </div>
    </form>
  )
}

function SignForm({ issue, onSigned }) {
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
      setError(failure.fieldErrors?.typedName ?? failure.message ?? 'Could not sign the receipt.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <form onSubmit={submit} noValidate className="flex flex-wrap items-start gap-2.5">
      <TextField
        label="Employee's full name"
        name="typedName"
        autoComplete="off"
        required
        value={typedName}
        onChange={(event) => {
          setTypedName(event.target.value)
          setError(null)
        }}
        error={error}
        hint={`${issue.employeeName} types their name to confirm receiving the items above.`}
        className="min-w-0 flex-1"
      />

      <div className="pt-5">
        <Button type="submit" busy={busy} busyLabel="Signing…" className="px-3 py-2">
          Sign
        </Button>
      </div>
    </form>
  )
}

function HeaderCell({ children }) {
  return <th className="label py-2 pr-3 font-normal">{children}</th>
}
