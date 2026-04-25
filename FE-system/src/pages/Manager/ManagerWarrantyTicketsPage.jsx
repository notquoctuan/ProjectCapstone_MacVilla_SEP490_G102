import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { ApiRequestError } from "@/services/api/apiEnvelope";
import {
  createAdminWarrantyTicket,
  fetchAdminWarrantyStatuses,
  fetchAdminWarrantyTicketByNumber,
  fetchAdminWarrantyTickets,
  labelAdminWarrantyTicketStatus,
  warrantyTicketStatusBadgeClass,
} from "@/services/admin/adminWarrantyApi";
import { ChevronLeft, ChevronRight, Loader2, Plus, RefreshCw, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { cn } from "@/lib/utils";

const PAGE_SIZE_OPTIONS = [10, 20, 50];
const WARRANTY_BASE = "/manager/after-sales/warranty";

const fieldInput = cn(
  "h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-foreground shadow-sm transition-all",
  "placeholder:text-slate-400 hover:border-slate-300",
  "focus-visible:border-primary/50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/20",
  "dark:border-slate-700 dark:bg-slate-950 dark:hover:border-slate-600"
);

const fieldSelect = cn(fieldInput, "cursor-pointer appearance-none bg-transparent pr-10");

function pickRow(obj, camel, pascal) {
  if (!obj || typeof obj !== "object") return undefined;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel];
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal];
  return undefined;
}

function formatDate(iso) {
  if (!iso) return "—";
  try {
    return new Date(iso).toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
  } catch {
    return String(iso);
  }
}

function fromDatetimeLocalToIso(local) {
  const t = String(local || "").trim();
  if (!t) return undefined;
  const d = new Date(t);
  if (Number.isNaN(d.getTime())) return undefined;
  return d.toISOString();
}

function emptyCreateForm() {
  return {
    customerId: "",
    orderId: "",
    contractId: "",
    validUntil: "",
  };
}

/**
 * Danh sách phiếu bảo hành (Manager). `defaultStatusLocked` (vd. `Pending`) — màn «chờ xử lý».
 * @param {{ defaultStatusLocked?: string }} [props]
 */
export function ManagerWarrantyTicketsPage({ defaultStatusLocked } = {}) {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { accessToken, isAuthenticated } = useAuth();

  const lockedStatus = defaultStatusLocked != null && String(defaultStatusLocked).trim() !== "" ? String(defaultStatusLocked).trim() : null;

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [status, setStatus] = useState("");
  const statusForApi = lockedStatus ?? status;
  const [customerId, setCustomerId] = useState("");
  const [orderId, setOrderId] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [data, setData] = useState(null);
  const [ticketStatusOptions, setTicketStatusOptions] = useState([]);

  const [createOpen, setCreateOpen] = useState(false);
  const [createSubmitting, setCreateSubmitting] = useState(false);
  const [createError, setCreateError] = useState("");
  const [createForm, setCreateForm] = useState(emptyCreateForm);

  const [lookupSubmitting, setLookupSubmitting] = useState(false);
  const [lookupError, setLookupError] = useState("");

  useEffect(() => {
    const t = window.setTimeout(() => setSearch(searchInput), 400);
    return () => window.clearTimeout(t);
  }, [searchInput]);

  useEffect(() => {
    const oid = searchParams.get("orderId");
    if (oid && /^\d+$/.test(oid)) setOrderId(oid);
    const cid = searchParams.get("customerId");
    if (cid && /^\d+$/.test(cid)) setCustomerId(cid);
  }, [searchParams]);

  useEffect(() => {
    if (!isAuthenticated || !accessToken) return;
    let cancelled = false;
    (async () => {
      try {
        const { ticketStatuses } = await fetchAdminWarrantyStatuses(accessToken);
        if (!cancelled) setTicketStatusOptions(ticketStatuses.filter((o) => o.value));
      } catch {
        if (!cancelled) setTicketStatusOptions([]);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [accessToken, isAuthenticated]);

  useEffect(() => {
    setPage(1);
  }, [statusForApi, customerId, orderId, fromDate, toDate, search, pageSize]);

  const load = useCallback(async () => {
    if (!isAuthenticated || !accessToken) {
      setLoading(false);
      return;
    }
    setLoading(true);
    setError("");
    try {
      const cid = customerId.trim();
      const oid = orderId.trim();
      const result = await fetchAdminWarrantyTickets(accessToken, {
        page,
        pageSize,
        status: statusForApi || undefined,
        customerId: cid && /^\d+$/.test(cid) ? cid : undefined,
        orderId: oid && /^\d+$/.test(oid) ? oid : undefined,
        fromDate: fromDate.trim() || undefined,
        toDate: toDate.trim() || undefined,
        search: search.trim() || undefined,
      });
      setData(result);
    } catch (e) {
      const msg =
        e instanceof ApiRequestError
          ? e.message
          : e instanceof Error
            ? e.message
            : "Không tải được danh sách phiếu bảo hành.";
      setError(msg);
      setData(null);
    } finally {
      setLoading(false);
    }
  }, [accessToken, isAuthenticated, page, pageSize, statusForApi, customerId, orderId, fromDate, toDate, search]);

  useEffect(() => {
    void load();
  }, [load]);

  const totalCount = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize) || 1);
  const items = data?.items ?? [];

  const statusFilterOptions = useMemo(
    () => [
      { value: "", label: "Tất cả trạng thái" },
      ...ticketStatusOptions.map((o) => ({
        value: o.value,
        label: labelAdminWarrantyTicketStatus(o.value, ticketStatusOptions),
      })),
    ],
    [ticketStatusOptions]
  );

  const clearFilters = () => {
    if (!lockedStatus) setStatus("");
    setCustomerId("");
    setOrderId("");
    setFromDate("");
    setToDate("");
    setSearchInput("");
    setSearch("");
  };

  const hasActiveFilters = Boolean(
    status || customerId.trim() || orderId.trim() || fromDate || toDate || searchInput.trim()
  );

  const submitCreate = async () => {
    if (!accessToken || createSubmitting) return;
    const cust = Number(createForm.customerId);
    if (!Number.isFinite(cust) || cust < 1) {
      setCreateError("Nhập mã khách hàng (customerId) hợp lệ.");
      return;
    }
    setCreateSubmitting(true);
    setCreateError("");
    try {
      /** @type {Record<string, unknown>} */
      const body = { customerId: cust };
      const oid = createForm.orderId.trim();
      if (oid && /^\d+$/.test(oid)) body.orderId = Number(oid);
      const ct = createForm.contractId.trim();
      if (ct && /^\d+$/.test(ct)) body.contractId = Number(ct);
      const vu = fromDatetimeLocalToIso(createForm.validUntil);
      if (vu) body.validUntil = vu;

      const created = await createAdminWarrantyTicket(accessToken, body);
      const raw = created && typeof created === "object" ? /** @type {Record<string, unknown>} */ (created) : {};
      const newId = raw.id ?? raw.Id;
      setCreateOpen(false);
      setCreateForm(emptyCreateForm());
      void load();
      if (newId != null && Number.isFinite(Number(newId))) {
        navigate(`${WARRANTY_BASE}/${Number(newId)}`);
      }
    } catch (e) {
      setCreateError(e instanceof ApiRequestError ? e.message : e instanceof Error ? e.message : "Tạo phiếu thất bại.");
    } finally {
      setCreateSubmitting(false);
    }
  };

  const lookupByNumber = async () => {
    if (!accessToken) return;
    const q = searchInput.trim();
    if (!q) {
      setLookupError("Nhập mã phiếu (số phiếu).");
      return;
    }
    setLookupSubmitting(true);
    setLookupError("");
    try {
      const d = await fetchAdminWarrantyTicketByNumber(accessToken, q);
      const raw = d && typeof d === "object" ? /** @type {Record<string, unknown>} */ (d) : {};
      const tid = raw.id ?? raw.Id;
      if (tid != null && Number.isFinite(Number(tid))) {
        navigate(`${WARRANTY_BASE}/${Number(tid)}`);
      } else {
        setLookupError("Không tìm thấy phiếu với mã này.");
      }
    } catch (e) {
      setLookupError(e instanceof ApiRequestError ? e.message : e instanceof Error ? e.message : "Tra cứu thất bại.");
    } finally {
      setLookupSubmitting(false);
    }
  };

  return (
    <div className="space-y-6">
      <Dialog
        open={createOpen}
        onOpenChange={(open) => {
          setCreateOpen(open);
          if (!open) {
            setCreateError("");
            setCreateForm(emptyCreateForm());
          }
        }}
      >
        <DialogContent
          className="max-h-[min(90dvh,calc(100vh-2rem))] w-[calc(100vw-1.5rem)] max-w-lg overflow-y-auto sm:max-w-lg"
          onPointerDownOutside={(e) => createSubmitting && e.preventDefault()}
        >
          <DialogHeader>
            <DialogTitle>Tạo phiếu bảo hành</DialogTitle>
            <DialogDescription>POST /api/admin/warranty-tickets — `bao-hanh.md`.</DialogDescription>
          </DialogHeader>
          <div className="grid gap-3 sm:grid-cols-2">
            <div className="space-y-2 sm:col-span-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="wt-cust">
                Khách (customerId) <span className="text-red-600">*</span>
              </label>
              <input
                id="wt-cust"
                type="text"
                inputMode="numeric"
                className={fieldInput}
                value={createForm.customerId}
                onChange={(e) => setCreateForm((f) => ({ ...f, customerId: e.target.value }))}
                disabled={createSubmitting}
              />
            </div>
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="wt-ord">
                Đơn (orderId)
              </label>
              <input
                id="wt-ord"
                type="text"
                inputMode="numeric"
                className={fieldInput}
                value={createForm.orderId}
                onChange={(e) => setCreateForm((f) => ({ ...f, orderId: e.target.value }))}
                disabled={createSubmitting}
              />
            </div>
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="wt-ct">
                Hợp đồng (contractId)
              </label>
              <input
                id="wt-ct"
                type="text"
                inputMode="numeric"
                className={fieldInput}
                value={createForm.contractId}
                onChange={(e) => setCreateForm((f) => ({ ...f, contractId: e.target.value }))}
                disabled={createSubmitting}
              />
            </div>
            <div className="space-y-2 sm:col-span-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="wt-until">
                Hiệu lực đến (validUntil) — để trống: BE mặc định 12 tháng
              </label>
              <input
                id="wt-until"
                type="datetime-local"
                className={fieldInput}
                value={createForm.validUntil}
                onChange={(e) => setCreateForm((f) => ({ ...f, validUntil: e.target.value }))}
                disabled={createSubmitting}
              />
            </div>
          </div>
          {createError ? (
            <p className="text-sm text-red-700 dark:text-red-300" role="alert">
              {createError}
            </p>
          ) : null}
          <DialogFooter className="gap-2 sm:gap-0">
            <Button type="button" variant="outline" disabled={createSubmitting} onClick={() => setCreateOpen(false)}>
              Hủy
            </Button>
            <Button type="button" disabled={createSubmitting} onClick={() => void submitCreate()}>
              {createSubmitting ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : null}
              Tạo phiếu
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-slate-50">Phiếu bảo hành</h1>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-400">
            Manager theo dõi phiếu và cập nhật trạng thái claim (PUT claim/status); đồng bộ vận hành kho khi cần.
          </p>
          {lockedStatus ? (
            <p className="mt-2 text-sm">
              <span className="rounded-md bg-amber-50 px-2 py-1 font-medium text-amber-900 ring-1 ring-amber-200/80 dark:bg-amber-950/30 dark:text-amber-100 dark:ring-amber-800/50">
                Đang lọc trạng thái phiếu: {lockedStatus}
              </span>
              <Link className="ml-3 text-sm font-medium text-violet-700 underline-offset-2 hover:underline dark:text-violet-400" to={WARRANTY_BASE}>
                Xem tất cả phiếu bảo hành
              </Link>
            </p>
          ) : null}
        </div>
        <div className="flex flex-wrap gap-2">
          <Button type="button" variant="outline" size="sm" className="gap-1.5" disabled={loading} onClick={() => void load()}>
            <RefreshCw className={cn("h-4 w-4", loading && "animate-spin")} aria-hidden />
            Tải lại
          </Button>
          <Button
            type="button"
            size="sm"
            className="gap-1.5"
            onClick={() => {
              setCreateError("");
              setCreateForm((f) => ({
                ...emptyCreateForm(),
                customerId: customerId.trim() && /^\d+$/.test(customerId.trim()) ? customerId.trim() : f.customerId,
                orderId: orderId.trim() && /^\d+$/.test(orderId.trim()) ? orderId.trim() : f.orderId,
              }));
              setCreateOpen(true);
            }}
          >
            <Plus className="h-4 w-4" aria-hidden />
            Tạo phiếu
          </Button>
        </div>
      </div>

      <Card className="border-slate-200/80 dark:border-slate-800">
        <CardHeader className="pb-3">
          <CardTitle className="text-base">Bộ lọc</CardTitle>
          <CardDescription>Trạng thái phiếu, khách, đơn, ngày, tìm kiếm (SĐT / mã…)</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600 dark:text-slate-400" htmlFor="w-f-st">
              Trạng thái
            </label>
            <select
              id="w-f-st"
              className={fieldSelect}
              value={lockedStatus ?? status}
              disabled={!!lockedStatus}
              onChange={(e) => setStatus(e.target.value)}
            >
              {statusFilterOptions.map((o) => (
                <option key={o.value || "all"} value={o.value}>
                  {o.label}
                </option>
              ))}
            </select>
          </div>
          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600 dark:text-slate-400" htmlFor="w-f-cust">
              Khách (ID)
            </label>
            <input
              id="w-f-cust"
              type="text"
              inputMode="numeric"
              className={fieldInput}
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
            />
          </div>
          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600 dark:text-slate-400" htmlFor="w-f-ord">
              Đơn (ID)
            </label>
            <input
              id="w-f-ord"
              type="text"
              inputMode="numeric"
              className={fieldInput}
              value={orderId}
              onChange={(e) => setOrderId(e.target.value)}
            />
          </div>
          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600 dark:text-slate-400" htmlFor="w-f-from">
              Từ ngày
            </label>
            <input id="w-f-from" type="date" className={fieldInput} value={fromDate} onChange={(e) => setFromDate(e.target.value)} />
          </div>
          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600 dark:text-slate-400" htmlFor="w-f-to">
              Đến ngày
            </label>
            <input id="w-f-to" type="date" className={fieldInput} value={toDate} onChange={(e) => setToDate(e.target.value)} />
          </div>
          <div className="space-y-1.5 sm:col-span-2 lg:col-span-2">
            <label className="text-xs font-medium text-slate-600 dark:text-slate-400" htmlFor="w-f-q">
              Tìm kiếm / mã phiếu
            </label>
            <div className="flex gap-2">
              <input
                id="w-f-q"
                type="search"
                className={fieldInput}
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                placeholder="SĐT, tên, mã phiếu…"
              />
              <Button
                type="button"
                variant="secondary"
                size="sm"
                className="shrink-0 gap-1"
                disabled={lookupSubmitting}
                onClick={() => void lookupByNumber()}
                title="GET by-number — tra đúng số phiếu"
              >
                {lookupSubmitting ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : <Search className="h-4 w-4" aria-hidden />}
                Tra mã
              </Button>
            </div>
            {lookupError ? <p className="text-xs text-red-600 dark:text-red-400">{lookupError}</p> : null}
          </div>
          {hasActiveFilters ? (
            <div className="flex items-end sm:col-span-2 lg:col-span-4">
              <Button type="button" variant="ghost" size="sm" onClick={clearFilters}>
                Xóa bộ lọc
              </Button>
            </div>
          ) : null}
        </CardContent>
      </Card>

      {error ? (
        <div
          className="rounded-xl border border-red-200 bg-red-50/90 px-4 py-3 text-sm text-red-800 dark:border-red-900/50 dark:bg-red-950/30 dark:text-red-200"
          role="alert"
        >
          {error}
        </div>
      ) : null}

      <Card className="border-slate-200/80 dark:border-slate-800">
        <CardHeader className="flex flex-row flex-wrap items-center justify-between gap-3 space-y-0 pb-3">
          <div>
            <CardTitle className="text-base">Danh sách phiếu</CardTitle>
            <CardDescription>{totalCount.toLocaleString("vi-VN")} phiếu</CardDescription>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <select
              value={pageSize}
              onChange={(e) => setPageSize(Number(e.target.value))}
              className="h-9 rounded-md border border-input bg-background px-2 text-sm"
            >
              {PAGE_SIZE_OPTIONS.map((n) => (
                <option key={n} value={n}>
                  {n}/trang
                </option>
              ))}
            </select>
            <span className="text-xs text-muted-foreground">
              Trang {page}/{totalPages}
            </span>
          </div>
        </CardHeader>
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            <table className="w-full min-w-[900px] border-collapse text-left text-sm">
              <thead>
                <tr className="border-b border-slate-200 bg-slate-50/95 text-[11px] font-semibold uppercase text-slate-500 dark:border-slate-800 dark:bg-slate-900/80 dark:text-slate-400">
                  <th className="px-4 py-3 pl-6">Mã phiếu</th>
                  <th className="px-4 py-3">Trạng thái</th>
                  <th className="px-4 py-3">Khách</th>
                  <th className="px-4 py-3">Đơn</th>
                  <th className="px-4 py-3">Hiệu lực đến</th>
                  <th className="px-4 py-3 pr-6">Tạo</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                {loading && items.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-16 text-center">
                      <Loader2 className="mx-auto h-8 w-8 animate-spin text-violet-600/70" />
                    </td>
                  </tr>
                ) : null}
                {!loading && items.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-slate-500">
                      Không có phiếu phù hợp.
                    </td>
                  </tr>
                ) : null}
                {items.map((row, idx) => {
                  const rid = pickRow(row, "id", "Id");
                  const num = pickRow(row, "ticketNumber", "TicketNumber");
                  const st = pickRow(row, "status", "Status");
                  const cname = pickRow(row, "customerName", "CustomerName");
                  const cid = pickRow(row, "customerId", "CustomerId");
                  const oid = pickRow(row, "orderId", "OrderId");
                  const until = pickRow(row, "validUntil", "ValidUntil");
                  const created = pickRow(row, "createdAt", "CreatedAt");
                  return (
                    <tr
                      key={rid != null ? String(rid) : idx}
                      className={cn(
                        "cursor-pointer transition-colors hover:bg-violet-500/[0.04] dark:hover:bg-violet-500/[0.06]",
                        idx % 2 === 1 && "bg-slate-50/40 dark:bg-slate-900/20"
                      )}
                      onClick={() => rid != null && navigate(`${WARRANTY_BASE}/${encodeURIComponent(String(rid))}`)}
                    >
                      <td className="px-4 py-3 pl-6 font-mono text-xs font-semibold text-slate-900 dark:text-slate-100">
                        {num != null ? String(num) : rid != null ? `#${rid}` : "—"}
                      </td>
                      <td className="px-4 py-3">
                        <span
                          className={cn(
                            "inline-flex rounded-full px-2.5 py-0.5 text-xs font-semibold",
                            warrantyTicketStatusBadgeClass(st != null ? String(st) : "")
                          )}
                        >
                          {labelAdminWarrantyTicketStatus(st != null ? String(st) : "", ticketStatusOptions)}
                        </span>
                      </td>
                      <td className="max-w-[200px] px-4 py-3">
                        <div className="truncate font-medium text-slate-900 dark:text-slate-100">
                          {cname != null ? String(cname) : "—"}
                        </div>
                        {cid != null ? <div className="font-mono text-[11px] text-slate-500">#{cid}</div> : null}
                      </td>
                      <td className="px-4 py-3 font-mono text-xs">
                        {oid != null ? (
                          <Link
                            to={`/manager/sales/orders/${encodeURIComponent(String(oid))}`}
                            className="text-violet-700 underline-offset-2 hover:underline dark:text-violet-400"
                            onClick={(e) => e.stopPropagation()}
                          >
                            #{oid}
                          </Link>
                        ) : (
                          "—"
                        )}
                      </td>
                      <td className="whitespace-nowrap px-4 py-3 text-xs text-slate-600 dark:text-slate-400">
                        {formatDate(until != null ? String(until) : "")}
                      </td>
                      <td className="whitespace-nowrap px-4 py-3 pr-6 text-xs text-slate-600 dark:text-slate-400">
                        {formatDate(created != null ? String(created) : "")}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
          <div className="flex items-center justify-between gap-2 border-t border-slate-200 px-4 py-3 dark:border-slate-800">
            <Button type="button" variant="outline" size="sm" disabled={loading || page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>
              <ChevronLeft className="h-4 w-4" />
              Trước
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              disabled={loading || page >= totalPages}
              onClick={() => setPage((p) => p + 1)}
            >
              Sau
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
