import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { PartnerHeaderUser } from '../components/partner/PartnerHeaderUser'
import { B2B_ORDERS_TABS, B2B_ORDERS_LIST } from '../data/b2bDashboard'

export function PartnerOrdersPage() {
  const [activeTab, setActiveTab] = useState('all')
  const [search, setSearch] = useState('')

  return (
    <>
      {/* Header section - clone temp.html */}
      <header className="p-8 pb-0">
        <div className="flex justify-between items-end mb-6">
          <div>
            <h2 className="text-3xl font-black tracking-tight mb-2 text-slate-900 dark:text-slate-50">
              Quản lý Đơn hàng
            </h2>
            <p className="text-slate-500 dark:text-slate-400">
              Theo dõi và quản lý tất cả đơn hàng trong hệ thống B2B của bạn.
            </p>
          </div>
          <div className="flex items-center gap-4">
            <button
              type="button"
              className="w-10 h-10 rounded-full bg-slate-200 dark:bg-slate-800 flex items-center justify-center text-slate-600 dark:text-slate-400 hover:text-primary transition-colors"
              aria-label="Thông báo"
            >
              <Icon name="notifications" className="text-xl" />
            </button>
            <PartnerHeaderUser hideTextOnMobile={false} />
          </div>
        </div>
        {/* Tabs */}
        <div className="flex border-b border-slate-200 dark:border-slate-800 gap-8 overflow-x-auto">
          {B2B_ORDERS_TABS.map((tab) => (
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
      </header>

      {/* Filters + Table */}
      <section className="p-8">
        <div className="bg-white dark:bg-slate-900 p-4 rounded-xl border border-slate-200 dark:border-slate-800 flex flex-wrap gap-4 items-center">
          <div className="flex-1 min-w-[300px] relative">
            <Icon name="search" className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Tìm theo mã đơn hàng hoặc tên sản phẩm..."
              className="w-full pl-10 pr-4 py-2.5 bg-slate-100 dark:bg-slate-800 border-none rounded-lg focus:ring-2 focus:ring-primary text-sm text-slate-900 dark:text-slate-100 placeholder-slate-400"
            />
          </div>
          <div className="flex gap-3">
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2.5 bg-slate-100 dark:bg-slate-800 rounded-lg text-sm font-medium hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors text-slate-700 dark:text-slate-200"
            >
              <Icon name="calendar_today" className="text-lg" />
              Khoảng ngày: Tất cả
              <Icon name="expand_more" className="text-lg" />
            </button>
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2.5 bg-slate-100 dark:bg-slate-800 rounded-lg text-sm font-medium hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors text-slate-700 dark:text-slate-200"
            >
              <Icon name="assignment" className="text-lg" />
              Dự án: Tất cả
              <Icon name="expand_more" className="text-lg" />
            </button>
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2.5 bg-primary/10 text-primary rounded-lg text-sm font-bold hover:bg-primary/20 transition-colors"
            >
              <Icon name="filter_alt" className="text-lg" />
              Lọc nâng cao
            </button>
          </div>
        </div>

        {/* Orders Table */}
        <div className="mt-6 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Mã đơn hàng</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Ngày đặt</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Dự án</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Tổng tiền</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-center">Trạng thái</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
              {B2B_ORDERS_LIST.map((row) => (
                <tr key={row.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/30 transition-colors">
                  <td className="px-6 py-4">
                    <span className="font-bold text-primary">#{row.id}</span>
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-400">{row.date}</td>
                  <td className="px-6 py-4">
                    {row.projectSub ? (
                      <div className="flex flex-col">
                        <span className="text-sm font-medium text-slate-800 dark:text-slate-100">{row.project}</span>
                        <span className="text-xs text-slate-400">{row.projectSub}</span>
                      </div>
                    ) : (
                      <span className="text-sm font-medium text-slate-800 dark:text-slate-100">{row.project}</span>
                    )}
                  </td>
                  <td className="px-6 py-4 text-sm font-bold text-right text-slate-900 dark:text-slate-100">{row.total}</td>
                  <td className="px-6 py-4 text-center">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold ${row.statusClass}`}>
                      {row.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <Link
                        to={`/partner/orders/${row.id}`}
                        className="p-1.5 text-slate-400 hover:text-primary transition-colors"
                        title="Xem chi tiết"
                      >
                        <Icon name="visibility" className="text-lg" />
                      </Link>
                      <button
                        type="button"
                        className="p-1.5 text-slate-400 hover:text-primary transition-colors disabled:opacity-30 disabled:cursor-not-allowed"
                        title="Tải hóa đơn"
                        disabled={!row.canDownload}
                      >
                        <Icon name="download" className="text-lg" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {/* Pagination */}
          <div className="px-6 py-4 bg-slate-50 dark:bg-slate-800/50 flex items-center justify-between border-t border-slate-200 dark:border-slate-800">
            <p className="text-sm text-slate-500 dark:text-slate-400">Hiển thị 1 - 5 trên 128 đơn hàng</p>
            <div className="flex gap-2">
              <button
                type="button"
                className="w-9 h-9 flex items-center justify-center rounded-lg border border-slate-300 dark:border-slate-700 text-slate-500 hover:bg-white dark:hover:bg-slate-800 transition-colors"
              >
                <Icon name="chevron_left" className="text-lg" />
              </button>
              <button
                type="button"
                className="w-9 h-9 flex items-center justify-center rounded-lg bg-primary text-white font-bold text-sm"
              >
                1
              </button>
              <button
                type="button"
                className="w-9 h-9 flex items-center justify-center rounded-lg border border-transparent text-slate-500 hover:bg-white dark:hover:bg-slate-800 transition-colors text-sm font-medium"
              >
                2
              </button>
              <button
                type="button"
                className="w-9 h-9 flex items-center justify-center rounded-lg border border-transparent text-slate-500 hover:bg-white dark:hover:bg-slate-800 transition-colors text-sm font-medium"
              >
                3
              </button>
              <span className="px-2 self-center text-slate-400">...</span>
              <button
                type="button"
                className="w-9 h-9 flex items-center justify-center rounded-lg border border-transparent text-slate-500 hover:bg-white dark:hover:bg-slate-800 transition-colors text-sm font-medium"
              >
                15
              </button>
              <button
                type="button"
                className="w-9 h-9 flex items-center justify-center rounded-lg border border-slate-300 dark:border-slate-700 text-slate-500 hover:bg-white dark:hover:bg-slate-800 transition-colors"
              >
                <Icon name="chevron_right" className="text-lg" />
              </button>
            </div>
          </div>
        </div>
      </section>
    </>
  )
}
