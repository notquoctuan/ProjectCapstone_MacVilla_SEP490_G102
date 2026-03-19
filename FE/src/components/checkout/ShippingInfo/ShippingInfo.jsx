import { Icon } from '../../ui/Icon'
import {
  PROVINCES,
  DISTRICTS_PLACEHOLDER,
  WARDS_PLACEHOLDER,
} from '../../../data/checkout'

export function ShippingInfo() {
  return (
    <section className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800">
      <div className="flex items-center gap-2 mb-6 text-primary">
        <Icon name="local_shipping" className="text-2xl" />
        <h2 className="text-xl font-bold tracking-tight">
          Thông tin nhận hàng
        </h2>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="md:col-span-2">
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Họ và tên *
          </label>
          <input
            type="text"
            className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100"
            placeholder="Nhập họ và tên người nhận"
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Số điện thoại *
          </label>
          <input
            type="tel"
            className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100"
            placeholder="0xxx xxx xxx"
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Email (để nhận hóa đơn)
          </label>
          <input
            type="email"
            className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100"
            placeholder="example@gmail.com"
          />
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Tỉnh/Thành phố *
          </label>
          <select className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100">
            {PROVINCES.map((p) => (
              <option key={p} value={p}>
                {p}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Quận/Huyện *
          </label>
          <select className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100">
            <option>{DISTRICTS_PLACEHOLDER}</option>
          </select>
        </div>
        <div>
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Phường/Xã *
          </label>
          <select className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100">
            <option>{WARDS_PLACEHOLDER}</option>
          </select>
        </div>
        <div className="md:col-span-2">
          <label className="block text-sm font-semibold mb-1.5 text-slate-700 dark:text-slate-300">
            Địa chỉ cụ thể *
          </label>
          <input
            type="text"
            className="w-full rounded-lg border border-slate-200 dark:border-slate-700 dark:bg-slate-800 focus:border-primary focus:ring-2 focus:ring-primary/20 p-3 text-slate-900 dark:text-slate-100"
            placeholder="Số nhà, tên đường..."
          />
        </div>
        <div className="md:col-span-2 mt-2">
          <label className="flex items-center gap-3 cursor-pointer group">
            <input
              type="checkbox"
              className="rounded border-slate-300 text-primary focus:ring-primary"
            />
            <span className="text-sm font-medium text-slate-600 dark:text-slate-400 group-hover:text-primary transition-colors">
              Giao hàng đến địa chỉ khác
            </span>
          </label>
        </div>
      </div>
    </section>
  )
}
