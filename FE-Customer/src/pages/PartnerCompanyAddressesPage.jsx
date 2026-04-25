import { useState, useEffect, useCallback } from 'react'
import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { PartnerPaymentsPageHeader } from '../components/partner/PartnerPaymentsPageHeader'
import { useAuth } from '../contexts/AuthContext'
import {
  storeFetchAddresses,
  storeCreateAddress,
} from '../api/store/storeAddressesApi'
import { ApiError } from '../api/httpClient'
import { getApiErrorMessage } from '../lib/errors/apiErrorMessage'
import { mapValidationErrorsToFirstMessage } from '../lib/auth/mapValidationErrors'

/**
 * @typedef {{ id: number, receiverName: string, receiverPhone: string, addressLine: string, isDefault: boolean }} StoreAddressRow
 */

const inputClass =
  'w-full px-4 py-2.5 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400 focus:ring-2 focus:ring-primary focus:border-primary'

function resetAddressForm() {
  return {
    receiverName: '',
    receiverPhone: '',
    addressLine: '',
    isDefault: false,
  }
}

export function PartnerCompanyAddressesPage() {
  const { accessToken, isAuthenticated } = useAuth()
  const [items, setItems] = useState(/** @type {StoreAddressRow[]} */ ([]))
  const [loading, setLoading] = useState(true)
  const [fetchError, setFetchError] = useState('')

  const [createOpen, setCreateOpen] = useState(false)
  const [receiverName, setReceiverName] = useState('')
  const [receiverPhone, setReceiverPhone] = useState('')
  const [addressLine, setAddressLine] = useState('')
  const [isDefault, setIsDefault] = useState(false)
  const [creating, setCreating] = useState(false)
  const [createError, setCreateError] = useState('')
  const [createSuccess, setCreateSuccess] = useState('')
  const [fieldErrors, setFieldErrors] = useState(
    /** @type {Record<string, string>} */ ({})
  )

  const load = useCallback(
    async (options = /** @type {{ showLoading?: boolean }} */ ({})) => {
      const showLoading = options.showLoading !== false
      if (!accessToken) {
        setItems([])
        setLoading(false)
        setFetchError('')
        return
      }
      if (showLoading) setLoading(true)
      setFetchError('')
      try {
        const list = await storeFetchAddresses(accessToken)
        setItems(Array.isArray(list) ? list : [])
      } catch (err) {
        setFetchError(getApiErrorMessage(err))
        setItems([])
      } finally {
        if (showLoading) setLoading(false)
      }
    },
    [accessToken]
  )

  useEffect(() => {
    void load()
  }, [load])

  useEffect(() => {
    if (!createOpen) return
    const prevOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    const onKey = (e) => {
      if (e.key === 'Escape' && !creating) setCreateOpen(false)
    }
    window.addEventListener('keydown', onKey)
    return () => {
      document.body.style.overflow = prevOverflow
      window.removeEventListener('keydown', onKey)
    }
  }, [createOpen, creating])

  useEffect(() => {
    if (!createSuccess) return
    const t = window.setTimeout(() => setCreateSuccess(''), 4500)
    return () => window.clearTimeout(t)
  }, [createSuccess])

  const openCreateModal = () => {
    const r = resetAddressForm()
    setReceiverName(r.receiverName)
    setReceiverPhone(r.receiverPhone)
    setAddressLine(r.addressLine)
    setIsDefault(r.isDefault)
    setCreateError('')
    setFieldErrors({})
    setCreateOpen(true)
  }

  const closeCreateModal = () => {
    if (creating) return
    setCreateOpen(false)
  }

  const handleCreate = async (e) => {
    e.preventDefault()
    if (!accessToken) return
    setCreateError('')
    setFieldErrors({})

    const next = /** @type {Record<string, string>} */ ({})
    if (!receiverName.trim()) next.receiverName = 'Vui lòng nhập tên người nhận.'
    if (!receiverPhone.trim()) next.receiverPhone = 'Vui lòng nhập số điện thoại.'
    if (!addressLine.trim()) next.addressLine = 'Vui lòng nhập địa chỉ.'
    if (Object.keys(next).length > 0) {
      setFieldErrors(next)
      return
    }

    setCreating(true)
    try {
      await storeCreateAddress(accessToken, {
        receiverName: receiverName.trim(),
        receiverPhone: receiverPhone.trim(),
        addressLine: addressLine.trim(),
        isDefault,
      })
      setCreateOpen(false)
      const r = resetAddressForm()
      setReceiverName(r.receiverName)
      setReceiverPhone(r.receiverPhone)
      setAddressLine(r.addressLine)
      setIsDefault(r.isDefault)
      setCreateSuccess('Thêm địa chỉ thành công.')
      await load({ showLoading: false })
    } catch (err) {
      if (err instanceof ApiError && err.errorCode === 'VALIDATION_ERROR') {
        setFieldErrors(mapValidationErrorsToFirstMessage(err.errors))
      }
      setCreateError(getApiErrorMessage(err))
    } finally {
      setCreating(false)
    }
  }

  if (!isAuthenticated) {
    return (
      <>
        <PartnerPaymentsPageHeader
          title="Địa chỉ giao hàng"
          subtitle="Đăng nhập để xem danh sách địa chỉ đã lưu."
        />
        <section className="p-8 pt-2">
          <Link
            to="/login"
            className="inline-flex items-center gap-2 text-primary font-semibold text-sm hover:underline"
          >
            <Icon name="login" className="text-lg" />
            Đăng nhập
          </Link>
        </section>
      </>
    )
  }

  return (
    <>
      <PartnerPaymentsPageHeader
        title="Địa chỉ giao hàng"
        subtitle="Các địa chỉ nhận hàng đã lưu trên tài khoản của bạn."
      />

      <section className="p-8 pt-6 max-w-6xl">
        <div className="flex flex-wrap items-center justify-between gap-3 mb-6">
          <Link
            to="/partner/company"
            className="inline-flex items-center gap-1 text-sm text-slate-500 hover:text-primary transition-colors"
          >
            <Icon name="chevron_left" className="text-lg" />
            Thông tin doanh nghiệp
          </Link>
          <div className="flex flex-wrap items-center gap-2">
            <button
              type="button"
              onClick={openCreateModal}
              className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-white text-sm font-bold hover:bg-primary/90 transition-colors"
            >
              <Icon name="add" className="text-lg" />
              Thêm địa chỉ
            </button>
            <button
              type="button"
              onClick={() => void load()}
              disabled={loading}
              className="inline-flex items-center gap-2 px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 text-sm font-semibold text-slate-700 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-slate-800 disabled:opacity-50"
            >
              <Icon name="refresh" className={`text-lg ${loading ? 'animate-spin' : ''}`} />
              Tải lại
            </button>
          </div>
        </div>

        {createSuccess ? (
          <div
            className="mb-4 rounded-lg border border-emerald-200 dark:border-emerald-900/50 bg-emerald-50 dark:bg-emerald-950/30 px-4 py-3 text-sm text-emerald-800 dark:text-emerald-200"
            role="status"
          >
            {createSuccess}
          </div>
        ) : null}

        {fetchError ? (
          <div
            className="mb-6 rounded-lg border border-red-200 dark:border-red-900/50 bg-red-50 dark:bg-red-950/30 px-4 py-3 text-sm text-red-800 dark:text-red-200"
            role="alert"
          >
            {fetchError}
          </div>
        ) : null}

        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
          <div className="px-6 py-4 border-b border-slate-200 dark:border-slate-800">
            <h3 className="font-bold text-slate-900 dark:text-slate-50">Danh sách địa chỉ</h3>
            <p className="text-xs text-slate-500 mt-1">
              {loading ? 'Đang tải…' : `${items.length} địa chỉ`}
            </p>
          </div>
          <div className="overflow-x-auto">
            {loading ? (
              <p className="text-center text-slate-500 py-12 text-sm">Đang tải danh sách…</p>
            ) : items.length === 0 ? (
              <div className="text-center py-12 px-6">
                <Icon name="location_off" className="text-4xl text-slate-400 mx-auto mb-3" />
                <p className="text-slate-600 dark:text-slate-400 text-sm font-medium">
                  Chưa có địa chỉ giao hàng.
                </p>
                <p className="text-slate-500 text-xs mt-2 max-w-md mx-auto">
                  Nhấn <span className="font-semibold text-slate-700 dark:text-slate-300">Thêm địa chỉ</span> hoặc thêm khi thanh toán trên cửa hàng.
                </p>
                <div className="flex flex-wrap justify-center gap-3 mt-5">
                  <button
                    type="button"
                    onClick={openCreateModal}
                    className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-white text-sm font-bold hover:bg-primary/90"
                  >
                    <Icon name="add" className="text-lg" />
                    Thêm địa chỉ
                  </button>
                  <Link
                    to="/checkout"
                    className="inline-flex items-center gap-2 px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 text-sm font-semibold text-slate-700 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-slate-800"
                  >
                    Đến thanh toán
                  </Link>
                </div>
              </div>
            ) : (
              <table className="w-full text-left border-collapse min-w-[720px]">
                <thead>
                  <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                    <th className="px-4 sm:px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider whitespace-nowrap">
                      ID
                    </th>
                    <th className="px-4 sm:px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider">
                      Người nhận
                    </th>
                    <th className="px-4 sm:px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider whitespace-nowrap">
                      Điện thoại
                    </th>
                    <th className="px-4 sm:px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider min-w-[200px]">
                      Địa chỉ
                    </th>
                    <th className="px-4 sm:px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider text-center whitespace-nowrap">
                      Mặc định
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                  {items.map((row) => (
                    <tr
                      key={row.id}
                      className="hover:bg-slate-50 dark:hover:bg-slate-800/30 transition-colors"
                    >
                      <td className="px-4 sm:px-6 py-3.5 text-sm font-mono text-slate-500 whitespace-nowrap">
                        {row.id}
                      </td>
                      <td className="px-4 sm:px-6 py-3.5 text-sm font-semibold text-slate-900 dark:text-slate-100">
                        {row.receiverName || '—'}
                      </td>
                      <td className="px-4 sm:px-6 py-3.5 text-sm text-slate-600 dark:text-slate-400 font-mono whitespace-nowrap">
                        {row.receiverPhone || '—'}
                      </td>
                      <td className="px-4 sm:px-6 py-3.5 text-sm text-slate-700 dark:text-slate-300">
                        {row.addressLine || '—'}
                      </td>
                      <td className="px-4 sm:px-6 py-3.5 text-center">
                        {row.isDefault ? (
                          <span className="inline-flex text-[10px] font-bold uppercase tracking-wider px-2.5 py-1 rounded-full bg-primary/10 text-primary border border-primary/20">
                            Có
                          </span>
                        ) : (
                          <span className="text-sm text-slate-400">—</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      </section>

      {createOpen ? (
        <div
          className="fixed inset-0 z-[100] flex items-center justify-center p-4"
          role="presentation"
        >
          <button
            type="button"
            aria-label="Đóng"
            disabled={creating}
            onClick={closeCreateModal}
            className="absolute inset-0 bg-slate-900/60 dark:bg-black/70 backdrop-blur-[2px] disabled:cursor-not-allowed"
          />
          <div
            role="dialog"
            aria-modal="true"
            aria-labelledby="addr-dialog-title"
            className="relative z-[1] w-full max-w-lg rounded-xl border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 shadow-2xl max-h-[min(90vh,640px)] flex flex-col"
          >
            <div className="flex items-start justify-between gap-4 px-6 py-4 border-b border-slate-200 dark:border-slate-800 shrink-0">
              <div>
                <h2
                  id="addr-dialog-title"
                  className="text-lg font-bold text-slate-900 dark:text-slate-50"
                >
                  Thêm địa chỉ giao hàng
                </h2>
                <p className="text-xs text-slate-500 mt-1">
                  Điền thông tin người nhận và địa chỉ.
                </p>
              </div>
              <button
                type="button"
                onClick={closeCreateModal}
                disabled={creating}
                className="p-2 rounded-lg text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800 disabled:opacity-50"
                aria-label="Đóng hộp thoại"
              >
                <Icon name="close" className="text-xl" />
              </button>
            </div>

            <form
              id="partner-address-create-form"
              onSubmit={handleCreate}
              className="px-6 py-5 overflow-y-auto flex-1 min-h-0 space-y-4"
            >
              {createError ? (
                <div
                  className="rounded-lg border border-red-200 dark:border-red-900/50 bg-red-50 dark:bg-red-950/30 px-4 py-3 text-sm text-red-800 dark:text-red-200"
                  role="alert"
                >
                  {createError}
                </div>
              ) : null}

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label
                    htmlFor="addr-dlg-name"
                    className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-1.5"
                  >
                    Người nhận
                  </label>
                  <input
                    id="addr-dlg-name"
                    type="text"
                    value={receiverName}
                    onChange={(e) => setReceiverName(e.target.value)}
                    className={inputClass}
                    autoComplete="name"
                  />
                  {fieldErrors.receiverName ? (
                    <p className="text-xs text-red-600 mt-1">{fieldErrors.receiverName}</p>
                  ) : null}
                </div>
                <div>
                  <label
                    htmlFor="addr-dlg-phone"
                    className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-1.5"
                  >
                    Số điện thoại
                  </label>
                  <input
                    id="addr-dlg-phone"
                    type="tel"
                    value={receiverPhone}
                    onChange={(e) => setReceiverPhone(e.target.value)}
                    className={inputClass}
                    autoComplete="tel"
                  />
                  {fieldErrors.receiverPhone ? (
                    <p className="text-xs text-red-600 mt-1">{fieldErrors.receiverPhone}</p>
                  ) : null}
                </div>
              </div>

              <div>
                <label
                  htmlFor="addr-dlg-line"
                  className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-1.5"
                >
                  Địa chỉ
                </label>
                <textarea
                  id="addr-dlg-line"
                  rows={3}
                  value={addressLine}
                  onChange={(e) => setAddressLine(e.target.value)}
                  className={`${inputClass} resize-y min-h-[88px]`}
                />
                {fieldErrors.addressLine ? (
                  <p className="text-xs text-red-600 mt-1">{fieldErrors.addressLine}</p>
                ) : null}
              </div>

              <label className="flex items-center gap-2 cursor-pointer select-none text-sm text-slate-700 dark:text-slate-300">
                <input
                  type="checkbox"
                  checked={isDefault}
                  onChange={(e) => setIsDefault(e.target.checked)}
                  className="rounded border-slate-300 text-primary focus:ring-primary"
                />
                Đặt làm địa chỉ mặc định
              </label>
            </form>

            <div className="px-6 py-4 border-t border-slate-200 dark:border-slate-800 flex flex-wrap justify-end gap-2 shrink-0 bg-slate-50/80 dark:bg-slate-900/80">
              <button
                type="button"
                onClick={closeCreateModal}
                disabled={creating}
                className="px-4 py-2.5 rounded-lg border border-slate-200 dark:border-slate-600 text-sm font-semibold text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800 disabled:opacity-50"
              >
                Hủy
              </button>
              <button
                type="submit"
                form="partner-address-create-form"
                disabled={creating}
                className="px-6 py-2.5 rounded-lg bg-primary text-white text-sm font-bold hover:bg-primary/90 disabled:opacity-60"
              >
                {creating ? 'Đang lưu…' : 'Lưu địa chỉ'}
              </button>
            </div>
          </div>
        </div>
      ) : null}
    </>
  )
}
