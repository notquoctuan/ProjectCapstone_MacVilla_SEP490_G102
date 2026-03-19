import { ProductListingCard } from '../ProductListingCard'

export function ProductGrid({
  products,
  compareIds = new Set(),
  onCompareChange,
}) {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
      {products.map((product) => (
        <ProductListingCard
          key={product.id}
          productId={product.id}
          name={product.name}
          tag={product.tag}
          image={product.image}
          imageAlt={product.imageAlt}
          price={product.price}
          originalPrice={product.originalPrice ?? undefined}
          badges={product.badges}
          compareChecked={compareIds.has(product.id)}
          onCompareChange={(checked) =>
            onCompareChange?.(product.id, checked)
          }
        />
      ))}
    </div>
  )
}
