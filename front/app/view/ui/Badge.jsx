/*
  Status pill.

  Colour is never the only carrier of meaning here: the word is always there
  too. Roughly one man in twelve cannot separate the red from the green, and
  "the red one" is a bad way to tell somebody their account is switched off.

  `late` is the H alarm: solid, condensed, uppercase — the one badge meant to
  be seen from across the room.
*/

const TONES = {
  neutral: 'border border-line bg-surface text-muted',
  active: 'border border-success/30 bg-success-wash text-success',
  inactive: 'border border-late/40 bg-danger-wash text-danger',
  warning: 'border border-accent/30 bg-gold-wash text-accent',
  late: 'bg-danger-strong font-display text-[13px] font-bold tracking-[0.08em] text-on-danger uppercase',
}

export function Badge({ tone = 'neutral', children }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-3 py-0.5 text-[12.5px] font-medium whitespace-nowrap ${TONES[tone] ?? TONES.neutral}`}
    >
      {children}
    </span>
  )
}
