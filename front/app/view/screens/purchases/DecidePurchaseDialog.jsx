import { useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

export function DecidePurchaseDialog({ open, request, onClose, onDecided }) {
  const t = useT()
  const [approved, setApproved] = useState(true)
  const [rejectionReason, setRejectionReason] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState(null)

  if (!request) return null

  const handleSubmit = async (e) => {
    e.preventDefault()
    setBusy(true)
    setError(null)

    if (!approved && !rejectionReason.trim()) {
      setError('Please provide a reason for rejecting the request.')
      setBusy(false)
      return
    }

    try {
      await onDecided({
        approved,
        rejectionReason: approved ? undefined : rejectionReason.trim(),
      })
      onClose()
    } catch (err) {
      setError(err.message || t('decidePurchaseDialog.couldNotProcessDecision'))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title={t('decidePurchaseDialog.decidePurchaseRequest')}
      description={t('decidePurchaseDialog.requestIdProjectProjectname', { id: request.id, projectName: request.projectName })}
    >
      <form onSubmit={handleSubmit} className="p-6">
        {error && (
          <div className="mb-4 rounded-2xl bg-danger-wash p-3 text-sm text-danger">
            {error}
          </div>
        )}

        <div className="mb-4 flex gap-4">
          <label className="flex items-center gap-2 text-sm font-medium">
            <input
              type="radio"
              name="decision"
              checked={approved}
              onChange={() => setApproved(true)}
            />
            {t('decidePurchaseDialog.approveRequest')}
          </label>
          <label className="flex items-center gap-2 text-sm font-medium text-danger">
            <input
              type="radio"
              name="decision"
              checked={!approved}
              onChange={() => setApproved(false)}
            />
            {t('decidePurchaseDialog.rejectRequest')}
          </label>
        </div>

        {!approved && (
          <div className="mb-4">
            <TextField
              label={t('decidePurchaseDialog.rejectionReason')}
              placeholder={t('decidePurchaseDialog.exItemOutOfSpecification')}
              value={rejectionReason}
              onChange={(e) => setRejectionReason(e.target.value)}
              required
            />
          </div>
        )}

        <div className="mt-6 flex justify-end gap-3 border-t border-line-soft pt-4">
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            {t('common.cancel')}
          </Button>
          <Button
            type="submit"
            variant={approved ? 'primary' : 'danger'}
            disabled={busy}
          >
            {busy ? t('decidePurchaseDialog.processing') : approved ? t('decidePurchaseDialog.confirmApproval') : t('decidePurchaseDialog.confirmRejection')}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
