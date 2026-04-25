/**
 * Sidebar theo tài liệu `dev/Documents/UI/*.md` (không gồm customer B2C/B2B).
 * Icon: Material SymbolsOutlined (chuỗi tên icon).
 */
export type ActorNavItem = {
  to: string;
  /** NavLink `end` — chỉ khớp đúng path */
  end?: boolean;
  icon: string;
  label: string;
  /** Ghi đè trạng thái active (tránh prefix trùng như /orders vs /orders/create) */
  isActive?: (pathname: string) => boolean;
};

/** Nhóm mục trong sidebar (ví dụ Manager). */
export type ActorNavGroup = {
  type: "group";
  title: string;
  items: ActorNavItem[];
};

export type ActorNavEntry = ActorNavItem | ActorNavGroup;

export function isActorNavGroup(entry: ActorNavEntry): entry is ActorNavGroup {
  return (entry as ActorNavGroup).type === "group";
}

function salerQuotationsAllActive(pathname: string) {
  const parts = pathname.split("/").filter(Boolean);
  if (parts[0] !== "saler" || parts[1] !== "quotations") return false;
  const reserved = new Set(["queue", "mine", "create"]);
  if (parts.length === 2) return true;
  if (parts.length === 3 && !reserved.has(parts[2])) return true;
  return false;
}

function salerOrdersListActive(pathname: string) {
  const parts = pathname.split("/").filter(Boolean);
  if (parts[0] !== "saler" || parts[1] !== "orders") return false;
  if (parts[2] === "create") return false;
  if (parts.length === 2) return true;
  return parts.length === 3;
}

function salerOrdersCreateActive(pathname: string) {
  return pathname === "/saler/orders/create" || pathname.startsWith("/saler/orders/create/");
}

/** List + chi tiết hợp đồng dưới `/saler/contracts`. */
function salerContractsNavActive(pathname: string) {
  return pathname === "/saler/contracts" || pathname.startsWith("/saler/contracts/");
}

function managerWarrantyTicketsNavActive(pathname: string) {
  if (pathname === "/manager/after-sales/warranty" || pathname === "/manager/after-sales/warranty/") return true;
  return /^\/manager\/after-sales\/warranty\/\d+$/.test(pathname);
}

function managerQuotationsAllActive(pathname: string) {
  const parts = pathname.split("/").filter(Boolean);
  if (parts[0] !== "manager" || parts[1] !== "sales" || parts[2] !== "quotations") return false;
  if (parts.length === 3) return true;
  if (parts.length === 4 && parts[3] === "pending") return false;
  if (parts.length === 4) return true;
  return false;
}

function stockFulfillmentsHubActive(pathname: string) {
  if (!pathname.startsWith("/stock-manager/fulfillments")) return false;
  if (pathname.startsWith("/stock-manager/fulfillments/create")) return false;
  return true;
}

/**
 * Sales — sidebar gọn theo luồng nghiệp vụ (bán hàng → kế toán xem → hậu mãi → báo cáo).
 * Route thực tế khớp `App.jsx` `/saler/*`.
 */
export const SALER_NAV_ITEMS: ActorNavEntry[] = [
  { to: "/saler", end: true, icon: "dashboard", label: "Dashboard" },
  {
    type: "group",
    title: "Bán hàng",
    items: [
      { to: "/saler/quotations/queue", end: true, icon: "inbox", label: "Chờ tiếp nhận BG" },
      { to: "/saler/quotations/mine", end: true, icon: "person", label: "Báo giá của tôi" },
      {
        to: "/saler/quotations",
        end: false,
        icon: "description",
        label: "Tất cả báo giá",
        isActive: salerQuotationsAllActive,
      },
      { to: "/saler/quotations/create", end: true, icon: "add_circle", label: "Tạo báo giá" },
      {
        to: "/saler/contracts",
        end: false,
        icon: "article",
        label: "Hợp đồng",
        isActive: salerContractsNavActive,
      },
      {
        to: "/saler/orders",
        end: false,
        icon: "shopping_cart",
        label: "Đơn hàng",
        isActive: salerOrdersListActive,
      },
      {
        to: "/saler/orders/create",
        end: true,
        icon: "post_add",
        label: "Tạo đơn hộ khách",
        isActive: salerOrdersCreateActive,
      },
      { to: "/saler/customers", end: false, icon: "groups", label: "Khách hàng" },
    ],
  },
  {
    type: "group",
    title: "Kế toán (xem)",
    items: [
      { to: "/saler/invoices", end: false, icon: "receipt_long", label: "Hóa đơn" },
      { to: "/saler/payments", end: false, icon: "payments", label: "Thanh toán" },
      { to: "/saler/transfer-notifications", end: false, icon: "account_balance", label: "TB chuyển khoản" },
    ],
  },
  {
    type: "group",
    title: "Hậu mãi",
    items: [
      { to: "/saler/warranty", end: false, icon: "verified_user", label: "Bảo hành" },
      { to: "/saler/returns", end: false, icon: "assignment_return", label: "Đổi / trả" },
    ],
  },
  {
    type: "group",
    title: "Báo cáo & kho tư vấn",
    items: [
      { to: "/saler/revenue", end: false, icon: "trending_up", label: "Doanh thu" },
      { to: "/saler/products", end: false, icon: "inventory_2", label: "Sản phẩm" },
      { to: "/saler/warehouse", end: true, icon: "warehouse", label: "Tra cứu kho" },
    ],
  },
];

/** Manager — gom nhóm sidebar gọn (`manager.md` + kế toán / đối soát CK). */
export const MANAGER_NAV_ITEMS: ActorNavEntry[] = [
  { to: "/manager", end: true, icon: "dashboard", label: "Dashboard" },
  {
    type: "group",
    title: "Bán hàng",
    items: [
      { to: "/manager/sales/quotations/pending", end: false, icon: "rule", label: "Chờ duyệt báo giá" },
      {
        to: "/manager/sales/quotations",
        end: false,
        icon: "description",
        label: "Tất cả báo giá",
        isActive: managerQuotationsAllActive,
      },
      { to: "/manager/sales/contracts", end: false, icon: "article", label: "Hợp đồng" },
      { to: "/manager/sales/orders", end: false, icon: "shopping_cart", label: "Đơn hàng" },
      { to: "/manager/sales/customers", end: false, icon: "groups", label: "Khách hàng" },
    ],
  },
  {
    type: "group",
    title: "Kế toán",
    items: [
      { to: "/manager/accounting/invoices", end: false, icon: "receipt_long", label: "Hóa đơn" },
      { to: "/manager/accounting/payments", end: false, icon: "payments", label: "Thanh toán" },
      { to: "/manager/accounting/transfer-notifications", end: false, icon: "account_balance", label: "Đối soát CK" },
    ],
  },
  {
    type: "group",
    title: "Kho & giao hàng",
    items: [
      { to: "/manager/logistics/fulfillments", end: false, icon: "local_shipping", label: "Fulfillment" },
      { to: "/manager/logistics/inventory", end: false, icon: "inventory", label: "Tồn kho" },
      { to: "/manager/logistics/stock-movements", end: false, icon: "swap_horiz", label: "Xuất nhập kho" },
    ],
  },
  {
    type: "group",
    title: "Hậu mãi",
    items: [
      { to: "/manager/after-sales/returns", end: false, icon: "assignment_return", label: "Đổi trả" },
      { to: "/manager/after-sales/warranty/pending", end: false, icon: "hourglass_empty", label: "BH chờ xử lý" },
      { to: "/manager/after-sales/warranty/claims-queue", end: true, icon: "list_alt", label: "Claim đang xử lý" },
      {
        to: "/manager/after-sales/warranty",
        end: false,
        icon: "shield",
        label: "Phiếu bảo hành",
        isActive: managerWarrantyTicketsNavActive,
      },
    ],
  },
];

/**
 * Stock Manager — sidebar gọn theo nhóm: xuất kho → tồn & giao dịch → đơn/hậu mãi.
 * (URL `/inventory/adjustments`, `/inventory/warehouse`, `/inventory/sku-lookup` chuyển về `/stock-manager/inventory`; không lặp trong menu.)
 */
export const STOCK_MANAGER_NAV_ITEMS: ActorNavEntry[] = [
  { to: "/stock-manager", end: true, icon: "home", label: "Trang chủ kho" },
  {
    type: "group",
    title: "Xuất kho",
    items: [
      {
        to: "/stock-manager/fulfillments",
        end: false,
        icon: "local_shipping",
        label: "Danh sách phiếu",
        isActive: stockFulfillmentsHubActive,
      },
      { to: "/stock-manager/fulfillments/create", end: false, icon: "add_box", label: "Tạo phiếu" },
    ],
  },
  {
    type: "group",
    title: "Tồn kho",
    items: [
      { to: "/stock-manager/inventory", end: true, icon: "inventory_2", label: "Tồn kho & cảnh báo" },
      { to: "/stock-manager/inventory/transactions", end: false, icon: "swap_horiz", label: "Giao dịch kho" },
    ],
  },
  {
    type: "group",
    title: "Đơn & đổi trả",
    items: [
      { to: "/stock-manager/orders", end: false, icon: "shopping_cart", label: "Đơn hàng" },
      { to: "/stock-manager/returns", end: false, icon: "assignment_return", label: "Đổi / trả" },
    ],
  },
];

/** Worker — theo `dev/Documents/UI/worker.md` */
export const WORKER_NAV_ITEMS: ActorNavItem[] = [
  { to: "/worker", end: true, icon: "task_alt", label: "Việc của tôi" },
  { to: "/worker/fulfillments", end: false, icon: "local_shipping", label: "Phiếu được giao" },
  { to: "/worker/inventory/sku-lookup", end: false, icon: "search", label: "Tra SKU" },
];
