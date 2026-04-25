import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { PartnerPaymentsPageHeader } from '../components/partner/PartnerPaymentsPageHeader'
import { B2B_PAYMENTS_DEBT_KPIS, B2B_PAYMENTS_DEBT_ROWS } from '../data/b2bDashboard'

export function PartnerPaymentsDebtPage() {
  const [search, setSearch] = useState('')
  const rows = useMemo(() => {
    if (!search.trim()) return B2B_PAYMENTS_DEBT_ROWS
    const q = search.trim().toLowerCase()
    return B2B_PAYMENTS_DEBT_ROWS.filter(
      (r) =>
        r.ref.toLowerCase().includes(q) ||
        r.project.toLowerCase().includes(q) ||
        r.type.toLowerCase().includes(q)
    )
  }, [search])

  return (
    <>
      <PartnerPaymentsPageHeader
        title="Công nợ hiện tại"
        subtitle="Theo dõi hạn mức, các khoản phải trả và hạn thanh toán theo đơn hàng, hóa đơn và cọc."
      />

      <section className="p-8 pt-6">
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4 mb-8">
          {B2B_PAYMENTS_DEBT_KPIS.map((k) => (
            <div
              key={k.id}
              className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-5 shadow-sm"
            >
              <div className="flex items-start justify-between gap-2 mb-3">
                <div className={`p-2 rounded-lg ${k.iconClass}`}>
                  <Icon name={k.icon} className="text-2xl" />
                </div>
              </div>
              <p className="text-sm font-medium text-slate-500 dark:text-slate-400">{k.label}</p>
              <p className="text-xl font-black text-slate-900 dark:text-slate-50 mt-1 tracking-tight">{k.value}</p>
              <p className="text-xs text-slate-500 dark:text-slate-400 mt-2 leading-relaxed">{k.sub}</p>
            </div>
          ))}
        </div>

        <div className="bg-white dark:bg-slate-900 p-4 rounded-xl border border-slate-200 dark:border-slate-800 flex flex-wrap gap-3 items-center mb-6">
          <div className="flex-1 min-w-[240px] relative">
            <Icon name="search" className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Tìm theo mã chứng từ, dự án…"
              className="w-full pl-10 pr-4 py-2.5 bg-slate-100 dark:bg-slate-800 border-none rounded-lg focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400"
            />
          </div>
          <Link
            to="/partner/payments/pay"
            className="inline-flex items-center gap-2 px-4 py-2.5 bg-primary text-white rounded-lg text-sm font-bold hover:bg-primary/90 transition-colors"
          >
            <Icon name="payments" className="text-lg" />
            Thanh toán ngay
          </Link>
          <Link
            to="/partner/payments/upload"
            className="inline-flex items-center gap-2 px-4 py-2.5 bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-200 rounded-lg text-sm font-semibold hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors"
          >
            <Icon name="upload_file" className="text-lg" />
            Upload chứng từ
          </Link>
        </div>

        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
          <div className="px-6 py-4 border-b border-slate-200 dark:border-slate-800 flex flex-wrap justify-between items-center gap-2">
            <h3 className="font-bold text-slate-900 dark:text-slate-50">Chi tiết công nợ</h3>
            <span className="text-xs text-slate-500">Dữ liệu minh họa — kết nối API sau</span>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse min-w-[920px]">
              <thead>
                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider">Mã tham chiếu</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider">Loại</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider">Dự án / Nội dung</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider">Phát sinh</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider">Hạn TT</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Phải trả</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Đã trả</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Còn lại</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider text-center">Trạng thái</th>
                  <th className="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                {rows.map((row) => (
                  <tr key={row.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/30 transition-colors">
                    <td className="px-6 py-4">
                      <span className="font-bold text-primary">{row.ref}</span>
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400">{row.type}</td>
                    <td className="px-6 py-4 text-sm font-medium text-slate-800 dark:text-slate-100 max-w-[200px] truncate">
                      {row.project}
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400 whitespace-nowrap">{row.issuedAt}</td>
                    <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400 whitespace-nowrap">{row.dueAt}</td>
                    <td className="px-6 py-4 text-sm font-semibold text-right text-slate-900 dark:text-slate-100">{row.total}</td>
                    <td className="px-6 py-4 text-sm text-right text-slate-600 dark:text-slate-400">{row.paid}</td>
                    <td className="px-6 py-4 text-sm font-bold text-right text-slate-900 dark:text-slate-100">{row.remaining}</td>
                    <td className="px-6 py-4 text-center">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold ${row.statusClass}`}>
                        {row.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      {row.remaining !== '0đ' ? (
                        <Link
                          to={`/partner/payments/pay?ref=${encodeURIComponent(row.ref)}`}
                          className="text-sm font-bold text-primary hover:underline"
                        >
                          Thanh toán
                        </Link>
                      ) : (
                        <span className="text-sm text-slate-400">—</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {rows.length === 0 ? (
              <p className="text-center text-slate-500 py-12 text-sm">Không có dòng công nợ phù hợp.</p>
            ) : null}
          </div>
        </div>
      </section>
    </>
  )
}
