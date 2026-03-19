import { Icon } from '../../ui/Icon'

function formatPrice(value) {
  return `$${Number(value).toLocaleString('en-US', { minimumFractionDigits: 2 })}`
}

function StarRating({ value, size = 'normal' }) {
  const full = Math.floor(value)
  const half = value - full >= 0.5 ? 1 : 0
  const empty = 5 - full - half
  const scaleClass = size === 'small' ? 'scale-75 origin-left' : ''

  return (
    <div className={`flex text-yellow-500 ${scaleClass}`}>
      {Array.from({ length: full }, (_, i) => (
        <Icon key={`f-${i}`} name="star" className="fill-1" />
      ))}
      {half ? (
        <Icon name="star_half" />
      ) : null}
      {Array.from({ length: empty }, (_, i) => (
        <Icon key={`e-${i}`} name="star" />
      ))}
    </div>
  )
}

export function ProductInfo({
  name,
  rating,
  reviewCount,
  inStock,
  price,
  originalPrice,
  discountPercent,
  highlights = [],
}) {
  return (
    <div className="flex flex-col">
      <h1 className="text-2xl md:text-3xl font-bold leading-tight mb-4">
        {name}
      </h1>
      <div className="flex items-center gap-4 mb-6">
        <div className="flex items-center text-yellow-500">
          <StarRating value={rating} />
          <span className="ml-2 text-sm text-slate-500 font-medium">
            {rating} ({reviewCount} đánh giá)
          </span>
        </div>
        <span className="h-4 w-px bg-slate-300" />
        <span className="text-sm text-green-600 font-bold uppercase tracking-wider">
          {inStock ? 'Còn hàng' : 'Hết hàng'}
        </span>
      </div>
      <div className="bg-slate-50 dark:bg-slate-800 rounded-xl p-4 mb-6">
        <div className="flex items-baseline gap-3">
          <span className="text-3xl font-black text-primary">
            {formatPrice(price)}
          </span>
          {originalPrice != null && (
            <span className="text-lg text-slate-400 line-through">
              {formatPrice(originalPrice)}
            </span>
          )}
          {discountPercent != null && (
            <span className="bg-red-100 text-red-700 text-xs font-bold px-2 py-1 rounded">
              -{discountPercent}%
            </span>
          )}
        </div>
      </div>
      <div className="grid grid-cols-2 gap-3 mb-8">
        {highlights.map((item) => (
          <div
            key={item.title}
            className="flex items-center gap-3 p-3 bg-primary/5 dark:bg-primary/20 border border-primary/20 rounded-lg"
          >
            <Icon name={item.icon} className="text-primary" />
            <div>
              <p className="text-xs font-bold">{item.title}</p>
              <p className="text-[10px] text-slate-500">{item.subtitle}</p>
            </div>
          </div>
        ))}
      </div>
      <div className="flex flex-col gap-3 mt-auto">
        <div className="grid grid-cols-2 gap-4">
          <button
            type="button"
            className="bg-primary hover:bg-primary/90 text-white font-bold py-4 rounded-xl transition-all shadow-lg flex flex-col items-center justify-center"
          >
            <span className="uppercase">Mua ngay</span>
            <span className="text-[10px] font-normal opacity-80">
              Giao hàng & lắp đặt tận nhà miễn phí
            </span>
          </button>
          <button
            type="button"
            className="border-2 border-primary text-primary hover:bg-primary/5 font-bold py-4 rounded-xl transition-all flex flex-col items-center justify-center"
          >
            <Icon name="add_shopping_cart" />
            <span className="text-[10px] font-normal">Thêm vào giỏ</span>
          </button>
        </div>
        <button
          type="button"
          className="w-full bg-slate-900 dark:bg-slate-700 text-white font-bold py-3 rounded-xl flex items-center justify-center gap-2"
        >
          <Icon name="credit_card" />
          Trả góp 0% lãi suất
        </button>
      </div>
    </div>
  )
}
