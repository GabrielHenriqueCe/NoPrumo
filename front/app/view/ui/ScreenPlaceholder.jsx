/*
  Stand-in for a screen that is registered in the routes and in the menu but
  not built yet. It exists so the preparation commit could wire every screen
  at once: whoever builds the real screen only replaces the content of their
  own folder, and nobody touches the shared files again.
*/

export function ScreenPlaceholder({ title }) {
  return (
    <>
      <header className="border-b border-line bg-white px-8 py-5">
        <p className="label">Master data</p>
        <h1 className="mt-1 text-xl font-semibold tracking-tight">{title}</h1>
      </header>

      <div className="p-8">
        <p className="max-w-[54em] text-sm leading-relaxed text-muted">
          This screen is not built yet.
        </p>
      </div>
    </>
  )
}
