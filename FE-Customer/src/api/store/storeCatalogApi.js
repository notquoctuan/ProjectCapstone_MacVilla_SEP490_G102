import { apiJson } from '../httpClient'

/**
 * @typedef {object} CategoryTreeNode
 * @property {number} id
 * @property {number | null} parentId
 * @property {string} name
 * @property {string} slug
 * @property {CategoryTreeNode[]} children
 */

/**
 * GET /api/store/categories — cây danh mục (công khai).
 * @returns {Promise<CategoryTreeNode[]>}
 */
export function storeFetchCategoryTree() {
  return apiJson('/api/store/categories').then((r) => r.data)
}

/**
 * @param {{
 *   page?: number,
 *   pageSize?: number,
 *   categoryId?: number | null,
 *   includeSubcategories?: boolean,
 *   search?: string | null,
 * }} params
 * @returns {Promise<{ items: object[], totalCount: number, page: number, pageSize: number }>}
 */
export function storeFetchProducts(params = {}) {
  const q = new URLSearchParams()
  const page = params.page ?? 1
  const pageSize = params.pageSize ?? 12
  q.set('page', String(page))
  q.set('pageSize', String(pageSize))
  if (
    params.categoryId != null &&
    params.categoryId !== '' &&
    Number.isFinite(Number(params.categoryId))
  ) {
    q.set('categoryId', String(params.categoryId))
  }
  if (params.includeSubcategories === false) {
    q.set('includeSubcategories', 'false')
  } else {
    q.set('includeSubcategories', 'true')
  }
  if (params.search && String(params.search).trim()) {
    q.set('search', String(params.search).trim())
  }
  return apiJson(`/api/store/products?${q.toString()}`).then((r) => r.data)
}

/**
 * GET /api/store/products/{slugOrId} — chi tiết (slug hoặc id).
 * @param {string | number} slugOrId
 * @returns {Promise<object>} StoreProductDetailDto (camelCase)
 */
export function storeFetchProductDetail(slugOrId) {
  const seg = encodeURIComponent(String(slugOrId ?? '').trim())
  if (!seg) {
    return Promise.reject(new Error('Thiếu mã hoặc slug sản phẩm.'))
  }
  return apiJson(`/api/store/products/${seg}`).then((r) => r.data)
}

/**
 * GET /api/store/variants/by-sku/{sku} — tra cứu biến thể theo SKU (công khai).
 * @param {string} sku
 * @returns {Promise<{
 *   id: number
 *   productId: number
 *   productName: string
 *   productSlug: string
 *   sku: string
 *   variantName: string
 *   retailPrice: number
 *   imageUrl: string | null
 *   quantityAvailable?: number
 * }>}
 */
export function storeFetchVariantBySku(sku) {
  const seg = encodeURIComponent(String(sku ?? '').trim())
  if (!seg) {
    return Promise.reject(new Error('Vui lòng nhập SKU.'))
  }
  return apiJson(`/api/store/variants/by-sku/${seg}`).then((r) => r.data)
}
