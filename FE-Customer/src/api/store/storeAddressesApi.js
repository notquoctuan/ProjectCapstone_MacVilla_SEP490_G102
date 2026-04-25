import { apiJson } from '../httpClient'

/**
 * GET /api/store/me/addresses
 * @param {string} token
 * @returns {Promise<object[]>} StoreAddressDto[] (camelCase)
 */
export function storeFetchAddresses(token) {
  return apiJson('/api/store/me/addresses', {
    method: 'GET',
    token,
  }).then((r) => (Array.isArray(r.data) ? r.data : []))
}

/**
 * POST /api/store/me/addresses
 *
 * Body (JSON): receiverName, receiverPhone, addressLine, isDefault.
 *
 * Envelope: { success, data: { id, receiverName, receiverPhone, addressLine, isDefault }, message, errorCode, errors, … }
 *
 * @param {string} token
 * @param {{ receiverName: string, receiverPhone: string, addressLine: string, isDefault?: boolean }} body
 * @returns {Promise<{ id: number, receiverName: string, receiverPhone: string, addressLine: string, isDefault: boolean }|undefined>}
 */
export function storeCreateAddress(token, body) {
  return apiJson('/api/store/me/addresses', {
    method: 'POST',
    token,
    json: {
      receiverName: String(body.receiverName ?? '').trim(),
      receiverPhone: String(body.receiverPhone ?? '').trim(),
      addressLine: String(body.addressLine ?? '').trim(),
      isDefault: Boolean(body.isDefault),
    },
  }).then((r) => r.data)
}
