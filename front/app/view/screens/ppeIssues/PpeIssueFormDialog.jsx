import { useEffect, useState } from 'react'
import { todayIso } from '../../format/dates'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'

// Only PPE items come from the API; an expired CA is shown and refused by the API.
let nextLineKey = 0
const newLine = () => ({ key: (nextLineKey += 1), stockItemId: '', quantity: '1' })

export function PpeIssueFormDialog({ open, projectId, onClose, onIssued }) {
  const { ppeIssues } = useContainer()

  const [options, setOptions] = useState({ employees: [], items: [] })
  const [optionsError, setOptionsError] = useState(null)
  const [form, setForm] = useState(() => ({ employeeId: '', date: todayIso(), notes: '', lines: [newLine()] }))
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return undefined
    setForm({ employeeId: '', date: todayIso(), notes: '', lines: [newLine()] })
    setFieldErrors({})
    setFormError(null)
    setOptionsError(null)

    const controller = new AbortController()
    ppeIssues
      .options({ signal: controller.signal })
      .then(setOptions)
      .catch((error) => {
        if (!controller.signal.aborted) setOptionsError(error.message ?? 'Could not load employees and PPE items.')
      })

    return () => controller.abort()
  }, [open, ppeIssues])

  const update = (field) => (event) => {
    setForm((current) => ({ ...current, [field]: event.target.value }))
    setFieldErrors((current) => ({ ...current, [field]: undefined }))
  }

  const updateLine = (key, field) => (event) => {
    const value = event.target.value
    setForm((current) => ({
      ...current,
      lines: current.lines.map((line) => (line.key === key ? { ...line, [field]: value } : line)),
    }))
    setFieldErrors((current) => ({ ...current, items: undefined }))
  }

  const addLine = () => setForm((current) => ({ ...current, lines: [...current.lines, newLine()] }))

  const removeLine = (key) =>
    setForm((current) => ({ ...current, lines: current.lines.filter((line) => line.key !== key) }))

  const submit = async (event) => {
    event.preventDefault()
    setBusy(true)
    setFieldErrors({})
    setFormError(null)

    try {
      const issue = await ppeIssues.create({
        employeeId: form.employeeId ? Number(form.employeeId) : null,
        projectId,
        date: form.date || null,
        notes: form.notes.trim() || null,
        items: form.lines.map((line) => ({
          stockItemId: line.stockItemId ? Number(line.stockItemId) : null,
          quantity: line.quantity === '' ? null : Number(line.quantity),
        })),
      })
      onIssued(issue)
    } catch (error) {
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not issue the PPE.')
    } finally {
      setBusy(false)
    }
  }

  const itemOptions = options.items.map((item) => ({
    value: item.id,
    label: [item.name, item.ca ? `CA ${item.ca}` : 'no CA', item.caExpired ? 'CA expired' : null]
      .filter(Boolean)
      .join(' · '),
  }))

  return (
    <Dialog
      open={open}
      onClose={busy ? undefined : onClose}
      title="New PPE issue"
      description="Each item leaves the stock for the employee. The employee signs the receipt next."
    >
      <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
        <SelectField
          label="Employee"
          name="employeeId"
          required
          value={form.employeeId}
          onChange={update('employeeId')}
          error={fieldErrors.employeeId}
          placeholder={options.employees.length > 0 ? 'Select an employee…' : 'No active employee'}
          options={options.employees.map((employee) => ({
            value: employee.id,
            label: employee.registrationNumber ? `${employee.name} · ${employee.registrationNumber}` : employee.name,
          }))}
        />

        <TextField
          label="Date"
          name="date"
          type="date"
          required
          max={todayIso()}
          value={form.date}
          onChange={update('date')}
          error={fieldErrors.date}
        />

        <fieldset className="flex flex-col gap-3 border-t border-line-soft pt-4">
          <legend className="label mb-1">Items</legend>

          {form.lines.map((line, index) => (
            <div key={line.key} className="grid grid-cols-[minmax(0,1fr)_6rem_auto] items-start gap-3">
              <SelectField
                label={`Item ${index + 1}`}
                name={`stockItemId-${line.key}`}
                required
                value={line.stockItemId}
                onChange={updateLine(line.key, 'stockItemId')}
                placeholder={itemOptions.length > 0 ? 'Select a PPE item…' : 'No PPE item registered'}
                options={itemOptions}
              />

              <TextField
                label="Quantity"
                name={`quantity-${line.key}`}
                type="number"
                min="0"
                step="1"
                required
                value={line.quantity}
                onChange={updateLine(line.key, 'quantity')}
              />

              <div className="pt-5">
                <Button
                  type="button"
                  variant="outline"
                  className="px-3 py-2"
                  disabled={form.lines.length === 1}
                  aria-label={`Remove item ${index + 1}`}
                  onClick={() => removeLine(line.key)}
                >
                  Remove
                </Button>
              </div>
            </div>
          ))}

          {fieldErrors.items && (
            <p role="alert" className="text-[12.5px] text-danger">
              {fieldErrors.items}
            </p>
          )}

          <div>
            <Button type="button" variant="outline" className="px-3 py-1.5" onClick={addLine}>
              Add item
            </Button>
          </div>
        </fieldset>

        <TextField
          label="Notes"
          name="notes"
          value={form.notes}
          onChange={update('notes')}
          error={fieldErrors.notes}
          hint="Optional."
        />

        {(formError || optionsError) && (
          <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
            {formError ?? optionsError}
          </p>
        )}

        <div className="mt-2 flex items-center gap-2.5">
          <Button type="submit" busy={busy} busyLabel="Issuing…">
            Issue PPE
          </Button>
          <Button type="button" variant="outline" onClick={onClose} disabled={busy}>
            Cancel
          </Button>
        </div>
      </form>
    </Dialog>
  )
}
