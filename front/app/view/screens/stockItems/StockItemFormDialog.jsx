import { useEffect, useMemo, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

/*
  Create or edit a stock item.

  Ca/CaExpiryDate only make sense for PPE — the form shows them only when the
  selected group belongs to the PPE category, using groups.stockCategoryName.

  ReferencePrice only appears here for accounts with view_finance. The API is
  the real gate (it silently ignores the field otherwise); this just avoids
  showing a control that would do nothing.
*/

const EMPTY = {
    stockGroupId: '',
    code: '',
    name: '',
    unit: '',
    minQuantity: '',
    ca: '',
    caExpiryDate: '',
    referencePrice: '',
}

export function StockItemFormDialog({ open, item, groups, canSeePrice, onClose, onSubmit }) {
    const editing = Boolean(item)

    const [form, setForm] = useState(EMPTY)
    const [fieldErrors, setFieldErrors] = useState({})
    const [formError, setFormError] = useState(null)
    const [busy, setBusy] = useState(false)

    useEffect(() => {
        if (!open) return
        setForm(
            item
                ? {
                    stockGroupId: String(item.stockGroupId ?? ''),
                    code: item.code ?? '',
                    name: item.name ?? '',
                    unit: item.unit ?? '',
                    minQuantity: String(item.minQuantity ?? ''),
                    ca: item.ca ?? '',
                    caExpiryDate: item.caExpiryDate ?? '',
                    referencePrice: item.referencePrice != null ? String(item.referencePrice) : '',
                }
                : EMPTY,
        )
        setFieldErrors({})
        setFormError(null)
    }, [open, item])

    const selectedGroup = groups.find((group) => String(group.id) === form.stockGroupId)
    const isPpe = selectedGroup?.stockCategoryName === 'PPE'

    const update = (field) => (event) => {
        setForm((current) => ({ ...current, [field]: event.target.value }))
        setFieldErrors((current) => ({ ...current, [field]: undefined }))
    }

    const submit = async (event) => {
        event.preventDefault()
        setBusy(true)
        setFieldErrors({})
        setFormError(null)

        try {
            await onSubmit({
                stockGroupId: Number(form.stockGroupId),
                code: form.code.trim() || null,
                name: form.name.trim(),
                unit: form.unit.trim(),
                minQuantity: Number(form.minQuantity),
                ca: isPpe ? form.ca.trim() || null : null,
                caExpiryDate: isPpe ? form.caExpiryDate || null : null,
                // Sending it even without view_finance is harmless: the API ignores
                // the field for accounts that cannot see the price.
                referencePrice: canSeePrice && form.referencePrice !== '' ? Number(form.referencePrice) : null,
            })
        } catch (error) {
            if (error.isValidation) setFieldErrors(error.fieldErrors)
            else setFormError(error.message ?? 'Could not save the item.')
        } finally {
            setBusy(false)
        }
    }

    return (
        <Dialog
            open={open}
            onClose={busy ? undefined : onClose}
            title={editing ? 'Edit stock item' : 'New stock item'}
            description="The item belongs to a group, which belongs to a category."
        >
            <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
                <SelectField
                    label="Group"
                    name="stockGroupId"
                    required
                    value={form.stockGroupId}
                    onChange={update('stockGroupId')}
                    error={fieldErrors.stockGroupId}
                    placeholder="Select a group…"
                    options={groups.map((group) => ({
                        value: group.id,
                        label: `${group.name} (${group.stockCategoryName})`,
                    }))}
                />

                <TextField
                    label="Name"
                    name="name"
                    placeholder="e.g. White helmet"
                    required
                    value={form.name}
                    onChange={update('name')}
                    error={fieldErrors.name}
                />

                <div className="grid grid-cols-2 gap-4">
                    <TextField
                        label="Code"
                        name="code"
                        placeholder="e.g. CAP-01"
                        value={form.code}
                        onChange={update('code')}
                        error={fieldErrors.code}
                        hint="Optional."
                    />

                    <TextField
                        label="Unit"
                        name="unit"
                        placeholder="e.g. un, kg, m"
                        required
                        value={form.unit}
                        onChange={update('unit')}
                        error={fieldErrors.unit}
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <TextField
                        label="Minimum quantity"
                        name="minQuantity"
                        type="number"
                        step="0.01"
                        min="0"
                        required
                        value={form.minQuantity}
                        onChange={update('minQuantity')}
                        error={fieldErrors.minQuantity}
                    />

                    {canSeePrice && (
                        <TextField
                            label="Reference price"
                            name="referencePrice"
                            type="number"
                            step="0.01"
                            min="0"
                            value={form.referencePrice}
                            onChange={update('referencePrice')}
                            error={fieldErrors.referencePrice}
                            hint="Leave blank to keep it at 0."
                        />
                    )}
                </div>

                {isPpe && (
                    <div className="grid grid-cols-2 gap-4 border-t border-line-soft pt-4">
                        <TextField
                            label="CA"
                            name="ca"
                            placeholder="Certificate of Approval"
                            value={form.ca}
                            onChange={update('ca')}
                            error={fieldErrors.ca}
                            hint="Only PPE items have a CA."
                        />

                        <TextField
                            label="CA expiry date"
                            name="caExpiryDate"
                            type="date"
                            value={form.caExpiryDate}
                            onChange={update('caExpiryDate')}
                            error={fieldErrors.caExpiryDate}
                        />
                    </div>
                )}

                {formError && (
                    <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
                        {formError}
                    </p>
                )}

                <div className="mt-2 flex items-center gap-2.5">
                    <Button type="submit" busy={busy} busyLabel="Saving…">
                        {editing ? 'Save changes' : 'Create item'}
                    </Button>
                    <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
                        Cancel
                    </Button>
                </div>
            </form>
        </Dialog>
    )
}