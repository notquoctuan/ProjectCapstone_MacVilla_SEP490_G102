import { Icon } from '../ui/Icon'
import { PartnerHeaderUser } from './PartnerHeaderUser'

/**
 * Header đồng bộ với các trang partner (Orders, Payments).
 * @param {{ title: string, subtitle: string, below?: import('react').ReactNode }} props
 */
export function PartnerPaymentsPageHeader({ title, subtitle, below }) {
  return (
    <header className="p-8 pb-0">
      <div className="flex justify-between items-end mb-6 gap-4 flex-wrap">
        <div>
          <h2 className="text-3xl font-black tracking-tight mb-2 text-slate-900 dark:text-slate-50">
            {title}
          </h2>
          <p className="text-slate-500 dark:text-slate-400 max-w-2xl">{subtitle}</p>
        </div>
        <div className="flex items-center gap-4 shrink-0">
          <button
            type="button"
            className="w-10 h-10 rounded-full bg-slate-200 dark:bg-slate-800 flex items-center justify-center text-slate-600 dark:text-slate-400 hover:text-primary transition-colors"
            aria-label="Thông báo"
          >
            <Icon name="notifications" className="text-xl" />
          </button>
          <PartnerHeaderUser hideTextOnMobile={false} />
        </div>
      </div>
      {below}
    </header>
  )
}
