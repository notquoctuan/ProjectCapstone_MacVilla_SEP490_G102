import { ManagerWarrantyTicketsPage } from "./ManagerWarrantyTicketsPage";

/**
 * Manager — Phiếu bảo hành chờ xử lý (`/manager/after-sales/warranty/pending`).
 * Cố định lọc `status=Pending` (theo domain BE); danh sách đầy đủ: `/manager/after-sales/warranty`.
 */
export function ManagerWarrantyPendingPage() {
  return <ManagerWarrantyTicketsPage defaultStatusLocked="Pending" />;
}
