import {
  OrdersPageHeader,
  OrderKpiCards,
  OrderFilters,
  OrderTable,
  OrderPagination,
} from "../../components/Orders";
import styles from "./SalerOrdersPage.module.css";

export function SalerOrdersPage() {
  const handleSearch = (q) => console.log("Search", q);
  const handleCreateOrder = () => console.log("Create order");
  const handleApplyFilter = (f) => console.log("Apply filter", f);
  const handleRefresh = () => console.log("Refresh");
  const handleEdit = (row) => console.log("Edit", row);
  const handlePrint = (row) => console.log("Print", row);
  const handlePageChange = (page) => console.log("Page", page);

  return (
    <div className={styles.wrap}>
      <OrdersPageHeader
        onSearch={handleSearch}
        onCreateOrder={handleCreateOrder}
      />
      <div className={styles.scrollArea}>
        <OrderKpiCards />
        <OrderFilters onApply={handleApplyFilter} onRefresh={handleRefresh} />
        <section className={styles.tableSection}>
          <OrderTable onEdit={handleEdit} onPrint={handlePrint} />
          <OrderPagination
            from={1}
            to={5}
            total={42}
            currentPage={1}
            totalPages={9}
            onPageChange={handlePageChange}
          />
        </section>
      </div>
    </div>
  );
}
