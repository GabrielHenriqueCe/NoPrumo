import { useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

export function RegisterPurchaseDialog({ open, request, onClose, onPurchased }) {
  const { suppliers } = useContainer()

  const [supplierList, setSupplierList] = useState([])
  const [supplierId, setSupplierId] = useState('')
  const [invoiceNumber, setInvoiceNumber] = useState('')
  const [itemPurchases, setItemPurchases] = useState([])
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    if (!open || !request) return

    setSupplierId('')
    setInvoiceNumber('')
    setError(null)

    const initialItems = (request.items || []).map((item) => {
      const remaining = Math.max(0, item.requestedQuantity - item.fulfilledQuantity)
      return {
        purchaseRequestItemId: item.id,
        stockItemName: item.stockItemName,
        unit: item.unit,
        remaining,
        quantity: String(remaining),
        unitCost: '',
      }
    })
    setItemPurchases(initialItems)

    const controller = new AbortController()
    suppliers
      .list({ size: 100 }, { signal: controller.signal })
      .then((data) => setSupplierList(data.items || []))
      .catch(() => {})

    return () => controller.abort()
  }, [open, request, suppliers])

  if (!request) return null

  const handleItemCostChange = (index, value) => {
    setItemPurchases((current) => {
      const copy = [...current]
      copy[index] = { ...copy[index], unitCost: value }
      return copy
    })
  }

  const handleItemQtyChange = (index, value) => {
    setItemPurchases((current) => {
      const copy = [...current]
      copy[index] = { ...copy[index], quantity: value }
      return copy
    })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setBusy(true)
    setError(null)

    const validItems = itemPurchases.filter(
      (item) => Number(item.quantity) > 0 && Number(item.unitCost) >= 0 && item.unitCost !== '',
    )

    if (validItems.length === 0) {
      setError('Preencha ao menos um item com quantidade e preço unitário válidos.')
      setBusy(false)
      return
    }

    try {
      await onPurchased({
        supplierId: supplierId ? Number(supplierId) : undefined,
        invoiceNumber: invoiceNumber.trim() || undefined,
        items: validItems.map((item) => ({
          purchaseRequestItemId: item.purchaseRequestItemId,
          quantity: Number(item.quantity),
          unitCost: Number(item.unitCost),
          supplierId: supplierId ? Number(supplierId) : undefined,
          invoiceNumber: invoiceNumber.trim() || undefined,
        })),
      })
      onClose()
    } catch (err) {
      setError(err.message || 'Erro ao registrar compra.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Registrar Compra Realizada"
      description={`Solicitação #${request.id} · Obra: ${request.projectName}`}
    >
      <form onSubmit={handleSubmit} className="p-6">
        {error && (
          <div className="mb-4 border border-danger-soft bg-[#faefea] p-3 text-sm text-danger">
            {error}
          </div>
        )}

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <SelectField
            label="Fornecedor"
            value={supplierId}
            onChange={(e) => setSupplierId(e.target.value)}
          >
            <option value="">Selecione o fornecedor (opcional)...</option>
            {supplierList.map((sup) => (
              <option key={sup.id} value={sup.id}>
                {sup.name}
              </option>
            ))}
          </SelectField>

          <TextField
            label="Número da Nota Fiscal"
            placeholder="Ex: NF-e 12345"
            value={invoiceNumber}
            onChange={(e) => setInvoiceNumber(e.target.value)}
          />
        </div>

        <div className="mt-6 border-t border-line-soft pt-4">
          <div className="label mb-3">Itens e Valores da Compra</div>
          <div className="space-y-3">
            {itemPurchases.map((item, index) => (
              <div key={item.purchaseRequestItemId} className="rounded border border-line bg-cream-soft p-3">
                <div className="mb-2 font-medium text-graphite">
                  {item.stockItemName} (Pendente: {item.remaining} {item.unit})
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <TextField
                    label={`Qtd comprada (${item.unit})`}
                    type="number"
                    step="0.01"
                    min="0.01"
                    value={item.quantity}
                    onChange={(e) => handleItemQtyChange(index, e.target.value)}
                    required
                  />
                  <TextField
                    label="Preço Unitário (R$)"
                    type="number"
                    step="0.01"
                    min="0"
                    placeholder="0,00"
                    value={item.unitCost}
                    onChange={(e) => handleItemCostChange(index, e.target.value)}
                    required
                  />
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="mt-6 flex justify-end gap-3 border-t border-line-soft pt-4">
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancelar
          </Button>
          <Button type="submit" variant="primary" disabled={busy}>
            {busy ? 'Gravando...' : 'Confirmar e Dar Entrada'}
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
