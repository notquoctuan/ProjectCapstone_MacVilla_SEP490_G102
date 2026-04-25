import { useState } from 'react'
import { Link, NavLink, Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { Icon } from '../components/ui/Icon'
import {
  ACCOUNT_QUICK_LINKS,
  ACCOUNT_SIDEBAR_NAV,
  PROFILE_AVATAR,
} from '../data/account'
import { WARRANTY_FILTERS, WARRANTY_PRODUCTS } from '../data/warranty'

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

export function AccountWarrantyPage() {
  const { user } = useAuth()
  const [filter, setFilter] = useState('all')
  const [phone, setPhone] = useState('')
  const [serial, setSerial] = useState('')

  if (!user) return <Navigate to="/login" replace />

  const totalOrders = 12
  const accumulatedSpending = 150000000
  const products =
    filter === 'active'
      ? WARRANTY_PRODUCTS.filter((p) => p.status === 'active')
      : WARRANTY_PRODUCTS

  return (
    <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Profile Card */}
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

        {/* Main: Tra cứu bảo hành */}
        <section className="lg:col-span-9">
          <div className="mb-8">
            <h2 className="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight">
              Tra cứu bảo hành điện tử
            </h2>
            <p className="text-slate-500 dark:text-slate-400 mt-1">
              Quản lý và theo dõi thời hạn bảo hành các thiết bị Macvilla của bạn.
            </p>
          </div>

          {/* Search */}
          <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-200 dark:border-slate-800 p-6 mb-10">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 items-end">
              <div>
                <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">
                  Số điện thoại
                </label>
                <input
                  type="text"
                  placeholder="Nhập số điện thoại mua hàng"
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  className="w-full px-4 py-3 rounded-xl border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/10 text-slate-900 dark:text-slate-100"
                />
              </div>
              <div>
                <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2">
                  Mã Serial / Mã bảo hành
                </label>
                <input
                  type="text"
                  placeholder="Ví dụ: SN-99283-MV"
                  value={serial}
                  onChange={(e) => setSerial(e.target.value)}
                  className="w-full px-4 py-3 rounded-xl border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/10 text-slate-900 dark:text-slate-100"
                />
              </div>
              <div>
                <button
                  type="button"
                  className="w-full bg-primary hover:bg-primary/90 text-white font-bold py-3 px-6 rounded-xl transition-all shadow-lg shadow-primary/20 flex items-center justify-center gap-2"
                >
                  <Icon name="search" />
                  Tra cứu ngay
                </button>
              </div>
            </div>
          </div>

          {/* Sản phẩm của bạn */}
          <div className="flex flex-wrap items-center justify-between gap-4 mb-6">
            <h3 className="text-xl font-bold flex items-center gap-2 text-slate-900 dark:text-white">
              <Icon name="inventory_2" className="text-primary" />
              Sản phẩm của bạn
            </h3>
            <div className="flex gap-2">
              {WARRANTY_FILTERS.map((f) => (
                <button
                  key={f.id}
                  type="button"
                  onClick={() => setFilter(f.id)}
                  className={`px-3 py-1 text-xs font-medium rounded-full transition-colors ${
                    filter === f.id
                      ? 'bg-primary/10 text-primary border border-primary/20'
                      : 'bg-slate-100 dark:bg-slate-800 text-slate-500 dark:text-slate-400'
                  }`}
                >
                  {f.label}
                </button>
              ))}
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {products.map((product) => (
              <div
                key={product.id}
                className="bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm hover:shadow-md transition-shadow"
              >
                <div className="p-5 flex gap-5">
                  <div className="w-32 h-32 bg-slate-100 dark:bg-slate-800 rounded-xl flex-shrink-0 flex items-center justify-center overflow-hidden">
                    {product.image ? (
                      <img
                        src={product.image}
                        alt={product.imageAlt}
                        className="w-24 h-24 object-contain mix-blend-multiply dark:mix-blend-normal"
                      />
                    ) : (
                      <div className="w-24 h-24 bg-primary/5 rounded-full flex items-center justify-center">
                        <Icon name={product.imageIcon || 'kitchen'} className="text-4xl text-primary/30" />
                      </div>
                    )}
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex justify-between items-start mb-1 gap-2">
                      <span className={`px-2.5 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider shrink-0 ${product.statusClass}`}>
                        {product.statusLabel}
                      </span>
                      <p className="text-xs text-slate-400 font-medium">#{product.id}</p>
                    </div>
                    <h4 className="text-lg font-bold text-slate-900 dark:text-white leading-tight">
                      {product.name}
                    </h4>
                    <div className="mt-3 space-y-1.5">
                      <div className="flex justify-between text-xs">
                        <span className="text-slate-500">Ngày kích hoạt:</span>
                        <span className="font-semibold text-slate-900 dark:text-slate-100">{product.activatedDate}</span>
                      </div>
                      <div className="flex justify-between text-xs">
                        <span className="text-slate-500">Thời hạn còn lại:</span>
                        <span className={`font-semibold ${product.remainingClass}`}>{product.remainingLabel}</span>
                      </div>
                      <div className="w-full bg-slate-100 dark:bg-slate-800 h-2 rounded-full mt-2 overflow-hidden">
                        <div
                          className={`h-2 rounded-full transition-all ${product.progressBarClass}`}
                          style={{ width: `${product.progressPercent}%` }}
                        />
                      </div>
                    </div>
                  </div>
                </div>
                <div className="bg-slate-50 dark:bg-slate-800/50 px-5 py-3 flex gap-3">
                  <button
                    type="button"
                    className="flex-1 bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 py-2 rounded-lg text-xs font-bold hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors flex items-center justify-center gap-1 text-slate-700 dark:text-slate-200"
                  >
                    <Icon name="download" className="text-sm" />
                    Tải HDSD
                  </button>
                  {product.canRepair ? (
                    <button
                      type="button"
                      className="flex-1 bg-primary text-white py-2 rounded-lg text-xs font-bold hover:bg-primary/90 transition-colors flex items-center justify-center gap-1"
                    >
                      <Icon name="build" className="text-sm" />
                      Yêu cầu sửa chữa
                    </button>
                  ) : (
                    <button
                      type="button"
                      disabled
                      className="flex-1 bg-slate-200 dark:bg-slate-700 text-slate-500 dark:text-slate-400 cursor-not-allowed py-2 rounded-lg text-xs font-bold flex items-center justify-center gap-1"
                    >
                      <Icon name="lock" className="text-sm" />
                      Gia hạn bảo hành
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>

          {/* Quy định bảo hành */}
          <div className="mt-12 p-8 bg-primary/5 dark:bg-primary/10 rounded-3xl border border-primary/20">
            <div className="flex flex-col md:flex-row gap-8 items-center">
              <div className="flex-1">
                <h4 className="text-2xl font-bold text-primary mb-3">
                  Quy định bảo hành tại Macvilla
                </h4>
                <ul className="space-y-3">
                  <li className="flex items-start gap-2 text-sm text-slate-700 dark:text-slate-200">
                    <Icon name="check_circle" className="text-primary text-lg shrink-0 mt-0.5" />
                    <span>Bảo hành chính hãng 100% cho tất cả các thiết bị nhập khẩu.</span>
                  </li>
                  <li className="flex items-start gap-2 text-sm text-slate-700 dark:text-slate-200">
                    <Icon name="check_circle" className="text-primary text-lg shrink-0 mt-0.5" />
                    <span>Hỗ trợ bảo trì định kỳ miễn phí cho khách hàng VIP.</span>
                  </li>
                  <li className="flex items-start gap-2 text-sm text-slate-700 dark:text-slate-200">
                    <Icon name="check_circle" className="text-primary text-lg shrink-0 mt-0.5" />
                    <span>Thay thế linh kiện chính hãng kèm bảo hành linh kiện riêng.</span>
                  </li>
                </ul>
                <button
                  type="button"
                  className="mt-6 text-primary font-bold text-sm underline underline-offset-4 hover:opacity-80 transition-opacity"
                >
                  Xem chi tiết chính sách bảo hành
                </button>
              </div>
              <div className="w-full md:w-64 h-48 bg-white dark:bg-slate-800 rounded-2xl shadow-inner flex items-center justify-center">
                <div className="text-center p-6">
                  <Icon name="support_agent" className="text-5xl text-primary/40 mb-2 mx-auto block" />
                  <p className="text-xs text-slate-500 mb-1">Tổng đài hỗ trợ 24/7</p>
                  <p className="text-xl font-black text-primary">1900 6789</p>
                </div>
              </div>
            </div>
          </div>
        </section>
      </div>
    </main>
  )
}
