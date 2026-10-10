import { useCallback, useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { TextField } from '../../ui/TextField'

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
      .catch((err) => setError(err.message || 'Could not load project portal links.'))
      .finally(() => setLoading(false))
  }, [projectId, projectLinks])

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
      setError(err.message || 'Could not generate new link.')
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
      setError(err.message || 'Could not revoke link.')
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
      title="Client Portal Links"
      description={`External anonymous access management for project “${projectName || 'Project'}”.`}
    >
      <div className="p-6">
        {error && (
          <div className="mb-4 border border-danger-soft bg-[#faefea] p-3 text-sm text-danger">
            {error}
          </div>
        )}

        {/* Newly created link alert with copy button */}
        {newlyCreatedUrl && (
          <div className="mb-6 border border-bronze bg-gold-wash p-4">
            <div className="label mb-1 text-bronze">Link successfully created!</div>
            <p className="mb-2 text-xs text-muted">
              For security reasons, this token will never be displayed again. Copy and share it with the client now.
            </p>
            <div className="flex items-center gap-2">
              <input
                readOnly
                value={newlyCreatedUrl}
                className="w-full border border-line bg-white px-2.5 py-1.5 font-mono text-xs text-graphite outline-none select-all"
              />
              <Button type="button" variant="primary" className="px-3 py-1.5 text-xs whitespace-nowrap" onClick={handleCopy}>
                {copied ? 'Copied!' : 'Copy'}
              </Button>
            </div>
          </div>
        )}

        {/* Form to generate new link */}
        <form onSubmit={handleCreateLink} className="mb-6 border border-line bg-cream-soft p-4">
          <div className="label mb-3">Generate New Access Link</div>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
            <div className="sm:col-span-2">
              <TextField
                label="Label (optional)"
                placeholder="Ex: Client Family Access"
                value={label}
                onChange={(e) => setLabel(e.target.value)}
              />
            </div>
            <div>
              <TextField
                label="Validity (days)"
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
              {creating ? 'Generating...' : 'Generate link'}
            </Button>
          </div>
        </form>

        {/* Links list */}
        <div>
          <div className="label mb-2">Active Links & History</div>
          {loading ? (
            <p className="py-4 text-center text-xs text-muted">Loading links...</p>
          ) : links.length === 0 ? (
            <p className="py-4 text-center text-xs text-muted">No portal link generated for this project yet.</p>
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
                          {link.label || 'Unlabeled link'}
                        </span>
                        {link.isActive && <Badge tone="active">Active</Badge>}
                        {isRevoked && <Badge tone="inactive">Revoked</Badge>}
                        {isExpired && <Badge tone="warning">Expired</Badge>}
                      </div>
                      <div className="mt-1 flex flex-wrap gap-x-3 text-xs text-muted">
                        <span>Created: {formatDate(link.createdAt)}</span>
                        <span>Expires: {formatDate(link.expiresAt)}</span>
                        <span>Accesses: {link.accessCount}</span>
                        <span>Last access: {formatDateTime(link.lastAccessAt)}</span>
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
                          {revokingId === link.id ? 'Revoking...' : 'Revoke'}
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
            Close
          </Button>
        </div>
      </div>
    </Dialog>
  )
}
