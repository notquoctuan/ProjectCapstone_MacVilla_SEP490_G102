import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link, useLocation, useNavigate, useParams } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { useCartCount } from '../contexts/CartCountContext'
import { Breadcrumbs } from '../components/layout/Breadcrumbs'
import {
  ProductGallery,
  ProductInfo,
  ComboPromo,
  TechnicalSpecs,
} from '../components/productDetail'
import {
  storeFetchCategoryTree,
  storeFetchProductDetail,
} from '../api/store/storeCatalogApi'
import { storeAddCartItem } from '../api/store/storeCartApi'
import { ApiError } from '../api/httpClient'
import { findCategoryPathById } from '../lib/catalog/categoryTreePath'
import {
  buildGalleryFromDetail,
  buildHighlightsFromDetail,
  buildTechnicalSpecsFromDetail,
  mapVariantsForUi,
  resolveDetailPricing,
  variantLooksInStock,
} from '../lib/catalog/mapStoreProductDetail'
import { getApiErrorMessage } from '../lib/errors/apiErrorMessage'
import { PRODUCT_DETAIL } from '../data/productDetail'
import { CartSuccessSnackbar } from '../components/ui/CartSuccessSnackbar'

const DEFAULT_SERVICES = PRODUCT_DETAIL.services

export function ProductDetailPage() {
  const { id: slugOrId } = useParams()
  const navigate = useNavigate()
  const location = useLocation()
  const { accessToken } = useAuth()
  const { applyCartDto } = useCartCount()

  const [loadState, setLoadState] = useState('loading')
  const [detail, setDetail] = useState(null)
  const [loadError, setLoadError] = useState('')
  const [notFound, setNotFound] = useState(false)
  const [categoryTree, setCategoryTree] = useState(null)
  const [selectedVariantId, setSelectedVariantId] = useState(null)
  const [addToCartBusy, setAddToCartBusy] = useState(false)
  const [addToCartError, setAddToCartError] = useState('')
  const [cartSnackMessage, setCartSnackMessage] = useState('')

  useEffect(() => {
    if (!cartSnackMessage) return
    const t = window.setTimeout(() => setCartSnackMessage(''), 5500)
    return () => window.clearTimeout(t)
  }, [cartSnackMessage])

  useEffect(() => {
    setAddToCartError('')
    setCartSnackMessage('')
  }, [selectedVariantId])

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
    const key = slugOrId
    queueMicrotask(() => {
      if (cancelled) return
      setLoadState('loading')
      setLoadError('')
      setNotFound(false)
      setDetail(null)
      setSelectedVariantId(null)
    })
    storeFetchProductDetail(key)
      .then((data) => {
        if (cancelled) return
        setDetail(data && typeof data === 'object' ? data : null)
        const variants = Array.isArray(data?.variants) ? data.variants : []
        const firstVariant = variants.find(
          (v) => v && typeof v.id === 'number'
        )
        setSelectedVariantId(firstVariant != null ? firstVariant.id : null)
        setLoadState('success')
      })
      .catch((err) => {
        if (cancelled) return
        setDetail(null)
        if (err instanceof ApiError && err.status === 404) {
          setNotFound(true)
          setLoadState('error')
          return
        }
        setLoadError(getApiErrorMessage(err))
        setLoadState('error')
      })
    return () => {
      cancelled = true
    }
  }, [slugOrId])

  const selectedVariant = useMemo(() => {
    if (!detail || selectedVariantId == null) return null
    const list = Array.isArray(detail.variants) ? detail.variants : []
    return list.find((v) => v?.id === selectedVariantId) ?? null
  }, [detail, selectedVariantId])

  const gallery = useMemo(
    () => (detail ? buildGalleryFromDetail(detail) : null),
    [detail]
  )

  const variantsForUi = useMemo(
    () => (detail ? mapVariantsForUi(detail) : []),
    [detail]
  )

  const pricing = useMemo(
    () => resolveDetailPricing(detail ?? {}, selectedVariant),
    [detail, selectedVariant]
  )

  const specs = useMemo(
    () =>
      detail
        ? buildTechnicalSpecsFromDetail(detail, selectedVariant)
        : [],
    [detail, selectedVariant]
  )

  const highlights = useMemo(
    () => (detail ? buildHighlightsFromDetail(detail) : []),
    [detail]
  )

  const categoryPath = useMemo(() => {
    const cid = detail?.categoryId
    if (typeof cid !== 'number' || !categoryTree?.length) return null
    return findCategoryPathById(categoryTree, cid)
  }, [detail, categoryTree])

  const breadcrumbs = useMemo(() => {
    const items = [
      { label: 'Trang chủ', href: '/' },
      { label: 'Sản phẩm', href: '/products' },
    ]
    if (categoryPath?.length) {
      categoryPath.forEach((node) => {
        items.push({
          label: node.name,
          href: `/products?categoryId=${node.id}`,
        })
      })
    } else if (detail?.categoryName) {
      items.push({
        label: String(detail.categoryName),
        href:
          typeof detail.categoryId === 'number'
            ? `/products?categoryId=${detail.categoryId}`
            : '/products',
      })
    }
    if (detail?.name) {
      items.push({ label: String(detail.name), href: null })
    }
    return items
  }, [categoryPath, detail])

  const handleVariantChange = useCallback((nextId) => {
    setSelectedVariantId(nextId)
  }, [])

  const handleAddToCart = useCallback(async () => {
    if (!accessToken) {
      navigate('/login', {
        state: { from: `${location.pathname}${location.search}` },
      })
      return
    }
    if (selectedVariantId == null || typeof selectedVariantId !== 'number') {
      setAddToCartError('Sản phẩm chưa có biến thể để thêm vào giỏ.')
      return
    }
    setAddToCartBusy(true)
    setAddToCartError('')
    setCartSnackMessage('')
    try {
      const { message, data } = await storeAddCartItem(accessToken, {
        variantId: selectedVariantId,
        quantity: 1,
      })
      if (data && typeof data === 'object') {
        applyCartDto(data)
      }
      const line = data?.lines?.find(
        (l) => l.variantId === selectedVariantId
      )
      let text = message
      if (line?.insufficientStock) {
        text +=
          ' Lưu ý: số lượng trong giỏ có thể vượt tồn kho.'
      }
      setCartSnackMessage(text)
    } catch (err) {
      if (err instanceof ApiError && err.status === 401) {
        navigate('/login', {
          state: { from: `${location.pathname}${location.search}` },
        })
        return
      }
      setAddToCartError(getApiErrorMessage(err))
    } finally {
      setAddToCartBusy(false)
    }
  }, [
    accessToken,
    location.pathname,
    location.search,
    navigate,
    selectedVariantId,
    applyCartDto,
  ])

  const inStock = variantLooksInStock(selectedVariant)
  const addToCartDisabled =
    variantsForUi.length === 0 ||
    selectedVariantId == null ||
    typeof selectedVariantId !== 'number'

  if (loadState === 'loading') {
    return (
      <main className="max-w-7xl mx-auto w-full px-4 lg:px-8 pt-6 pb-20">
        <p className="py-16 text-center text-slate-500 dark:text-slate-400">
          Đang tải sản phẩm…
        </p>
      </main>
    )
  }

  if (loadState === 'error') {
    return (
      <main className="max-w-7xl mx-auto w-full px-4 lg:px-8 pt-6 pb-20">
        <div className="py-16 text-center space-y-4">
          <p className="text-slate-600 dark:text-slate-400" role="alert">
            {notFound
              ? 'Không tìm thấy sản phẩm.'
              : loadError || 'Không tải được sản phẩm.'}
          </p>
          <Link
            to="/products"
            className="inline-block text-primary font-bold hover:underline"
          >
            ← Quay lại danh sách sản phẩm
          </Link>
        </div>
      </main>
    )
  }

  if (!detail || !gallery) {
    return (
      <main className="max-w-7xl mx-auto w-full px-4 lg:px-8 pt-6 pb-20">
        <div className="py-16 text-center space-y-4">
          <p className="text-slate-600 dark:text-slate-400">
            Không có dữ liệu sản phẩm.
          </p>
          <Link
            to="/products"
            className="inline-block text-primary font-bold hover:underline"
          >
            ← Quay lại danh sách sản phẩm
          </Link>
        </div>
      </main>
    )
  }

  return (
    <>
    <main className="max-w-7xl mx-auto w-full px-4 lg:px-8 pt-6 pb-20">
      <Breadcrumbs items={breadcrumbs} />
      <div className="bg-white dark:bg-slate-900 rounded-xl p-6 shadow-sm mb-6">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
          <ProductGallery
            key={detail.id}
            mainImage={gallery.mainImage}
            mainImageAlt={gallery.mainImageAlt}
            thumbnails={gallery.thumbnails}
            moreThumbsCount={gallery.moreThumbsCount}
          />
          <ProductInfo
            name={detail.name}
            rating={0}
            reviewCount={0}
            inStock={inStock}
            price={pricing.price}
            originalPrice={pricing.originalPrice}
            discountPercent={pricing.discountPercent}
            highlights={highlights}
            variants={variantsForUi}
            selectedVariantId={selectedVariantId}
            onVariantChange={handleVariantChange}
            storeServicesItems={DEFAULT_SERVICES}
            onAddToCart={handleAddToCart}
            addToCartDisabled={addToCartDisabled}
            addToCartBusy={addToCartBusy}
            addToCartError={addToCartError}
          />
        </div>
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 mb-6 items-start">
        {detail.description ? (
          <section className="lg:col-span-8 bg-white dark:bg-slate-900 rounded-xl p-5 shadow-sm min-w-0">
            <h2 className="text-base font-bold mb-2 text-slate-900 dark:text-slate-100">
              Mô tả sản phẩm
            </h2>
            <div className="text-sm text-slate-600 dark:text-slate-400 whitespace-pre-wrap leading-relaxed max-h-[min(52vh,26rem)] overflow-y-auto pr-1">
              {detail.description}
            </div>
          </section>
        ) : (
          <div
            className="hidden lg:block lg:col-span-8"
            aria-hidden="true"
          />
        )}
        <div className="lg:col-span-4 space-y-6 min-w-0">
          <TechnicalSpecs items={specs} />
        </div>
      </div>
      <ComboPromo items={[]} />
    </main>
    <CartSuccessSnackbar
      message={cartSnackMessage}
      onDismiss={() => setCartSnackMessage('')}
    />
    </>
  )
}
