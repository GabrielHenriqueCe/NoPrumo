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
  if (!isoString) return 'Nunca acessado'
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
      .catch((err) => setError(err.message || 'Erro ao carregar links da obra.'))
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
      setError(err.message || 'Erro ao gerar novo link.')
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
      setError(err.message || 'Erro ao revogar link.')
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
      title="Links do Portal do Cliente"
      description={`Gerenciamento de acessos externos sem login para a obra “${projectName || 'Obra'}”.`}
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
            <div className="label mb-1 text-bronze">Link gerado com sucesso!</div>
            <p className="mb-2 text-xs text-muted">
              Por segurança, este token nunca mais será exibido. Copie e envie agora ao cliente.
            </p>
            <div className="flex items-center gap-2">
              <input
                readOnly
                value={newlyCreatedUrl}
                className="w-full border border-line bg-white px-2.5 py-1.5 font-mono text-xs text-graphite outline-none select-all"
              />
              <Button type="button" variant="primary" className="px-3 py-1.5 text-xs whitespace-nowrap" onClick={handleCopy}>
                {copied ? 'Copiado!' : 'Copiar'}
              </Button>
            </div>
          </div>
        )}

        {/* Form to generate new link */}
        <form onSubmit={handleCreateLink} className="mb-6 border border-line bg-cream-soft p-4">
          <div className="label mb-3">Gerar Novo Link de Acesso</div>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
            <div className="sm:col-span-2">
              <TextField
                label="Identificação (opcional)"
                placeholder="Ex: Acesso do Cliente"
                value={label}
                onChange={(e) => setLabel(e.target.value)}
              />
            </div>
            <div>
              <TextField
                label="Validade (dias)"
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
              {creating ? 'Gerando...' : 'Gerar Link'}
            </Button>
          </div>
        </form>

        {/* Links list */}
        <div>
          <div className="label mb-2">Links Ativos e Histórico</div>
          {loading ? (
            <p className="py-4 text-center text-xs text-muted">Carregando links...</p>
          ) : links.length === 0 ? (
            <p className="py-4 text-center text-xs text-muted">Nenhum link gerado para esta obra ainda.</p>
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
                          {link.label || 'Link sem identificação'}
                        </span>
                        {link.isActive && <Badge tone="active">Ativo</Badge>}
                        {isRevoked && <Badge tone="inactive">Revogado</Badge>}
                        {isExpired && <Badge tone="warning">Expirado</Badge>}
                      </div>
                      <div className="mt-1 flex flex-wrap gap-x-3 text-xs text-muted">
                        <span>Criado: {formatDate(link.createdAt)}</span>
                        <span>Expira: {formatDate(link.expiresAt)}</span>
                        <span>Acessos: {link.accessCount}</span>
                        <span>Último: {formatDateTime(link.lastAccessAt)}</span>
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
                          {revokingId === link.id ? 'Revogando...' : 'Revogar'}
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
            Fechar
          </Button>
        </div>
      </div>
    </Dialog>
  )
}
