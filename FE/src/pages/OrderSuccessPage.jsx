import { useLocation } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import {
  OrderSuccessProgress,
  OrderSuccessSummary,
  OrderSuccessActions,
  OrderSuccessYouMayLike,
  OrderSuccessSupport,
} from '../components/checkout'

function generateOrderId() {
  return '#HDG' + String(Math.floor(100000 + Math.random() * 900000))
}

export function OrderSuccessPage() {
  const location = useLocation()
  const state = location.state || {}
  const orderId = state.orderId || generateOrderId()
  const paymentMethod = state.paymentMethod || 'Chuyển khoản ngân hàng'

  return (
    <main className="flex-1 flex flex-col items-center py-8 px-4">
      <div className="w-full max-w-[800px] space-y-8">
        <OrderSuccessProgress />

        <div className="text-center py-6">
          <div className="inline-flex items-center justify-center size-20 rounded-full bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400 mb-6 border-4 border-white dark:border-slate-800 shadow-lg">
            <Icon name="check_circle" className="text-5xl" />
          </div>
          <h1 className="text-slate-900 dark:text-white text-3xl font-bold mb-3">
            Cảm ơn bạn đã mua hàng!
          </h1>
          <p className="text-slate-600 dark:text-slate-400 max-w-md mx-auto leading-relaxed">
            Đơn hàng{' '}
            <span className="font-bold text-slate-900 dark:text-white">
              {orderId}
            </span>{' '}
            đã được đặt thành công. Chúng tôi đã gửi email xác nhận chi tiết
            đến địa chỉ của bạn.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <OrderSuccessSummary
            orderId={orderId}
            paymentMethod={paymentMethod}
            deliveryEstimate="2-3 ngày làm việc"
          />
          <OrderSuccessActions />
        </div>

        <OrderSuccessYouMayLike />
        <OrderSuccessSupport />
      </div>
    </main>
  )
}
