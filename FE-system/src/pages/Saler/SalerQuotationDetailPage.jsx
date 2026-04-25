import { useParams } from "react-router-dom";
import {
  QuotationDetailHeader,
  QuotationDetailCustomerCard,
  QuotationDetailProductTable,
  QuotationDetailCustomerNoteCard,
  QuotationDetailStatusCard,
  QuotationDetailInternalNotesCard,
  QuotationDetailSummaryCard,
  QuotationDetailHelpCard,
} from "../../components/QuotationDetail";
import styles from "./SalerQuotationDetailPage.module.css";

export function SalerQuotationDetailPage() {
  const { id } = useParams();
  const quotationId = id ?? "QT-2024-001";

  const handlePrint = () => console.log("Print quotation");
  const handleSendToCustomer = () => console.log("Send to customer");
  const handleReject = () => console.log("Reject");
  const handleApprove = () => console.log("Approve");
  const handleEditCustomer = () => console.log("Edit customer");
  const handleSaveNote = (note) => console.log("Save note", note);
  const handleDiscountChange = (productId, percent) => console.log("Discount", productId, percent);

  return (
    <div className={styles.wrap}>
      <QuotationDetailHeader
        quotationId={quotationId}
        listHref="/saler/quotations"
        onPrint={handlePrint}
        onSendToCustomer={handleSendToCustomer}
        onReject={handleReject}
        onApprove={handleApprove}
      />
      <div className={styles.body}>
        <div className={styles.grid}>
          <div className={styles.main}>
            <QuotationDetailCustomerCard onEdit={handleEditCustomer} />
            <QuotationDetailProductTable onDiscountChange={handleDiscountChange} />
            <QuotationDetailCustomerNoteCard />
          </div>
          <aside className={styles.sidebar}>
            <QuotationDetailStatusCard />
            <QuotationDetailInternalNotesCard onSave={handleSaveNote} />
            <QuotationDetailSummaryCard />
            <QuotationDetailHelpCard />
          </aside>
        </div>
      </div>
    </div>
  );
}
