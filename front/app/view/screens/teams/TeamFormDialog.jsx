import { useCallback, useEffect, useId, useRef, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { TeamMembersPanel } from './TeamMembersPanel'

/*
  Create or edit a team, with its members on a second tab.

  The members tab only exists once the team does: a stint hangs off the
  team's id. That is why creating a team keeps the dialog open and moves
  straight to members — it is the next thing anyone does with a new team.

  Each tab saves on its own. Details go with "Save changes"; on the members
  tab every add and every end is one call to the API, made the moment it is
  confirmed, so closing the dialog never loses anything.
*/

const EMPTY = { name: '', departmentId: '' }

const TABS = [
  { id: 'details', label: 'Details' },
  { id: 'members', label: 'Members' },
]

export function TeamFormDialog({
  open,
  team,
  initialTab = 'details',
  departments,
  onClose,
  onSubmit,
  onMembersChanged,
}) {
  const editing = Boolean(team)
  const baseId = useId()
  const tabRefs = useRef({})

  const [tab, setTab] = useState('details')
  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  /*
    Closing is refused while a save is in flight, through a ref instead of
    swapping onClose for undefined: the dialog moves the focus back to its
    first control every time onClose changes, which here would yank it off
    the tab the person is on.
  */
  const busyRef = useRef(false)
  useEffect(() => {
    busyRef.current = busy
  }, [busy])

  const close = useCallback(() => {
    if (!busyRef.current) onClose()
  }, [onClose])

  useEffect(() => {
    if (!open) return
    setForm(team ? { name: team.name ?? '', departmentId: String(team.departmentId ?? '') } : EMPTY)
    setFieldErrors({})
    setFormError(null)
    setTab(team ? initialTab : 'details')
  }, [open, team, initialTab])

  // The dialog focuses its first control, which is the first tab. Whoever
  // opened it on "Members" (or just created the team) should land there.
  useEffect(() => {
    if (!open || !team) return undefined
    const timer = setTimeout(() => tabRefs.current[tab]?.focus(), 0)
    return () => clearTimeout(timer)
  }, [open, team, tab])

  // An inactive department takes no new team, but a team that is already in
  // one keeps showing it — otherwise the dropdown would quietly go blank.
  const departmentOptions = departments
    .filter((department) => department.active || department.id === team?.departmentId)
    .map((department) => ({
      value: department.id,
      label: department.active ? department.name : `${department.name} (inactive)`,
    }))

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
        departmentId: form.departmentId ? Number(form.departmentId) : null,
      })
    } catch (error) {
      // The API is the one that validates. The form only shows where it hurt.
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? 'Could not save the team.')
    } finally {
      setBusy(false)
    }
  }

  // Arrow keys move between tabs, as in any tab list; Tab moves into the panel.
  const onTabKeyDown = (event) => {
    if (event.key !== 'ArrowRight' && event.key !== 'ArrowLeft') return
    event.preventDefault()

    const index = TABS.findIndex((item) => item.id === tab)
    const step = event.key === 'ArrowRight' ? 1 : -1
    setTab(TABS[(index + step + TABS.length) % TABS.length].id)
  }

  const detailsForm = (
    <form onSubmit={submit} noValidate className="flex flex-col gap-4 p-6">
      <TextField
        label="Name"
        name="name"
        placeholder="e.g. Masonry crew A"
        required
        value={form.name}
        onChange={update('name')}
        error={fieldErrors.name}
      />

      <SelectField
        label="Department"
        name="departmentId"
        required
        value={form.departmentId}
        onChange={update('departmentId')}
        error={fieldErrors.departmentId}
        placeholder={departmentOptions.length > 0 ? 'Select a department…' : 'No department registered yet'}
        options={departmentOptions}
      />

      {formError && (
        <p role="alert" className="border border-danger-soft bg-[#faefea] px-3 py-2 text-[12.5px] text-danger">
          {formError}
        </p>
      )}

      <div className="mt-2 flex items-center gap-2.5">
        <Button type="submit" busy={busy} busyLabel="Saving…">
          {editing ? 'Save changes' : 'Create team'}
        </Button>
        <Button type="button" variant="outline" onClick={close} disabled={busy}>
          Cancel
        </Button>
      </div>
    </form>
  )

  return (
    <Dialog
      open={open}
      onClose={close}
      title={editing ? 'Edit team' : 'New team'}
      description={
        editing
          ? `${team.name} · ${team.departmentName}`
          : 'Name the team and pick its department. Members come right after.'
      }
    >
      {!editing && detailsForm}

      {editing && (
        <>
          <div
            role="tablist"
            aria-label="Team"
            onKeyDown={onTabKeyDown}
            className="flex gap-1 border-b border-line-soft px-6"
          >
            {TABS.map((item) => {
              const selected = tab === item.id

              return (
                <button
                  key={item.id}
                  ref={(node) => {
                    tabRefs.current[item.id] = node
                  }}
                  type="button"
                  role="tab"
                  id={`${baseId}-tab-${item.id}`}
                  aria-selected={selected}
                  /* Only the selected panel is rendered: members load on demand. */
                  aria-controls={selected ? `${baseId}-panel-${item.id}` : undefined}
                  tabIndex={selected ? 0 : -1}
                  onClick={() => setTab(item.id)}
                  className={[
                    '-mb-px cursor-pointer border-b-2 px-3 py-2.5 text-sm transition-colors',
                    selected
                      ? 'border-gold font-semibold text-graphite'
                      : 'border-transparent text-muted hover:text-graphite',
                  ].join(' ')}
                >
                  {item.label}
                </button>
              )
            })}
          </div>

          <div
            role="tabpanel"
            id={`${baseId}-panel-${tab}`}
            aria-labelledby={`${baseId}-tab-${tab}`}
          >
            {tab === 'details' ? (
              detailsForm
            ) : (
              <TeamMembersPanel team={team} onChanged={onMembersChanged} onDone={close} />
            )}
          </div>
        </>
      )}
    </Dialog>
  )
}
