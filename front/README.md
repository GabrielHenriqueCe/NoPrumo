# No Prumo — front

React + Vite. Runs entirely on the local machine: this dev server, the .NET API
on `localhost:5262` and MySQL behind it. No CDN, no hosted service.

## Running

```
npm install
npm run dev      # http://localhost:5173
npm run build    # production bundle in dist/
npm run lint
```

`.env` holds the API address. Nothing secret goes in it.

## Layout

```
app/
├── container.js     wires everything; the only file that knows the API address
├── data/            talks to the API — no screen imports from here directly
│   ├── http/        fetch client + ProblemDetails translation
│   ├── gateways/    one per subject (auth, users)
│   └── storage/     where the token lives
└── view/            React only
    ├── providers/   container injection + session state
    ├── guards/      route gate
    ├── shell/       sidebar, header, the logo in SVG
    ├── ui/          Button, TextField, SelectField, Dialog, Badge, Pagination
    └── screens/     auth/LoginScreen, users/UsersScreen
```

The rule: `view` may import from `data`'s gateways through the container, never
the other way round. No screen calls `fetch`, and no screen computes anything —
costs, balances and "is it late" are the back end's job (CLAUDE.md).

## What the API has to provide

**None of these endpoints exist yet** — `back/01-Presentation/Controllers/` is
empty. The front is written against the contract below; until it is there, the
login screen answers "Could not reach the API".

Base: `http://localhost:5262/api`. Bearer token in `Authorization`.
Errors follow ProblemDetails, with `errors` keyed per field on validation.

### Auth

```
POST /auth/login
  { "username": "admin", "password": "…" }
  200 { "token": "...", "user": { … }, "mustChangePassword": false }
  401 wrong credentials — same answer for unknown user and wrong password
  403 inactive account

GET /auth/me
  200 user          — rebuilds the session after a page reload

POST /auth/change-password
  { "currentPassword": "…", "newPassword": "…" }
  204
```

### Users

```
GET   /users?page=1&size=10&search=
  200 { "items": [user], "page": 1, "size": 10, "total": 3, "totalPages": 1 }

POST  /users
  { "name": "…", "username": "…", "email": "…", "roleId": 3 }
  201 { "user": { … }, "temporaryPassword": "Prumo@4821" }
  400 ProblemDetails with errors: { "username": ["Already taken"] }

PUT   /users/{id}
  { "name": "…", "email": "…", "roleId": 3 }
  200 user

PATCH /users/{id}/activate
PATCH /users/{id}/deactivate
  204     — accounts are switched off, never deleted

POST  /users/{id}/reset-password
  200 { "temporaryPassword": "Prumo@7134" }

GET   /roles
  200 [ { "id": 1, "name": "admin", "label": "Administrator" } ]
```

### The user shape the front expects

```json
{
  "id": 1,
  "username": "admin",
  "name": "Gabriel Henrique Cé",
  "email": "admin@noprumo.com.br",
  "roleId": 1,
  "roleName": "admin",
  "roleLabel": "Administrator",
  "permissions": ["manage_users"],
  "active": true,
  "mustChangePassword": false,
  "lastLoginAt": "2026-09-17T12:00:00Z"
}
```

`permissions` is what the UI hides controls by — never the role name. The API
is what actually refuses; the front only avoids showing a button that would be
rejected.

`passwordHash` must never appear in any of these payloads.

### Order this needs to be built in

1. **CORS** for `http://localhost:5173`, or the browser blocks every call
2. **Seed** of roles and permissions — the tables are empty
3. **First admin**, since no user exists and nobody can sign in without one
4. **BCrypt** hashing (cost 12) and the JWT
5. `POST /auth/login`, then `GET /auth/me`
6. The `/users` endpoints
