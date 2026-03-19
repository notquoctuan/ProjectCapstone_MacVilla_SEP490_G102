import { useState } from 'react'
import { ProductFilters } from '../components/products/ProductFilters'
import { SortBar } from '../components/products/SortBar'
import { ProductGrid } from '../components/products/ProductGrid'
import { Pagination } from '../components/products/Pagination'
import { CompareBar } from '../components/products/CompareBar'
import {
  LISTING_PRODUCTS,
  BREADCRUMBS,
  PAGINATION,
} from '../data/productListingProducts'
import { Breadcrumbs } from '../components/layout/Breadcrumbs'

const MAX_COMPARE = 3

export function ProductsPage() {
  const [compareIds, setCompareIds] = useState(new Set())

  const handleCompareChange = (productId, checked) => {
    setCompareIds((prev) => {
      const next = new Set(prev)
      if (checked) {
        if (next.size < MAX_COMPARE) next.add(productId)
      } else next.delete(productId)
      return next
    })
  }

  return (
    <main className="container mx-auto px-4 lg:px-8 py-6">
      <Breadcrumbs items={BREADCRUMBS} />
      <div className="mb-8">
        <h2 className="text-3xl font-black text-slate-900 dark:text-slate-100 mb-2">
          Giải pháp nhà bếp cao cấp
        </h2>
        <p className="text-slate-500 dark:text-slate-400 max-w-2xl">
          Nâng tầm trải nghiệm nấu nướng với bộ sưu tập thiết bị thông minh
          tiết kiệm điện mới nhất từ HDG Việt Hàn.
        </p>
      </div>
      <div className="flex flex-col lg:flex-row gap-8">
        <ProductFilters />
        <div className="flex-1">
          <SortBar totalProducts={PAGINATION.totalProducts} />
          <ProductGrid
            products={LISTING_PRODUCTS}
            compareIds={compareIds}
            onCompareChange={handleCompareChange}
          />
          <Pagination
            current={PAGINATION.current}
            total={PAGINATION.total}
          />
        </div>
      </div>
      {compareIds.size > 0 && (
        <CompareBar compareCount={compareIds.size} />
      )}
    </main>
  )
}
