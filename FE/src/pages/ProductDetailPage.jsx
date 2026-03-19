import { useParams } from 'react-router-dom'
import { Breadcrumbs } from '../components/layout/Breadcrumbs'
import {
  ProductGallery,
  ProductInfo,
  ComboPromo,
  ReviewsSection,
  QASection,
  TechnicalSpecs,
  StoreServices,
} from '../components/productDetail'
import {
  PRODUCT_DETAIL_BREADCRUMBS,
  PRODUCT_DETAIL,
} from '../data/productDetail'

export function ProductDetailPage() {
  const { id } = useParams()
  // In real app, fetch product by id; for now use mock
  const product = PRODUCT_DETAIL

  return (
    <main className="max-w-7xl mx-auto w-full px-4 pb-20">
      <Breadcrumbs items={PRODUCT_DETAIL_BREADCRUMBS} />
      <div className="bg-white dark:bg-slate-900 rounded-xl p-6 shadow-sm mb-6">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
          <ProductGallery
            mainImage={product.mainImage}
            mainImageAlt={product.mainImageAlt}
            thumbnails={product.thumbnails}
            moreThumbsCount={product.moreThumbsCount}
          />
          <ProductInfo
            name={product.name}
            rating={product.rating}
            reviewCount={product.reviewCount}
            inStock={product.inStock}
            price={product.price}
            originalPrice={product.originalPrice}
            discountPercent={product.discountPercent}
            highlights={product.highlights}
          />
        </div>
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          <ComboPromo items={product.comboOffers} />
          <section className="bg-white dark:bg-slate-900 rounded-xl p-6 shadow-sm">
            <ReviewsSection reviews={product.reviews} />
            <QASection items={product.qa} />
          </section>
        </div>
        <div className="lg:col-span-1 space-y-6">
          <TechnicalSpecs items={product.specs} />
          <StoreServices items={product.services} />
        </div>
      </div>
    </main>
  )
}
