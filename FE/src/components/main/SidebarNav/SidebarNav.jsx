import { NavItem } from './NavItem'
import { SIDEBAR_NAV } from '../../../data/navigation'

export function SidebarNav() {
  return (
    <aside className="hidden lg:block lg:col-span-3 bg-white dark:bg-slate-800 rounded-xl shadow-sm overflow-hidden border border-slate-200 dark:border-slate-700">
      <nav className="flex flex-col">
        {SIDEBAR_NAV.map((item, index) => (
          <NavItem
            key={item.id}
            icon={item.icon}
            label={item.label}
            children={item.children}
            isLast={index === SIDEBAR_NAV.length - 1}
          />
        ))}
      </nav>
    </aside>
  )
}
