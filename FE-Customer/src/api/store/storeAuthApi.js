import { apiJson } from '../httpClient'

/**
 * @typedef {object} StoreCustomerProfile
 * @property {number} id
 * @property {string} customerType
 * @property {string} fullName
 * @property {string | null} email
 * @property {string} phone
 */

/**
 * @typedef {object} StoreAuthPayload
 * @property {string} accessToken
 * @property {string} expiresAtUtc - ISO 8601
 * @property {StoreCustomerProfile} customer
 */

/**
 * @param {{ fullName: string, email: string, phone: string, password: string }} body
 * @returns {Promise<StoreAuthPayload>}
 */
export function storeRegister(body) {
  return apiJson('/api/store/auth/register', {
    method: 'POST',
    json: body,
  }).then((r) => r.data)
}

/**
 * @param {{ email: string, password: string }} body
 * @returns {Promise<StoreAuthPayload>}
 */
export function storeLogin(body) {
  return apiJson('/api/store/auth/login', {
    method: 'POST',
    json: body,
  }).then((r) => r.data)
}

/**
 * @param {string} token
 * @returns {Promise<StoreCustomerProfile>}
 */
export function storeFetchMe(token) {
  return apiJson('/api/store/auth/me', {
    method: 'GET',
    token,
  }).then((r) => r.data)
}

/**
 * @param {string} token
 * @param {{ fullName: string, email: string, phone: string }} body
 * @returns {Promise<StoreCustomerProfile>}
 */
export function storeUpdateMe(token, body) {
  return apiJson('/api/store/auth/me', {
    method: 'PUT',
    token,
    json: body,
  }).then((r) => r.data)
}
