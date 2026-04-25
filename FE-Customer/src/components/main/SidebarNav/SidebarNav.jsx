import { useEffect, useState, useMemo } from 'react'
import { NavItem } from './NavItem'
import { SIDEBAR_NAV } from '../../../data/navigation'
import { storeFetchCategoryTree } from '../../../api/store/storeCatalogApi'
import { mapCategoryTreeToSidebarItems } from '../../../lib/catalog/mapCategoryTreeToSidebar'

export function SidebarNav() {
  const [tree, setTree] = useState(null)
  const [loadState, setLoadState] = useState('loading')

  useEffect(() => {
    let cancelled = false
    storeFetchCategoryTree()
      .then((data) => {
        if (cancelled) return
        setTree(Array.isArray(data) ? data : [])
        setLoadState('success')
      })
      .catch(() => {
        if (cancelled) return
        setTree(null)
        setLoadState('error')
      })
    return () => {
      cancelled = true
    }
  }, [])

  const items = useMemo(() => {
    const mapped = mapCategoryTreeToSidebarItems(tree)
    if (mapped.length > 0) return mapped
    if (loadState === 'loading') return []
    return SIDEBAR_NAV.map((item) => ({
      ...item,
      to: '/products',
      children: item.children?.map((c) => ({
        label: c.label,
        to: c.href && c.href !== '#' ? c.href : '/products',
      })),
    }))
  }, [tree, loadState])

  return (
    <aside className="hidden lg:block w-full bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 overflow-visible">
      <div className="px-4 py-2 border-b border-slate-100 dark:border-slate-700 bg-slate-50/80 dark:bg-slate-900/40 rounded-t-xl overflow-hidden">
        <p className="text-xs font-bold uppercase tracking-wider text-slate-500 dark:text-slate-400">
          Danh mục sản phẩm
        </p>
      </div>
      <nav className="flex flex-col min-h-[120px]">
        {loadState === 'loading' && items.length === 0 ? (
          <div className="px-4 py-6 text-sm text-slate-500 dark:text-slate-400">
            Đang tải danh mục…
          </div>
        ) : null}
        {items.map((item, index) => (
          <NavItem
            key={item.id}
            icon={item.icon}
            label={item.label}
            to={item.to}
            children={item.children}
            isLast={index === items.length - 1}
          />
        ))}
        {loadState === 'error' && items.length > 0 ? (
          <p className="px-4 py-2 text-[11px] text-amber-600 dark:text-amber-400 border-t border-slate-100 dark:border-slate-700">
            Không tải được danh mục từ máy chủ — đang hiển thị mẫu tĩnh.
          </p>
        ) : null}
      </nav>
    </aside>
  )
}
