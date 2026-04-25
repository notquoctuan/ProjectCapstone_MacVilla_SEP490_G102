import { Link } from 'react-router-dom'
import { Icon } from '../../ui/Icon'

function formatPrice(value) {
  const n = Number(value)
  if (value == null || Number.isNaN(n) || n <= 0) {
    return 'Liên hệ'
  }
  return new Intl.NumberFormat('vi-VN').format(n) + 'đ'
}

/**
 * Badge: "Bảo hành chính hãng" (primary) hoặc "Tiết kiệm điện A+++" (green + icon)
 */
function BadgePill({ label }) {
  const isEnergy = label.includes('A+++') || label.includes('Energy')
  return (
    <span
      className={`text-[10px] font-bold px-2 py-1 rounded-full flex items-center gap-1 ${
        isEnergy
          ? 'bg-green-600 text-white'
          : 'bg-primary text-white'
      }`}
    >
      {isEnergy && (
        <Icon name="energy_savings_leaf" className="text-[12px]" />
      )}
      {!isEnergy && <Icon name="verified" className="text-[12px]" />}
      {label}
    </span>
  )
}

export function ProductListingCard({
  productId,
  slug,
  name,
  tag,
  image,
  imageAlt,
  price,
  originalPrice,
  badges = [],
  onCompareChange,
  compareChecked,
}) {
  const detailUrl =
    slug && String(slug).trim()
      ? `/products/${encodeURIComponent(String(slug).trim())}`
      : `/products/${productId ?? ''}`

  return (
    <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm hover:shadow-md transition-shadow group relative">
      <div className="absolute top-3 left-3 z-10">
        <label className="flex items-center gap-1.5 bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm px-2 py-1 rounded-md border border-slate-200 dark:border-slate-700 cursor-pointer shadow-sm">
          <input
            type="checkbox"
            checked={compareChecked ?? false}
            onChange={(e) => onCompareChange?.(e.target.checked)}
            className="w-3.5 h-3.5 rounded border-slate-300 text-primary focus:ring-primary"
          />
          <span className="text-[10px] font-bold uppercase">So sánh</span>
        </label>
      </div>
      <Link to={detailUrl} className="block aspect-square bg-slate-100 dark:bg-slate-800 relative p-8">
        <img
          src={image}
          alt={imageAlt}
          className="w-full h-full object-contain mix-blend-multiply dark:mix-blend-normal"
        />
        <div className="absolute top-3 right-3 flex flex-col gap-1">
          {badges.map((label) => (
            <BadgePill key={label} label={label} />
          ))}
        </div>
      </Link>
      <div className="p-4">
        <div className="mb-2">
          <span className="text-[10px] font-bold text-primary uppercase tracking-wider bg-primary/10 px-2 py-0.5 rounded">
            {tag}
          </span>
        </div>
        <Link to={detailUrl}>
          <h3 className="font-bold text-slate-900 dark:text-slate-100 line-clamp-2 mb-2 group-hover:text-primary transition-colors">
            {name}
          </h3>
        </Link>
        <div className="flex items-center gap-2 mb-4">
          <span className="text-lg font-black text-red-600">
            {formatPrice(price)}
          </span>
          {originalPrice != null && (
            <span className="text-sm text-slate-400 line-through">
              {formatPrice(originalPrice)}
            </span>
          )}
        </div>
        <div className="flex items-center gap-2 border-t border-slate-100 dark:border-slate-800 pt-4 mt-auto">
          <button
            type="button"
            className="flex-1 bg-primary text-white py-2 rounded-lg text-sm font-bold flex items-center justify-center gap-2 hover:bg-primary/90 transition-colors"
          >
            <Icon name="add_shopping_cart" className="text-sm" />
            Mua ngay
          </button>
          <button
            type="button"
            className="p-2 border border-slate-200 dark:border-slate-700 rounded-lg text-slate-400 hover:text-red-500 hover:border-red-500 transition-colors"
            aria-label="Thêm vào yêu thích"
          >
            <Icon name="favorite" />
          </button>
        </div>
      </div>
    </div>
  )
}
