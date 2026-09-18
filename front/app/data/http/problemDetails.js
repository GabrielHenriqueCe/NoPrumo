/*
  Translates an API failure into an error the UI can act on.

  ASP.NET Core answers failures with ProblemDetails (RFC 9457):

    { "title": "...", "status": 400, "detail": "...",
      "errors": { "username": ["Already taken"] } }

  Callers should not have to know that shape, nor guess whether the message
  lives in `detail`, in `title` or inside `errors`. Everything leaving this
  module is an ApiError with two things: a message to show, and — when the
  failure is a validation one — the per-field map a form needs to highlight.
*/

export class ApiError extends Error {
  constructor({ message, status, fieldErrors = {}, cause }) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors
    this.cause = cause
  }

  /** Validation failure: the screen must highlight fields, not just warn. */
  get isValidation() {
    return Object.keys(this.fieldErrors).length > 0
  }

  get isUnauthorized() {
    return this.status === 401
  }

  get isForbidden() {
    return this.status === 403
  }

  /** No response at all: API down, CORS blocked or no network. */
  get isOffline() {
    return this.status === 0
  }
}

const MESSAGE_BY_STATUS = {
  400: 'Check the highlighted fields.',
  401: 'Your session has expired. Sign in again.',
  403: 'You do not have access to this information.',
  404: 'We could not find what you asked for.',
  409: 'This record already exists.',
  422: 'Check the highlighted fields.',
  500: 'The API failed to process the request. Tell the team.',
}

/*
  ASP.NET keys `errors` by the C# property name (`Username`, `RoleId`) and
  sometimes nests it (`user.Username`). Forms know their fields in camelCase,
  so the key is normalized on the way in instead of leaking that conversion
  into every screen.
*/
function normalizeFieldName(key) {
  const last = key.split('.').pop() ?? key
  return last.charAt(0).toLowerCase() + last.slice(1)
}

function extractFieldErrors(body) {
  if (!body?.errors || typeof body.errors !== 'object') return {}

  return Object.entries(body.errors).reduce((acc, [key, value]) => {
    const message = Array.isArray(value) ? value[0] : String(value)
    if (message) acc[normalizeFieldName(key)] = message
    return acc
  }, {})
}

/** Builds an ApiError from whatever the API answered. */
export function apiErrorFromResponse(status, body) {
  const fieldErrors = extractFieldErrors(body)
  const message =
    body?.detail ||
    (Object.keys(fieldErrors).length ? MESSAGE_BY_STATUS[400] : null) ||
    body?.title ||
    MESSAGE_BY_STATUS[status] ||
    'The operation could not be completed.'

  return new ApiError({ message, status, fieldErrors })
}

/** Failure before any response exists: network, CORS, timeout. */
export function apiErrorFromNetwork(cause, baseUrl) {
  const aborted = cause?.name === 'AbortError'
  return new ApiError({
    message: aborted
      ? 'The API took too long to answer.'
      : `Could not reach the API at ${baseUrl}. Check that the back is running.`,
    status: 0,
    cause,
  })
}
