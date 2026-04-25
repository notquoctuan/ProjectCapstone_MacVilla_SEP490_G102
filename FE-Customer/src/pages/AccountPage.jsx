import { Link, NavLink, Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { Icon } from '../components/ui/Icon'
import {
  ACCOUNT_QUICK_LINKS,
  ACCOUNT_SIDEBAR_NAV,
  ACCOUNT_RECENT_ORDERS,
  PROFILE_AVATAR,
} from '../data/account'

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

export function AccountPage() {
  const { user } = useAuth()

  if (!user) {
    return <Navigate to="/login" replace />
  }

  const totalOrders = 12
  const accumulatedSpending = 150000000

  return (
    <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Top Profile Card */}
      <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-6 mb-8 overflow-hidden relative">
        <div className="absolute top-0 right-0 w-64 h-64 bg-primary/5 rounded-full -mr-20 -mt-20" />
        <div className="flex flex-col md:flex-row items-start md:items-center justify-between gap-6 relative z-10">
          <div className="flex items-center gap-6">
            <div className="relative">
              <div className="h-24 w-24 rounded-2xl bg-primary/10 flex items-center justify-center overflow-hidden border-4 border-white dark:border-slate-800 shadow-lg">
                <img
                  src={PROFILE_AVATAR}
                  alt=""
                  className="w-full h-full object-cover"
                />
              </div>
              <div className="absolute -bottom-2 -right-2 bg-accent-orange text-white px-2 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider">
                VIP Gold
              </div>
            </div>
            <div>
              <h2 className="text-2xl font-bold text-slate-900 dark:text-white">
                {user.name}
              </h2>
              <div className="flex items-center gap-2 mt-1 flex-wrap">
                <Icon name="stars" className="text-primary text-sm" />
                <span className="text-primary font-semibold text-sm">
                  Thành viên VIP
                </span>
                <span className="mx-2 text-slate-300">|</span>
                <span className="text-slate-500 text-sm">
                  Thành viên từ 2021
                </span>
              </div>
            </div>
          </div>
          <div className="flex gap-4 w-full md:w-auto">
            <div className="flex-1 md:flex-none px-6 py-3 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
              <p className="text-xs text-slate-500 font-medium uppercase mb-1">
                Tổng đơn hàng
              </p>
              <p className="text-xl font-bold text-slate-900 dark:text-white">
                {totalOrders}
              </p>
            </div>
            <div className="flex-1 md:flex-none px-6 py-3 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
              <p className="text-xs text-slate-500 font-medium uppercase mb-1">
                Tổng chi tiêu
              </p>
              <p className="text-xl font-bold text-primary">
                {formatPrice(accumulatedSpending)}
              </p>
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
            <div
              className={`p-3 rounded-lg transition-colors ${item.colorClass}`}
            >
              <Icon name={item.icon} className="text-xl" />
            </div>
            <span className="font-semibold text-sm text-slate-900 dark:text-slate-100">
              {item.label}
            </span>
          </Link>
        ))}
      </div>

      {/* Main Body: Sidebar + Content */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        {/* Left Sidebar */}
        <aside className="lg:col-span-3 space-y-6">
          <nav className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-4">
            <h3 className="px-4 py-2 text-xs font-bold text-slate-400 uppercase tracking-widest mb-2">
              Menu chính
            </h3>
            <div className="space-y-1">
              {ACCOUNT_SIDEBAR_NAV.map((item) => (
                <NavLink
                  key={item.label}
                  to={item.href}
                  end={item.href === '/account'}
                  className={({ isActive }) =>
                    `flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${
                      isActive
                        ? 'bg-primary text-white shadow-md'
                        : 'text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800'
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
            <h4 className="text-xl font-bold mb-2 relative z-10">
              Ưu đãi của bạn
            </h4>
            <p className="text-blue-100 text-sm mb-4 leading-relaxed relative z-10">
              Giảm 15% độc quyền cho máy rửa bát Bosch trong tháng.
            </p>
            <button
              type="button"
              className="w-full py-2 bg-white text-primary font-bold rounded-lg hover:bg-slate-100 transition-colors relative z-10"
            >
              Nhận ngay
            </button>
          </div>
        </aside>

        {/* Main Content */}
        <section className="lg:col-span-9">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-xl font-bold text-slate-900 dark:text-white flex items-center gap-2">
              <Icon name="shopping_bag" className="text-primary" />
              Đơn hàng gần đây
            </h3>
            <Link
              to="/account/orders"
              className="text-primary font-semibold text-sm hover:underline"
            >
              Xem tất cả đơn hàng
            </Link>
          </div>
          <div className="space-y-4">
            {ACCOUNT_RECENT_ORDERS.map((order) => (
              <div
                key={order.id}
                className="bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 p-5 shadow-sm hover:shadow-md transition-shadow group"
              >
                <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                  <div className="flex items-center gap-5">
                    <div className="h-20 w-20 bg-slate-50 dark:bg-slate-800 rounded-xl flex-shrink-0 border border-slate-100 dark:border-slate-700 overflow-hidden group-hover:border-primary/30 transition-colors">
                      <img
                        src={order.image}
                        alt={order.imageAlt}
                        className="w-full h-full object-cover"
                      />
                    </div>
                    <div>
                      <h4 className="font-bold text-slate-900 dark:text-white text-lg">
                        {order.title}
                      </h4>
                      <div className="flex items-center gap-4 mt-1 flex-wrap">
                        <span className="text-sm text-slate-500 font-medium">
                          ID: #{order.id}
                        </span>
                        <span className="text-sm text-slate-500">
                          {order.date}
                        </span>
                      </div>
                    </div>
                  </div>
                  <div className="flex flex-col items-end gap-2 w-full sm:w-auto">
                    <span
                      className={`px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wide ${order.statusClass}`}
                    >
                      {order.statusLabel}
                    </span>
                    <p className="text-xl font-bold text-slate-900 dark:text-white">
                      {formatPrice(order.total)}
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Featured Section Below Orders */}
          <div className="mt-8 grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="bg-slate-100 dark:bg-slate-800/40 rounded-2xl p-6 border border-dashed border-slate-300 dark:border-slate-700 flex items-center justify-center text-center">
              <div>
                <Icon
                  name="add_circle"
                  className="text-4xl text-slate-400 mb-2 mx-auto"
                />
                <p className="text-slate-500 font-medium">
                  Đăng ký bảo hành sản phẩm mới
                </p>
              </div>
            </div>
            <div className="bg-slate-100 dark:bg-slate-800/40 rounded-2xl p-6 border border-dashed border-slate-300 dark:border-slate-700 flex items-center justify-center text-center">
              <div>
                <Icon
                  name="support_agent"
                  className="text-4xl text-slate-400 mb-2 mx-auto"
                />
                <p className="text-slate-500 font-medium">
                  Cần hỗ trợ? Chat với chuyên gia
                </p>
              </div>
            </div>
          </div>
        </section>
      </div>
    </main>
  )
}
