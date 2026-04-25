import { useParams } from "react-router-dom";
import {
  OrderDetailHeader,
  OrderProgressCard,
  OrderProductTable,
  OrderPaymentCard,
  OrderCustomerCard,
  OrderDeliveryCard,
  OrderNotesHistoryCard,
  OrderProgressProjectCard,
  OrderProductTableB2B,
  OrderDeliveryHistoryCard,
  OrderPaymentDebtCard,
  OrderBusinessInfoCard,
  OrderNotesHistoryB2BCard,
} from "../../components/OrderDetail";
import styles from "./SalerOrderDetailPage.module.css";

const B2B_ORDER_ADDRESS =
  "Kho công trình Zone 3, Dự án Vinhomes Ocean Park 3, Huyện Văn Giang, Tỉnh Hưng Yên.";

function isB2BOrder(orderId) {
  return String(orderId).toUpperCase().includes("B2B");
}

/**
 * Chi tiết đơn hàng — Khách lẻ hoặc Khách doanh nghiệp (B2B).
 * B2B khi orderId chứa "B2B" (vd: ORD-2024-B2B-001).
 */
export function SalerOrderDetailPage() {
  const { id } = useParams();
  const orderId = id ?? "ORD-2024-001";
  const b2b = isB2BOrder(orderId);

  const handlePrint = () => console.log("Print invoice");
  const handleExport = () => console.log("Export warehouse slip");
  const handleCancel = () => console.log("Cancel order");
  const handleConfirm = () => console.log("Confirm order");
  const handleSendNote = (note) => console.log("Send note", note);

  if (b2b) {
    return (
      <div className={styles.wrap}>
        <OrderDetailHeader
          orderId={orderId}
          title={`Chi tiết đơn hàng Doanh nghiệp #${orderId}`}
          listHref="/saler/orders"
          variant="b2b"
          onPrint={handlePrint}
          onExport={handleExport}
          onCancel={handleCancel}
          onConfirm={handleConfirm}
        />
        <div className={styles.body}>
          <div className={styles.b2bSection}>
            <OrderProgressProjectCard projectCode="Vinhomes Ocean Park 3" />
          </div>
          <div className={styles.grid}>
            <div className={styles.main}>
              <OrderProductTableB2B />
              <OrderDeliveryHistoryCard />
              <OrderPaymentDebtCard />
            </div>
            <aside className={styles.sidebar}>
              <OrderBusinessInfoCard />
              <OrderDeliveryCard address={B2B_ORDER_ADDRESS} />
              <OrderNotesHistoryB2BCard onSendNote={handleSendNote} />
            </aside>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.wrap}>
      <OrderDetailHeader
        orderId={orderId}
        title="Chi tiết đơn hàng khách lẻ"
        listHref="/saler/orders"
        onPrint={handlePrint}
        onCancel={handleCancel}
        onConfirm={handleConfirm}
      />
      <div className={styles.body}>
        <div className={styles.grid}>
          <div className={styles.main}>
            <OrderProgressCard />
            <OrderProductTable />
            <OrderPaymentCard />
          </div>
          <aside className={styles.sidebar}>
            <OrderCustomerCard />
            <OrderDeliveryCard />
            <OrderNotesHistoryCard />
          </aside>
        </div>
      </div>
    </div>
  );
}
