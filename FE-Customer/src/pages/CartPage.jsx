import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { useCartCount } from '../contexts/CartCountContext'
import {
  CartStepper,
  CartItemList,
  OrderSummary,
} from '../components/cart'
import {
  storeFetchCart,
  storeSetCartLineQuantity,
  storeClearCart,
} from '../api/store/storeCartApi'
import { getApiErrorMessage } from '../lib/errors/apiErrorMessage'
import { mapStoreCartLinesToUiItems } from '../lib/cart/mapStoreCartToUi'

function summaryFromCart(cart, uiItems) {
  if (!cart && uiItems.length === 0) {
    return {
      itemCount: 0,
      subtotal: 0,
      shipping: 0,
      shippingLabel: 'Miễn phí',
      couponDiscount: 0,
      total: 0,
    }
  }
  const sub =
    cart?.merchandiseSubtotal != null
      ? Number(cart.merchandiseSubtotal)
      : uiItems.reduce((s, i) => s + i.price * i.quantity, 0)
  const itemCount = uiItems.reduce((s, i) => s + i.quantity, 0)
  const subtotal = Number.isFinite(sub) ? sub : 0
  return {
    itemCount,
    subtotal,
    shipping: 0,
    shippingLabel: 'Miễn phí',
    couponDiscount: 0,
    total: subtotal,
  }
}

export function CartPage() {
  const { accessToken, isAuthenticated } = useAuth()
  const { applyCartDto } = useCartCount()
  const location = useLocation()

  const [cart, setCart] = useState(null)
  const [loadState, setLoadState] = useState('idle')
  const [loadError, setLoadError] = useState('')
  const [actionError, setActionError] = useState('')

  const loadCart = useCallback(async () => {
    if (!accessToken) return
    setLoadState('loading')
    setLoadError('')
    try {
      const data = await storeFetchCart(accessToken)
      setCart(data && typeof data === 'object' ? data : null)
      setLoadState('success')
    } catch (err) {
      setCart(null)
      setLoadError(getApiErrorMessage(err))
      setLoadState('error')
    }
  }, [accessToken])

  useEffect(() => {
    if (!accessToken) {
      queueMicrotask(() => {
        setCart(null)
        setLoadState('idle')
        setLoadError('')
      })
      return
    }
    let cancelled = false
    queueMicrotask(() => {
      if (cancelled) return
      void loadCart()
    })
    return () => {
      cancelled = true
    }
  }, [accessToken, loadCart])

  useEffect(() => {
    if (!accessToken) return
    applyCartDto(cart && typeof cart === 'object' ? cart : null)
  }, [accessToken, cart, applyCartDto])

  const uiItems = useMemo(
    () => mapStoreCartLinesToUiItems(cart?.lines),
    [cart]
  )

  const summary = useMemo(
    () => summaryFromCart(cart, uiItems),
    [cart, uiItems]
  )

  const handleQuantityChange = useCallback(
    async (variantId, quantity) => {
      if (!accessToken) return
      setActionError('')
      try {
        const data = await storeSetCartLineQuantity(
          accessToken,
          variantId,
          quantity
        )
        setCart(data && typeof data === 'object' ? data : null)
      } catch (err) {
        setActionError(getApiErrorMessage(err))
        await loadCart()
      }
    },
    [accessToken, loadCart]
  )

  const handleClearAll = useCallback(async () => {
    if (!accessToken) return
    setActionError('')
    try {
      await storeClearCart(accessToken)
      await loadCart()
    } catch (err) {
      setActionError(getApiErrorMessage(err))
    }
  }, [accessToken, loadCart])

  const showLoginCta = !isAuthenticated || !accessToken

  return (
    <main className="max-w-screen-2xl mx-auto w-full px-4 py-6 lg:px-10">
      <CartStepper currentStep={1} />
      {showLoginCta ? (
        <div className="mb-4 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-950 dark:border-amber-900/50 dark:bg-amber-950/40 dark:text-amber-100">
          <span>Đăng nhập để xem giỏ hàng đã lưu trên tài khoản. </span>
          <Link
            to="/login"
            state={{ from: `${location.pathname}${location.search}` }}
            className="font-bold text-primary hover:underline"
          >
            Đăng nhập
          </Link>
        </div>
      ) : null}
      {!showLoginCta && loadState === 'loading' ? (
        <p className="mb-4 text-center text-slate-500 dark:text-slate-400 text-sm">
          Đang tải giỏ hàng…
        </p>
      ) : null}
      {!showLoginCta && loadState === 'error' ? (
        <p
          className="mb-4 text-center text-red-600 dark:text-red-400 text-sm"
          role="alert"
        >
          {loadError}
        </p>
      ) : null}
      {actionError ? (
        <p
          className="mb-4 text-center text-red-600 dark:text-red-400 text-sm"
          role="alert"
        >
          {actionError}
        </p>
      ) : null}
      <div className="flex flex-col lg:flex-row gap-6">
        <div className="flex-1 space-y-4">
          <CartItemList
            items={showLoginCta ? [] : uiItems}
            onQuantityChange={handleQuantityChange}
            onClearAll={handleClearAll}
          />
        </div>
        <div className="w-full lg:w-96 space-y-4">
          <OrderSummary
            itemCount={summary.itemCount}
            subtotal={summary.subtotal}
            shipping={summary.shipping}
            shippingLabel={summary.shippingLabel}
            couponDiscount={summary.couponDiscount}
            total={summary.total}
            cartItems={showLoginCta ? [] : uiItems}
          />
        </div>
      </div>
    </main>
  )
}
