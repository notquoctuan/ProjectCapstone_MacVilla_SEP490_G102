import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { ApiRequestError } from "@/services/api/apiEnvelope";
import {
  createAdminWarrantyTicketClaim,
  fetchAdminWarrantyStatuses,
  fetchAdminWarrantyTicketDetail,
  labelAdminWarrantyClaimStatus,
  labelAdminWarrantyTicketStatus,
  warrantyTicketStatusBadgeClass,
} from "@/services/admin/adminWarrantyApi";
import { ChevronRight, Loader2, Plus } from "lucide-react";
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

const WARRANTY_LIST = "/manager/after-sales/warranty";
const CLAIM = (id) => `/manager/after-sales/warranty/claims/${encodeURIComponent(String(id))}`;

const fieldInput = cn(
  "h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-foreground shadow-sm transition-all",
  "placeholder:text-slate-400 hover:border-slate-300",
  "focus-visible:border-primary/50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/20",
  "dark:border-slate-700 dark:bg-slate-950 dark:hover:border-slate-600"
);

function pick(obj, camel, pascal) {
  if (!obj || typeof obj !== "object") return undefined;
  if (obj[camel] !== undefined && obj[camel] !== null) return obj[camel];
  if (obj[pascal] !== undefined && obj[pascal] !== null) return obj[pascal];
  return undefined;
}

function formatMoneyVnd(n) {
  if (n == null || Number.isNaN(Number(n))) return "—";
  return `${Number(n).toLocaleString("vi-VN")} đ`;
}

function formatDate(iso) {
  if (!iso) return "—";
  try {
    return new Date(iso).toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
  } catch {
    return String(iso);
  }
}

function emptyClaimForm() {
  return {
    variantId: "",
    defectDescription: "",
    imagesUrl: "",
    estimatedCost: "0",
    note: "",
  };
}

export function ManagerWarrantyTicketDetailPage() {
  const { ticketId: ticketIdParam } = useParams();
  const navigate = useNavigate();
  const { accessToken, isAuthenticated } = useAuth();
  const ticketId = String(ticketIdParam ?? "").trim();

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [detail, setDetail] = useState(/** @type {Record<string, unknown> | null} */ (null));
  const [ticketStatusOptions, setTicketStatusOptions] = useState([]);
  const [claimStatusOptions, setClaimStatusOptions] = useState([]);

  const [claimOpen, setClaimOpen] = useState(false);
  const [claimSubmitting, setClaimSubmitting] = useState(false);
  const [claimError, setClaimError] = useState("");
  const [claimForm, setClaimForm] = useState(emptyClaimForm);

  useEffect(() => {
    if (!isAuthenticated || !accessToken) return;
    let cancelled = false;
    (async () => {
      try {
        const { ticketStatuses, claimStatuses } = await fetchAdminWarrantyStatuses(accessToken);
        if (!cancelled) {
          setTicketStatusOptions(ticketStatuses.filter((o) => o.value));
          setClaimStatusOptions(claimStatuses.filter((o) => o.value));
        }
      } catch {
        if (!cancelled) {
          setTicketStatusOptions([]);
          setClaimStatusOptions([]);
        }
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [accessToken, isAuthenticated]);

  const load = useCallback(async () => {
    if (!isAuthenticated || !accessToken || !ticketId) {
      setLoading(false);
      return;
    }
    setLoading(true);
    setError("");
    try {
      const d = await fetchAdminWarrantyTicketDetail(accessToken, ticketId);
      setDetail(d && typeof d === "object" ? /** @type {Record<string, unknown>} */ (d) : null);
    } catch (e) {
      const msg =
        e instanceof ApiRequestError
          ? e.message
          : e instanceof Error
            ? e.message
            : "Không tải được phiếu.";
      setError(msg);
      setDetail(null);
    } finally {
      setLoading(false);
    }
  }, [accessToken, isAuthenticated, ticketId]);

  useEffect(() => {
    void load();
  }, [load]);

  const num = detail ? pick(detail, "ticketNumber", "TicketNumber") : null;
  const st = detail ? String(pick(detail, "status", "Status") ?? "") : "";
  const cid = detail ? pick(detail, "customerId", "CustomerId") : null;
  const cname = detail ? pick(detail, "customerName", "CustomerName") : null;
  const oid = detail ? pick(detail, "orderId", "OrderId") : null;
  const ctid = detail ? pick(detail, "contractId", "ContractId") : null;
  const until = detail ? pick(detail, "validUntil", "ValidUntil") : null;
  const created = detail ? pick(detail, "createdAt", "CreatedAt") : null;
  const claimsRaw = detail ? pick(detail, "claims", "Claims") : null;
  const claims = Array.isArray(claimsRaw) ? claimsRaw : [];

  const submitClaim = async () => {
    if (!accessToken || !ticketId || claimSubmitting) return;
    const vid = Number(claimForm.variantId);
    if (!Number.isFinite(vid) || vid < 1) {
      setClaimError("Nhập variantId (biến thể) hợp lệ.");
      return;
    }
    const est = Number(String(claimForm.estimatedCost).replace(/\s/g, "").replace(",", "."));
    if (!Number.isFinite(est) || est < 0) {
      setClaimError("Chi phí dự kiến phải là số ≥ 0.");
      return;
    }
    setClaimSubmitting(true);
    setClaimError("");
    try {
      const body = {
        variantId: vid,
        estimatedCost: est,
      };
      const dd = claimForm.defectDescription.trim();
      if (dd) body.defectDescription = dd;
      const img = claimForm.imagesUrl.trim();
      if (img) body.imagesUrl = img;
      const note = claimForm.note.trim();
      if (note) body.note = note;

      await createAdminWarrantyTicketClaim(accessToken, ticketId, body);
      setClaimOpen(false);
      setClaimForm(emptyClaimForm());
      void load();
    } catch (e) {
      setClaimError(e instanceof ApiRequestError ? e.message : e instanceof Error ? e.message : "Tạo yêu cầu thất bại.");
    } finally {
      setClaimSubmitting(false);
    }
  };

  return (
    <div className="mx-auto max-w-5xl space-y-8 pb-10">
      <nav className="flex flex-wrap items-center gap-1 text-xs font-medium text-slate-500 dark:text-slate-400" aria-label="Breadcrumb">
        <Link to="/manager" className="rounded-md px-1.5 py-0.5 hover:bg-slate-100 dark:hover:bg-slate-800">
          Dashboard
        </Link>
        <ChevronRight className="h-3.5 w-3.5 opacity-60" aria-hidden />
        <Link to={WARRANTY_LIST} className="rounded-md px-1.5 py-0.5 hover:bg-slate-100 dark:hover:bg-slate-800">
          Bảo hành
        </Link>
        <ChevronRight className="h-3.5 w-3.5 opacity-60" aria-hidden />
        <span className="px-1.5 font-semibold text-slate-800 dark:text-slate-200">
          {num != null ? String(num) : ticketId ? `#${ticketId}` : "—"}
        </span>
      </nav>

      <Dialog
        open={claimOpen}
        onOpenChange={(open) => {
          setClaimOpen(open);
          if (!open) {
            setClaimError("");
            setClaimForm(emptyClaimForm());
          }
        }}
      >
        <DialogContent
          className="max-h-[min(90dvh,calc(100vh-2rem))] w-[calc(100vw-1.5rem)] max-w-lg overflow-y-auto sm:max-w-lg"
          onPointerDownOutside={(e) => claimSubmitting && e.preventDefault()}
        >
          <DialogHeader>
            <DialogTitle>Tạo yêu cầu bảo hành (claim)</DialogTitle>
            <DialogDescription>POST …/warranty-tickets/&#123;id&#125;/claims — `bao-hanh.md`.</DialogDescription>
          </DialogHeader>
          <div className="grid gap-3">
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="cl-v">
                Biến thể (variantId) <span className="text-red-600">*</span>
              </label>
              <input
                id="cl-v"
                type="text"
                inputMode="numeric"
                className={fieldInput}
                value={claimForm.variantId}
                onChange={(e) => setClaimForm((f) => ({ ...f, variantId: e.target.value }))}
                disabled={claimSubmitting}
              />
            </div>
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="cl-d">
                Mô tả lỗi
              </label>
              <textarea
                id="cl-d"
                rows={3}
                className={cn(fieldInput, "min-h-[88px] resize-y py-2")}
                value={claimForm.defectDescription}
                onChange={(e) => setClaimForm((f) => ({ ...f, defectDescription: e.target.value }))}
                disabled={claimSubmitting}
              />
            </div>
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="cl-img">
                Ảnh (URL, cách nhau dấu phẩy hoặc ;)
              </label>
              <input
                id="cl-img"
                type="text"
                className={fieldInput}
                value={claimForm.imagesUrl}
                onChange={(e) => setClaimForm((f) => ({ ...f, imagesUrl: e.target.value }))}
                disabled={claimSubmitting}
              />
            </div>
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="cl-cost">
                Chi phí dự kiến (đ) <span className="text-red-600">*</span>
              </label>
              <input
                id="cl-cost"
                type="text"
                inputMode="decimal"
                className={fieldInput}
                value={claimForm.estimatedCost}
                onChange={(e) => setClaimForm((f) => ({ ...f, estimatedCost: e.target.value }))}
                disabled={claimSubmitting}
              />
            </div>
            <div className="space-y-2">
              <label className="text-xs font-semibold uppercase text-slate-500" htmlFor="cl-note">
                Ghi chú
              </label>
              <textarea
                id="cl-note"
                rows={2}
                className={cn(fieldInput, "min-h-[72px] resize-y py-2")}
                value={claimForm.note}
                onChange={(e) => setClaimForm((f) => ({ ...f, note: e.target.value }))}
                disabled={claimSubmitting}
              />
            </div>
          </div>
          {claimError ? (
            <p className="text-sm text-red-700 dark:text-red-300" role="alert">
              {claimError}
            </p>
          ) : null}
          <DialogFooter className="gap-2 sm:gap-0">
            <Button type="button" variant="outline" disabled={claimSubmitting} onClick={() => setClaimOpen(false)}>
              Hủy
            </Button>
            <Button type="button" disabled={claimSubmitting} onClick={() => void submitClaim()}>
              {claimSubmitting ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : null}
              Gửi yêu cầu
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {loading && !detail ? (
        <div className="flex justify-center py-20">
          <Loader2 className="h-10 w-10 animate-spin text-violet-600/70" aria-hidden />
        </div>
      ) : null}

      {error && !loading ? (
        <div
          className="rounded-xl border border-red-200 bg-red-50/90 px-4 py-3 text-sm text-red-800 dark:border-red-900/50 dark:bg-red-950/30 dark:text-red-200"
          role="alert"
        >
          {error}
        </div>
      ) : null}

      {!loading && !error && detail ? (
        <>
          <header className="flex flex-col gap-4 border-b border-slate-200 pb-6 dark:border-slate-800 sm:flex-row sm:items-start sm:justify-between">
            <div>
              <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-slate-50">
                {num != null ? String(num) : `Phiếu #${ticketId}`}
              </h1>
              <div className="mt-2">
                <span
                  className={cn(
                    "inline-flex rounded-full px-2.5 py-0.5 text-xs font-semibold",
                    warrantyTicketStatusBadgeClass(st)
                  )}
                >
                  {labelAdminWarrantyTicketStatus(st, ticketStatusOptions)}
                </span>
              </div>
            </div>
            <Button type="button" size="sm" className="gap-1.5 self-start" onClick={() => setClaimOpen(true)}>
              <Plus className="h-4 w-4" aria-hidden />
              Tạo claim
            </Button>
          </header>

          <div className="grid gap-6 lg:grid-cols-2">
            <Card className="border-slate-200/80 dark:border-slate-800">
              <CardHeader>
                <CardTitle className="text-base">Thông tin phiếu</CardTitle>
                <CardDescription>Khách, đơn, hiệu lực</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3 text-sm">
                <div className="flex justify-between gap-4">
                  <span className="text-slate-500">Khách</span>
                  <span className="text-right font-medium">
                    {cid != null ? (
                      <Link className="text-violet-700 hover:underline dark:text-violet-400" to={`/manager/sales/customers/${encodeURIComponent(String(cid))}`}>
                        {cname != null ? String(cname) : `#${cid}`}
                      </Link>
                    ) : (
                      "—"
                    )}
                    {cid != null ? <span className="ml-2 font-mono text-xs text-slate-500">#{cid}</span> : null}
                  </span>
                </div>
                <div className="flex justify-between gap-4">
                  <span className="text-slate-500">Đơn</span>
                  <span className="text-right">
                    {oid != null ? (
                      <Link className="font-mono text-violet-700 hover:underline dark:text-violet-400" to={`/manager/sales/orders/${encodeURIComponent(String(oid))}`}>
                        #{oid}
                      </Link>
                    ) : (
                      "—"
                    )}
                  </span>
                </div>
                <div className="flex justify-between gap-4">
                  <span className="text-slate-500">Hợp đồng</span>
                  <span className="font-mono text-right">{ctid != null ? String(ctid) : "—"}</span>
                </div>
                <div className="flex justify-between gap-4">
                  <span className="text-slate-500">Hiệu lực đến</span>
                  <span>{formatDate(until != null ? String(until) : "")}</span>
                </div>
                <div className="flex justify-between gap-4">
                  <span className="text-slate-500">Tạo lúc</span>
                  <span>{formatDate(created != null ? String(created) : "")}</span>
                </div>
              </CardContent>
            </Card>

            <Card className="border-slate-200/80 dark:border-slate-800">
              <CardHeader>
                <CardTitle className="text-base">Yêu cầu bảo hành</CardTitle>
                <CardDescription>Click một dòng để mở chi tiết — cập nhật trạng thái hoặc in biên nhận</CardDescription>
              </CardHeader>
              <CardContent className="p-0">
                {claims.length === 0 ? (
                  <p className="px-6 py-8 text-center text-sm text-slate-500">Chưa có claim.</p>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full border-collapse text-left text-sm">
                      <thead>
                        <tr className="border-b border-slate-200 bg-slate-50/90 text-[11px] font-semibold uppercase text-slate-500 dark:border-slate-800 dark:bg-slate-900/50">
                          <th className="px-4 py-2 pl-6">ID</th>
                          <th className="px-4 py-2">Trạng thái</th>
                          <th className="px-4 py-2">Biến thể</th>
                          <th className="px-4 py-2 text-right">Dự kiến</th>
                          <th className="px-4 py-2 pr-6">Mô tả</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                        {claims.map((row, idx) => {
                          const o = /** @type {Record<string, unknown>} */ (row && typeof row === "object" ? row : {});
                          const id = o.id ?? o.Id;
                          const cs = o.status ?? o.Status;
                          const vid = o.variantId ?? o.VariantId;
                          const cost = o.estimatedCost ?? o.EstimatedCost;
                          const desc = o.defectDescription ?? o.DefectDescription;
                          return (
                            <tr
                              key={id != null ? String(id) : idx}
                              tabIndex={id != null ? 0 : undefined}
                              className={cn(
                                id != null && "cursor-pointer hover:bg-violet-500/[0.04] dark:hover:bg-violet-500/[0.06]",
                                id == null && "hover:bg-slate-500/[0.03] dark:hover:bg-slate-500/[0.05]"
                              )}
                              onClick={(e) => {
                                if (id == null) return;
                                const el = e.target;
                                if (el instanceof Element && el.closest("a")) return;
                                navigate(CLAIM(id));
                              }}
                              onKeyDown={(e) => {
                                if (id == null) return;
                                if (e.key !== "Enter" && e.key !== " ") return;
                                e.preventDefault();
                                navigate(CLAIM(id));
                              }}
                              aria-label={id != null ? `Mở yêu cầu bảo hành số ${id}` : undefined}
                            >
                              <td className="px-4 py-2.5 pl-6 font-mono text-xs">
                                {id != null ? (
                                  <Link className="font-semibold text-violet-700 hover:underline dark:text-violet-400" to={CLAIM(id)}>
                                    #{id}
                                  </Link>
                                ) : (
                                  "—"
                                )}
                              </td>
                              <td className="px-4 py-2.5 text-xs">
                                {labelAdminWarrantyClaimStatus(cs != null ? String(cs) : "", claimStatusOptions)}
                              </td>
                              <td className="px-4 py-2.5 font-mono text-xs">{vid != null ? String(vid) : "—"}</td>
                              <td className="px-4 py-2.5 text-right text-xs tabular-nums">{formatMoneyVnd(cost)}</td>
                              <td className="max-w-[220px] truncate px-4 py-2.5 pr-6 text-xs text-slate-600 dark:text-slate-400" title={desc != null ? String(desc) : ""}>
                                {desc != null ? String(desc) : "—"}
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>
        </>
      ) : null}
    </div>
  );
}
