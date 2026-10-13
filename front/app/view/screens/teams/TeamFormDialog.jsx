import { useCallback, useEffect, useId, useRef, useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { SelectField } from '../../ui/SelectField'
import { TextField } from '../../ui/TextField'
import { TeamMembersPanel } from './TeamMembersPanel'
import { useT } from '../../preferences/preferencesContext'

// Members hang off the team's id, so the members tab only exists once the team does.

const EMPTY = { name: '', departmentId: '' }

const TABS = [
  { id: 'details', labelKey: 'extras.details' },
  { id: 'members', labelKey: 'extras.members' },
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
  const t = useT()
  const editing = Boolean(team)
  const baseId = useId()
  const tabRefs = useRef({})

  const [tab, setTab] = useState('details')
  const [form, setForm] = useState(EMPTY)
  const [fieldErrors, setFieldErrors] = useState({})
  const [formError, setFormError] = useState(null)
  const [busy, setBusy] = useState(false)

  // Closing is blocked through a ref, not by swapping onClose: the Dialog
  // refocuses its first control whenever onClose changes.
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

  // The Dialog focuses the first tab; whoever opened "Members" should land there.
  useEffect(() => {
    if (!open || !team) return undefined
    const timer = setTimeout(() => tabRefs.current[tab]?.focus(), 0)
    return () => clearTimeout(timer)
  }, [open, team, tab])

  // An inactive department takes no new team, but a team already in one keeps showing it.
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
      if (error.isValidation) setFieldErrors(error.fieldErrors)
      else setFormError(error.message ?? t('teamFormDialog.couldNotSaveTheTeam'))
    } finally {
      setBusy(false)
    }
  }

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
        label={t('common.name')}
        name="name"
        placeholder={t('teamFormDialog.eGMasonryCrewA')}
        required
        value={form.name}
        onChange={update('name')}
        error={fieldErrors.name}
      />

      <SelectField
        label={t('teamFormDialog.department')}
        name="departmentId"
        required
        value={form.departmentId}
        onChange={update('departmentId')}
        error={fieldErrors.departmentId}
        placeholder={departmentOptions.length > 0 ? t('teamFormDialog.selectADepartment') : t('teamFormDialog.noDepartmentRegisteredYet')}
        options={departmentOptions}
      />

      {formError && (
        <p role="alert" className="rounded-2xl bg-danger-wash px-3 py-2 text-[12.5px] text-danger">
          {formError}
        </p>
      )}

      <div className="mt-2 flex items-center gap-2.5">
        <Button type="submit" busy={busy} busyLabel={t('common.saving')}>
          {editing ? t('common.saveChanges') : t('teamFormDialog.createTeam')}
        </Button>
        <Button type="button" variant="outline" onClick={close} disabled={busy}>
          {t('common.cancel')}
        </Button>
      </div>
    </form>
  )

  return (
    <Dialog
      open={open}
      onClose={close}
      title={editing ? t('teamFormDialog.editTeam') : t('teamFormDialog.newTeam')}
      description={
        editing
          ? `${team.name} · ${team.departmentName}`
          : t('teamFormDialog.nameTheTeamAndPick')
      }
    >
      {!editing && detailsForm}

      {editing && (
        <>
          <div
            role="tablist"
            aria-label={t('teamFormDialog.team')}
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
                  {t(item.labelKey)}
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
