import { useSearchParams } from "react-router-dom";
import { AdminInventoryTransactionsPage } from "@/pages/Admin/AdminInventoryTransactionsPage";

/**
 * Stock Manager — Giao dịch kho (`/stock-manager/inventory/transactions`).
 */
export function StockManagerInventoryTransactionsPage() {
  const [sp] = useSearchParams();
  const variantFromQuery = (sp.get("variantId") ?? "").trim();
  return <AdminInventoryTransactionsPage initialVariantFilter={variantFromQuery} />;
}
