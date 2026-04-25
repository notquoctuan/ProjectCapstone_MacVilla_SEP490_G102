import { useSearchParams } from "react-router-dom";
import { AdminInventoryTransactionsPage } from "@/pages/Admin/AdminInventoryTransactionsPage";

/**
 * Manager — Giao dịch kho (`/manager/logistics/stock-movements`).
 * Dùng chung UI Admin; hỗ trợ `?variantId=` từ màn Tồn kho.
 */
export function ManagerInventoryTransactionsPage() {
  const [sp] = useSearchParams();
  const variantFromQuery = (sp.get("variantId") ?? "").trim();
  return <AdminInventoryTransactionsPage initialVariantFilter={variantFromQuery} />;
}
