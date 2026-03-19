import { Link } from 'react-router-dom'
import { Icon } from '../../ui/Icon'

/**
 * Single nav item with optional dropdown (children). Clicking category navigates to /products.
 */
export function NavItem({ icon, label, children, isLast = false }) {
  const hasChildren = children && children.length > 0

  return (
    <div
      className={`group relative px-4 py-3 hover:bg-primary/10 transition-colors ${
        isLast ? '' : 'border-b border-slate-100 dark:border-slate-700'
      }`}
    >
      <Link
        to="/products"
        className="flex items-center justify-between cursor-pointer"
      >
        <div className="flex items-center gap-3">
          <Icon name={icon} className="text-primary" />
          <span className="font-medium">{label}</span>
        </div>
        {hasChildren && (
          <Icon name="chevron_right" className="text-slate-400" />
        )}
      </Link>
      {hasChildren && (
        <div className="hidden group-hover:block absolute left-full top-0 w-64 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 shadow-xl z-10 rounded-r-xl py-2">
          {children.map((link) => (
            <Link
              key={link.label}
              to="/products"
              className="block px-4 py-2 hover:text-primary font-semibold"
            >
              {link.label}
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
