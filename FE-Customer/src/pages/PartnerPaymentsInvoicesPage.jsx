import { useState } from 'react'
import { Icon } from '../components/ui/Icon'
import { PartnerPaymentsPageHeader } from '../components/partner/PartnerPaymentsPageHeader'
import { B2B_PAYMENTS_INVOICE_ROWS } from '../data/b2bDashboard'

export function PartnerPaymentsInvoicesPage() {
  const [search, setSearch] = useState('')

  const rows = B2B_PAYMENTS_INVOICE_ROWS.filter((r) => {
    if (!search.trim()) return true
    const q = search.trim().toLowerCase()
    const no = `${r.seriesNo}${r.number}`.toLowerCase()
    return no.includes(q) || r.issuedAt.includes(q)
  })

  return (
    <>
      <PartnerPaymentsPageHeader
        title="Hóa đơn VAT"
        subtitle="Danh sách hóa đơn điện tử đã phát hành cho doanh nghiệp. Tải PDF/XML khi cần hạch toán hoặc đối chiếu."
      />

      <section className="p-8 pt-6">
        <div className="bg-white dark:bg-slate-900 p-4 rounded-xl border border-slate-200 dark:border-slate-800 flex flex-wrap gap-4 items-center mb-6">
          <div className="flex-1 min-w-[280px] relative">
            <Icon name="search" className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Tìm theo ký hiệu, số HĐ, ngày…"
              className="w-full pl-10 pr-4 py-2.5 bg-slate-100 dark:bg-slate-800 border-none rounded-lg focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400"
            />
          </div>
          <button
            type="button"
            className="flex items-center gap-2 px-4 py-2.5 bg-slate-100 dark:bg-slate-800 rounded-lg text-sm font-medium hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors text-slate-700 dark:text-slate-200"
          >
            <Icon name="calendar_today" className="text-lg" />
            Tháng phát hành
            <Icon name="expand_more" className="text-lg" />
          </button>
        </div>

        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Ký hiệu / Số</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Ngày HĐ</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Người mua</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Trước thuế</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thuế GTGT</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Tổng cộng</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-center">Trạng thái</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Tải về</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
              {rows.map((row) => (
                <tr key={row.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/30 transition-colors">
                  <td className="px-6 py-4">
                    <div className="flex flex-col">
                      <span className="text-xs text-slate-500 font-mono">{row.seriesNo}</span>
                      <span className="font-bold text-slate-900 dark:text-slate-50">{row.number}</span>
                    </div>
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400 whitespace-nowrap">{row.issuedAt}</td>
                  <td className="px-6 py-4 text-sm font-medium text-slate-800 dark:text-slate-100 max-w-[180px] truncate">
                    {row.buyer}
                  </td>
                  <td className="px-6 py-4 text-sm text-right text-slate-700 dark:text-slate-300">{row.beforeVat}</td>
                  <td className="px-6 py-4 text-sm text-right text-slate-600 dark:text-slate-400">{row.vat}</td>
                  <td className="px-6 py-4 text-sm font-bold text-right text-slate-900 dark:text-slate-100">{row.total}</td>
                  <td className="px-6 py-4 text-center">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold ${row.statusClass}`}>
                      {row.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <button
                      type="button"
                      disabled={!row.hasPdf}
                      className="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-semibold bg-primary/10 text-primary hover:bg-primary/20 transition-colors disabled:opacity-40 disabled:cursor-not-allowed"
                    >
                      <Icon name="picture_as_pdf" className="text-lg" />
                      PDF
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {rows.length === 0 ? (
            <p className="text-center text-slate-500 py-12 text-sm">Không tìm thấy hóa đơn.</p>
          ) : null}
        </div>
      </section>
    </>
  )
}
