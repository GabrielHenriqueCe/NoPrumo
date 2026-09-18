import { apiErrorFromNetwork, apiErrorFromResponse } from './problemDetails'

/*
  The only place in the app that talks HTTP.

  Built on fetch instead of a client library: what we actually need is a
  bearer token, JSON in and out, a timeout and cancellation — all of which
  the platform already gives us.

  Cancellation matters more than it looks. A screen that unmounts while a
  request is in flight (user clicks through the menu quickly) would otherwise
  resolve into a dead component, and two list requests racing can paint the
  older answer over the newer one. Every call takes a signal for that.
*/

const DEFAULT_TIMEOUT_MS = 15000

/** Combines the caller's cancellation with our own timeout. */
function withTimeout(signal, timeoutMs) {
  const timeoutController = new AbortController()
  const timer = setTimeout(() => timeoutController.abort(), timeoutMs)

  const signals = [timeoutController.signal]
  if (signal) signals.push(signal)

  const combined =
    typeof AbortSignal.any === 'function' ? AbortSignal.any(signals) : timeoutController.signal

  return { signal: combined, done: () => clearTimeout(timer) }
}

function buildUrl(baseUrl, path, params) {
  const url = new URL(`${baseUrl.replace(/\/+$/, '')}/${path.replace(/^\/+/, '')}`)

  Object.entries(params ?? {}).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      url.searchParams.set(key, String(value))
    }
  })

  return url.toString()
}

async function readBody(response) {
  if (response.status === 204) return null

  const contentType = response.headers.get('content-type') ?? ''
  if (!contentType.includes('json')) {
    const text = await response.text()
    return text ? { detail: text } : null
  }

  try {
    return await response.json()
  } catch {
    // A body that claims to be JSON but is not tells us nothing useful.
    return null
  }
}

export function createHttpClient({ baseUrl, getToken, onUnauthorized, timeoutMs = DEFAULT_TIMEOUT_MS }) {
  async function request(method, path, { body, params, signal } = {}) {
    const url = buildUrl(baseUrl, path, params)
    const timeout = withTimeout(signal, timeoutMs)
    const token = getToken?.()

    let response
    try {
      response = await fetch(url, {
        method,
        signal: timeout.signal,
        headers: {
          Accept: 'application/json',
          ...(body ? { 'Content-Type': 'application/json' } : {}),
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        ...(body ? { body: JSON.stringify(body) } : {}),
      })
    } catch (cause) {
      // A cancellation asked for by the caller is not a failure to report.
      if (signal?.aborted) throw cause
      throw apiErrorFromNetwork(cause, baseUrl)
    } finally {
      timeout.done()
    }

    const payload = await readBody(response)

    if (!response.ok) {
      const error = apiErrorFromResponse(response.status, payload)
      // An expired token is a session problem, not a screen problem: whoever
      // holds the session decides what to do, once, for the whole app.
      if (error.isUnauthorized) onUnauthorized?.(error)
      throw error
    }

    return payload
  }

  return {
    get: (path, options) => request('GET', path, options),
    post: (path, body, options) => request('POST', path, { ...options, body }),
    put: (path, body, options) => request('PUT', path, { ...options, body }),
    patch: (path, body, options) => request('PATCH', path, { ...options, body }),
    remove: (path, options) => request('DELETE', path, options),
  }
}
