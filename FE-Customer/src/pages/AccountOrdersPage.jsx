import { useState } from 'react'
import { Link, NavLink, Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { Icon } from '../components/ui/Icon'
import {
  ACCOUNT_QUICK_LINKS,
  ACCOUNT_SIDEBAR_NAV,
  PROFILE_AVATAR,
  PURCHASE_HISTORY_FILTERS,
  PURCHASE_HISTORY_ORDERS,
} from '../data/account'

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

export function AccountOrdersPage() {
  const { user } = useAuth()
  const [filter, setFilter] = useState('all')
  const [search, setSearch] = useState('')

  if (!user) {
    return <Navigate to="/login" replace />
  }

  const totalOrders = 12
  const accumulatedSpending = 150000000

  return (
    <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Top Profile Card - giống AccountPage */}
      <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-6 mb-8 overflow-hidden relative">
        <div className="absolute top-0 right-0 w-64 h-64 bg-primary/5 rounded-full -mr-20 -mt-20" />
        <div className="flex flex-col md:flex-row items-start md:items-center justify-between gap-6 relative z-10">
          <div className="flex items-center gap-6">
            <div className="relative">
              <div className="h-24 w-24 rounded-2xl bg-primary/10 flex items-center justify-center overflow-hidden border-4 border-white dark:border-slate-800 shadow-lg">
                <img src={PROFILE_AVATAR} alt="" className="w-full h-full object-cover" />
              </div>
              <div className="absolute -bottom-2 -right-2 bg-accent-orange text-white px-2 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider">
                VIP Gold
              </div>
            </div>
            <div>
              <h2 className="text-2xl font-bold text-slate-900 dark:text-white">{user.name}</h2>
              <div className="flex items-center gap-2 mt-1 flex-wrap">
                <Icon name="stars" className="text-primary text-sm" />
                <span className="text-primary font-semibold text-sm">Thành viên VIP</span>
                <span className="mx-2 text-slate-300">|</span>
                <span className="text-slate-500 text-sm">Thành viên từ 2021</span>
              </div>
            </div>
          </div>
          <div className="flex gap-4 w-full md:w-auto">
            <div className="flex-1 md:flex-none px-6 py-3 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
              <p className="text-xs text-slate-500 font-medium uppercase mb-1">Tổng đơn hàng</p>
              <p className="text-xl font-bold text-slate-900 dark:text-white">{totalOrders}</p>
            </div>
            <div className="flex-1 md:flex-none px-6 py-3 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
              <p className="text-xs text-slate-500 font-medium uppercase mb-1">Tổng chi tiêu</p>
              <p className="text-xl font-bold text-primary">{formatPrice(accumulatedSpending)}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Quick Access */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        {ACCOUNT_QUICK_LINKS.map((item) => (
          <Link
            key={item.label}
            to={item.href}
            className="group bg-white dark:bg-slate-900 p-4 rounded-xl border border-slate-200 dark:border-slate-800 hover:border-primary transition-all shadow-sm flex items-center gap-4"
          >
            <div className={`p-3 rounded-lg transition-colors ${item.colorClass}`}>
              <Icon name={item.icon} className="text-xl" />
            </div>
            <span className="font-semibold text-sm text-slate-900 dark:text-slate-100">{item.label}</span>
          </Link>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        {/* Sidebar */}
        <aside className="lg:col-span-3 space-y-6">
          <nav className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-4">
            <h3 className="px-4 py-2 text-xs font-bold text-slate-400 uppercase tracking-widest mb-2">Menu chính</h3>
            <div className="space-y-1">
              {ACCOUNT_SIDEBAR_NAV.map((item) => (
                <NavLink
                  key={item.label}
                  to={item.href}
                  end={item.href === '/account'}
                  className={({ isActive }) =>
                    `flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${
                      isActive ? 'bg-primary text-white shadow-md' : 'text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800'
                    }`
                  }
                >
                  <Icon name={item.icon} className="text-xl" />
                  <span className="font-medium">{item.label}</span>
                </NavLink>
              ))}
            </div>
          </nav>
          <div className="bg-gradient-to-br from-primary to-blue-800 rounded-2xl p-6 text-white shadow-lg relative overflow-hidden">
            <div className="absolute -right-4 -top-4 opacity-20">
              <Icon name="local_offer" className="text-8xl" />
            </div>
            <h4 className="text-xl font-bold mb-2 relative z-10">Ưu đãi của bạn</h4>
            <p className="text-blue-100 text-sm mb-4 leading-relaxed relative z-10">
              Giảm 15% độc quyền cho máy rửa bát Bosch trong tháng.
            </p>
            <button type="button" className="w-full py-2 bg-white text-primary font-bold rounded-lg hover:bg-slate-100 transition-colors relative z-10">
              Nhận ngay
            </button>
          </div>
        </aside>

        {/* Main: Lịch sử mua hàng */}
        <section className="lg:col-span-9">
          <div className="mb-6">
            <h1 className="text-2xl font-bold text-slate-900 dark:text-white mb-2">
              Lịch sử mua hàng - Macvilla
            </h1>
            <p className="text-slate-500 dark:text-slate-400 text-sm">
              Theo dõi và quản lý các đơn hàng đã đặt của bạn.
            </p>
          </div>

          {/* Filters & Search */}
          <div className="bg-white dark:bg-slate-900 rounded-xl p-4 shadow-sm border border-slate-200 dark:border-slate-800 mb-6 space-y-4">
            <div className="relative w-full">
              <Icon name="search" className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                type="text"
                placeholder="Tìm kiếm theo mã đơn hàng hoặc tên sản phẩm..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border-none rounded-lg text-sm focus:ring-2 focus:ring-primary text-slate-900 dark:text-slate-100"
              />
            </div>
            <div className="flex flex-wrap gap-2">
              {PURCHASE_HISTORY_FILTERS.map((f) => (
                <button
                  key={f.id}
                  type="button"
                  onClick={() => setFilter(f.id)}
                  className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
                    filter === f.id
                      ? 'bg-primary text-white shadow-md shadow-primary/20'
                      : 'bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400 hover:bg-slate-200 dark:hover:bg-slate-700'
                  }`}
                >
                  {f.label}
                </button>
              ))}
            </div>
          </div>

          {/* Order List */}
          <div className="space-y-4">
            {PURCHASE_HISTORY_ORDERS.map((order) => (
              <div
                key={order.id}
                className={`bg-white dark:bg-slate-900 rounded-xl shadow-sm border border-slate-200 dark:border-slate-800 overflow-hidden ${
                  order.status === 'cancelled' ? 'opacity-80' : ''
                }`}
              >
                <div className="px-6 py-4 border-b border-slate-100 dark:border-slate-800 flex flex-wrap justify-between items-center bg-slate-50/50 dark:bg-slate-800/50 gap-4">
                  <div className="flex items-center gap-4">
                    <div>
                      <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider">Mã đơn hàng</p>
                      <p className="font-bold text-primary">#{order.id}</p>
                    </div>
                    <div className="h-8 w-px bg-slate-200 dark:bg-slate-700" />
                    <div>
                      <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider">Ngày đặt</p>
                      <p className="text-sm font-medium text-slate-900 dark:text-slate-100">{order.date}</p>
                    </div>
                  </div>
                  <div className={`flex items-center gap-2 px-3 py-1 rounded-full text-xs font-bold ${order.statusClass}`}>
                    <Icon name={order.statusIcon} className="text-sm" />
                    {order.statusLabel}
                  </div>
                </div>
                <div className="p-6">
                  {order.items.map((item, idx) => (
                    <div key={idx} className="flex items-start gap-4 mb-4">
                      <div className="w-20 h-20 bg-slate-100 dark:bg-slate-800 rounded-lg flex-shrink-0 border border-slate-100 dark:border-slate-700 overflow-hidden">
                        <img src={item.image} alt={item.imageAlt} className="w-full h-full object-cover rounded-lg" />
                      </div>
                      <div className="flex-grow min-w-0">
                        <h4 className="font-semibold text-slate-900 dark:text-white line-clamp-1">{item.name}</h4>
                        <p className="text-sm text-slate-500">Số lượng: {item.qty}</p>
                        <p className="text-sm font-medium text-slate-900 dark:text-slate-100 mt-1">
                          {formatPrice(item.price * item.qty)}
                        </p>
                      </div>
                    </div>
                  ))}
                  <div className="flex flex-wrap items-center justify-between gap-4 pt-4 border-t border-slate-100 dark:border-slate-800">
                    <div>
                      <p className="text-sm text-slate-500">Tổng thanh toán</p>
                      <p className="text-lg font-bold text-primary">{formatPrice(order.total)}</p>
                    </div>
                    <div className="flex flex-wrap gap-2">
                      {(order.actions || []).includes('detail') && (
                        <Link
                          to={`/account/orders/${order.id}`}
                          className="px-4 py-2 border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-300 rounded-lg text-sm font-medium hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors inline-block"
                        >
                          Xem chi tiết
                        </Link>
                      )}
                      {(order.actions || []).includes('track') && (
                        <button
                          type="button"
                          className="px-4 py-2 border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-300 rounded-lg text-sm font-medium hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
                        >
                          Theo dõi đơn hàng
                        </button>
                      )}
                      {(order.actions || []).includes('warranty') && (
                        <button
                          type="button"
                          className="px-4 py-2 border border-primary text-primary rounded-lg text-sm font-medium hover:bg-primary/5 transition-colors"
                        >
                          Yêu cầu bảo hành
                        </button>
                      )}
                      {(order.actions || []).includes('reorder') && (
                        <button
                          type="button"
                          className="px-4 py-2 bg-primary text-white rounded-lg text-sm font-medium hover:bg-primary/90 transition-colors"
                        >
                          Mua lại
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Pagination */}
          <div className="mt-8 flex justify-center">
            <nav className="flex items-center gap-2" aria-label="Phân trang">
              <button
                type="button"
                className="w-10 h-10 flex items-center justify-center rounded-lg border border-slate-200 dark:border-slate-800 text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
                aria-label="Trang trước"
              >
                <Icon name="chevron_left" />
              </button>
              <button
                type="button"
                className="w-10 h-10 flex items-center justify-center rounded-lg bg-primary text-white font-medium"
              >
                1
              </button>
              <button
                type="button"
                className="w-10 h-10 flex items-center justify-center rounded-lg border border-slate-200 dark:border-slate-800 text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
              >
                2
              </button>
              <button
                type="button"
                className="w-10 h-10 flex items-center justify-center rounded-lg border border-slate-200 dark:border-slate-800 text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
              >
                3
              </button>
              <button
                type="button"
                className="w-10 h-10 flex items-center justify-center rounded-lg border border-slate-200 dark:border-slate-800 text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
                aria-label="Trang sau"
              >
                <Icon name="chevron_right" />
              </button>
            </nav>
          </div>
        </section>
      </div>
    </main>
  )
}
