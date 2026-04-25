import {
  DashboardHeader,
  MetricCard,
  RevenueChart,
  QuotationTable,
  WarehouseActivityList,
} from "../../components/Dashboard";
import styles from "./SalerDashboardPage.module.css";

export function SalerDashboardPage() {
  return (
    <div className={styles.wrap}>
      <DashboardHeader />
      <div className={styles.scrollArea}>
        <section className={styles.metricsGrid} aria-label="Metrics">
          <MetricCard
            icon="monetization_on"
            iconVariant="primary"
            badge="+12.5%"
            badgeVariant="positive"
            label="Total Sales Today"
            value="245,000,000"
            valueSuffix="VND"
          />
          <MetricCard
            icon="request_quote"
            iconVariant="orange"
            badge="+3"
            badgeVariant="positive"
            label="Pending Quotations"
            value="12"
          />
          <MetricCard
            icon="shopping_bag"
            iconVariant="blue"
            badge="-5%"
            badgeVariant="negative"
            label="New Orders"
            value="28"
          />
          <MetricCard
            icon="warning"
            iconVariant="red"
            badge="Alert"
            badgeVariant="danger"
            label="Inventory Alerts"
            value="5"
            valueDanger
          />
        </section>

        <section className={styles.chartSection} aria-label="Revenue trend">
          <RevenueChart />
        </section>

        <section className={styles.tablesGrid} aria-label="Recent activity">
          <QuotationTable onCreateQuote={(row) => console.log("Create quote", row)} />
          <WarehouseActivityList onFilter={() => console.log("Filter warehouse")} />
        </section>
      </div>
    </div>
  );
}
