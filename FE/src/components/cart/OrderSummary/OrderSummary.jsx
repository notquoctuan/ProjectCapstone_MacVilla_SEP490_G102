import { Link } from 'react-router-dom'
import { Icon } from '../../ui/Icon'

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

export function OrderSummary({
  itemCount = 0,
  subtotal = 0,
  shipping = 0,
  shippingLabel = 'Miễn phí',
  couponDiscount = 0,
  total = 0,
  cartItems,
}) {
  return (
    <div className="bg-white dark:bg-slate-900 rounded-xl shadow-sm border border-slate-100 dark:border-slate-800 sticky top-24">
      <div className="p-4 border-b border-slate-100 dark:border-slate-800">
        <h3 className="font-bold text-lg">Tóm tắt đơn hàng</h3>
      </div>
      <div className="p-4 space-y-3">
        <div className="flex justify-between text-sm">
          <span className="text-slate-500">
            Tạm tính ({itemCount} sản phẩm)
          </span>
          <span className="font-semibold text-slate-800 dark:text-slate-100">
            {formatPrice(subtotal)}
          </span>
        </div>
        <div className="flex justify-between text-sm">
          <span className="text-slate-500">Phí vận chuyển</span>
          <span className="text-emerald-600 font-medium">{shippingLabel}</span>
        </div>
        <div className="flex justify-between text-sm">
          <span className="text-slate-500">Giảm giá mã coupon</span>
          <span className="text-slate-800 dark:text-slate-100">
            {formatPrice(couponDiscount)}
          </span>
        </div>
        <div className="pt-3 border-t border-dashed border-slate-200 dark:border-slate-700 flex justify-between items-end">
          <span className="font-bold text-slate-800 dark:text-slate-100 uppercase">
            Tổng tiền
          </span>
          <div className="text-right">
            <p className="text-accent-red text-2xl font-bold">
              {formatPrice(total)}
            </p>
            <p className="text-[10px] text-slate-400 font-medium">
              (Đã bao gồm VAT nếu có)
            </p>
          </div>
        </div>
        <Link
          to={{
            pathname: '/checkout',
            state: cartItems != null ? { cartItems } : undefined,
          }}
          className="w-full bg-primary text-white font-bold py-4 rounded-xl shadow-lg shadow-primary/30 hover:bg-primary/90 transition-all flex flex-col items-center justify-center gap-0 mt-4 block text-center"
        >
          <span className="text-lg">TIẾP TỤC ĐẶT HÀNG</span>
          <span className="text-xs opacity-80 font-normal">
            Nhanh chóng trong 1 phút
          </span>
        </Link>
      </div>
      <div className="p-4 bg-slate-50 dark:bg-slate-800/50 rounded-b-xl">
        <p className="text-[11px] text-slate-500 text-center flex items-center justify-center gap-1">
          <Icon name="verified_user" className="text-[14px]" />
          Cam kết hàng chính hãng 100%
        </p>
      </div>
    </div>
  )
}
