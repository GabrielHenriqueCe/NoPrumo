import { useEffect, useState } from 'react'
import { useContainer } from '../../providers/containerContext'
import { Button } from '../../ui/Button'
import { Dialog } from '../../ui/Dialog'
import { ProjectLinksDialog } from '../projects/ProjectLinksDialog'

export function ClientPortalLinksDialog({ client, open, onClose }) {
  const { projects } = useContainer()
  const [clientProjects, setClientProjects] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [selectedProject, setSelectedProject] = useState(null)

  useEffect(() => {
    if (!open || !client) return

    const controller = new AbortController()
    setLoading(true)
    setError(null)

    projects
      .list({ size: 100 }, { signal: controller.signal })
      .then((data) => {
        const filtered = (data.items || []).filter((p) => p.clientId === client.id)
        setClientProjects(filtered)
      })
      .catch((err) => {
        if (!controller.signal.aborted) {
          setError(err.message || 'Erro ao carregar obras do cliente.')
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      })

    return () => controller.abort()
  }, [open, client, projects])

  return (
    <>
      <Dialog
        open={open && !selectedProject}
        onClose={onClose}
        title="Portal do Cliente · Obras"
        description={`Selecione a obra de “${client?.name || 'Cliente'}” para gerenciar os links de acesso.`}
      >
        <div className="p-6">
          {error && (
            <div className="mb-4 border border-danger-soft bg-[#faefea] p-3 text-sm text-danger">
              {error}
            </div>
          )}

          {loading ? (
            <p className="py-4 text-center text-xs text-muted">Buscando obras do cliente...</p>
          ) : clientProjects.length === 0 ? (
            <div className="border border-line bg-cream-soft p-6 text-center">
              <p className="text-sm text-muted">Este cliente não possui nenhuma obra vinculada.</p>
            </div>
          ) : (
            <div className="divide-y divide-line-soft border border-line">
              {clientProjects.map((proj) => (
                <div
                  key={proj.id}
                  className="flex flex-col gap-2 p-3 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div>
                    <div className="font-medium text-graphite">{proj.name}</div>
                    <div className="font-mono text-xs text-muted">
                      {proj.code} {proj.city ? `· ${proj.city}` : ''}
                    </div>
                  </div>
                  <div>
                    <Button
                      variant="outline"
                      className="px-3 py-1.5 text-xs"
                      onClick={() => setSelectedProject(proj)}
                    >
                      Gerenciar Links
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="mt-6 flex justify-end border-t border-line-soft pt-4">
            <Button type="button" variant="outline" onClick={onClose}>
              Fechar
            </Button>
          </div>
        </div>
      </Dialog>

      {selectedProject && (
        <ProjectLinksDialog
          open={!!selectedProject}
          projectId={selectedProject.id}
          projectName={selectedProject.name}
          onClose={() => setSelectedProject(null)}
        />
      )}
    </>
  )
}
