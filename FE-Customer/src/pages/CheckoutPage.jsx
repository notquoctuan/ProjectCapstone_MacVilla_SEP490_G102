import { useCallback, useEffect, useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { useCartCount } from '../contexts/CartCountContext'
import { CartStepper } from '../components/cart'
import {
  ShippingInfo,
  PaymentMethods,
  CheckoutOrderSummary,
} from '../components/checkout'
import { CART_ITEMS } from '../data/cart'
import { PAYMENT_METHODS as CHECKOUT_PAYMENT_METHODS } from '../data/checkout'
import {
  storeFetchAddresses,
  storeCreateAddress,
} from '../api/store/storeAddressesApi'
import { storeCreateOrder, storePreviewOrder } from '../api/store/storeOrdersApi'
import { ApiError } from '../api/httpClient'
import { getApiErrorMessage } from '../lib/errors/apiErrorMessage'
import { mapOrderPreviewLinesToSummaryItems } from '../lib/checkout/mapOrderPreviewToSummary'

/**
 * @typedef {{
 *   lines?: unknown[],
 *   merchandiseSubtotal?: number,
 *   discountAmount?: number,
 *   payableTotal?: number,
 * }} StoreOrderPreviewData
 */

function recalcSummary(items) {
  const subtotal = items.reduce((sum, i) => sum + i.price * i.quantity, 0)
  const couponDiscount = 250000
  const total = Math.max(0, subtotal - couponDiscount)
  return {
    itemCount: items.reduce((s, i) => s + i.quantity, 0),
    subtotal,
    shipping: 0,
    shippingLabel: 'Miễn phí',
    couponDiscount: items.length > 0 ? couponDiscount : 0,
    total: items.length > 0 ? total : 0,
  }
}

function pickInitialAddressId(list) {
  if (!list.length) return null
  const def = list.find((a) => a.isDefault)
  return def?.id ?? list[0].id
}

export function CheckoutPage() {
  const location = useLocation()
  const navigate = useNavigate()
  const { accessToken } = useAuth()
  const { applyCartDto } = useCartCount()

  const [paymentId, setPaymentId] = useState(
    CHECKOUT_PAYMENT_METHODS.find((p) => p.defaultChecked)?.id ??
      CHECKOUT_PAYMENT_METHODS[0].id
  )

  const [addresses, setAddresses] = useState([])
  const [addressLoadState, setAddressLoadState] = useState('idle')
  const [addressLoadError, setAddressLoadError] = useState('')
  const [selectedAddressId, setSelectedAddressId] = useState(
    /** @type {number | null} */ (null)
  )

  const [previewData, setPreviewData] = useState(
    /** @type {StoreOrderPreviewData | null} */ (null)
  )
  const [previewLoading, setPreviewLoading] = useState(false)
  const [previewError, setPreviewError] = useState('')
  const [voucherError, setVoucherError] = useState('')
  const [promoInput, setPromoInput] = useState('')
  const [appliedVoucherCode, setAppliedVoucherCode] = useState(
    /** @type {string | null} */ (null)
  )
  const [placeOrderLoading, setPlaceOrderLoading] = useState(false)
  const [placeOrderError, setPlaceOrderError] = useState('')

  const cartItems =
    location.state?.cartItems && location.state.cartItems.length > 0
      ? location.state.cartItems
      : CART_ITEMS
  const fallbackSummary = recalcSummary(cartItems)
  const summaryItems = previewData
    ? mapOrderPreviewLinesToSummaryItems(previewData.lines)
    : cartItems
  const displaySubtotal =
    previewData && typeof previewData.merchandiseSubtotal === 'number'
      ? previewData.merchandiseSubtotal
      : fallbackSummary.subtotal
  const displayDiscount =
    previewData && typeof previewData.discountAmount === 'number'
      ? previewData.discountAmount
      : fallbackSummary.couponDiscount
  const displayTotal =
    previewData && typeof previewData.payableTotal === 'number'
      ? previewData.payableTotal
      : fallbackSummary.total

  const paymentTitle =
    CHECKOUT_PAYMENT_METHODS.find((p) => p.id === paymentId)?.title ?? 'PayOS'

  const canPlaceOrder =
    selectedAddressId != null &&
    previewData != null &&
    !previewLoading &&
    addressLoadState === 'success' &&
    !addressLoadError

  const loadAddresses = useCallback(async () => {
    if (!accessToken) return
    setAddressLoadState('loading')
    setAddressLoadError('')
    try {
      const list = await storeFetchAddresses(accessToken)
      setAddresses(Array.isArray(list) ? list : [])
      setAddressLoadState('success')
      setSelectedAddressId((prev) => {
        const arr = Array.isArray(list) ? list : []
        if (prev != null && arr.some((a) => a.id === prev)) return prev
        return pickInitialAddressId(arr)
      })
    } catch (err) {
      setAddresses([])
      setAddressLoadError(getApiErrorMessage(err))
      setAddressLoadState('error')
    }
  }, [accessToken])

  useEffect(() => {
    if (!accessToken) {
      navigate('/login', {
        state: { from: `${location.pathname}${location.search}` },
        replace: true,
      })
      return
    }
    let cancelled = false
    queueMicrotask(() => {
      if (cancelled) return
      void loadAddresses()
    })
    return () => {
      cancelled = true
    }
  }, [accessToken, navigate, location.pathname, location.search, loadAddresses])

  useEffect(() => {
    if (
      !accessToken ||
      selectedAddressId == null ||
      addressLoadState !== 'success'
    ) {
      return
    }
    let cancelled = false
    const voucherForRequest = appliedVoucherCode

    queueMicrotask(() => {
      if (cancelled) return
      setPreviewLoading(true)
      setPreviewError('')
      void storePreviewOrder(accessToken, {
        shippingAddressId: selectedAddressId,
        voucherCode: voucherForRequest,
        paymentMethod: paymentId,
      })
        .then((data) => {
          if (cancelled) return
          setPreviewData(data)
          setPreviewError('')
          setVoucherError('')
        })
        .catch((e) => {
          if (cancelled) return
          const msg = getApiErrorMessage(e)
          const isNotFound = e instanceof ApiError && e.errorCode === 'NOT_FOUND'
          const hadVoucher =
            voucherForRequest != null &&
            String(voucherForRequest).trim() !== ''
          if (isNotFound && hadVoucher) {
            setVoucherError(msg)
            setAppliedVoucherCode(null)
          } else {
            setPreviewError(msg)
            setPreviewData(null)
          }
        })
        .finally(() => {
          if (!cancelled) setPreviewLoading(false)
        })
    })

    return () => {
      cancelled = true
    }
  }, [
    accessToken,
    selectedAddressId,
    paymentId,
    appliedVoucherCode,
    addressLoadState,
  ])

  const handleApplyPromo = useCallback(() => {
    setVoucherError('')
    const t = promoInput.trim()
    setAppliedVoucherCode(t === '' ? null : t)
  }, [promoInput])

  const handleCreateAddress = useCallback(
    async (payload) => {
      if (!accessToken) {
        throw new Error('Phiên đăng nhập không hợp lệ.')
      }
      const created = await storeCreateAddress(accessToken, payload)
      await loadAddresses()
      if (created && typeof created.id === 'number') {
        setSelectedAddressId(created.id)
      }
    },
    [accessToken, loadAddresses]
  )

  const handlePlaceOrder = useCallback(async () => {
    setPlaceOrderError('')
    if (!accessToken) {
      setPlaceOrderError('Phiên đăng nhập không hợp lệ.')
      return
    }
    if (selectedAddressId == null) {
      setPlaceOrderError('Vui lòng chọn địa chỉ nhận hàng.')
      return
    }
    if (!previewData || previewLoading) {
      setPlaceOrderError('Vui lòng chờ hệ thống cập nhật đơn hàng.')
      return
    }
    setPlaceOrderLoading(true)
    try {
      const data = await storeCreateOrder(accessToken, {
        shippingAddressId: selectedAddressId,
        voucherCode: appliedVoucherCode,
        paymentMethod: paymentId,
      })
      const orderCode =
        data && typeof data.orderCode === 'string' ? data.orderCode : ''
      const payableTotal =
        data && typeof data.payableTotal === 'number'
          ? data.payableTotal
          : null
      applyCartDto({ lines: [] })
      navigate('/checkout/process', {
        replace: true,
        state: {
          orderCode,
          paymentMethod:
            (data && typeof data.paymentMethod === 'string'
              ? data.paymentMethod
              : null) ?? paymentTitle,
          payableTotal,
        },
      })
    } catch (e) {
      setPlaceOrderError(getApiErrorMessage(e))
    } finally {
      setPlaceOrderLoading(false)
    }
  }, [
    accessToken,
    selectedAddressId,
    previewData,
    previewLoading,
    appliedVoucherCode,
    paymentId,
    paymentTitle,
    navigate,
    applyCartDto,
  ])

  if (!accessToken) {
    return (
      <main className="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
        <p className="text-center text-slate-600 dark:text-slate-400 text-sm">
          Đang chuyển đến trang đăng nhập…
        </p>
        <p className="text-center mt-2">
          <Link to="/login" className="text-primary font-bold hover:underline">
            Đăng nhập
          </Link>
        </p>
      </main>
    )
  }

  return (
    <main className="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
      <CartStepper currentStep={2} />
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        <div className="lg:col-span-7 space-y-6">
          <ShippingInfo
            addresses={addresses}
            loading={addressLoadState === 'loading'}
            error={addressLoadError}
            selectedAddressId={selectedAddressId}
            onSelectAddressId={setSelectedAddressId}
            onCreateAddress={handleCreateAddress}
          />
          <PaymentMethods value={paymentId} onChange={setPaymentId} />
        </div>
        <div className="lg:col-span-5 space-y-6">
          <CheckoutOrderSummary
            items={summaryItems}
            subtotal={displaySubtotal}
            shipping={fallbackSummary.shipping}
            shippingLabel={fallbackSummary.shippingLabel}
            couponDiscount={displayDiscount}
            total={displayTotal}
            onPlaceOrder={handlePlaceOrder}
            promoCode={promoInput}
            onPromoCodeChange={setPromoInput}
            onApplyPromo={handleApplyPromo}
            voucherError={voucherError}
            previewError={previewError}
            previewLoading={previewLoading}
            placeOrderLoading={placeOrderLoading}
            placeOrderError={placeOrderError}
            canPlaceOrder={canPlaceOrder}
          />
        </div>
      </div>
    </main>
  )
}
