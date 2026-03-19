import { useState } from 'react'
import { SORT_OPTIONS } from '../../../data/productListingProducts'

/**
 * Sort options bar + product count
 */
export function SortBar({ totalProducts = 24 }) {
  const [activeId, setActiveId] = useState(SORT_OPTIONS[0].id)

  return (
    <div className="bg-white dark:bg-slate-900 rounded-xl p-3 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-wrap items-center justify-between gap-4 mb-6">
      <div className="flex items-center gap-4">
        <span className="text-sm font-bold text-slate-500">Sắp xếp:</span>
        <div className="flex items-center gap-2">
          {SORT_OPTIONS.map((opt) => (
            <button
              key={opt.id}
              type="button"
              onClick={() => setActiveId(opt.id)}
              className={`px-4 py-1.5 rounded-lg text-sm font-semibold transition-colors ${
                activeId === opt.id
                  ? 'bg-primary text-white'
                  : 'bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700'
              }`}
            >
              {opt.label}
            </button>
          ))}
        </div>
      </div>
      <p className="text-sm text-slate-500 font-medium">
        Tìm thấy {totalProducts} sản phẩm
      </p>
    </div>
  )
}
