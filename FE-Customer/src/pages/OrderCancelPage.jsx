import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'
import { CartStepper } from '../components/cart'

export function OrderCancelPage() {
  return (
    <main className="max-w-xl mx-auto px-4 py-10 sm:py-16">
      <CartStepper currentStep={2} />
      <div className="mt-8 rounded-2xl border border-amber-200 dark:border-amber-900/50 bg-amber-50/80 dark:bg-amber-950/20 p-8 sm:p-10 text-center">
        <div className="inline-flex items-center justify-center size-16 rounded-full bg-amber-100 dark:bg-amber-900/40 text-amber-700 dark:text-amber-400 mb-6">
          <Icon name="payments" className="text-4xl" />
        </div>
        <h1 className="text-xl sm:text-2xl font-bold text-slate-900 dark:text-white mb-3">
          Thanh toán chưa hoàn tất
        </h1>
        <p className="text-sm text-slate-600 dark:text-slate-400 mb-8 leading-relaxed">
          Bạn đã hủy hoặc chưa hoàn tất giao dịch trên PayOS. Đơn hàng có thể
          vẫn ở trạng thái chờ thanh toán — bạn có thể thử lại từ giỏ hàng hoặc
          mục đơn hàng.
        </p>
        <div className="flex flex-col sm:flex-row gap-3 justify-center">
          <Link
            to="/cart"
            className="inline-flex justify-center items-center rounded-xl bg-primary text-white font-bold px-6 py-3 hover:bg-primary/90"
          >
            Về giỏ hàng
          </Link>
          <Link
            to="/account/orders"
            className="inline-flex justify-center items-center rounded-xl border border-slate-300 dark:border-slate-600 font-semibold px-6 py-3 hover:bg-white/60 dark:hover:bg-slate-800"
          >
            Đơn hàng của tôi
          </Link>
        </div>
      </div>
    </main>
  )
}
