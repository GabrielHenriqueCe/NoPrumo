import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

/*
  Create or edit a stock group. A group belongs to one category, and the API
  refuses to move a group that already has items to a different category —
  the form does not need to repeat that check, just show the error it sends.
*/

const EMPTY = { name: '', stockCategoryId: '' }

export function StockGroupFormDialog({ open, group, categories, onClose, onSubmit }) {
    const editing = Boolean(group)

    const [form, setForm] = useState(EMPTY)
    const [fieldErrors, setFieldErrors] = useState({})
    const [formError, setFormError] = useState(null)
    const [busy, setBusy] = useState(false)

    useEffect(() => {
        if (!open) return
        setForm(
            group
                ? { name: group.name ?? '', stockCategoryId: String(group.stockCategoryId ?? '') }
                : EMPTY,
        )
        setFieldErrors({})
        setFormError(null)
    }, [open, group])

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
                name: form.name.trim(),
                stockCategoryId: Number(form.stockCategoryId),
            })
        } catch (error) {
            // The API is the one that validates. The form only shows where it hurt.
            if (error.isValidation) setFieldErrors(error.fieldErrors)
            else setFormError(error.message ?? 'Could not save the group.')
        } finally {
            setBusy(false)
        }
    }

    return (
        <Dialog
            open={open}
            onClose={busy ? undefined : onClose}
            title={editing ? 'Edit stock group' : 'New stock group'}
            description="A group belongs to one category, like a shelf inside an aisle."
        >
            <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
                <TextField
                    label="Name"
                    name="name"
                    placeholder="e.g. Helmets"
                    required
                    value={form.name}
                    onChange={update('name')}
                    error={fieldErrors.name}
                />

                <SelectField
                    label="Category"
                    name="stockCategoryId"
                    required
                    value={form.stockCategoryId}
                    onChange={update('stockCategoryId')}
                    error={fieldErrors.stockCategoryId}
                    placeholder="Select a category…"
                    options={categories.map((category) => ({ value: category.id, label: category.name }))}
                />

                {formError && (
                    <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
                        {formError}
                    </p>
                )}

                <div className="mt-2 flex items-center gap-2.5">
                    <Button type="submit" busy={busy} busyLabel="Saving…">
                        {editing ? 'Save changes' : 'Create group'}
                    </Button>
                    <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
                        Cancel
                    </Button>
                </div>
            </form>
        </Dialog>
    )
}