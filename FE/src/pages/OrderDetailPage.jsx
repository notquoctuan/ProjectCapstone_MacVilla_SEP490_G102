import { Link, useParams, Navigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { Icon } from '../components/ui/Icon'
import { Breadcrumbs } from '../components/layout/Breadcrumbs'
import { getOrderDetail, ORDER_DETAIL_SHIPPING_STEPS } from '../data/orderDetail'

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

export function OrderDetailPage() {
  const { orderId } = useParams()
  const { user } = useAuth()
  if (!user) return <Navigate to="/login" replace />

  const order = getOrderDetail(orderId)
  const breadcrumbItems = [
    { label: 'Trang chủ', href: '/' },
    { label: 'Tài khoản', href: '/account' },
    { label: 'Lịch sử mua hàng', href: '/account/orders' },
    { label: `Chi tiết đơn hàng #${order.id}`, href: null },
  ]

  return (
    <main className="max-w-[1280px] mx-auto w-full px-4 md:px-10 py-6">
      <Breadcrumbs items={breadcrumbItems} />

      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-8">
        <div>
          <h1 className="text-slate-900 dark:text-slate-50 text-3xl font-black leading-tight tracking-tight">
            Đơn hàng #{order.id}
          </h1>
          <p className="text-slate-500 dark:text-slate-400 mt-1">
            Ngày đặt hàng: {order.orderDate} | {order.orderTime}
          </p>
        </div>
        <div className="flex flex-wrap gap-3">
          <button
            type="button"
            className="px-5 py-2.5 bg-primary text-white rounded-lg font-bold text-sm hover:bg-primary/90 transition-all flex items-center gap-2 shadow-lg shadow-primary/20"
          >
            <Icon name="refresh" className="text-lg" />
            Mua lại
          </button>
          <button
            type="button"
            className="px-5 py-2.5 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-200 rounded-lg font-bold text-sm hover:bg-slate-50 dark:hover:bg-slate-700 transition-all flex items-center gap-2"
          >
            <Icon name="receipt" className="text-lg" />
            Yêu cầu hóa đơn VAT
          </button>
        </div>
      </div>

      <div className="flex flex-col lg:flex-row gap-8">
        {/* Left column ~70% */}
        <div className="flex-1 w-full lg:w-[70%] space-y-8">
          {/* Trạng thái vận chuyển */}
          <div className="bg-white dark:bg-slate-900 p-8 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm">
            <h3 className="text-lg font-bold mb-8 text-slate-800 dark:text-slate-100">
              Trạng thái vận chuyển
            </h3>
            <div className="relative flex justify-between items-start">
              <div className="absolute top-5 left-0 w-full h-1 bg-slate-100 dark:bg-slate-800 -z-0 rounded-full overflow-hidden">
                <div
                  className="h-full bg-primary transition-all"
                  style={{ width: `${order.progressPercent ?? 75}%` }}
                />
              </div>
              {ORDER_DETAIL_SHIPPING_STEPS.map((step, index) => (
                <div
                  key={step.label}
                  className="relative flex flex-col items-center text-center gap-3 z-10 w-1/4"
                >
                  <div
                    className={`w-10 h-10 rounded-full flex items-center justify-center ring-4 ring-white dark:ring-slate-900 ${
                      step.done ? 'bg-primary text-white' : 'bg-slate-200 dark:bg-slate-700 text-slate-500 dark:text-slate-400'
                    }`}
                  >
                    <Icon name={step.icon} className="text-xl" />
                  </div>
                  <div className="space-y-1">
                    <p
                      className={`text-sm font-bold ${
                        step.done ? 'text-primary' : 'text-slate-500 dark:text-slate-400'
                      }`}
                    >
                      {step.label}
                    </p>
                    <p className="text-slate-500 text-xs">{step.time}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Chi tiết sản phẩm - bảng */}
          <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
            <div className="px-6 py-4 border-b border-slate-200 dark:border-slate-800">
              <h3 className="text-lg font-bold text-slate-800 dark:text-slate-100">
                Chi tiết sản phẩm
              </h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="bg-slate-50 dark:bg-slate-800/50">
                    <th className="px-6 py-4 text-xs font-bold uppercase tracking-wider text-slate-500">
                      Sản phẩm
                    </th>
                    <th className="px-6 py-4 text-xs font-bold uppercase tracking-wider text-slate-500 text-right">
                      Đơn giá
                    </th>
                    <th className="px-6 py-4 text-xs font-bold uppercase tracking-wider text-slate-500 text-center">
                      Số lượng
                    </th>
                    <th className="px-6 py-4 text-xs font-bold uppercase tracking-wider text-slate-500 text-right">
                      Thành tiền
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                  {order.products.map((row, idx) => (
                    <tr key={idx}>
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-4">
                          <div
                            className="size-16 rounded-lg bg-slate-100 dark:bg-slate-800 bg-center bg-cover flex-shrink-0 border border-slate-100 dark:border-slate-700"
                            style={{ backgroundImage: `url('${row.image}')` }}
                            role="img"
                            aria-label={row.imageAlt}
                          />
                          <div>
                            <p className="font-bold text-slate-800 dark:text-slate-100">{row.name}</p>
                            <p className="text-xs text-slate-500">Mã SP: {row.sku}</p>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 text-right text-slate-700 dark:text-slate-300 font-medium">
                        {formatPrice(row.unitPrice)}
                      </td>
                      <td className="px-6 py-4 text-center text-slate-700 dark:text-slate-300">
                        {row.quantity}
                      </td>
                      <td className="px-6 py-4 text-right text-primary font-bold">
                        {formatPrice(row.subtotal)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        {/* Right column ~30% */}
        <div className="w-full lg:w-[30%] space-y-6">
          <div className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm">
            <h3 className="text-base font-bold mb-4 flex items-center gap-2 text-slate-800 dark:text-slate-100">
              <Icon name="location_on" className="text-primary" />
              Thông tin giao hàng
            </h3>
            <div className="space-y-3">
              <div>
                <p className="text-xs text-slate-500 uppercase font-bold tracking-wider">Người nhận</p>
                <p className="font-bold text-slate-800 dark:text-slate-200">{order.shipping.recipient}</p>
              </div>
              <div>
                <p className="text-xs text-slate-500 uppercase font-bold tracking-wider">Số điện thoại</p>
                <p className="text-slate-800 dark:text-slate-200">{order.shipping.phone}</p>
              </div>
              <div>
                <p className="text-xs text-slate-500 uppercase font-bold tracking-wider">Địa chỉ</p>
                <p className="text-slate-800 dark:text-slate-200 leading-relaxed">{order.shipping.address}</p>
              </div>
            </div>
          </div>

          <div className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm">
            <h3 className="text-base font-bold mb-4 flex items-center gap-2 text-slate-800 dark:text-slate-100">
              <Icon name="payments" className="text-primary" />
              Thanh toán
            </h3>
            <div className="p-3 bg-slate-50 dark:bg-slate-800 rounded-lg flex items-center gap-3">
              <Icon name={order.payment.icon} className="text-slate-400 text-xl" />
              <div>
                <p className="text-sm font-bold text-slate-800 dark:text-slate-200">{order.payment.method}</p>
                <p
                  className={`text-xs font-medium ${
                    order.payment.statusSuccess ? 'text-green-600 dark:text-green-400' : 'text-slate-500'
                  }`}
                >
                  {order.payment.status}
                </p>
              </div>
            </div>
          </div>

          <div className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm">
            <h3 className="text-base font-bold mb-4 text-slate-800 dark:text-slate-100">
              Tóm tắt đơn hàng
            </h3>
            <div className="space-y-3 pb-4 border-b border-slate-100 dark:border-slate-800">
              <div className="flex justify-between">
                <span className="text-slate-500">Tạm tính</span>
                <span className="text-slate-800 dark:text-slate-200 font-medium">
                  {formatPrice(order.summary.subtotal)}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-slate-500">Phí vận chuyển</span>
                <span className="text-slate-800 dark:text-slate-200 font-medium">
                  {formatPrice(order.summary.shippingFee)}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-slate-500">Thuế (10% VAT)</span>
                <span className="text-slate-800 dark:text-slate-200 font-medium">
                  {formatPrice(order.summary.vat)}
                </span>
              </div>
            </div>
            <div className="pt-4 flex justify-between items-center mb-6">
              <span className="text-lg font-black text-slate-900 dark:text-slate-50">Tổng cộng</span>
              <span className="text-2xl font-black text-primary">{formatPrice(order.summary.total)}</span>
            </div>
            <button
              type="button"
              className="w-full py-3 bg-slate-100 dark:bg-slate-800 hover:bg-primary/10 hover:text-primary text-slate-700 dark:text-slate-300 font-bold rounded-lg transition-all flex items-center justify-center gap-2"
            >
              <Icon name="support_agent" />
              Liên hệ hỗ trợ
            </button>
          </div>
        </div>
      </div>
    </main>
  )
}
