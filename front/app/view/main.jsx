import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { ContainerProvider } from './providers/ContainerProvider'
import { SessionProvider } from './providers/SessionProvider'
import { AppRoutes } from './routes'
import './theme.css'

/*
  Start-up order matters: the container has to exist before the session can
  ask it anything, and the session has to exist before a route can check it.
*/
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ContainerProvider>
      <SessionProvider>
        <BrowserRouter>
          <AppRoutes />
        </BrowserRouter>
      </SessionProvider>
    </ContainerProvider>
  </StrictMode>,
)
