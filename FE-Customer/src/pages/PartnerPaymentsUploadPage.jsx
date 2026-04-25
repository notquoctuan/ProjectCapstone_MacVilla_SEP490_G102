import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { PartnerPaymentsPageHeader } from '../components/partner/PartnerPaymentsPageHeader'
import { B2B_PAYMENTS_UPLOAD_RECENT } from '../data/b2bDashboard'

export function PartnerPaymentsUploadPage() {
  const [fileName, setFileName] = useState('')
  const [ref, setRef] = useState('')
  const [amount, setAmount] = useState('')
  const [transferDate, setTransferDate] = useState('')
  const [note, setNote] = useState('')

  const onFile = (e) => {
    const f = e.target.files?.[0]
    setFileName(f ? f.name : '')
  }

  return (
    <>
      <PartnerPaymentsPageHeader
        title="Upload chứng từ"
        subtitle="Gửi biên lai / sao kê chuyển khoản kèm mã tham chiếu để đội kế toán khớp thanh toán với đơn hàng hoặc hóa đơn."
      />

      <section className="p-8 pt-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            <form
              className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm overflow-hidden"
              onSubmit={(e) => {
                e.preventDefault()
              }}
            >
              <div className="px-6 py-4 border-b border-slate-200 dark:border-slate-800">
                <h3 className="font-bold text-slate-900 dark:text-slate-50">Gửi chứng từ mới</h3>
                <p className="text-sm text-slate-500 mt-1">Định dạng: PDF, JPG, PNG — tối đa 10MB (minh họa).</p>
              </div>
              <div className="p-6 space-y-6">
                <div>
                  <label className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">
                    Tệp đính kèm
                  </label>
                  <label className="flex flex-col items-center justify-center border-2 border-dashed border-slate-300 dark:border-slate-600 rounded-xl py-12 px-4 cursor-pointer hover:border-primary/50 hover:bg-primary/5 transition-colors">
                    <Icon name="cloud_upload" className="text-4xl text-slate-400 mb-2" />
                    <span className="text-sm font-semibold text-slate-700 dark:text-slate-200">
                      Kéo thả hoặc bấm để chọn
                    </span>
                    <span className="text-xs text-slate-500 mt-1">{fileName || 'Chưa chọn tệp'}</span>
                    <input type="file" accept=".pdf,.jpg,.jpeg,.png" className="hidden" onChange={onFile} />
                  </label>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label
                      htmlFor="pay-upload-ref"
                      className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-2"
                    >
                      Mã tham chiếu
                    </label>
                    <input
                      id="pay-upload-ref"
                      type="text"
                      value={ref}
                      onChange={(e) => setRef(e.target.value)}
                      placeholder="VD: ORD-2024-002"
                      className="w-full px-4 py-2.5 rounded-lg bg-slate-100 dark:bg-slate-800 border-none focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400"
                    />
                  </div>
                  <div>
                    <label
                      htmlFor="pay-upload-amount"
                      className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-2"
                    >
                      Số tiền chuyển
                    </label>
                    <input
                      id="pay-upload-amount"
                      type="text"
                      value={amount}
                      onChange={(e) => setAmount(e.target.value)}
                      placeholder="VD: 20.000.000"
                      className="w-full px-4 py-2.5 rounded-lg bg-slate-100 dark:bg-slate-800 border-none focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400"
                    />
                  </div>
                </div>

                <div>
                  <label
                    htmlFor="pay-upload-date"
                    className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-2"
                  >
                    Ngày giờ chuyển khoản
                  </label>
                  <input
                    id="pay-upload-date"
                    type="datetime-local"
                    value={transferDate}
                    onChange={(e) => setTransferDate(e.target.value)}
                    className="w-full max-w-xs px-4 py-2.5 rounded-lg bg-slate-100 dark:bg-slate-800 border-none focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100"
                  />
                </div>

                <div>
                  <label
                    htmlFor="pay-upload-note"
                    className="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-2"
                  >
                    Ghi chú (tuỳ chọn)
                  </label>
                  <textarea
                    id="pay-upload-note"
                    value={note}
                    onChange={(e) => setNote(e.target.value)}
                    rows={3}
                    placeholder="Nội dung hiển thị trên sao kê…"
                    className="w-full px-4 py-2.5 rounded-lg bg-slate-100 dark:bg-slate-800 border-none focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400 resize-y min-h-[88px]"
                  />
                </div>

                <div className="flex flex-wrap gap-3 pt-2">
                  <button
                    type="submit"
                    className="inline-flex items-center gap-2 px-6 py-3 rounded-lg bg-primary text-white font-bold text-sm hover:bg-primary/90 transition-colors"
                  >
                    <Icon name="send" />
                    Gửi chứng từ
                  </button>
                  <Link
                    to="/partner/payments/history"
                    className="inline-flex items-center gap-2 px-6 py-3 rounded-lg bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-200 font-semibold text-sm hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors"
                  >
                    Xem lịch sử
                  </Link>
                </div>
              </div>
            </form>
          </div>

          <div>
            <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm overflow-hidden">
              <div className="px-5 py-4 border-b border-slate-200 dark:border-slate-800">
                <h3 className="font-bold text-slate-900 dark:text-slate-50 text-sm">Gần đây</h3>
              </div>
              <ul className="divide-y divide-slate-200 dark:divide-slate-800 max-h-[480px] overflow-y-auto">
                {B2B_PAYMENTS_UPLOAD_RECENT.map((u) => (
                  <li key={u.id} className="p-4 hover:bg-slate-50 dark:hover:bg-slate-800/40 transition-colors">
                    <div className="flex items-start gap-3">
                      <div className="p-2 rounded-lg bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400 shrink-0">
                        <Icon name="description" className="text-xl" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-sm font-semibold text-slate-900 dark:text-slate-100 truncate">{u.fileName}</p>
                        <p className="text-xs text-slate-500 mt-0.5">{u.uploadedAt}</p>
                        <p className="text-xs text-primary font-medium mt-1">{u.ref}</p>
                        <div className="flex flex-wrap justify-between items-center gap-2 mt-2">
                          <span className="text-sm font-bold text-slate-800 dark:text-slate-200">{u.amount}</span>
                          <span className={`text-[10px] font-bold px-2 py-0.5 rounded-full ${u.statusClass}`}>
                            {u.status}
                          </span>
                        </div>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>

            <div className="mt-4 rounded-xl border border-blue-200 dark:border-blue-900/50 bg-blue-50/80 dark:bg-blue-950/20 p-4">
              <p className="text-sm font-bold text-blue-900 dark:text-blue-200 flex items-center gap-2">
                <Icon name="info" className="text-lg" />
                Lưu ý
              </p>
              <ul className="mt-2 text-xs text-blue-800/90 dark:text-blue-300/90 space-y-1.5 list-disc list-inside leading-relaxed">
                <li>Nội dung CK trùng với hướng dẫn trên trang Thanh toán.</li>
                <li>Một chứng từ chỉ gắn một mã tham chiếu chính.</li>
                <li>Thời gian xử lý dự kiến trong giờ hành chính.</li>
              </ul>
            </div>
          </div>
        </div>
      </section>
    </>
  )
}
