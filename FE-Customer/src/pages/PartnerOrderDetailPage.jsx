import { Link, useParams } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { getB2BOrderDetail } from '../data/b2bDashboard'

export function PartnerOrderDetailPage() {
  const { orderId } = useParams()
  const order = getB2BOrderDetail(orderId)
  const { project, financial, deliveries, deliveryStats } = order

  return (
    <>
      <div className="p-8">
        {/* Header */}
        <div className="flex flex-wrap justify-between items-start gap-6 mb-8">
          <div>
            <nav className="flex text-slate-500 text-sm mb-2 gap-2 items-center">
              <Link to="/partner/orders" className="hover:text-primary transition-colors">
                Đơn hàng
              </Link>
              <Icon name="chevron_right" className="text-xs" />
              <span className="text-slate-900 dark:text-slate-100 font-medium">#{order.id}</span>
            </nav>
            <h2 className="text-3xl font-black text-slate-900 dark:text-white tracking-tight">
              Chi tiết đơn hàng #{order.id}
            </h2>
            <div className="flex items-center gap-4 mt-2 flex-wrap">
              <p className="text-slate-500 dark:text-slate-400">
                Ngày tạo: <span className="text-slate-900 dark:text-slate-200 font-medium">{order.createdAt}</span>
              </p>
              <span className="h-1 w-1 bg-slate-300 rounded-full" />
              <span className={`px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider ${order.statusClass}`}>
                {order.status}
              </span>
            </div>
          </div>
          <div className="flex gap-3">
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg text-slate-700 dark:text-slate-200 text-sm font-bold hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
            >
              <Icon name="download" className="text-lg" />
              Xuất hóa đơn PDF
            </button>
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2 bg-primary text-white rounded-lg text-sm font-bold hover:bg-primary/90 transition-colors"
            >
              <Icon name="edit" className="text-lg" />
              Chỉnh sửa
            </button>
          </div>
        </div>

        {/* Project + Financial grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
          <div className="lg:col-span-2 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
            <div className="p-6 border-b border-slate-100 dark:border-slate-800 flex items-center justify-between">
              <h3 className="text-lg font-bold flex items-center gap-2 text-slate-900 dark:text-white">
                <Icon name="info" className="text-primary" />
                Thông tin chung dự án
              </h3>
            </div>
            <div className="p-6 grid grid-cols-1 sm:grid-cols-2 gap-y-6 gap-x-8">
              <div>
                <p className="text-slate-400 text-xs font-semibold uppercase tracking-wider mb-1">Tên dự án</p>
                <p className="text-slate-900 dark:text-white font-medium">{project.name}</p>
              </div>
              <div>
                <p className="text-slate-400 text-xs font-semibold uppercase tracking-wider mb-1">Chủ đầu tư</p>
                <p className="text-slate-900 dark:text-white font-medium">{project.owner}</p>
              </div>
              <div>
                <p className="text-slate-400 text-xs font-semibold uppercase tracking-wider mb-1">Địa chỉ công trình</p>
                <p className="text-slate-900 dark:text-white font-medium">{project.address}</p>
              </div>
              <div>
                <p className="text-slate-400 text-xs font-semibold uppercase tracking-wider mb-1">Người phụ trách</p>
                <p className="text-slate-900 dark:text-white font-medium">{project.contact}</p>
              </div>
            </div>
          </div>
          <div className="bg-primary text-white rounded-xl p-6 shadow-lg shadow-primary/20 flex flex-col justify-between">
            <div>
              <h3 className="text-lg font-bold mb-6 flex items-center gap-2">
                <Icon name="payments" className="text-white" />
                Tóm tắt tài chính
              </h3>
              <div className="space-y-3">
                <div className="flex justify-between text-white/80 text-sm">
                  <span>Tạm tính (MSRP):</span>
                  <span className="font-medium">{financial.subtotal}</span>
                </div>
                <div className="flex justify-between text-white/80 text-sm">
                  <span>{financial.discountLabel}</span>
                  <span className="font-medium text-emerald-300">{financial.discount}</span>
                </div>
                <div className="flex justify-between text-white/80 text-sm">
                  <span>Thuế VAT (10%):</span>
                  <span className="font-medium">{financial.vat}</span>
                </div>
              </div>
            </div>
            <div className="pt-6 border-t border-white/20 mt-6">
              <div className="flex justify-between items-end">
                <span className="text-sm font-medium">TỔNG CỘNG</span>
                <span className="text-2xl font-black">{financial.total}</span>
              </div>
            </div>
          </div>
        </div>

        {/* Partial Deliveries */}
        <div className="mb-8">
          <div className="flex flex-wrap items-center justify-between gap-4 mb-6">
            <h3 className="text-xl font-bold flex items-center gap-2 text-slate-900 dark:text-white">
              <Icon name="local_shipping" className="text-primary" />
              Theo dõi các đợt giao hàng (Partial Deliveries)
            </h3>
            <div className="flex gap-2">
              <span className="flex items-center gap-1 text-xs font-medium px-2 py-1 bg-slate-100 dark:bg-slate-800 rounded text-slate-700 dark:text-slate-200">
                <span className="w-2 h-2 rounded-full bg-emerald-500" /> Hoàn thành: {deliveryStats.completed}
              </span>
              <span className="flex items-center gap-1 text-xs font-medium px-2 py-1 bg-slate-100 dark:bg-slate-800 rounded text-slate-700 dark:text-slate-200">
                <span className="w-2 h-2 rounded-full bg-blue-500" /> Đang xử lý: {deliveryStats.processing}
              </span>
            </div>
          </div>
          <div className="space-y-6">
            {deliveries.map((d) => (
              <div
                key={d.id}
                className={`bg-white dark:bg-slate-900 rounded-xl border-l-4 ${d.borderClass} border border-slate-200 dark:border-slate-800 shadow-sm overflow-hidden ${d.items.length === 0 ? 'opacity-75 grayscale hover:grayscale-0 hover:opacity-100 transition-all' : ''}`}
              >
                <div className={`p-5 flex flex-wrap items-center justify-between gap-4 ${d.headerClass}`}>
                  <div className="flex items-center gap-4">
                    <div className={`w-10 h-10 rounded-full flex items-center justify-center ${d.iconClass}`}>
                      <Icon name={d.icon} className="text-xl" />
                    </div>
                    <div>
                      <h4 className="font-bold text-slate-900 dark:text-white">{d.title}</h4>
                      <p className="text-xs text-slate-500">{d.date}</p>
                    </div>
                  </div>
                  <div className="flex gap-8">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Mã vận đơn</p>
                      <p className={`text-sm font-mono font-semibold ${d.trackingCode === 'Chưa cập nhật' ? 'text-slate-400' : 'text-primary'}`}>
                        {d.trackingCode}
                      </p>
                    </div>
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Trạng thái</p>
                      <span className={`text-xs font-bold ${d.statusClass}`}>{d.status}</span>
                    </div>
                  </div>
                </div>
                {d.items.length > 0 && (
                  <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                      <thead className="bg-slate-50 dark:bg-slate-800/50">
                        <tr>
                          <th className="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Sản phẩm</th>
                          <th className="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-center">Số lượng</th>
                          <th className="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-right">Đơn giá</th>
                          <th className="px-6 py-3 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-right">Thành tiền</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                        {d.items.map((item, i) => (
                          <tr key={i}>
                            <td className="px-6 py-4">
                              <div className="flex items-center gap-3">
                                <div className="w-12 h-12 bg-slate-100 dark:bg-slate-800 rounded overflow-hidden shrink-0">
                                  <img src={item.image} alt="" className="w-full h-full object-cover" />
                                </div>
                                <div>
                                  <p className="text-sm font-semibold text-slate-900 dark:text-white">{item.name}</p>
                                  <p className="text-xs text-slate-500">{item.spec}</p>
                                </div>
                              </div>
                            </td>
                            <td className="px-6 py-4 text-sm text-center font-medium text-slate-700 dark:text-slate-200">{item.qty}</td>
                            <td className="px-6 py-4 text-sm text-right text-slate-700 dark:text-slate-200">{item.unitPrice}</td>
                            <td className="px-6 py-4 text-sm text-right font-bold text-slate-900 dark:text-white">{item.total}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Footer support */}
        <div className="bg-blue-50 dark:bg-blue-900/10 p-6 rounded-xl border border-blue-100 dark:border-blue-900/30">
          <div className="flex gap-4">
            <Icon name="contact_support" className="text-primary text-2xl shrink-0" />
            <div>
              <h4 className="text-primary font-bold text-sm mb-1">Cần hỗ trợ về đơn hàng này?</h4>
              <p className="text-slate-600 dark:text-slate-400 text-sm">
                Quý khách vui lòng liên hệ trực tiếp với bộ phận chăm sóc khách hàng B2B qua số hotline <strong>1900 8888</strong> hoặc nhắn tin cho quản lý dự án qua cổng thông tin nội bộ.
              </p>
            </div>
          </div>
        </div>
      </div>
    </>
  )
}
