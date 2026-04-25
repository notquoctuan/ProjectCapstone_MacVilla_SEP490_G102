import {
  createContext,
  useContext,
  useState,
  useCallback,
  useEffect,
  useMemo,
} from 'react'
import { assertApiBaseUrl } from '../api/config'
import {
  storeLogin,
  storeRegister,
  storeFetchMe,
} from '../api/store/storeAuthApi'
import {
  storeB2bRegister,
  storeB2bLogin,
  storeB2bFetchMe,
  storeB2bUpdateMe,
} from '../api/store/storeB2bAuthApi'
import {
  loadStoreSession,
  saveStoreSession,
  clearStoreSession,
  isSessionExpired,
} from '../lib/auth/storeSession'

const AuthContext = createContext(null)

/**
 * @param {string} token
 * @param {string | undefined} customerType - từ session; B2B dùng /b2b/auth/me
 */
async function fetchStoreCustomerMe(token, customerType) {
  if (customerType === 'B2B') {
    return storeB2bFetchMe(token)
  }
  return storeFetchMe(token)
}

/** @param {import('../api/store/storeAuthApi.js').StoreCustomerProfile & Record<string, unknown>} customer */
function toViewUser(customer) {
  if (!customer) return null
  return {
    id: customer.id,
    customerType: customer.customerType,
    fullName: customer.fullName,
    name: customer.fullName,
    email: customer.email ?? '',
    phone: customer.phone ?? '',
    companyName: typeof customer.companyName === 'string' ? customer.companyName : '',
    taxCode: typeof customer.taxCode === 'string' ? customer.taxCode : '',
    companyAddress:
      typeof customer.companyAddress === 'string' ? customer.companyAddress : '',
    debtBalance:
      typeof customer.debtBalance === 'number' ? customer.debtBalance : undefined,
  }
}

export function AuthProvider({ children }) {
  const [session, setSession] = useState(() => {
    const s = loadStoreSession()
    if (!s) return null
    if (isSessionExpired(s.expiresAtUtc)) {
      clearStoreSession()
      return null
    }
    return s
  })

  useEffect(() => {
    assertApiBaseUrl()
  }, [])

  useEffect(() => {
    saveStoreSession(session)
  }, [session])

  const applyAuthPayload = useCallback((payload) => {
    setSession({
      accessToken: payload.accessToken,
      expiresAtUtc: payload.expiresAtUtc,
      customer: payload.customer,
    })
  }, [])

  /** Đồng bộ profile từ server (sau khi đã có token hợp lệ) */
  const refreshProfile = useCallback(async () => {
    const s = session
    if (!s?.accessToken || isSessionExpired(s.expiresAtUtc)) return null
    const customer = await fetchStoreCustomerMe(
      s.accessToken,
      s.customer?.customerType
    )
    setSession((prev) =>
      prev
        ? { ...prev, customer }
        : {
            accessToken: s.accessToken,
            expiresAtUtc: s.expiresAtUtc,
            customer,
          }
    )
    return customer
  }, [session])

  useEffect(() => {
    if (!session?.accessToken || isSessionExpired(session.expiresAtUtc)) return
    let cancelled = false
    const token = session.accessToken
    const customerType = session.customer?.customerType
    fetchStoreCustomerMe(token, customerType)
      .then((customer) => {
        if (cancelled) return
        setSession((prev) =>
          prev && prev.accessToken === token ? { ...prev, customer } : prev
        )
      })
      .catch(() => {
        if (cancelled) return
        clearStoreSession()
        setSession(null)
      })
    return () => {
      cancelled = true
    }
  }, [session?.accessToken, session?.expiresAtUtc, session?.customer?.customerType])

  const loginWithPassword = useCallback(
    async (email, password) => {
      const payload = await storeLogin({
        email: email.trim(),
        password,
      })
      applyAuthPayload(payload)
      return payload
    },
    [applyAuthPayload]
  )

  const loginB2bWithPassword = useCallback(
    async (email, password) => {
      const payload = await storeB2bLogin({
        email: email.trim(),
        password,
      })
      applyAuthPayload(payload)
      return payload
    },
    [applyAuthPayload]
  )

  const registerAndLogin = useCallback(
    async ({ fullName, email, phone, password }) => {
      const payload = await storeRegister({
        fullName: fullName.trim(),
        email: email.trim(),
        phone: phone.trim(),
        password,
      })
      applyAuthPayload(payload)
      return payload
    },
    [applyAuthPayload]
  )

  const registerB2bAndLogin = useCallback(
    async ({
      fullName,
      email,
      phone,
      password,
      companyName,
      taxCode,
      companyAddress,
    }) => {
      const payload = await storeB2bRegister({
        fullName: fullName.trim(),
        email: email.trim(),
        phone: phone.trim(),
        password,
        companyName: companyName.trim(),
        taxCode: taxCode.trim(),
        companyAddress: companyAddress.trim(),
      })
      applyAuthPayload(payload)
      return payload
    },
    [applyAuthPayload]
  )

  const logout = useCallback(() => {
    clearStoreSession()
    setSession(null)
  }, [])

  /** Cập nhật hồ sơ B2B (PUT /b2b/auth/me), đồng bộ `session.customer`. */
  const updateB2bProfile = useCallback(
    async ({
      fullName,
      email,
      phone,
      companyName,
      taxCode,
      companyAddress,
    }) => {
      const s = session
      if (!s?.accessToken || isSessionExpired(s.expiresAtUtc)) {
        throw new Error('Vui lòng đăng nhập.')
      }
      if (s.customer?.customerType !== 'B2B') {
        throw new Error('Chỉ tài khoản doanh nghiệp mới cập nhật được hồ sơ này.')
      }
      const token = s.accessToken
      const customer = await storeB2bUpdateMe(token, {
        fullName: fullName.trim(),
        email: email.trim(),
        phone: phone.trim(),
        companyName: companyName.trim(),
        taxCode: taxCode.trim(),
        companyAddress: companyAddress.trim(),
      })
      setSession((prev) =>
        prev && prev.accessToken === token ? { ...prev, customer } : prev
      )
      return customer
    },
    [session]
  )

  const value = useMemo(() => {
    const expired = session ? isSessionExpired(session.expiresAtUtc) : true
    const user =
      session && !expired ? toViewUser(session.customer) : null
    const accessToken =
      session && !expired ? session.accessToken : null

    return {
      user,
      accessToken,
      isAuthenticated: Boolean(accessToken),
      loginWithPassword,
      loginB2bWithPassword,
      registerAndLogin,
      registerB2bAndLogin,
      refreshProfile,
      updateB2bProfile,
      logout,
    }
  }, [
    session,
    loginWithPassword,
    loginB2bWithPassword,
    registerAndLogin,
    registerB2bAndLogin,
    refreshProfile,
    updateB2bProfile,
    logout,
  ])

  return (
    <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
