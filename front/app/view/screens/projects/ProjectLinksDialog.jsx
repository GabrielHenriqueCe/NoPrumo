import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'
import { useT } from '../../preferences/preferencesContext'

function formatDate(isoString) {
  if (!isoString) return '—'
  const date = new Date(isoString)
  return isNaN(date.getTime())
    ? '—'
    : date.toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
      })
}

function formatDateTime(isoString) {
  if (!isoString) return 'Never accessed'
  const date = new Date(isoString)
  return isNaN(date.getTime())
    ? '—'
    : date.toLocaleString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      })
}

export function ProjectLinksDialog({ projectId, projectName, open, onClose }) {
  const t = useT()
  const { projectLinks } = useContainer()

  const [links, setLinks] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [revokingId, setRevokingId] = useState(null)

  // Form for creating a new link
  const [label, setLabel] = useState('')
  const [daysValid, setDaysValid] = useState('90')
  const [creating, setCreating] = useState(false)
  const [newlyCreatedUrl, setNewlyCreatedUrl] = useState(null)
  const [copied, setCopied] = useState(false)

  const loadLinks = useCallback(() => {
    if (!projectId) return
    setLoading(true)
    setError(null)
    projectLinks
      .listByProject(projectId)
      .then((data) => setLinks(data))
      .catch((err) => setError(err.message || t('projectLinksDialog.couldNotLoadProjectPortal')))
      .finally(() => setLoading(false))
  }, [projectId, projectLinks, t])

  useEffect(() => {
    if (open && projectId) {
      loadLinks()
      setNewlyCreatedUrl(null)
      setCopied(false)
      setLabel('')
      setDaysValid('90')
    }
  }, [open, projectId, loadLinks])

  const handleCreateLink = async (e) => {
    e.preventDefault()
    setCreating(true)
    setError(null)
    setNewlyCreatedUrl(null)
    setCopied(false)

    try {
      const parsedDays = parseInt(daysValid, 10) || 90
      const response = await projectLinks.create(projectId, {
        label: label.trim() || undefined,
        daysValid: parsedDays,
      })

      const fullUrl = `${window.location.origin}/portal/${response.token}`
      setNewlyCreatedUrl(fullUrl)
      setLabel('')
      loadLinks()
    } catch (err) {
      setError(err.message || t('projectLinksDialog.couldNotGenerateNewLink'))
    } finally {
      setCreating(false)
    }
  }

  const handleRevoke = async (linkId) => {
    setRevokingId(linkId)
    setError(null)
    try {
      await projectLinks.revoke(linkId)
      loadLinks()
    } catch (err) {
      setError(err.message || t('projectLinksDialog.couldNotRevokeLink'))
    } finally {
      setRevokingId(null)
    }
  }

  const handleCopy = () => {
    if (!newlyCreatedUrl) return
    navigator.clipboard.writeText(newlyCreatedUrl).then(() => {
      setCopied(true)
      setTimeout(() => setCopied(false), 3000)
    })
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title={t('projectLinksDialog.clientPortalLinks')}
      description={t('extras.projectLinksHint', { project: projectName || t('extras.project') })}
    >
      <div className="p-6">
        {error && (
          <div className="mb-4 rounded-2xl bg-danger-wash p-3 text-sm text-danger">
            {error}
          </div>
        )}

        {/* Newly created link alert with copy button */}
        {newlyCreatedUrl && (
          <div className="mb-6 border border-bronze bg-gold-wash p-4">
            <div className="label mb-1 text-bronze">{t('projectLinksDialog.linkSuccessfullyCreated')}</div>
            <p className="mb-2 text-xs text-muted">
              {t('projectLinksDialog.forSecurityReasonsThisToken')}
            </p>
            <div className="flex items-center gap-2">
              <input
                readOnly
                value={newlyCreatedUrl}
                className="w-full border border-line bg-surface px-2.5 py-1.5 font-mono text-xs text-graphite outline-none select-all"
              />
              <Button type="button" variant="primary" className="px-3 py-1.5 text-xs whitespace-nowrap" onClick={handleCopy}>
                {copied ? t('projectLinksDialog.copied') : t('projectLinksDialog.copy')}
              </Button>
            </div>
          </div>
        )}

        {/* Form to generate new link */}
        <form onSubmit={handleCreateLink} className="mb-6 border border-line bg-cream-soft p-4">
          <div className="label mb-3">{t('projectLinksDialog.generateNewAccessLink')}</div>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
            <div className="sm:col-span-2">
              <TextField
                label={t('projectLinksDialog.labelOptional')}
                placeholder={t('projectLinksDialog.exClientFamilyAccess')}
                value={label}
                onChange={(e) => setLabel(e.target.value)}
              />
            </div>
            <div>
              <TextField
                label={t('projectLinksDialog.validityDays')}
                type="number"
                min="1"
                max="365"
                value={daysValid}
                onChange={(e) => setDaysValid(e.target.value)}
              />
            </div>
          </div>
          <div className="mt-3 flex justify-end">
            <Button type="submit" variant="primary" disabled={creating}>
              {creating ? t('projectLinksDialog.generating') : t('projectLinksDialog.generateLink')}
            </Button>
          </div>
        </form>

        {/* Links list */}
        <div>
          <div className="label mb-2">{t('projectLinksDialog.activeLinksHistory')}</div>
          {loading ? (
            <p className="py-4 text-center text-xs text-muted">{t('projectLinksDialog.loadingLinks')}</p>
          ) : links.length === 0 ? (
            <p className="py-4 text-center text-xs text-muted">{t('projectLinksDialog.noPortalLinkGeneratedFor')}</p>
          ) : (
            <div className="divide-y divide-line-soft border border-line">
              {links.map((link) => {
                const isRevoked = !!link.revokedAt
                const isExpired = !link.isActive && !isRevoked

                return (
                  <div key={link.id} className="flex flex-col gap-2 p-3 sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <div className="flex items-center gap-2">
                        <span className="text-sm font-medium text-graphite">
                          {link.label || t('projectLinksDialog.unlabeledLink')}
                        </span>
                        {link.isActive && <Badge tone="active">{t('common.active')}</Badge>}
                        {isRevoked && <Badge tone="inactive">{t('projectLinksDialog.revoked')}</Badge>}
                        {isExpired && <Badge tone="warning">{t('projectLinksDialog.expired')}</Badge>}
                      </div>
                      <div className="mt-1 flex flex-wrap gap-x-3 text-xs text-muted">
                        <span>{t('projectLinksDialog.created')} {formatDate(link.createdAt)}</span>
                        <span>{t('projectLinksDialog.expires')} {formatDate(link.expiresAt)}</span>
                        <span>{t('projectLinksDialog.accesses')} {link.accessCount}</span>
                        <span>{t('projectLinksDialog.lastAccess')} {formatDateTime(link.lastAccessAt)}</span>
                      </div>
                    </div>

                    {link.isActive && (
                      <div className="sm:text-right">
                        <Button
                          variant="outline"
                          className="px-2.5 py-1 text-xs text-danger hover:border-danger hover:bg-danger/5"
                          disabled={revokingId === link.id}
                          onClick={() => handleRevoke(link.id)}
                        >
                          {revokingId === link.id ? t('projectLinksDialog.revoking') : t('projectLinksDialog.revoke')}
                        </Button>
                      </div>
                    )}
                  </div>
                )
              })}
            </div>
          )}
        </div>

        <div className="mt-6 flex justify-end border-t border-line-soft pt-4">
          <Button type="button" variant="outline" onClick={onClose}>
            {t('common.close')}
          </Button>
        </div>
      </div>
    </Dialog>
  )
}
