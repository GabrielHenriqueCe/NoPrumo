import { useEffect, useRef, useState } from 'react'
import { usePreferences } from '../../preferences/preferencesContext'

/*
  Cumulative cost of the project, day by day: labour at the bottom, material on
  top, so the top edge is the total the API sent (cumulativeCost). Only the
  running sum of labour is added up here, and only to know where to draw the
  line between the two layers.

  Drawn at the real width (ResizeObserver) instead of a stretched viewBox, so
  the axis text never gets squashed.
*/

const HEIGHT = 260
const PADDING = { top: 16, right: 16, bottom: 30, left: 64 }

function useWidth() {
  const ref = useRef(null)
  const [width, setWidth] = useState(0)

  useEffect(() => {
    if (!ref.current) return undefined
    const observer = new ResizeObserver(([entry]) => setWidth(entry.contentRect.width))
    observer.observe(ref.current)
    return () => observer.disconnect()
  }, [])

  return [ref, width]
}

export function CostChart({ days }) {
  const { t, format } = usePreferences()
  const [ref, width] = useWidth()
  const [hovered, setHovered] = useState(null)

  let laborSoFar = 0
  const points = days.map((day) => {
    laborSoFar += day.labor
    return { date: day.date, labor: laborSoFar, total: day.cumulativeCost }
  })

  const innerWidth = Math.max(width - PADDING.left - PADDING.right, 1)
  const innerHeight = HEIGHT - PADDING.top - PADDING.bottom
  const maximum = Math.max(...points.map((point) => point.total), 1)

  const x = (index) => PADDING.left + (points.length === 1 ? innerWidth / 2 : (index / (points.length - 1)) * innerWidth)
  const y = (value) => PADDING.top + innerHeight - (value / maximum) * innerHeight

  const area = (top, bottom) =>
    points.map((point, index) => `${index === 0 ? 'M' : 'L'}${x(index)},${y(top(point))}`).join(' ') +
    points
      .slice()
      .reverse()
      .map((point, reverseIndex) => `L${x(points.length - 1 - reverseIndex)},${y(bottom(point))}`)
      .join(' ') +
    ' Z'

  const line = (value) => points.map((point, index) => `${index === 0 ? 'M' : 'L'}${x(index)},${y(value(point))}`).join(' ')

  const ticks = [0, 0.25, 0.5, 0.75, 1].map((share) => maximum * share)
  const dateTicks = points.length > 1 ? [0, Math.floor((points.length - 1) / 2), points.length - 1] : [0]

  const pick = (event) => {
    const bounds = event.currentTarget.getBoundingClientRect()
    const position = (event.clientX - bounds.left - PADDING.left) / innerWidth
    const index = Math.round(position * (points.length - 1))
    setHovered(Math.min(Math.max(index, 0), points.length - 1))
  }

  const shown = hovered == null ? null : points[hovered]
  const last = points[points.length - 1]

  return (
    <figure className="m-0">
      <div className="mb-3 flex flex-wrap items-center gap-x-5 gap-y-1 text-[13px] text-muted">
        <Legend color="var(--accent)" label={t('finance.material')} />
        <Legend color="#808080" label={t('finance.labor')} />
        <span className="ml-auto font-mono text-[12.5px]" aria-live="polite">
          {shown
            ? `${format.date(shown.date)} · ${format.money(shown.total)}`
            : `${t('finance.untilNow')} · ${format.money(last.total)}`}
        </span>
      </div>

      <div ref={ref} className="w-full">
        {width > 0 && (
          <svg
            width={width}
            height={HEIGHT}
            role="img"
            aria-label={t('finance.chartLabel', {
              from: format.date(points[0].date),
              to: format.date(last.date),
              total: format.money(last.total),
            })}
            onMouseMove={pick}
            onMouseLeave={() => setHovered(null)}
          >
            {ticks.map((value) => (
              <g key={value}>
                <line x1={PADDING.left} x2={width - PADDING.right} y1={y(value)} y2={y(value)} stroke="var(--line)" />
                <text
                  x={PADDING.left - 8}
                  y={y(value)}
                  textAnchor="end"
                  dominantBaseline="middle"
                  fill="var(--muted)"
                  fontSize="11"
                  fontFamily="var(--font-mono)"
                >
                  {format.compactMoney(value)}
                </text>
              </g>
            ))}

            <path d={area((point) => point.labor, () => 0)} fill="#808080" fillOpacity="0.28" />
            <path d={area((point) => point.total, (point) => point.labor)} fill="var(--accent)" fillOpacity="0.3" />
            <path d={line((point) => point.labor)} fill="none" stroke="#808080" strokeWidth="1.5" />
            <path d={line((point) => point.total)} fill="none" stroke="var(--accent)" strokeWidth="2.5" />

            {dateTicks.map((index) => (
              <text
                key={index}
                x={x(index)}
                y={HEIGHT - 8}
                textAnchor={index === 0 ? 'start' : index === points.length - 1 ? 'end' : 'middle'}
                fill="var(--muted)"
                fontSize="11"
                fontFamily="var(--font-mono)"
              >
                {format.dayMonth(points[index].date)}
              </text>
            ))}

            {shown && (
              <g>
                <line
                  x1={x(hovered)}
                  x2={x(hovered)}
                  y1={PADDING.top}
                  y2={PADDING.top + innerHeight}
                  stroke="var(--ink)"
                  strokeOpacity="0.35"
                />
                <circle cx={x(hovered)} cy={y(shown.total)} r="4.5" fill="var(--accent)" stroke="var(--surface)" strokeWidth="2" />
              </g>
            )}
          </svg>
        )}
      </div>
    </figure>
  )
}

function Legend({ color, label }) {
  return (
    <span className="inline-flex items-center gap-2">
      <span aria-hidden="true" className="h-2.5 w-5 rounded-full" style={{ background: color }} />
      {label}
    </span>
  )
}
