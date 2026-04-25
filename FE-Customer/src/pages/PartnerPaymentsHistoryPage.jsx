import { useMemo, useState } from 'react'
import { Icon } from '../components/ui/Icon'
import { PartnerPaymentsPageHeader } from '../components/partner/PartnerPaymentsPageHeader'
import {
  B2B_PAYMENTS_HISTORY_TABS,
  B2B_PAYMENTS_HISTORY_ROWS,
} from '../data/b2bDashboard'

export function PartnerPaymentsHistoryPage() {
  const [activeTab, setActiveTab] = useState('all')
  const [search, setSearch] = useState('')

  const rows = useMemo(() => {
    let list = B2B_PAYMENTS_HISTORY_ROWS
    if (activeTab !== 'all') {
      list = list.filter((r) => r.statusKey === activeTab)
    }
    if (search.trim()) {
      const q = search.trim().toLowerCase()
      list = list.filter(
        (r) =>
          r.txId.toLowerCase().includes(q) ||
          r.ref.toLowerCase().includes(q) ||
          r.note.toLowerCase().includes(q)
      )
    }
    return list
  }, [activeTab, search])

  return (
    <>
      <PartnerPaymentsPageHeader
        title="Lịch sử thanh toán"
        subtitle="Mọi giao dịch ghi nhận trên tài khoản doanh nghiệp: chuyển khoản, cấn trừ và trạng thái đối soát."
        below={
          <div className="flex border-b border-slate-200 dark:border-slate-800 gap-6 overflow-x-auto">
            {B2B_PAYMENTS_HISTORY_TABS.map((tab) => (
              <button
                key={tab.id}
                type="button"
                onClick={() => setActiveTab(tab.id)}
                className={`pb-4 px-1 border-b-2 whitespace-nowrap text-sm font-medium transition-colors ${
                  activeTab === tab.id
                    ? 'border-primary text-primary font-bold'
                    : 'border-transparent text-slate-500 hover:text-slate-700 dark:hover:text-slate-300'
                }`}
              >
                {tab.label}
              </button>
            ))}
          </div>
        }
      />

      <section className="p-8 pt-4">
        <div className="bg-white dark:bg-slate-900 p-4 rounded-xl border border-slate-200 dark:border-slate-800 flex flex-wrap gap-4 items-center mb-6">
          <div className="flex-1 min-w-[280px] relative">
            <Icon name="search" className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Tìm theo mã giao dịch, tham chiếu…"
              className="w-full pl-10 pr-4 py-2.5 bg-slate-100 dark:bg-slate-800 border-none rounded-lg focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400"
            />
          </div>
          <button
            type="button"
            className="flex items-center gap-2 px-4 py-2.5 bg-slate-100 dark:bg-slate-800 rounded-lg text-sm font-medium hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors text-slate-700 dark:text-slate-200"
          >
            <Icon name="calendar_today" className="text-lg" />
            Khoảng ngày
            <Icon name="expand_more" className="text-lg" />
          </button>
        </div>

        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Thời gian</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Mã GD</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Tham chiếu</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Phương thức</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Số tiền</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-center">Trạng thái</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Ghi chú</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
              {rows.map((row) => (
                <tr key={row.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/30 transition-colors">
                  <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400 whitespace-nowrap">{row.at}</td>
                  <td className="px-6 py-4 text-sm font-mono font-medium text-slate-800 dark:text-slate-100">{row.txId}</td>
                  <td className="px-6 py-4 text-sm text-primary font-semibold">{row.ref}</td>
                  <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400">{row.method}</td>
                  <td className="px-6 py-4 text-sm font-bold text-right text-slate-900 dark:text-slate-100">{row.amount}</td>
                  <td className="px-6 py-4 text-center">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold ${row.statusClass}`}>
                      {row.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-500 dark:text-slate-400 max-w-[200px]">{row.note}</td>
                  <td className="px-6 py-4 text-right">
                    <button
                      type="button"
                      className="p-1.5 text-slate-400 hover:text-primary transition-colors"
                      title="Xem chi tiết"
                    >
                      <Icon name="visibility" className="text-lg" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {rows.length === 0 ? (
            <p className="text-center text-slate-500 py-12 text-sm">Không có giao dịch phù hợp bộ lọc.</p>
          ) : null}
          <div className="px-6 py-4 bg-slate-50 dark:bg-slate-800/50 border-t border-slate-200 dark:border-slate-800">
            <p className="text-sm text-slate-500 dark:text-slate-400">
              Hiển thị {rows.length} giao dịch (dữ liệu minh họa)
            </p>
          </div>
        </div>
      </section>
    </>
  )
}
