import { useId } from 'react'

/*
  The No Prumo mark, redrawn as SVG.

  Traced over Logo_NoPrumo.jpeg rather than eyeballed: the symbol measures
  226x283px inside that file, which is why the viewBox is 64x80 and not the
  square it looks like. Every coordinate below was measured against the
  original artwork.

  The artwork follows one geometric system, and keeping it is what makes the
  mark look drawn instead of assembled:

    - every bevel runs at dx/dy = 1.58 — the hexagon edges, the top and the
      foot of the N's stem, the P's top bar and its closing cut all share it;
    - the N's own diagonal is the single exception, at 45 degrees;
    - the hexagon's points are cut VERTICALLY, parallel to the plumb line,
      which is what opens the apex and the base for the line to drop through;
    - the letters are filled polygons, not strokes: a stroke cannot end on a
      bevel, and those bevels are the whole idea.

  SVG and not the .jpeg because the mark has to sit on a dark sidebar and on a
  light page, stay sharp at 32px and at 400px, and weigh under a kilobyte. The
  colours are sampled from the original file.

  Gradient ids are per instance — two copies sharing an id would make the
  second inherit the first one's fill.
*/

const HEXAGON_LEFT = 'M29.2 5.4 L29.2 13.7 L7.1 28 L7.1 54.8 L23.9 66 L25 75 L0 59 L0 23.9 Z'
const HEXAGON_RIGHT = 'M34.8 5.4 L34.8 13.7 L56.9 28 L56.9 54.8 L40.1 66 L39 75 L64 59 L64 23.9 Z'

// N and P share the middle stem; the counter of the P is the second subpath.
const MONOGRAM =
  'M15.9 25.9 L11 29 L11 53.4 L17.6 57.9 L17.6 33.9 L42.5 57.9 L42.5 49.6 ' +
  'L53 42.9 L53 29.2 L35.8 18.3 L35.7 43.2 L18.7 26 Z ' +
  'M42.6 29.9 L47.2 29.9 L47.2 40.3 L42.6 43 Z'

const BOB_CAP = 'M29 61.3 L35.2 61.3 L35.3 64.6 L28.9 64.6 Z'
const BOB_BODY = 'M28.9 64.6 L26 68 L32 80 L38.1 68 L35.3 64.6 Z'

const TONES = {
  light: { steelTop: '#808080', steelBottom: '#404040', goldTop: '#a8842b', goldBottom: '#886830', line: '#606060' },
  dark: { steelTop: '#f3f1ec', steelBottom: '#9ba3ab', goldTop: '#d9b451', goldBottom: '#a8842b', line: '#aab2ba' },
}

export function PlumbMark({ size = 44, tone = 'light', className = '' }) {
  const id = useId()
  const steelId = `steel-${id}`
  const goldId = `gold-${id}`
  const colors = TONES[tone] ?? TONES.light

  return (
    <svg
      viewBox="0 0 64 80"
      height={size}
      width={(size * 64) / 80}
      className={className}
      role="img"
      aria-label="No Prumo"
    >
      <defs>
        <linearGradient id={steelId} x1="0" y1="0" x2="0.3" y2="1">
          <stop offset="0%" stopColor={colors.steelTop} />
          <stop offset="100%" stopColor={colors.steelBottom} />
        </linearGradient>
        <linearGradient id={goldId} x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor={colors.goldTop} />
          <stop offset="100%" stopColor={colors.goldBottom} />
        </linearGradient>
      </defs>

      {/* the line hangs behind the monogram and reappears in the gap */}
      <rect x="30.6" y="0" width="2.8" height="61.5" fill={colors.line} />

      <g fill={`url(#${steelId})`}>
        <path d={HEXAGON_LEFT} />
        <path d={HEXAGON_RIGHT} />
        <path d={MONOGRAM} fillRule="evenodd" />
      </g>

      <path d={BOB_CAP} fill={colors.goldTop} />
      <path d={BOB_BODY} fill={`url(#${goldId})`} />
    </svg>
  )
}

export function Brand({ size = 40, tone = 'light', tagline = true, className = '' }) {
  const dark = tone === 'dark'

  return (
    <div className={`flex items-center gap-3 ${className}`}>
      <PlumbMark size={size} tone={tone} />
      <div className="leading-none">
        <div className={`text-[19px] font-bold tracking-tight ${dark ? 'text-cream' : 'text-graphite'}`}>
          No Prumo
        </div>
        {tagline && (
          <div className={`label mt-1.5 ${dark ? 'text-muted-dark' : 'text-muted'}`}>
            Construction management
          </div>
        )}
      </div>
    </div>
  )
}
