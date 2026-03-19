import { Icon } from '../../ui/Icon'

export function StoreServices({ items = [] }) {
  return (
    <section className="bg-white dark:bg-slate-900 rounded-xl p-6 shadow-sm">
      <h3 className="text-lg font-bold mb-4">Dịch vụ cửa hàng</h3>
      <div className="space-y-4">
        {items.map((item) => (
          <div key={item.title} className="flex items-start gap-3">
            <Icon
              name={item.icon}
              className={`${item.iconColor ?? 'text-primary'} mt-0.5`}
            />
            <div>
              <p className="text-sm font-bold">{item.title}</p>
              <p className="text-xs text-slate-500">{item.subtitle}</p>
            </div>
          </div>
        ))}
      </div>
    </section>
  )
}
