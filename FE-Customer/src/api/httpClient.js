import { API_BASE_URL } from './config'

export class ApiError extends Error {
  /**
   * @param {object} p
   * @param {number} p.status
   * @param {string} p.message
   * @param {string} [p.errorCode]
   * @param {Record<string, string[]>} [p.errors]
   */
  constructor({ status, message, errorCode, errors }) {
    super(message || 'Lỗi không xác định')
    this.name = 'ApiError'
    this.status = status
    this.errorCode = errorCode
    this.errors = errors
  }
}

function joinUrl(path) {
  const base = API_BASE_URL || ''
  const p = path.startsWith('/') ? path : `/${path}`
  return `${base}${p}`
}

/**
 * Gọi API JSON, envelope ResponseDto (success / data / message / errorCode / errors).
 * @param {string} path - ví dụ /api/store/auth/login
 * @param {RequestInit & { json?: object, token?: string | null }} [options]
 */
export async function apiJson(path, options = {}) {
  const { json: body, token, headers: extraHeaders, ...init } = options
  const headers = new Headers(extraHeaders)
  headers.set('Accept', 'application/json')
  if (body !== undefined) {
    headers.set('Content-Type', 'application/json')
  }
  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  const res = await fetch(joinUrl(path), {
    ...init,
    headers,
    body: body !== undefined ? JSON.stringify(body) : init.body,
  })

  const text = await res.text()
  let payload = null
  if (text) {
    try {
      payload = JSON.parse(text)
    } catch {
      payload = null
    }
  }

  if (res.status === 401 && (!payload || payload.success !== false)) {
    throw new ApiError({
      status: 401,
      message:
        payload?.message ||
        'Phiên đăng nhập hết hạn hoặc thông tin đăng nhập không đúng.',
      errorCode: payload?.errorCode || 'UNAUTHORIZED',
      errors: payload?.errors,
    })
  }

  if (!res.ok) {
    if (payload && payload.success === false) {
      throw new ApiError({
        status: res.status,
        message: payload.message || res.statusText,
        errorCode: payload.errorCode,
        errors: payload.errors,
      })
    }
    throw new ApiError({
      status: res.status,
      message: payload?.message || text || res.statusText,
      errorCode: payload?.errorCode,
    })
  }

  if (payload && payload.success === false) {
    throw new ApiError({
      status: res.status,
      message: payload.message || 'Yêu cầu thất bại',
      errorCode: payload.errorCode,
      errors: payload.errors,
    })
  }

  return payload
}
