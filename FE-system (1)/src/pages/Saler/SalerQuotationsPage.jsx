import { useNavigate } from "react-router-dom";
import {
  QuotationsPageHeader,
  QuotationKpiCards,
  QuotationFilters,
  QuotationTable,
  QuotationPagination,
} from "../../components/Quotations";
import styles from "./SalerQuotationsPage.module.css";

export function SalerQuotationsPage() {
  const navigate = useNavigate();

  const handleSearch = (q) => console.log("Search", q);
  const handleCreateQuotation = () => console.log("Create quotation");
  const handleApplyFilter = (f) => console.log("Apply filter", f);
  const handleResetFilter = () => console.log("Reset filter");
  const handleView = (row) => navigate("/saler/quotations/" + row.id);
  const handleEdit = (row) => console.log("Edit", row);
  const handleDownload = (row) => console.log("Download", row);
  const handlePageChange = (page) => console.log("Page", page);

  return (
    <div className={styles.wrap}>
      <QuotationsPageHeader
        onSearch={handleSearch}
        onCreateQuotation={handleCreateQuotation}
      />
      <div className={styles.scrollArea}>
        <QuotationKpiCards />
        <QuotationFilters onApply={handleApplyFilter} onReset={handleResetFilter} />
        <section className={styles.tableSection}>
          <QuotationTable
            onView={handleView}
            onEdit={handleEdit}
            onDownload={handleDownload}
          />
          <QuotationPagination
            from={1}
            to={5}
            total={24}
            currentPage={1}
            totalPages={5}
            onPageChange={handlePageChange}
          />
        </section>
      </div>
    </div>
  );
}
