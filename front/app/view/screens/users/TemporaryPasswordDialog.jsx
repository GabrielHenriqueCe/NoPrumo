import { useState } from 'react'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { useT } from '../../preferences/preferencesContext'

/*
  Shows the one-time password the API just generated.

  This is the only moment it exists in readable form — it is stored hashed, so
  closing this dialog without writing it down means resetting again. The copy
  falls back to selecting the text, since the clipboard API is refused outside
  a secure context and this app runs on plain http in development.
*/

export function TemporaryPasswordDialog({ open, user, password, onClose }) {
  const t = useT()
  const [copied, setCopied] = useState(false)

  const copy = async () => {
    try {
      await navigator.clipboard.writeText(password)
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    } catch {
      // No clipboard permission: the value is on screen and selectable.
      setCopied(false)
    }
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title={t('temporaryPasswordDialog.temporaryPassword')}
      description={t('extras.handThisTo', { name: user?.name ?? t('extras.theUser') })}
    >
      <div className="p-6">
        <p className="label mb-2">{t('temporaryPasswordDialog.password')}</p>

        <p className="border border-line bg-cream-soft px-4 py-3 font-mono text-lg tracking-wider select-all">
          {password}
        </p>

        <p className="mt-3 text-[12.5px] leading-relaxed text-muted">
         {t('temporaryPasswordDialog.theSystemStoresOnlyA')}
        </p>

        <div className="mt-5 flex items-center gap-2.5">
          <Button type="button" onClick={copy}>
            {copied ? t('temporaryPasswordDialog.copied') : t('temporaryPasswordDialog.copy')}
          </Button>
          <Button type="button" variant="outline" onClick={onClose}>
           {t('temporaryPasswordDialog.done')}
          </Button>
        </div>
      </div>
    </Dialog>
  )
}
