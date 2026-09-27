import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

/*
  Create or edit a stock category.

  tracksProjectBalance and requiresReturn are the two flags that decide the
  whole flow: consumable material has a balance per project, PPE goes from
  warehouse to employee, tools go out and come back. The three base
  categories come from the seed; this dialog only adds more, it never
  touches those flags for rows the seed already created (nothing here
  prevents editing them, but the team should agree before anyone does).
*/

const YES_NO_OPTIONS = [
    { value: 'true', label: 'Yes' },
    { value: 'false', label: 'No' },
]

const EMPTY = { name: '', tracksProjectBalance: 'false', requiresReturn: 'false' }

export function StockCategoryFormDialog({ open, category, onClose, onSubmit }) {
    const editing = Boolean(category)

    const [form, setForm] = useState(EMPTY)
    const [fieldErrors, setFieldErrors] = useState({})
    const [formError, setFormError] = useState(null)
    const [busy, setBusy] = useState(false)

    useEffect(() => {
        if (!open) return
        setForm(
            category
                ? {
                    name: category.name ?? '',
                    tracksProjectBalance: String(Boolean(category.tracksProjectBalance)),
                    requiresReturn: String(Boolean(category.requiresReturn)),
                }
                : EMPTY,
        )
        setFieldErrors({})
        setFormError(null)
    }, [open, category])

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
                tracksProjectBalance: form.tracksProjectBalance === 'true',
                requiresReturn: form.requiresReturn === 'true',
            })
        } catch (error) {
            // The API is the one that validates. The form only shows where it hurt.
            if (error.isValidation) setFieldErrors(error.fieldErrors)
            else setFormError(error.message ?? 'Could not save the category.')
        } finally {
            setBusy(false)
        }
    }

    return (
        <Dialog
            open={open}
            onClose={busy ? undefined : onClose}
            title={editing ? 'Edit stock category' : 'New stock category'}
            description="Consumable material, PPE and tool are the three flows the system is built around."
        >
            <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
                <TextField
                    label="Name"
                    name="name"
                    placeholder="e.g. Consumable material"
                    required
                    value={form.name}
                    onChange={update('name')}
                    error={fieldErrors.name}
                />

                <SelectField
                    label="Tracks project balance"
                    name="tracksProjectBalance"
                    required
                    value={form.tracksProjectBalance}
                    onChange={update('tracksProjectBalance')}
                    error={fieldErrors.tracksProjectBalance}
                    options={YES_NO_OPTIONS}
                />

                <SelectField
                    label="Requires return"
                    name="requiresReturn"
                    required
                    value={form.requiresReturn}
                    onChange={update('requiresReturn')}
                    error={fieldErrors.requiresReturn}
                    options={YES_NO_OPTIONS}
                />

                {formError && (
                    <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
                        {formError}
                    </p>
                )}

                <div className="mt-2 flex items-center gap-2.5">
                    <Button type="submit" busy={busy} busyLabel="Saving…">
                        {editing ? 'Save changes' : 'Create category'}
                    </Button>
                    <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
                        Cancel
                    </Button>
                </div>
            </form>
        </Dialog>
    )
}