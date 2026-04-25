import { useState, useMemo, useCallback } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { PartnerHeaderUser } from '../components/partner/PartnerHeaderUser'
import { storeFetchVariantBySku } from '../api/store/storeCatalogApi'
import { storeB2bCreateQuoteRequest } from '../api/store/storeB2bQuotesApi'
import { ApiError } from '../api/httpClient'
import { getApiErrorMessage } from '../lib/errors/apiErrorMessage'
import { mapValidationErrorsToFirstMessage } from '../lib/auth/mapValidationErrors'
import { useAuth } from '../contexts/AuthContext'

function formatMoney(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + ' đ'
}

function parseMoney(str) {
  const num = parseInt(String(str).replace(/\D/g, ''), 10)
  return isNaN(num) ? 0 : num
}

/** @param {Awaited<ReturnType<typeof storeFetchVariantBySku>>} v */
function variantToRow(v) {
  const price =
    v.retailPrice != null && !Number.isNaN(Number(v.retailPrice))
      ? String(Math.round(Number(v.retailPrice)))
      : '0'
  const vn = (v.variantName || '').trim()
  const isDefault =
    !vn || /^mặc định/i.test(vn) || vn.toLowerCase() === 'default'
  const name = isDefault
    ? v.productName
    : `${v.productName} — ${vn.replace(/\s*\(seed\)\s*$/i, '').trim()}`
  return {
    id: `variant-${v.id}`,
    variantId: v.id,
    productId: v.productId,
    sku: v.sku,
    name,
    unitPrice: price,
    qty: 1,
    note: '',
  }
}

/** Mỗi dòng: "SKU - ghi chú," rồi xuống dòng (theo yêu cầu gửi BE). */
function buildQuoteNotes(rows) {
  return rows
    .map((r) => {
      const note = (r.note || '').trim()
      return `${r.sku} - ${note || '—'},`
    })
    .join('\n')
}

export function PartnerQuotationCreatePage() {
  const [searchProduct, setSearchProduct] = useState('')
  const [items, setItems] = useState(
    /** @type {ReturnType<typeof variantToRow>[]} */ ([])
  )
  const [addingSku, setAddingSku] = useState(false)
  const [searchError, setSearchError] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState('')
  const [fieldErrors, setFieldErrors] = useState(
    /** @type {Record<string, string>} */ ({})
  )

  const { accessToken, isAuthenticated } = useAuth()
  const navigate = useNavigate()

  const totalAmount = useMemo(() => {
    return items.reduce((sum, row) => sum + parseMoney(row.unitPrice) * (Number(row.qty) || 0), 0)
  }, [items])

  const updateItem = (id, field, value) => {
    setItems((prev) =>
      prev.map((row) => (row.id === id ? { ...row, [field]: value } : row))
    )
  }

  const removeItem = (id) => {
    setItems((prev) => prev.filter((row) => row.id !== id))
  }

  const handleAddBySku = useCallback(async () => {
    const sku = searchProduct.trim()
    setSearchError('')
    if (!sku) {
      setSearchError('Vui lòng nhập SKU.')
      return
    }
    setAddingSku(true)
    try {
      const data = await storeFetchVariantBySku(sku)
      const row = variantToRow(data)
      setItems((prev) => {
        const idx = prev.findIndex(
          (r) => r.sku.toLowerCase() === row.sku.toLowerCase()
        )
        if (idx >= 0) {
          const next = [...prev]
          const cur = next[idx]
          next[idx] = {
            ...cur,
            qty: Number(cur.qty) + 1,
            unitPrice: row.unitPrice,
            name: row.name,
            variantId: row.variantId,
            productId: row.productId,
          }
          return next
        }
        return [...prev, row]
      })
      setSearchProduct('')
    } catch (err) {
      setSearchError(getApiErrorMessage(err))
    } finally {
      setAddingSku(false)
    }
  }, [searchProduct])

  const handleSubmit = async (e) => {
    e.preventDefault()
    setSubmitError('')
    setFieldErrors({})
    if (!isAuthenticated || !accessToken) {
      setSubmitError('Vui lòng đăng nhập tài khoản doanh nghiệp để gửi yêu cầu.')
      return
    }
    if (items.length === 0) {
      setSubmitError('Thêm ít nhất một sản phẩm (theo SKU) trước khi gửi.')
      return
    }
    const missingVariant = items.find(
      (r) => r.variantId == null || Number.isNaN(Number(r.variantId))
    )
    if (missingVariant) {
      setSubmitError(
        `Dòng "${missingVariant.sku}" thiếu mã biến thể. Chỉ thêm hàng bằng tra cứu SKU.`
      )
      return
    }
    const payloadItems = items.map((r) => ({
      variantId: Number(r.variantId),
      quantity: Math.max(1, Math.floor(Number(r.qty)) || 1),
    }))
    setSubmitting(true)
    try {
      const data = await storeB2bCreateQuoteRequest(accessToken, {
        items: payloadItems,
        notes: buildQuoteNotes(items),
      })
      navigate('/partner/quotation/history', {
        replace: false,
        state: {
          quoteSubmitted: true,
          quoteCode: data.quoteCode,
          quoteId: data.id,
        },
      })
    } catch (err) {
      if (err instanceof ApiError && err.errorCode === 'VALIDATION_ERROR') {
        setFieldErrors(mapValidationErrorsToFirstMessage(err.errors))
      }
      setSubmitError(getApiErrorMessage(err))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <>
      {/* Header */}
      <header className="h-16 border-b border-slate-200 dark:border-slate-800 bg-white/80 dark:bg-slate-900/80 backdrop-blur flex items-center justify-between px-4 sm:px-6 sticky top-0 z-10">
        <div className="flex items-center gap-2 text-sm">
          <Link to="/partner/quotation" className="text-slate-500 hover:text-primary transition-colors">
            Báo giá
          </Link>
          <Icon name="chevron_right" className="text-xs text-slate-400" />
          <span className="font-semibold text-slate-900 dark:text-slate-100">Tạo yêu cầu mới</span>
        </div>
        <div className="flex items-center gap-4">
          <button
            type="button"
            className="p-2 text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg relative"
            aria-label="Thông báo"
          >
            <Icon name="notifications" className="text-xl" />
            <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white dark:border-slate-900" />
          </button>
          <div className="h-8 w-px bg-slate-200 dark:bg-slate-800" />
          <PartnerHeaderUser size="sm" />
        </div>
      </header>

      {/* Body */}
      <div className="w-full flex-1 space-y-6 px-4 sm:px-6 py-6 sm:py-8">
        <div className="flex flex-wrap justify-between items-end gap-4">
          <div>
            <h2 className="text-2xl font-bold text-slate-900 dark:text-slate-100">Tạo yêu cầu báo giá</h2>
            <p className="text-slate-500 text-sm mt-1">
              Thêm sản phẩm và điều chỉnh số lượng để gửi yêu cầu báo giá.
            </p>
          </div>
          <div className="text-xs text-slate-500 dark:text-slate-400 bg-slate-50 dark:bg-slate-800/50 px-3 py-1.5 rounded-lg border border-slate-200 dark:border-slate-700 max-w-xs text-right">
            Mã báo giá sẽ được hệ thống cấp sau khi gửi thành công.
          </div>
        </div>

        {submitError ? (
          <div
            className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950/40 dark:text-red-200"
            role="alert"
          >
            {submitError}
          </div>
        ) : null}
        {Object.keys(fieldErrors).length > 0 ? (
          <div
            className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900 dark:border-amber-900 dark:bg-amber-950/40 dark:text-amber-100"
            role="alert"
          >
            {Object.values(fieldErrors)[0]}
          </div>
        ) : null}

        {/* Thêm sản phẩm nhanh */}
        <section className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm">
          <div className="flex items-center gap-2 mb-6 text-primary">
            <Icon name="add_shopping_cart" className="text-xl" />
            <h3 className="font-bold text-lg text-slate-900 dark:text-white">Thêm sản phẩm nhanh</h3>
          </div>
          <div className="flex gap-4">
            <div className="relative flex-1">
              <Icon name="search" className="absolute left-3 top-3 text-slate-400" />
              <input
                type="text"
                value={searchProduct}
                onChange={(e) => {
                  setSearchProduct(e.target.value)
                  setSearchError('')
                }}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    e.preventDefault()
                    void handleAddBySku()
                  }
                }}
                placeholder="Nhập đúng SKU (ví dụ: seed-v-seed-sp-sofa-goc-l)"
                autoComplete="off"
                className="w-full pl-10 pr-3 py-3 rounded-lg border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800/50 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100 placeholder-slate-400"
              />
            </div>
            <button
              type="button"
              onClick={() => void handleAddBySku()}
              disabled={addingSku}
              className="bg-primary hover:bg-primary/90 disabled:opacity-60 disabled:pointer-events-none text-white font-bold py-3 px-8 rounded-lg flex items-center gap-2 transition-all active:scale-95 shadow-lg shadow-primary/20 shrink-0"
            >
              <Icon name="add" className="text-lg" />
              {addingSku ? 'Đang tải…' : 'Thêm'}
            </button>
          </div>
          {searchError ? (
            <p className="mt-3 text-sm text-red-600 dark:text-red-400" role="alert">
              {searchError}
            </p>
          ) : (
            <p className="mt-3 text-xs text-slate-500 dark:text-slate-400">
              Tra cứu theo SKU từ kho — sản phẩm trùng SKU sẽ tăng số lượng thêm 1.
            </p>
          )}
        </section>

        {/* Danh sách sản phẩm */}
        <section className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm overflow-hidden">
          <div className="p-6 border-b border-slate-100 dark:border-slate-800 flex flex-wrap justify-between items-center gap-4">
            <div className="flex items-center gap-2 text-primary">
              <Icon name="list_alt" className="text-xl" />
              <h3 className="font-bold text-lg text-slate-900 dark:text-white">Danh sách sản phẩm yêu cầu</h3>
            </div>
            <span className="text-xs bg-slate-100 dark:bg-slate-800 px-3 py-1 rounded-full font-medium text-slate-500 uppercase tracking-tight">
              Số lượng: {items.length} sản phẩm
            </span>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-slate-50 dark:bg-slate-800/50 text-slate-500 uppercase text-[10px] font-bold tracking-widest">
                <tr>
                  <th className="px-6 py-4">Sản phẩm / SKU</th>
                  <th className="px-6 py-4 w-40">Đơn giá dự kiến</th>
                  <th className="px-6 py-4 w-32 text-center">Số lượng</th>
                  <th className="px-6 py-4">Ghi chú mục</th>
                  <th className="px-6 py-4 w-20 text-center">Xóa</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                {items.length === 0 ? (
                  <tr>
                    <td
                      colSpan={5}
                      className="px-6 py-12 text-center text-sm text-slate-500 dark:text-slate-400"
                    >
                      Chưa có dòng hàng. Nhập SKU ở trên và bấm <strong>Thêm</strong>.
                    </td>
                  </tr>
                ) : null}
                {items.map((row) => (
                  <tr key={row.id} className="hover:bg-slate-50/50 dark:hover:bg-slate-800/30 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex flex-col">
                        <span className="font-bold text-sm text-slate-900 dark:text-slate-100 uppercase">{row.sku}</span>
                        <span className="text-xs text-slate-500">{row.name}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="relative">
                        <input
                          type="text"
                          value={row.unitPrice}
                          onChange={(e) => updateItem(row.id, 'unitPrice', e.target.value)}
                          className="w-full py-1.5 px-2 text-sm border-transparent bg-transparent border-b border-slate-200 dark:border-slate-700 focus:border-primary focus:ring-0 outline-none text-right font-medium text-slate-900 dark:text-slate-100"
                        />
                        <span className="absolute right-0 top-1.5 text-[10px] text-slate-400">đ</span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center justify-center">
                        <input
                          type="number"
                          value={row.qty}
                          onChange={(e) => updateItem(row.id, 'qty', e.target.value)}
                          min={1}
                          className="w-20 py-1.5 px-2 text-center text-sm rounded border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 focus:ring-1 focus:ring-primary focus:border-primary outline-none text-slate-900 dark:text-slate-100"
                        />
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <input
                        type="text"
                        value={row.note}
                        onChange={(e) => updateItem(row.id, 'note', e.target.value)}
                        placeholder="Thêm ghi chú..."
                        className="w-full py-1.5 px-2 text-xs border-transparent bg-transparent border-b border-slate-200 dark:border-slate-700 focus:border-primary focus:ring-0 outline-none italic text-slate-900 dark:text-slate-100 placeholder-slate-400"
                      />
                    </td>
                    <td className="px-6 py-4 text-center">
                      <button
                        type="button"
                        onClick={() => removeItem(row.id)}
                        className="text-slate-400 hover:text-red-500 transition-colors p-1"
                        aria-label="Xóa"
                      >
                        <Icon name="delete_outline" className="text-xl" />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="p-4 bg-slate-50 dark:bg-slate-800/30 text-right border-t border-slate-100 dark:border-slate-800">
            <p className="text-sm font-medium text-slate-600 dark:text-slate-400">
              Tổng giá trị dự kiến: <span className="text-lg font-bold text-primary ml-2">{formatMoney(totalAmount)}</span>
            </p>
          </div>
        </section>
      </div>

      {/* Footer Actions */}
      <footer className="mt-auto border-t border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 px-4 sm:px-6 py-4 sticky bottom-0 z-10">
        <div className="flex w-full flex-wrap justify-between items-center gap-4">
          <Link
            to="/partner/quotation"
            className="flex items-center gap-2 text-slate-500 hover:text-slate-700 dark:hover:text-slate-300 font-semibold px-4 py-2 transition-all"
          >
            <Icon name="keyboard_backspace" />
            Quay lại
          </Link>
          <div className="flex gap-4">
            <button
              type="button"
              className="px-6 py-2.5 rounded-lg border border-slate-300 dark:border-slate-700 text-slate-700 dark:text-slate-300 font-bold hover:bg-slate-50 dark:hover:bg-slate-800 transition-all"
            >
              Lưu bản nháp
            </button>
            <button
              type="button"
              onClick={(e) => void handleSubmit(e)}
              disabled={submitting || items.length === 0}
              className="px-8 py-2.5 rounded-lg bg-primary text-white font-bold hover:bg-primary/90 disabled:opacity-60 disabled:pointer-events-none transition-all flex items-center gap-2 shadow-lg shadow-primary/20"
            >
              <Icon name="send" className="text-lg" />
              {submitting ? 'Đang gửi…' : 'Gửi yêu cầu báo giá'}
            </button>
          </div>
        </div>
      </footer>
    </>
  )
}
