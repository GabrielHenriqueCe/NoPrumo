import { useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

export function PurchaseRequestFormDialog({
  open,
  projectId: fixedProjectId,
  projectOptions = [],
  onClose,
  onSubmitted,
}) {
  const { stockItems, purchaseRequests } = useContainer()

  const [availableItems, setAvailableItems] = useState([])
  const [selectedProjectId, setSelectedProjectId] = useState(fixedProjectId ? String(fixedProjectId) : '')
  const [neededByDate, setNeededByDate] = useState('')
  const [notes, setNotes] = useState('')

  const [items, setItems] = useState([{ stockItemId: '', requestedQuantity: '', unit: '', notes: '' }])
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    if (!open) return

    setSelectedProjectId(fixedProjectId ? String(fixedProjectId) : '')
    setNeededByDate('')
    setNotes('')
    setItems([{ stockItemId: '', requestedQuantity: '', unit: '', notes: '' }])
    setError(null)

    const controller = new AbortController()
    stockItems
      .list({ size: 100 }, { signal: controller.signal })
      .then((data) => {
        setAvailableItems(data.items || [])
      })
      .catch(() => {})

    return () => controller.abort()
  }, [open, fixedProjectId, stockItems])

  const handleItemChange = (index, field, value) => {
    setItems((current) => {
      const updated = [...current]
      const item = { ...updated[index], [field]: value }

      if (field === 'stockItemId') {
        const found = availableItems.find((s) => String(s.id) === String(value))
        if (found) {
          item.unit = found.unit || 'un'
        }
      }

      updated[index] = item
      return updated
    })
  }

  const addItemRow = () => {
    setItems((current) => [...current, { stockItemId: '', requestedQuantity: '', unit: '', notes: '' }])
  }

  const removeItemRow = (index) => {
    setItems((current) => (current.length > 1 ? current.filter((_, i) => i !== index) : current))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setBusy(true)
    setError(null)

    const targetProjectId = fixedProjectId || Number(selectedProjectId)
    if (!targetProjectId) {
      setError('Selecione a obra.')
      setBusy(false)
      return
    }

    const validItems = items.filter((i) => i.stockItemId && Number(i.requestedQuantity) > 0)
    if (validItems.length === 0) {
      setError('Adicione pelo menos um item válido com quantidade maior que zero.')
      setBusy(false)
      return
    }

    try {
      await purchaseRequests.create({
        projectId: targetProjectId,
        neededByDate: neededByDate || undefined,
        notes: notes.trim() || undefined,
        items: validItems.map((i) => ({
          stockItemId: Number(i.stockItemId),
          requestedQuantity: Number(i.requestedQuantity),
          unit: i.unit.trim() || 'un',
          notes: i.notes?.trim() || undefined,
        })),
      })

      onSubmitted?.()
      onClose?.()
    } catch (err) {
      setError(err.message || 'Erro ao registrar solicitação de compra.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Nova Solicitação de Compra"
      description="Solicitação de materiais e suprimentos para a obra (apenas quantidades)."
    >
      <form onSubmit={handleSubmit} className="p-6">
        {error && (
          <div className="mb-4 border border-danger-soft bg-[#faefea] p-3 text-sm text-danger">
            {error}
          </div>
        )}

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {!fixedProjectId && (
            <SelectField
              label="Obra"
              value={selectedProjectId}
              onChange={(e) => setSelectedProjectId(e.target.value)}
              required
            >
              <option value="">Selecione a obra...</option>
              {projectOptions.map((proj) => (
                <option key={proj.id} value={proj.id}>
                  {proj.name} ({proj.code})
                </option>
              ))}
            </SelectField>
          )}

          <TextField
            label="Precisa até"
            type="date"
            value={neededByDate}
            onChange={(e) => setNeededByDate(e.target.value)}
          />

          <div className="sm:col-span-2">
            <TextField
              label="Observações gerais (opcional)"
              placeholder="Ex: Urgência para concretagem de laje"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
            />
          </div>
        </div>

        {/* Items Section */}
        <div className="mt-6 border-t border-line-soft pt-4">
          <div className="mb-2 flex items-center justify-between">
            <span className="label">Itens Solicitados</span>
            <Button type="button" variant="outline" className="px-2.5 py-1 text-xs" onClick={addItemRow}>
              + Adicionar Item
            </Button>
          </div>

          <div className="space-y-3">
            {items.map((row, index) => (
              <div key={index} className="flex flex-col gap-2 rounded border border-line bg-cream-soft p-3 sm:flex-row sm:items-end">
                <div className="flex-1">
                  <SelectField
                    label="Material / Item"
                    value={row.stockItemId}
                    onChange={(e) => handleItemChange(index, 'stockItemId', e.target.value)}
                    required
                  >
                    <option value="">Selecione o item...</option>
                    {availableItems.map((item) => (
                      <option key={item.id} value={item.id}>
                        {item.name} ({item.unit})
                      </option>
                    ))}
                  </SelectField>
                </div>

                <div className="w-full sm:w-28">
                  <TextField
                    label="Quantidade"
                    type="number"
                    step="0.01"
                    min="0.01"
                    value={row.requestedQuantity}
                    onChange={(e) => handleItemChange(index, 'requestedQuantity', e.target.value)}
                    required
                  />
                </div>

                <div className="w-full sm:w-24">
                  <TextField
                    label="Unidade"
                    value={row.unit}
                    onChange={(e) => handleItemChange(index, 'unit', e.target.value)}
                    required
                  />
                </div>

                {items.length > 1 && (
                  <Button
                    type="button"
                    variant="outline"
                    className="px-2.5 py-2 text-xs text-danger hover:border-danger"
                    onClick={() => removeItemRow(index)}
                  >
                    Remover
                  </Button>
                )}
              </div>
            ))}
          </div>
        </div>

        <div className="mt-6 flex justify-end gap-3 border-t border-line-soft pt-4">
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancelar
          </Button>
          <Button type="submit" variant="primary" disabled={busy}>
            {busy ? 'Enviando...' : 'Solicitar Compra'}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
