import { useParams, useNavigate } from "react-router-dom";
import {
  ProductDetailHeader,
  ProductGallery,
  ProductDescriptionSpecs,
  ProductPriceHistory,
  ProductRecentOrders,
  ProductPriceCard,
  ProductInventoryCard,
  ProductBrandCard,
} from "../../components/ProductDetail";
import styles from "./SalerProductDetailPage.module.css";

export function SalerProductDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const handleEdit = () => console.log("Edit product");
  const handleCreateQuote = () => console.log("Create quote");
  const handleViewAllSpecs = () => console.log("View all specs");
  const handlePdfClick = (pdf) => console.log("PDF", pdf);
  const handleSupport = () => console.log("Support");

  return (
    <div className={styles.wrap}>
      <ProductDetailHeader
        productName="Vòi sen nhiệt độ Bosch Serie 6"
        productCode="BOS-TH-006"
        sku="88201293"
        listHref="/saler/products"
        onEdit={handleEdit}
        onCreateQuote={handleCreateQuote}
      />
      <div className={styles.body}>
        <div className={styles.main}>
          <section className={styles.section}>
            <ProductGallery badge="Premium" />
          </section>
          <section className={styles.section}>
            <ProductDescriptionSpecs
              onViewAll={handleViewAllSpecs}
              onPdfClick={handlePdfClick}
            />
          </section>
          <section className={styles.section}>
            <ProductPriceHistory />
          </section>
          <section className={styles.section}>
            <ProductRecentOrders />
          </section>
        </div>
        <aside className={styles.sidebar}>
          <div className={styles.sidebarCard}>
            <ProductPriceCard />
          </div>
          <div className={styles.sidebarCard}>
            <ProductInventoryCard />
          </div>
          <div className={styles.sidebarCard}>
            <ProductBrandCard onSupport={handleSupport} />
          </div>
        </aside>
      </div>
    </div>
  );
}
