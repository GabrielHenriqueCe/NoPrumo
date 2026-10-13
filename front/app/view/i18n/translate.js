import { en } from './messages/en'
import { enScreens } from './messages/en.screens'
import { ptBR } from './messages/ptBR'
import { ptBRScreens } from './messages/ptBR.screens'

const ENGLISH = { ...en, ...enScreens }
const MESSAGES = { en: ENGLISH, 'pt-BR': { ...ptBR, ...ptBRScreens } }

function lookup(messages, key) {
  return key.split('.').reduce((node, part) => (node == null ? undefined : node[part]), messages)
}

/*
  t('projects.count', { count: 3 }) → '3 projects'.

  A key missing in Portuguese falls back to English, and a key missing in both
  shows the key itself: a screen half-translated still reads, and the missing
  text is easy to spot.

  A value can be an object { one, other } for words that change with the count.
*/
export function createTranslator(language) {
  const messages = MESSAGES[language] ?? ENGLISH

  return function t(key, values = {}) {
    let text = lookup(messages, key) ?? lookup(ENGLISH, key) ?? key

    if (typeof text === 'object') {
      text = values.count === 1 ? text.one : text.other
    }

    return String(text).replace(/\{(\w+)\}/g, (match, name) => (name in values ? String(values[name]) : match))
  }
}
