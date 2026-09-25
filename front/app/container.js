import { createAuthGateway } from './data/gateways/authGateway'
import { createDepartmentGateway } from './data/gateways/departmentGateway'
import { createEmployeeGateway } from './data/gateways/employeeGateway'
import { createEmployeeTrainingGateway } from './data/gateways/employeeTrainingGateway'
import { createEmploymentRegimeGateway } from './data/gateways/employmentRegimeGateway'
import { createJobRoleGateway } from './data/gateways/jobRoleGateway'
import { createProjectGateway } from './data/gateways/projectGateway'
import { createStageGateway } from './data/gateways/stageGateway'
import { createStockCategoryGateway } from './data/gateways/stockCategoryGateway'
import { createStockGroupGateway } from './data/gateways/stockGroupGateway'
import { createStockItemGateway } from './data/gateways/stockItemGateway'
import { createTeamGateway } from './data/gateways/teamGateway'
import { createTrainingTypeGateway } from './data/gateways/trainingTypeGateway'
import { createUserGateway } from './data/gateways/userGateway'
import { createHttpClient } from './data/http/httpClient'
import { tokenStorage } from './data/storage/tokenStorage'

/*
  Composition root: the one file that knows how the app is wired.

  Screens never import from `data/` and never call fetch. They receive a
  gateway and use it, which keeps the API address, the token and the error
  translation in one place instead of spread across the screens.
*/

export function createContainer({
  baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5262/api',
} = {}) {
  /*
    A rejected token is a whole-app event, not a screen event: whichever call
    hits 401 first, the session has to end once. Listeners subscribe instead of
    the HTTP client reaching into React, which would invert the dependency.
  */
  const sessionExpiredListeners = new Set()
  const notifySessionExpired = (error) => {
    sessionExpiredListeners.forEach((listener) => listener(error))
  }

  const http = createHttpClient({
    baseUrl,
    getToken: () => tokenStorage.read(),
    onUnauthorized: notifySessionExpired,
  })

  return {
    auth: createAuthGateway(http),
    users: createUserGateway(http),

    departments: createDepartmentGateway(http),
    jobRoles: createJobRoleGateway(http),
    employmentRegimes: createEmploymentRegimeGateway(http),
    employees: createEmployeeGateway(http),
    teams: createTeamGateway(http),

    // Clients and suppliers: registered by slice 1, whose gateways already exist in its branch.

    stockCategories: createStockCategoryGateway(http),
    stockGroups: createStockGroupGateway(http),
    stockItems: createStockItemGateway(http),

    projects: createProjectGateway(http),
    stages: createStageGateway(http),

    trainingTypes: createTrainingTypeGateway(http),
    employeeTrainings: createEmployeeTrainingGateway(http),

    tokenStorage,
    baseUrl,

    /** Returns the unsubscribe function, so React can clean up on unmount. */
    onSessionExpired(listener) {
      sessionExpiredListeners.add(listener)
      return () => sessionExpiredListeners.delete(listener)
    },
  }
}
