import { useEffect, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

/*
  Create or edit a stock group. A group belongs to one category, and the API
  refuses to move a group that already has items to a different category —
  the form does not need to repeat that check, just show the error it sends.
*/

const EMPTY = { name: '', stockCategoryId: '' }

export function StockGroupFormDialog({ open, group, categories, onClose, onSubmit }) {
    const t = useT()
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
            else setFormError(error.message ?? t('stockGroupFormDialog.couldNotSaveTheGroup'))
        } finally {
            setBusy(false)
        }
    }

    return (
        <Dialog
            open={open}
            onClose={busy ? undefined : onClose}
            title={editing ? t('stockGroupFormDialog.editStockGroup') : t('stockGroupFormDialog.newStockGroup')}
            description={t('stockGroupFormDialog.aGroupBelongsToOne')}
        >
            <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
                <TextField
                    label={t('common.name')}
                    name="name"
                    placeholder={t('stockGroupFormDialog.eGHelmets')}
                    required
                    value={form.name}
                    onChange={update('name')}
                    error={fieldErrors.name}
                />

                <SelectField
                    label={t('stockGroupFormDialog.category')}
                    name="stockCategoryId"
                    required
                    value={form.stockCategoryId}
                    onChange={update('stockCategoryId')}
                    error={fieldErrors.stockCategoryId}
                    placeholder={t('stockGroupFormDialog.selectACategory')}
                    options={categories.map((category) => ({ value: category.id, label: category.name }))}
                />

                {formError && (
                    <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
                        {formError}
                    </p>
                )}

                <div className="mt-2 flex items-center gap-2.5">
                    <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
                        {editing ? t('common.saveChanges') : t('stockGroupFormDialog.createGroup')}
                    </Button>
                    <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
                        {t('common.cancel')}
                    </Button>
                </div>
            </form>
        </Dialog>
    )
}