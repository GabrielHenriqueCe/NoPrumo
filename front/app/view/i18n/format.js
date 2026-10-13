/*
  Numbers, money and dates in the reader's language. English uses en-GB so the
  date stays day/month/year in both languages; only the separators change
  (1,234.56 against 1.234,56).

  Dates travel as 'YYYY-MM-DD' (C# DateOnly) and are never parsed with
  `new Date('2026-10-02')`: that is UTC midnight, which in Brazil is still the 1st.
*/

const LOCALES = { en: 'en-GB', 'pt-BR': 'pt-BR' }

export function createFormatter(language) {
  const locale = LOCALES[language] ?? 'en-GB'

  const number = new Intl.NumberFormat(locale, { maximumFractionDigits: 2 })
  const quantity = new Intl.NumberFormat(locale, { maximumFractionDigits: 3 })
  const money = new Intl.NumberFormat(locale, { style: 'currency', currency: 'BRL' })
  const compactMoney = new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: 'BRL',
    notation: 'compact',
    maximumFractionDigits: 1,
  })
  const percent = new Intl.NumberFormat(locale, { maximumFractionDigits: 1 })
  const dayMonth = new Intl.DateTimeFormat(locale, { day: '2-digit', month: 'short' })

  return {
    locale,
    number: (value) => (value == null ? '—' : number.format(value)),
    quantity: (value) => (value == null ? '—' : quantity.format(value)),
    money: (value) => (value == null ? '—' : money.format(value)),
    compactMoney: (value) => (value == null ? '—' : compactMoney.format(value)),
    /** 12.5 → '12.5%' / '12,5%' — the value already comes from 0 to 100. */
    percent: (value) => (value == null ? '—' : `${percent.format(value)}%`),
    /** '2026-10-02' → '02/10/2026'. */
    date: (iso) => {
      if (!iso) return '—'
      const [year, month, day] = iso.slice(0, 10).split('-')
      return `${day}/${month}/${year}`
    },
    /** '2026-10-02' → '02 Oct' / '02 de out.' — for chart axes. */
    dayMonth: (iso) => {
      const [year, month, day] = iso.slice(0, 10).split('-').map(Number)
      return dayMonth.format(new Date(year, month - 1, day))
    },
  }
}
