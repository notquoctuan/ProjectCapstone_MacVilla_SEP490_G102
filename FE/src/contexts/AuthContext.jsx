import { createContext, useContext, useState, useCallback, useEffect } from 'react'

const STORAGE_KEY = 'hdg_viethan_user'

const AuthContext = createContext(null)

function loadUser() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null
    return JSON.parse(raw)
  } catch {
    return null
  }
}

function saveUser(user) {
  if (user) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user))
  } else {
    localStorage.removeItem(STORAGE_KEY)
  }
}

export function AuthProvider({ children }) {
  const [user, setUserState] = useState(loadUser)

  useEffect(() => {
    saveUser(user)
  }, [user])

  const login = useCallback((userData) => {
    setUserState(userData)
  }, [])

  const logout = useCallback(() => {
    setUserState(null)
  }, [])

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
