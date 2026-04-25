import { useCallback, useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { ApiRequestError } from "@/services/api/apiEnvelope";
import { fetchAdminWarehouseInventoryPage, fetchAdminWarehouseOverview } from "@/services/admin/adminWarehouseApi";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { ExternalLink, Loader2, PackageSearch, RefreshCw } from "lucide-react";

const fieldInput = cn(
  "h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-foreground shadow-sm transition-all",
  "placeholder:text-slate-400 hover:border-slate-300",
  "focus-visible:border-primary/50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/20",
  "dark:border-slate-700 dark:bg-slate-950 dark:hover:border-slate-600"
);

const fieldCheckbox = "h-4 w-4 rounded border-slate-300 text-primary focus:ring-primary/30";

/**
 * @param {Record<string, unknown>} obj
 * @param {string} camel
 * @param {string} pascal
 */
function pick(obj, camel, pascal) {
  if (!obj || typeof obj !== "object") return undefined;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel];
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal];
  return undefined;
}

/**
 * @param {unknown} data
 */
function asPagedItems(data) {
  if (!data || typeof data !== "object") return { items: [], totalCount: 0, page: 1, pageSize: 50 };
  const o = /** @type {Record<string, unknown>} */ (data);
  const itemsRaw = o.items ?? o.Items;
  const items = Array.isArray(itemsRaw)
    ? itemsRaw.map((x) => (x && typeof x === "object" ? /** @type {Record<string, unknown>} */ (x) : {}))
    : [];
  const totalCount = Number(pick(o, "totalCount", "TotalCount")) || 0;
  const page = Number(pick(o, "page", "Page")) || 1;
  const pageSize = Number(pick(o, "pageSize", "PageSize")) || 50;
  return { items, totalCount, page, pageSize };
}

/**
 * @param {string | number | undefined | null} productId
 * @param {string | number | undefined | null} variantId
 * @param {"admin" | "stock-manager"} linkMode
 */
function variantDetailHref(productId, variantId, linkMode) {
  const p = String(productId ?? "").trim();
  const v = String(variantId ?? "").trim();
  if (!p || !v) return null;
  if (linkMode === "stock-manager") {
    return `/stock-manager/inventory?${new URLSearchParams({ productId: p, variantId: v }).toString()}`;
  }
  return `/admin/products/${encodeURIComponent(p)}/variants/${encodeURIComponent(v)}`;
}

/**
 * Tổng quan tồn, một bảng danh sách (API phân trang `/warehouse/inventory`, W3) và báo cáo quản lý (R1) — `fe_tich_hop_ton_kho_reorder_api_doc.md`.
 * @param {{
 *   variantDetailLinkMode?: "admin" | "stock-manager";
 *   stockMovementsTo?: string;
 * }} [props]
 */
export function AdminWarehouseInventoryPage({
  variantDetailLinkMode = "admin",
  stockMovementsTo = "/admin/logistics/stock-movements",
} = {}) {
  const { accessToken, isAuthenticated } = useAuth();

  const [ovThreshold, setOvThreshold] = useState("10");
  const ovThresholdRef = useRef(ovThreshold);
  ovThresholdRef.current = ovThreshold;
  const [overview, setOverview] = useState(/** @type {Record<string, unknown> | null} */ (null));
  const [ovLoading, setOvLoading] = useState(false);
  const [ovErr, setOvErr] = useState("");

  const [invPage, setInvPage] = useState(1);
  const [invPageSize, setInvPageSize] = useState(50);
  const [invSearch, setInvSearch] = useState("");
  const [invWhLoc, setInvWhLoc] = useState("");
  const invTextFiltersRef = useRef({ search: "", wh: "" });
  invTextFiltersRef.current = { search: invSearch, wh: invWhLoc };
  const [invThreshold, setInvThreshold] = useState("10");
  const invThresholdRef = useRef(invThreshold);
  invThresholdRef.current = invThreshold;
  const [invOnlyOos, setInvOnlyOos] = useState(false);
  /** Mặc định bật: tương đương danh sách tồn thấp (trước đây gọi W2 riêng). */
  const [invOnlyBelow, setInvOnlyBelow] = useState(true);
  const [invItems, setInvItems] = useState(/** @type {Record<string, unknown>[]} */ ([]));
  const [invTotal, setInvTotal] = useState(0);
  const [invMetaPage, setInvMetaPage] = useState(1);
  const [invMetaPageSize, setInvMetaPageSize] = useState(50);
  const [invLoading, setInvLoading] = useState(false);
  const [invErr, setInvErr] = useState("");

  const loadOverview = useCallback(async () => {
    if (!accessToken) return;
    const thr = Math.max(0, Math.floor(Number(ovThresholdRef.current.replace(",", ".")) || 10));
    setOvLoading(true);
    setOvErr("");
    try {
      const data = await fetchAdminWarehouseOverview(accessToken, { lowStockThreshold: thr });
      setOverview(data && typeof data === "object" ? /** @type {Record<string, unknown>} */ (data) : null);
    } catch (e) {
      setOverview(null);
      setOvErr(e instanceof ApiRequestError ? e.message : e instanceof Error ? e.message : "Không tải được tổng quan.");
    } finally {
      setOvLoading(false);
    }
  }, [accessToken]);

  const loadInventory = useCallback(async () => {
    if (!accessToken) return;
    const thr = Math.max(0, Math.floor(Number(invThresholdRef.current.replace(",", ".")) || 10));
    setInvLoading(true);
    setInvErr("");
    try {
      const { search: s, wh: w } = invTextFiltersRef.current;
      const data = await fetchAdminWarehouseInventoryPage(accessToken, {
        page: invPage,
        pageSize: invPageSize,
        search: s.trim() || undefined,
        warehouseLocation: w.trim() || undefined,
        onlyOutOfStock: invOnlyOos,
        onlyBelowThreshold: invOnlyBelow,
        threshold: thr,
      });
      const { items, totalCount, page, pageSize } = asPagedItems(data);
      setInvItems(items);
      setInvTotal(totalCount);
      setInvMetaPage(page);
      setInvMetaPageSize(pageSize);
    } catch (e) {
      setInvItems([]);
      setInvTotal(0);
      setInvErr(e instanceof ApiRequestError ? e.message : e instanceof Error ? e.message : "Không tải danh sách tồn.");
    } finally {
      setInvLoading(false);
    }
  }, [accessToken, invPage, invPageSize, invOnlyOos, invOnlyBelow]);

  useEffect(() => {
    if (!isAuthenticated || !accessToken) return;
    void loadOverview();
  }, [isAuthenticated, accessToken, loadOverview]);

  useEffect(() => {
    if (!isAuthenticated || !accessToken) return;
    void loadInventory();
  }, [isAuthenticated, accessToken, invPage, invPageSize, invOnlyOos, invOnlyBelow, loadInventory]);

  const num = (v) => (v == null || Number.isNaN(Number(v)) ? "—" : Number(v).toLocaleString("vi-VN"));

  return (
    <div className="mx-auto max-w-6xl space-y-6 pb-10">
      <Card className="border-slate-200/80 shadow-sm dark:border-slate-800">
        <CardHeader className="border-b border-slate-100 dark:border-slate-800">
          <div className="flex flex-wrap items-start justify-between gap-4">
            <div className="flex items-start gap-3">
              <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-teal-500/10 text-teal-800 ring-1 ring-teal-500/20 dark:text-teal-300 dark:ring-teal-500/25">
                <PackageSearch className="h-5 w-5" strokeWidth={2} aria-hidden />
              </span>
              <div>
                <CardTitle className="text-lg font-semibold tracking-tight text-slate-900 dark:text-slate-50">
                  Tổng quan tồn và cảnh báo đặt hàng lại
                </CardTitle>
                <CardDescription className="mt-1 max-w-2xl text-sm">
                  Số hàng đang ở mức cần chú ý hoặc hết bán được được tính theo từng mặt hàng: nếu đã cấu hình ngưỡng riêng thì dùng mức đó, còn không thì dùng ngưỡng chung bạn nhập bên dưới.
                  {variantDetailLinkMode === "stock-manager" ? (
                    <> Để xem hoặc chỉnh từng SKU, bấm Chi tiết trên dòng — cửa sổ cập nhật tồn sẽ mở.</>
                  ) : (
                    <> Để xem hoặc chỉnh từng biến thể, vào trang chi tiết biến thể trong quản trị sản phẩm.</>
                  )}
                </CardDescription>
              </div>
            </div>
            <Button type="button" size="sm" variant="outline" className="gap-1.5" asChild>
              <Link to={stockMovementsTo}>
                Giao dịch kho
                <ExternalLink className="h-3.5 w-3.5 opacity-70" aria-hidden />
              </Link>
            </Button>
          </div>
        </CardHeader>
        <CardContent className="space-y-4 pt-6">
          <div className="flex flex-wrap items-end gap-3">
            <div className="space-y-1">
              <label className="text-xs font-medium text-slate-500 dark:text-slate-400" htmlFor="wh-ov-thr">
                Ngưỡng chung (khi SKU chưa cấu hình riêng)
              </label>
              <input
                id="wh-ov-thr"
                type="number"
                min={0}
                step={1}
                className={cn(fieldInput, "w-28")}
                value={ovThreshold}
                onChange={(e) => setOvThreshold(e.target.value)}
              />
            </div>
            <Button type="button" size="sm" variant="secondary" className="gap-1.5" disabled={!accessToken || ovLoading} onClick={() => void loadOverview()}>
              {ovLoading ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : <RefreshCw className="h-4 w-4" aria-hidden />}
              Tải tổng quan
            </Button>
          </div>
          {ovErr ? (
            <p className="text-sm text-red-700 dark:text-red-300" role="alert">
              {ovErr}
            </p>
          ) : null}
          {overview && !ovLoading ? (
            <dl className="grid gap-3 text-sm sm:grid-cols-2 lg:grid-cols-3">
              <div className="rounded-lg border border-slate-200 bg-slate-50/80 px-3 py-2 dark:border-slate-700 dark:bg-slate-900/40">
                <dt className="text-xs font-semibold uppercase text-slate-500">Hàng cần đặt lại / tồn thấp</dt>
                <dd className="font-mono text-xl font-semibold tabular-nums text-amber-800 dark:text-amber-200">
                  {num(pick(overview, "lowStockCount", "LowStockCount"))}
                </dd>
              </div>
              <div className="rounded-lg border border-slate-200 bg-slate-50/80 px-3 py-2 dark:border-slate-700 dark:bg-slate-900/40">
                <dt className="text-xs font-semibold uppercase text-slate-500">Hết hàng (không còn bán được)</dt>
                <dd className="font-mono text-xl font-semibold tabular-nums text-red-800 dark:text-red-200">
                  {num(pick(overview, "outOfStockCount", "OutOfStockCount"))}
                </dd>
              </div>
              <div className="rounded-lg border border-slate-200 bg-slate-50/80 px-3 py-2 dark:border-slate-700 dark:bg-slate-900/40">
                <dt className="text-xs font-semibold uppercase text-slate-500">Ngưỡng chung đang dùng</dt>
                <dd className="font-mono text-xl font-semibold tabular-nums text-slate-800 dark:text-slate-200">
                  {num(pick(overview, "lowStockThreshold", "LowStockThreshold"))}
                </dd>
              </div>
            </dl>
          ) : ovLoading ? (
            <div className="flex items-center gap-2 text-sm text-slate-500">
              <Loader2 className="h-4 w-4 animate-spin" aria-hidden />
              Đang tải tổng quan…
            </div>
          ) : null}
        </CardContent>
      </Card>

      <Card className="border-slate-200/80 shadow-sm dark:border-slate-800">
        <CardHeader>
          <CardTitle className="text-base">Danh sách tồn kho</CardTitle>
          <CardDescription>
            Một bảng duy nhất: mặc định chỉ hiện hàng dưới mức cần chú ý (giống danh sách cần đặt lại). Bỏ chọn lọc đó để duyệt toàn bộ tồn có phân trang; có thể kết hợp tìm SKU/tên, vị trí và lọc hết hàng.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
            <div className="space-y-1 sm:col-span-2">
              <label className="text-xs font-medium text-slate-500" htmlFor="wh-inv-q">
                Tìm SKU hoặc tên hàng
              </label>
              <input id="wh-inv-q" type="search" className={fieldInput} value={invSearch} onChange={(e) => setInvSearch(e.target.value)} placeholder="SKU / tên…" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-medium text-slate-500" htmlFor="wh-inv-loc">
                Vị trí trong kho
              </label>
              <input id="wh-inv-loc" type="text" className={fieldInput} value={invWhLoc} onChange={(e) => setInvWhLoc(e.target.value)} />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-medium text-slate-500" htmlFor="wh-inv-thr">
                Ngưỡng chung (khi hàng chưa cấu hình riêng)
              </label>
              <input id="wh-inv-thr" type="number" min={0} step={1} className={fieldInput} value={invThreshold} onChange={(e) => setInvThreshold(e.target.value)} />
            </div>
          </div>
          <div className="flex flex-wrap items-center gap-4">
            <label className="flex items-center gap-2 text-sm text-slate-700 dark:text-slate-300">
              <input
                type="checkbox"
                className={fieldCheckbox}
                checked={invOnlyOos}
                onChange={(e) => {
                  setInvOnlyOos(e.target.checked);
                  setInvPage(1);
                }}
              />
              Chỉ hết hàng
            </label>
            <label className="flex items-center gap-2 text-sm text-slate-700 dark:text-slate-300">
              <input
                type="checkbox"
                className={fieldCheckbox}
                checked={invOnlyBelow}
                onChange={(e) => {
                  setInvOnlyBelow(e.target.checked);
                  setInvPage(1);
                }}
              />
              Chỉ hàng dưới mức cần chú ý
            </label>
            <Button type="button" size="sm" variant="secondary" className="gap-1.5" disabled={!accessToken || invLoading} onClick={() => void loadInventory()}>
              {invLoading ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : <RefreshCw className="h-4 w-4" aria-hidden />}
              Tải danh sách
            </Button>
          </div>
          <div className="flex flex-wrap items-center gap-2 text-sm text-slate-600 dark:text-slate-400">
            <span>
              Trang {invMetaPage} / {Math.max(1, Math.ceil(invTotal / invMetaPageSize) || 1)} — {invTotal.toLocaleString("vi-VN")} dòng
            </span>
            <Button type="button" size="sm" variant="outline" disabled={invPage <= 1 || invLoading} onClick={() => setInvPage((p) => Math.max(1, p - 1))}>
              Trang trước
            </Button>
            <Button
              type="button"
              size="sm"
              variant="outline"
              disabled={invPage * invPageSize >= invTotal || invLoading}
              onClick={() => setInvPage((p) => p + 1)}
            >
              Trang sau
            </Button>
            <select
              className={cn(fieldInput, "w-auto min-w-[100px]")}
              value={String(invPageSize)}
              onChange={(e) => {
                setInvPageSize(Number(e.target.value) || 50);
                setInvPage(1);
              }}
            >
              {[20, 50, 100, 200].map((n) => (
                <option key={n} value={n}>
                  {n} / trang
                </option>
              ))}
            </select>
          </div>
          {invErr ? (
            <p className="text-sm text-red-700 dark:text-red-300" role="alert">
              {invErr}
            </p>
          ) : null}
          {invLoading ? (
            <div className="flex items-center gap-2 text-sm text-slate-500">
              <Loader2 className="h-4 w-4 animate-spin" aria-hidden />
              Đang tải danh sách…
            </div>
          ) : null}
          <div className="overflow-x-auto rounded-lg border border-slate-200 dark:border-slate-800">
            <table className="w-full min-w-[960px] border-collapse text-left text-sm">
              <thead className="bg-slate-50 text-xs font-semibold uppercase text-slate-600 dark:bg-slate-900/60 dark:text-slate-400">
                <tr>
                  <th className="px-3 py-2">SKU</th>
                  <th className="px-3 py-2">Sản phẩm</th>
                  <th className="px-3 py-2">Biến thể</th>
                  <th className="px-3 py-2">Vị trí</th>
                  <th className="px-3 py-2">Khả dụng</th>
                  <th className="px-3 py-2">Đang tồn thấp?</th>
                  <th className="px-3 py-2">Mức đang so</th>
                  <th className="px-3 py-2">Đặt lại / An toàn</th>
                  <th className="px-3 py-2"> </th>
                </tr>
              </thead>
              <tbody>
                {invItems.length === 0 && !invLoading ? (
                  <tr>
                    <td colSpan={9} className="px-3 py-6 text-center text-slate-500">
                      Không có dòng phù hợp. Thử bỏ bớt bộ lọc hoặc bấm &quot;Tải danh sách&quot;.
                    </td>
                  </tr>
                ) : null}
                {invItems.map((r, i) => {
                  const vid = pick(r, "variantId", "VariantId");
                  const pid = pick(r, "productId", "ProductId");
                  const href = variantDetailHref(pid, vid, variantDetailLinkMode);
                  const low = pick(r, "isLowStock", "IsLowStock");
                  const eff = pick(r, "effectiveLowStockThreshold", "EffectiveLowStockThreshold");
                  const rp = pick(r, "reorderPoint", "ReorderPoint");
                  const ss = pick(r, "safetyStock", "SafetyStock");
                  const loc = pick(r, "warehouseLocation", "WarehouseLocation");
                  return (
                    <tr key={`${String(pick(r, "inventoryId", "InventoryId"))}-${i}`} className="border-t border-slate-100 dark:border-slate-800">
                      <td className="px-3 py-2 font-mono text-xs">{String(pick(r, "sku", "Sku") ?? "—")}</td>
                      <td className="max-w-[140px] truncate px-3 py-2" title={String(pick(r, "productName", "ProductName") ?? "")}>
                        {String(pick(r, "productName", "ProductName") ?? "—")}
                      </td>
                      <td className="max-w-[140px] truncate px-3 py-2" title={String(pick(r, "variantName", "VariantName") ?? "")}>
                        {String(pick(r, "variantName", "VariantName") ?? "—")}
                      </td>
                      <td className="max-w-[100px] truncate px-3 py-2 font-mono text-xs text-slate-600 dark:text-slate-400">
                        {loc != null && String(loc).trim() !== "" ? String(loc) : "—"}
                      </td>
                      <td className="px-3 py-2 font-mono tabular-nums">{num(pick(r, "quantityAvailable", "QuantityAvailable"))}</td>
                      <td className="px-3 py-2">{low === true ? <span className="text-amber-800 dark:text-amber-200">Có</span> : low === false ? "Không" : "—"}</td>
                      <td className="px-3 py-2 font-mono tabular-nums">{num(eff)}</td>
                      <td className="px-3 py-2 font-mono text-xs tabular-nums text-slate-600 dark:text-slate-400">
                        {rp != null ? num(rp) : "—"} / {ss != null ? num(ss) : "—"}
                      </td>
                      <td className="px-3 py-2">
                        {href ? (
                          <Link className="text-xs font-medium text-teal-700 underline-offset-2 hover:underline dark:text-teal-400" to={href}>
                            Chi tiết
                          </Link>
                        ) : (
                          "—"
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
