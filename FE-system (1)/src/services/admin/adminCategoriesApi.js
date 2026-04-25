import { apiUrl } from "@/config/api.config";
import { bearerHeaders } from "@/services/api/http";
import { parseApiEnvelope } from "@/services/api/apiEnvelope";

/**
 * @param {Record<string, string | number | undefined | null>} params
 */
function buildQueryString(params) {
  const q = new URLSearchParams();
  Object.entries(params).forEach(([key, val]) => {
    if (val === undefined || val === null || val === "") return;
    q.set(key, String(val));
  });
  const s = q.toString();
  return s ? `?${s}` : "";
}

/**
 * @typedef {object} AdminCategoryTreeNode
 * @property {number} id
 * @property {number | null} parentId
 * @property {string} name
 * @property {string} slug
 * @property {string | null} [imageUrl]
 * @property {AdminCategoryTreeNode[]} [children]
 */

/**
 * GET /api/admin/categories/tree
 * @param {string} accessToken
 * @returns {Promise<AdminCategoryTreeNode[]>}
 */
export async function fetchAdminCategoryTree(accessToken) {
  if (!accessToken) throw new Error("Chưa có access token.");

  const res = await fetch(apiUrl("/api/admin/categories/tree"), {
    method: "GET",
    headers: {
      Accept: "*/*",
      ...bearerHeaders(accessToken),
    },
  });

  return parseApiEnvelope(res);
}

/**
 * @typedef {object} AdminCategoryListItem
 * @property {number} id
 * @property {number | null} parentId
 * @property {string} name
 * @property {string} slug
 * @property {string | null} [imageUrl]
 */

/**
 * @typedef {object} AdminCategoryListResult
 * @property {AdminCategoryListItem[]} items
 * @property {number} totalCount
 * @property {number} page
 * @property {number} pageSize
 */

/**
 * GET /api/admin/categories
 * @param {string} accessToken
 * @param {{ page?: number; pageSize?: number }} [query]
 * @returns {Promise<AdminCategoryListResult>}
 */
export async function fetchAdminCategoriesList(accessToken, query = {}) {
  if (!accessToken) throw new Error("Chưa có access token.");

  const qs = buildQueryString({
    page: query.page,
    pageSize: query.pageSize,
  });

  const res = await fetch(apiUrl(`/api/admin/categories${qs}`), {
    method: "GET",
    headers: {
      Accept: "*/*",
      ...bearerHeaders(accessToken),
    },
  });

  return parseApiEnvelope(res);
}

/**
 * Đếm tổng số nút (gồm gốc và con).
 * @param {AdminCategoryTreeNode[]} nodes
 */
export function countCategoryTreeNodes(nodes) {
  if (!nodes?.length) return 0;
  let n = 0;
  for (const node of nodes) {
    n += 1;
    if (node.children?.length) n += countCategoryTreeNodes(node.children);
  }
  return n;
}

/**
 * Thu thập id các nút có con (để "Mở tất cả").
 * @param {AdminCategoryTreeNode[]} nodes
 * @param {Set<number>} [into]
 */
export function collectCategoryIdsWithChildren(nodes, into = new Set()) {
  if (!nodes?.length) return into;
  for (const node of nodes) {
    if (node.children?.length) {
      into.add(node.id);
      collectCategoryIdsWithChildren(node.children, into);
    }
  }
  return into;
}

/**
 * Dẹt cây danh mục thành các option cho select (nhãn có thụt cấp).
 * @param {AdminCategoryTreeNode[]} nodes
 * @param {number} [depth]
 * @returns {{ id: number; label: string }[]}
 */
export function flattenCategoryTreeForSelect(nodes, depth = 0) {
  /** @type {{ id: number; label: string }[]} */
  const out = [];
  if (!nodes?.length) return out;
  const prefix = depth > 0 ? "\u2003".repeat(depth) : "";
  for (const node of nodes) {
    out.push({ id: node.id, label: `${prefix}${node.name}` });
    if (node.children?.length) {
      out.push(...flattenCategoryTreeForSelect(node.children, depth + 1));
    }
  }
  return out;
}
