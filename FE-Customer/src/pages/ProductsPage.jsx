import { useState, useEffect, useMemo, useCallback } from 'react'
import { useSearchParams } from 'react-router-dom'
import { ProductFilters } from '../components/products/ProductFilters'
import { SortBar } from '../components/products/SortBar'
import { ProductGrid } from '../components/products/ProductGrid'
import { Pagination } from '../components/products/Pagination'
import { CompareBar } from '../components/products/CompareBar'
import { Breadcrumbs } from '../components/layout/Breadcrumbs'
import {
  storeFetchProducts,
  storeFetchCategoryTree,
} from '../api/store/storeCatalogApi'
import { mapStoreProductToListingCard } from '../lib/catalog/mapStoreProductToListingCard'
import { findCategoryPathById } from '../lib/catalog/categoryTreePath'
import { getApiErrorMessage } from '../lib/errors/apiErrorMessage'

const MAX_COMPARE = 3
const PAGE_SIZE = 12

const DEFAULT_BREADCRUMBS = [
  { label: 'Trang chủ', href: '/' },
  { label: 'Sản phẩm', href: null },
]

export function ProductsPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const [compareIds, setCompareIds] = useState(new Set())

  const categoryIdRaw = searchParams.get('categoryId')
  const categoryId = useMemo(() => {
    if (categoryIdRaw == null || categoryIdRaw === '') return null
    const n = parseInt(categoryIdRaw, 10)
    return Number.isFinite(n) && n > 0 ? n : null
  }, [categoryIdRaw])

  const page = useMemo(() => {
    const p = parseInt(searchParams.get('page') || '1', 10)
    return Number.isFinite(p) && p > 0 ? p : 1
  }, [searchParams])

  const [loadState, setLoadState] = useState('loading')
  const [listPayload, setListPayload] = useState(null)
  const [loadError, setLoadError] = useState('')
  const [categoryTree, setCategoryTree] = useState(null)

  useEffect(() => {
    let cancelled = false
    storeFetchCategoryTree()
      .then((data) => {
        if (cancelled) return
        setCategoryTree(Array.isArray(data) ? data : [])
      })
      .catch(() => {
        if (cancelled) return
        setCategoryTree(null)
      })
    return () => {
      cancelled = true
    }
  }, [])

  useEffect(() => {
    let cancelled = false
    queueMicrotask(() => {
      if (cancelled) return
      setLoadState('loading')
      setLoadError('')
    })
    storeFetchProducts({
      page,
      pageSize: PAGE_SIZE,
      categoryId,
      includeSubcategories: true,
    })
      .then((data) => {
        if (cancelled) return
        setListPayload(data)
        setLoadState('success')
      })
      .catch((err) => {
        if (cancelled) return
        setListPayload(null)
        setLoadError(getApiErrorMessage(err))
        setLoadState('error')
      })
    return () => {
      cancelled = true
    }
  }, [page, categoryId])

  useEffect(() => {
    if (!listPayload || loadState !== 'success') return
    const tp = Math.max(
      1,
      Math.ceil((listPayload.totalCount || 0) / PAGE_SIZE)
    )
    if (page > tp) {
      queueMicrotask(() => {
        setSearchParams(
          (prev) => {
            const next = new URLSearchParams(prev)
            next.delete('page')
            return next
          },
          { replace: true }
        )
      })
    }
  }, [listPayload, loadState, page, setSearchParams])

  const products = useMemo(() => {
    const items = listPayload?.items
    if (!Array.isArray(items)) return []
    return items.map(mapStoreProductToListingCard)
  }, [listPayload])

  const totalCount = listPayload?.totalCount ?? 0
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE))

  const categoryPath = useMemo(() => {
    if (categoryId == null || !categoryTree?.length) return null
    return findCategoryPathById(categoryTree, categoryId)
  }, [categoryId, categoryTree])

  const headingTitle = useMemo(() => {
    if (categoryPath?.length) {
      return categoryPath[categoryPath.length - 1].name
    }
    if (categoryId && products.length > 0 && products[0]?.tag) {
      return products[0].tag
    }
    if (categoryId) return 'Sản phẩm theo danh mục'
    return 'Tất cả sản phẩm'
  }, [categoryId, categoryPath, products])

  const breadcrumbs = useMemo(() => {
    if (!categoryId) return DEFAULT_BREADCRUMBS

    if (categoryPath?.length) {
      const items = [
        { label: 'Trang chủ', href: '/' },
        { label: 'Sản phẩm', href: '/products' },
      ]
      categoryPath.forEach((node, idx) => {
        const isLast = idx === categoryPath.length - 1
        const href = isLast
          ? null
          : `/products?categoryId=${node.id}`
        items.push({ label: node.name, href })
      })
      return items
    }

    const label =
      products.length > 0 && products[0]?.tag
        ? products[0].tag
        : 'Danh mục'
    return [
      { label: 'Trang chủ', href: '/' },
      { label: 'Sản phẩm', href: '/products' },
      { label, href: null },
    ]
  }, [categoryId, categoryPath, products])

  const handlePageChange = useCallback(
    (nextPage) => {
      const p = Math.min(Math.max(1, nextPage), totalPages)
      setSearchParams((prev) => {
        const next = new URLSearchParams(prev)
        if (p <= 1) next.delete('page')
        else next.set('page', String(p))
        return next
      })
    },
    [setSearchParams, totalPages]
  )

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
      <Breadcrumbs items={breadcrumbs} />
      <div className="mb-8">
        <h2 className="text-3xl font-black text-slate-900 dark:text-slate-100 mb-2">
          {headingTitle}
        </h2>
        <p className="text-slate-500 dark:text-slate-400 max-w-2xl">
          {categoryId
            ? 'Sản phẩm đang hiển thị gồm cả danh mục con (theo cấu hình cửa hàng).'
            : 'Khám phá thiết bị phòng tắm, nhà bếp và giải pháp gia dụng từ Macvilla.'}
        </p>
      </div>
      <div className="flex flex-col lg:flex-row gap-8">
        <ProductFilters />
        <div className="flex-1 min-w-0">
          <SortBar totalProducts={totalCount} />
          {loadState === 'loading' ? (
            <p className="py-12 text-center text-slate-500 dark:text-slate-400">
              Đang tải sản phẩm…
            </p>
          ) : null}
          {loadState === 'error' ? (
            <p
              className="py-12 text-center text-red-600 dark:text-red-400"
              role="alert"
            >
              {loadError}
            </p>
          ) : null}
          {loadState === 'success' && products.length === 0 ? (
            <p className="py-12 text-center text-slate-500 dark:text-slate-400">
              Không có sản phẩm trong danh mục này.
            </p>
          ) : null}
          {loadState === 'success' && products.length > 0 ? (
            <ProductGrid
              products={products}
              compareIds={compareIds}
              onCompareChange={handleCompareChange}
            />
          ) : null}
          {loadState === 'success' && totalPages > 1 ? (
            <Pagination
              current={page}
              total={totalPages}
              onPageChange={handlePageChange}
            />
          ) : null}
        </div>
      </div>
      {compareIds.size > 0 && (
        <CompareBar compareCount={compareIds.size} />
      )}
    </main>
  )
}
