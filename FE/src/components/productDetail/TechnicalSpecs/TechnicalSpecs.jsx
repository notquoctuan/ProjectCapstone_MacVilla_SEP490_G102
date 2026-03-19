export function TechnicalSpecs({ items = [] }) {
  return (
    <section className="bg-white dark:bg-slate-900 rounded-xl p-6 shadow-sm">
      <h3 className="text-lg font-bold mb-4">Thông số kỹ thuật</h3>
      <div className="space-y-3">
        {items.map((item, index) => (
          <div
            key={item.label}
            className={`flex justify-between py-2 ${
              index < items.length - 1
                ? 'border-b border-slate-100 dark:border-slate-800'
                : ''
            }`}
          >
            <span className="text-sm text-slate-500">{item.label}</span>
            <span className="text-sm font-medium">{item.value}</span>
          </div>
        ))}
      </div>
      <button
        type="button"
        className="w-full mt-4 py-2 text-primary font-bold text-sm border border-primary/20 rounded-lg hover:bg-primary/5"
      >
        Xem tất cả thông số
      </button>
    </section>
  )
}
