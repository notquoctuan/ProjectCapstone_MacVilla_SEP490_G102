/**
 * Lỗi từ API envelope `{ success, message, errorCode }`.
 */
export class ApiRequestError extends Error {
  /**
   * @param {string} message
   * @param {string | null | undefined} errorCode
   * @param {unknown} [raw]
   */
  constructor(message, errorCode, raw = undefined) {
    super(message);
    this.name = "ApiRequestError";
    this.errorCode = errorCode ?? null;
    this.raw = raw;
  }
}

/**
 * Đọc JSON và trả `data` khi `success === true`.
 * @param {Response} res
 */
export async function parseApiEnvelope(res) {
  let json;
  try {
    json = await res.json();
  } catch {
    throw new ApiRequestError("Không đọc được phản hồi từ máy chủ.", "PARSE_ERROR");
  }

  if (!res.ok || json.success !== true) {
    throw new ApiRequestError(
      json.message || `Lỗi HTTP ${res.status}`,
      json.errorCode ?? null,
      json
    );
  }

  return json.data;
}
