import { useNavigate } from "react-router-dom";
import {
  ProductsPageHeader,
  ProductStatsCards,
  ProductFilters,
  ProductTable,
  ProductPagination,
} from "../../components/Products";
import styles from "./SalerProductsPage.module.css";

export function SalerProductsPage() {
  const navigate = useNavigate();

  const handleSearch = (q) => console.log("Search:", q);
  const handleAddProduct = () => console.log("Add product");
  const handleFilter = (key) => console.log("Filter:", key);
  const handleResetFilters = () => console.log("Reset filters");
  const handleEdit = (row) => console.log("Edit", row);
  const handleView = (row) => navigate(`/saler/products/${row.id}`);
  const handleUpdateStock = (row) => console.log("Update stock", row);
  const handlePageChange = (page) => console.log("Page", page);

  return (
    <div className={styles.wrap}>
      <ProductsPageHeader
        onSearch={handleSearch}
        onAddProduct={handleAddProduct}
      />
      <div className={styles.scrollArea}>
        <section className={styles.stats} aria-label="Thống kê nhanh">
          <ProductStatsCards />
        </section>
        <section className={styles.filters}>
          <ProductFilters onFilter={handleFilter} onReset={handleResetFilters} />
        </section>
        <section className={styles.tableSection}>
          <ProductTable
            onEdit={handleEdit}
            onView={handleView}
            onUpdateStock={handleUpdateStock}
          />
          <ProductPagination
            from={1}
            to={10}
            total={1250}
            currentPage={1}
            totalPages={125}
            onPageChange={handlePageChange}
          />
        </section>
      </div>
    </div>
  );
}
