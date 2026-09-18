/*
  Status pill.

  Colour is never the only carrier of meaning here: the word is always there
  too. Roughly one man in twelve cannot separate the red from the green, and
  "the red one" is a bad way to tell somebody their account is switched off.
*/

const TONES = {
  neutral: 'bg-cream text-muted border-line',
  active: 'bg-[#eef4ef] text-success border-[#cfe0d4]',
  inactive: 'bg-[#faefea] text-danger border-danger-soft',
  warning: 'bg-gold-wash text-bronze border-[#e8dcb8]',
}

export function Badge({ tone = 'neutral', children }) {
  return (
    <span
      className={`inline-block border px-2 py-0.5 text-[11.5px] font-medium ${TONES[tone] ?? TONES.neutral}`}
    >
      {children}
    </span>
  )
}
