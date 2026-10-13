/*
  The rail icons, drawn on a 24px grid with one stroke width so they read as a
  set. Projects, people, materials, commercial and settings are the H design's
  own drawings; home, planning and reports follow the same rules.
*/

const PATHS = {
  home: <path d="M4 4 H10 V10 H4 Z M14 4 H20 V10 H14 Z M4 14 H10 V20 H4 Z M14 14 H20 V20 H14 Z" />,
  projects: <path d="M4 21 V7 L12 3 L20 7 V21 Z M9 21 V15 H15 V21 M8 10 H10 M14 10 H16" />,
  people: (
    <>
      <circle cx="9" cy="8" r="3.5" />
      <path d="M3 20 C3 15.5 6 14 9 14 C12 14 15 15.5 15 20 M16 4.5 A3.5 3.5 0 0 1 16 11.5 M18 14.5 C20 15.2 21 17 21 20" />
    </>
  ),
  planning: <path d="M4 6 H20 V20 H4 Z M4 10 H20 M8 3 V7 M16 3 V7 M8 14 H10 M12 14 H14 M8 17 H10" />,
  materials: <path d="M3 8 L12 3 L21 8 L12 13 Z M3 8 V16 L12 21 L21 16 V8 M12 13 V21" />,
  commercial: <path d="M4 7 H20 V19 H4 Z M9 7 V4 H15 V7 M4 12 H20" />,
  reports: <path d="M4 20 H20 M6 20 V12 M11 20 V6 M16 20 V14 M6 9 L11 4 L16 10 L20 6" />,
  settings: (
    <>
      <circle cx="12" cy="12" r="3" />
      <path d="M12 2 V5 M12 19 V22 M2 12 H5 M19 12 H22 M4.9 4.9 L7 7 M17 17 L19.1 19.1 M4.9 19.1 L7 17 M17 7 L19.1 4.9" />
    </>
  ),
}

export function RailIcon({ name, active }) {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      aria-hidden="true"
      fill="none"
      strokeWidth="1.8"
      className={active ? 'stroke-accent-bright' : 'stroke-rail-icon'}
    >
      {PATHS[name]}
    </svg>
  )
}
