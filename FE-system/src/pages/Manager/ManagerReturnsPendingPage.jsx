import { ManagerReturnsPage } from "./ManagerReturnsPage";

/**
 * Manager — Đổi trả chờ duyệt (`/manager/after-sales/returns/pending`).
 * Cố định lọc `status=Pending`; xem toàn bộ tại `/manager/after-sales/returns`.
 */
export function ManagerReturnsPendingPage() {
  return <ManagerReturnsPage defaultStatusLocked="Pending" />;
}
