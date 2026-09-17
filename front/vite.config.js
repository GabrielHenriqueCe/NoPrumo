import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// Everything runs on this machine: the dev server here, the API on 5262 and
// MySQL behind it. No CDN, no hosted service — the app works offline.
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: 5173,
    open: false,
  },
})
