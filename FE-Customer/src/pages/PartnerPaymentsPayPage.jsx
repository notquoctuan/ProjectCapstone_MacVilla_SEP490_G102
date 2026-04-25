import { useMemo, useState, useEffect } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { PartnerPaymentsPageHeader } from '../components/partner/PartnerPaymentsPageHeader'
import {
  B2B_PAYMENTS_PAYABLE_LINES,
  B2B_PAYMENTS_BANK_INFO,
} from '../data/b2bDashboard'

function formatVnd(n) {
  return new Intl.NumberFormat('vi-VN').format(n) + 'đ'
}

export function PartnerPaymentsPayPage() {
  const [searchParams] = useSearchParams()
  const refFromUrl = searchParams.get('ref') || ''

  const [selected, setSelected] = useState(() => {
    const initial = new Set()
    if (refFromUrl) {
      const line = B2B_PAYMENTS_PAYABLE_LINES.find((p) => p.ref === refFromUrl)
      if (line) initial.add(line.id)
    }
    return initial
  })

  useEffect(() => {
    if (!refFromUrl) return
    const line = B2B_PAYMENTS_PAYABLE_LINES.find((p) => p.ref === refFromUrl)
    if (line) {
      setSelected((prev) => new Set(prev).add(line.id))
    }
  }, [refFromUrl])

  const toggle = (id) => {
    setSelected((prev) => {
      const next = new Set(prev)
      if (next.has(id)) next.delete(id)
      else next.add(id)
      return next
    })
  }

  const total = useMemo(() => {
    let sum = 0
    for (const line of B2B_PAYMENTS_PAYABLE_LINES) {
      if (selected.has(line.id)) sum += line.amountNum
    }
    return sum
  }, [selected])

  const primaryRef =
    [...selected]
      .map((id) => B2B_PAYMENTS_PAYABLE_LINES.find((l) => l.id === id)?.ref)
      .filter(Boolean)[0] || 'MA_THAM_CHIEU'

  const transferContent = `${B2B_PAYMENTS_BANK_INFO.transferSyntaxPrefix} ${primaryRef}`.toUpperCase()

  const copy = async (text) => {
    try {
      await navigator.clipboard.writeText(text)
    } catch {
      /* ignore */
    }
  }

  return (
    <>
      <PartnerPaymentsPageHeader
        title="Thanh toán đơn hàng"
        subtitle="Chọn các khoản cần thanh toán, sao chép thông tin chuyển khoản và nội dung CK đúng quy định để đối soát nhanh."
      />

      <section className="p-8 pt-6">
        <div className="grid grid-cols-1 xl:grid-cols-5 gap-8">
          <div className="xl:col-span-3 space-y-6">
            <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
              <div className="px-6 py-4 border-b border-slate-200 dark:border-slate-800 flex justify-between items-center flex-wrap gap-2">
                <h3 className="font-bold text-slate-900 dark:text-slate-50">Chọn khoản thanh toán</h3>
                <Link
                  to="/partner/payments/debt"
                  className="text-sm font-semibold text-primary hover:underline inline-flex items-center gap-1"
                >
                  <Icon name="arrow_back" className="text-base" />
                  Xem công nợ
                </Link>
              </div>
              <ul className="divide-y divide-slate-200 dark:divide-slate-800">
                {B2B_PAYMENTS_PAYABLE_LINES.map((line) => {
                  const on = selected.has(line.id)
                  return (
                    <li key={line.id}>
                      <label className="flex gap-4 p-5 cursor-pointer hover:bg-slate-50 dark:hover:bg-slate-800/40 transition-colors items-start">
                        <input
                          type="checkbox"
                          checked={on}
                          onChange={() => toggle(line.id)}
                          className="mt-1 w-4 h-4 rounded border-slate-300 text-primary focus:ring-primary"
                        />
                        <div className="flex-1 min-w-0">
                          <div className="flex flex-wrap justify-between gap-2">
                            <span className="font-bold text-primary">{line.ref}</span>
                            <span className="text-sm text-slate-500">Hạn: {line.dueAt}</span>
                          </div>
                          <p className="text-sm text-slate-700 dark:text-slate-200 mt-1">{line.label}</p>
                          <p className="text-lg font-black text-slate-900 dark:text-slate-50 mt-2">
                            {line.remaining}
                          </p>
                        </div>
                      </label>
                    </li>
                  )
                })}
              </ul>
              <div className="px-6 py-4 bg-slate-50 dark:bg-slate-800/50 border-t border-slate-200 dark:border-slate-800 flex flex-wrap justify-between items-center gap-3">
                <span className="text-sm font-medium text-slate-600 dark:text-slate-400">
                  Đã chọn {selected.size} khoản
                </span>
                <span className="text-xl font-black text-primary">{formatVnd(total)}</span>
              </div>
            </div>

            <div className="rounded-xl border border-dashed border-slate-300 dark:border-slate-600 bg-slate-50/80 dark:bg-slate-800/30 p-6 flex flex-col sm:flex-row gap-4 items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-3 rounded-xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700">
                  <Icon name="qr_code_2" className="text-4xl text-slate-400" />
                </div>
                <div>
                  <p className="font-bold text-slate-900 dark:text-slate-50">Quét mã VietQR</p>
                  <p className="text-sm text-slate-500 mt-0.5">Sẽ kích hoạt khi tích hợp cổng thanh toán.</p>
                </div>
              </div>
              <button
                type="button"
                disabled
                className="px-4 py-2.5 rounded-lg text-sm font-bold bg-slate-200 dark:bg-slate-700 text-slate-500 cursor-not-allowed"
              >
                Tải mã QR
              </button>
            </div>
          </div>

          <div className="xl:col-span-2">
            <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm overflow-hidden sticky top-6">
              <div className="px-6 py-4 border-b border-slate-200 dark:border-slate-800 bg-primary/5">
                <h3 className="font-bold text-slate-900 dark:text-slate-50 flex items-center gap-2">
                  <Icon name="account_balance" className="text-primary text-xl" />
                  Thông tin chuyển khoản
                </h3>
              </div>
              <div className="p-6 space-y-5">
                <div>
                  <p className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">Chủ tài khoản</p>
                  <p className="text-sm font-semibold text-slate-900 dark:text-slate-100 leading-snug">
                    {B2B_PAYMENTS_BANK_INFO.beneficiary}
                  </p>
                </div>
                <div>
                  <p className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">Số tài khoản</p>
                  <div className="flex flex-wrap items-center gap-2">
                    <span className="text-lg font-mono font-bold text-slate-900 dark:text-slate-50 tracking-wide">
                      {B2B_PAYMENTS_BANK_INFO.account}
                    </span>
                    <button
                      type="button"
                      onClick={() => void copy(B2B_PAYMENTS_BANK_INFO.account.replace(/\s/g, ''))}
                      className="p-1.5 rounded-lg text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800 hover:text-primary transition-colors"
                      title="Sao chép STK"
                    >
                      <Icon name="content_copy" className="text-lg" />
                    </button>
                  </div>
                </div>
                <div>
                  <p className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">Ngân hàng</p>
                  <p className="text-sm text-slate-700 dark:text-slate-200">{B2B_PAYMENTS_BANK_INFO.bank}</p>
                </div>
                <div className="rounded-lg bg-amber-50 dark:bg-amber-950/30 border border-amber-200 dark:border-amber-900/50 p-4">
                  <p className="text-xs font-bold text-amber-800 dark:text-amber-200 uppercase tracking-wider mb-2">
                    Nội dung chuyển khoản
                  </p>
                  <p className="font-mono text-sm font-bold text-amber-950 dark:text-amber-100 break-all">
                    {transferContent}
                  </p>
                  <button
                    type="button"
                    onClick={() => void copy(transferContent)}
                    className="mt-3 inline-flex items-center gap-2 text-sm font-bold text-amber-800 dark:text-amber-200 hover:underline"
                  >
                    <Icon name="content_copy" className="text-base" />
                    Sao chép nội dung
                  </button>
                </div>
                <div className="pt-2 border-t border-slate-200 dark:border-slate-800">
                  <p className="text-xs text-slate-500 mb-3">
                    Sau khi chuyển khoản, vui lòng upload chứng từ để kế toán đối soát.
                  </p>
                  <Link
                    to="/partner/payments/upload"
                    className="w-full flex items-center justify-center gap-2 py-3 rounded-lg bg-primary text-white font-bold text-sm hover:bg-primary/90 transition-colors"
                  >
                    <Icon name="upload_file" />
                    Upload chứng từ
                  </Link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  )
}
