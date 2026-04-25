import {
  RevenuePageHeader,
  RevenueSummaryCard,
  RevenueTrendChart,
  CategoryPieChart,
  TopProductsTable,
} from "../../components/Revenue";
import styles from "./SalerRevenuePage.module.css";

export function SalerRevenuePage() {
  const handlePeriodChange = (key) => {
    console.log("Period changed:", key);
  };

  const handleExport = () => {
    console.log("Export report");
  };

  const handleViewAllProducts = () => {
    console.log("View all products");
  };

  return (
    <div className={styles.wrap}>
      <RevenuePageHeader
        onPeriodChange={handlePeriodChange}
        onExport={handleExport}
      />
      <div className={styles.scrollArea}>
        <section className={styles.summaryGrid} aria-label="Tóm tắt">
          <RevenueSummaryCard
            label="Tổng doanh thu"
            value="2.450.000.000đ"
            changePercent="12.5%"
            trend="up"
          />
          <RevenueSummaryCard
            label="Lợi nhuận gộp"
            value="850.000.000đ"
            changePercent="2.1%"
            trend="down"
          />
          <RevenueSummaryCard
            label="Số đơn hàng thành công"
            value="1.240"
            changePercent="5.3%"
            trend="up"
          />
          <RevenueSummaryCard
            label="Giá trị đơn hàng (AOV)"
            value="1.975.000đ"
            changePercent="8.2%"
            trend="up"
          />
        </section>

        <section className={styles.chartsGrid} aria-label="Biểu đồ">
          <div className={styles.chartMain}>
            <RevenueTrendChart />
          </div>
          <div className={styles.chartSide}>
            <CategoryPieChart />
          </div>
        </section>

        <section className={styles.tableSection} aria-label="Top sản phẩm">
          <TopProductsTable onViewAll={handleViewAllProducts} />
        </section>
      </div>
    </div>
  );
}
