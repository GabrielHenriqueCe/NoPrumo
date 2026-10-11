import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { useContainer } from '../../providers/containerContext'
import { Badge } from '../../ui/Badge'

function formatDate(value) {
  if (!value) return 'A definir'
  const [year, month, day] = value.split('-')
  return `${day}/${month}/${year}`
}

function statusBadgeTone(status) {
  switch (status?.toLowerCase()) {
    case 'completed':
      return 'active'
    case 'in_progress':
    case 'inprogress':
      return 'neutral'
    case 'on_hold':
    case 'onhold':
      return 'warning'
    case 'cancelled':
      return 'inactive'
    default:
      return 'neutral'
  }
}

function statusLabel(status) {
  switch (status?.toLowerCase()) {
    case 'completed':
      return 'Concluída'
    case 'in_progress':
    case 'inprogress':
      return 'Em andamento'
    case 'on_hold':
    case 'onhold':
      return 'Pausada'
    case 'cancelled':
      return 'Cancelada'
    case 'planning':
      return 'Planejamento'
    default:
      return status || '—'
  }
}

export function PortalScreen() {
  const { token } = useParams()
  const { projectLinks } = useContainer()

  const [project, setProject] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(null)

    projectLinks
      .getPortalProject(token, { signal: controller.signal })
      .then((data) => {
        setProject(data)
      })
      .catch((err) => {
        if (!controller.signal.aborted) {
          setError(err.message || 'Link inválido, expirado ou revogado.')
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      })

    return () => controller.abort()
  }, [projectLinks, token])

  if (loading) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center bg-cream px-4">
        <div className="text-center">
          <div className="label mb-2">NoPrumo · Portal do Cliente</div>
          <p className="text-sm text-muted">Carregando os dados da sua obra...</p>
        </div>
      </div>
    )
  }

  if (error || !project) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center bg-cream px-4">
        <div className="w-full max-w-md border border-line bg-cream-soft p-8 text-center shadow-sm">
          <div className="label mb-2 text-bronze">Acesso Indisponível</div>
          <h1 className="mb-3 text-lg font-semibold text-graphite">Link expirado ou não encontrado</h1>
          <p className="mb-6 text-sm leading-relaxed text-muted">
            Este link de acompanhamento não é válido, expirou ou foi revogado.
            Por favor, entre em contato com a construtora responsável pela obra para solicitar um novo link de acesso.
          </p>
          <div className="border-t border-line-soft pt-4 text-[12px] text-muted">
            NoPrumo Gestão de Obras
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-cream text-graphite">
      {/* Top Header */}
      <header className="border-b border-line bg-cream-soft">
        <div className="mx-auto flex max-w-4xl items-center justify-between px-6 py-4">
          <div>
            <div className="label text-bronze">Acompanhamento de Obra</div>
            <div className="text-base font-semibold tracking-tight text-graphite">NoPrumo</div>
          </div>
          <Badge tone={statusBadgeTone(project.status)}>
            {statusLabel(project.status)}
          </Badge>
        </div>
      </header>

      {/* Main Content */}
      <main className="mx-auto max-w-4xl px-6 py-8">
        {/* Project Header Card */}
        <section className="mb-8 border border-line bg-cream-soft p-6 shadow-sm md:p-8">
          <div className="mb-4 flex flex-col gap-2 md:flex-row md:items-start md:justify-between">
            <div>
              <div className="label">Empreendimento</div>
              <h1 className="text-2xl font-bold tracking-tight text-graphite md:text-3xl">
                {project.name}
              </h1>
              {project.city && (
                <div className="mt-1 text-sm text-muted">
                  Localização: {project.city}
                </div>
              )}
            </div>

            <div className="mt-2 text-left md:mt-0 md:text-right">
              <div className="label">Previsão de Conclusão</div>
              <div className="text-base font-medium tabular-nums text-graphite">
                {formatDate(project.forecastDate)}
              </div>
            </div>
          </div>

          {/* Overall Progress */}
          <div className="mt-6 border-t border-line-soft pt-6">
            <div className="mb-2 flex items-baseline justify-between">
              <span className="text-sm font-medium text-graphite">Andamento Geral da Obra</span>
              <span className="font-mono text-xl font-bold tabular-nums text-bronze">
                {project.progressPercentage}%
              </span>
            </div>
            <div className="h-3 w-full overflow-hidden rounded-full bg-line-soft">
              <div
                className="h-full rounded-full bg-bronze transition-all duration-500"
                style={{ width: `${Math.min(100, Math.max(0, project.progressPercentage))}%` }}
              />
            </div>
          </div>
        </section>

        {/* Stages List */}
        <section>
          <div className="mb-4 flex items-center justify-between">
            <div>
              <div className="label">Cronograma</div>
              <h2 className="text-lg font-semibold text-graphite">Etapas da Construção</h2>
            </div>
            <div className="text-xs text-muted">
              {project.stages.length} {project.stages.length === 1 ? 'etapa' : 'etapas'}
            </div>
          </div>

          {project.stages.length === 0 ? (
            <div className="border border-line bg-cream-soft p-8 text-center text-sm text-muted">
              Nenhuma etapa cadastrada no momento.
            </div>
          ) : (
            <div className="space-y-3">
              {project.stages.map((stage, index) => (
                <div
                  key={index}
                  className="border border-line bg-cream-soft p-5 transition-shadow hover:shadow-xs"
                >
                  <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <div className="font-medium text-graphite">{stage.Name || stage.name}</div>
                      <div className="mt-1 flex flex-wrap items-center gap-2">
                        <Badge tone={statusBadgeTone(stage.Status || stage.status)}>
                          {statusLabel(stage.Status || stage.status)}
                        </Badge>
                        {(stage.Late || stage.late) && (
                          <Badge tone="warning">Atrasada</Badge>
                        )}
                      </div>
                    </div>

                    <div className="mt-2 text-right sm:mt-0">
                      <span className="font-mono text-sm font-semibold tabular-nums text-graphite">
                        {stage.Percentage ?? stage.percentage}%
                      </span>
                    </div>
                  </div>

                  <div className="mt-3 h-2 w-full overflow-hidden rounded-full bg-line-soft">
                    <div
                      className="h-full rounded-full bg-bronze transition-all duration-300"
                      style={{
                        width: `${Math.min(100, Math.max(0, stage.Percentage ?? stage.percentage ?? 0))}%`,
                      }}
                    />
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>
      </main>

      <footer className="mt-12 border-t border-line py-6 text-center text-xs text-muted">
        Acompanhamento oficial fornecido por NoPrumo Gestão de Obras · Atualizado em tempo real
      </footer>
    </div>
  )
}
