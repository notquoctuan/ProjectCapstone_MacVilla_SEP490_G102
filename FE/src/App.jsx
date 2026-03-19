import { Routes, Route, useLocation } from 'react-router-dom'
import { AnnouncementBar } from './components/layout/AnnouncementBar'
import { Header } from './components/layout/Header'
import { Footer } from './components/layout/Footer'
import { HomePage } from './pages/HomePage'
import { ProductsPage } from './pages/ProductsPage'
import { ProductDetailPage } from './pages/ProductDetailPage'
import { CartPage } from './pages/CartPage'
import { CheckoutPage } from './pages/CheckoutPage'
import { OrderSuccessPage } from './pages/OrderSuccessPage'
import { LoginPage } from './pages/LoginPage'
import { RegisterPage } from './pages/RegisterPage'
import { RegisterPartnerPage } from './pages/RegisterPartnerPage'
import { AccountPage } from './pages/AccountPage'
import { AccountOrdersPage } from './pages/AccountOrdersPage'
import { AccountWarrantyPage } from './pages/AccountWarrantyPage'
import { OrderDetailPage } from './pages/OrderDetailPage'
import { PartnerLayout } from './components/layout/PartnerLayout'
import { PartnerDashboardPage } from './pages/PartnerDashboardPage'
import { PartnerOrdersPage } from './pages/PartnerOrdersPage'
import { PartnerOrderDetailPage } from './pages/PartnerOrderDetailPage'
import { PartnerProjectsPage } from './pages/PartnerProjectsPage'
import { PartnerProjectCreatePage } from './pages/PartnerProjectCreatePage'
import { PartnerQuotationCreatePage } from './pages/PartnerQuotationCreatePage'
import { PartnerQuotationHistoryPage } from './pages/PartnerQuotationHistoryPage'
import { PartnerQuotationDetailPage } from './pages/PartnerQuotationDetailPage'
import { AuthProvider } from './contexts/AuthContext'
import './App.css'

export default function App() {
  const location = useLocation()
  const path = location.pathname

  if (path === '/login') return <AuthProvider><LoginPage /></AuthProvider>
  if (path === '/register') return <AuthProvider><RegisterPage /></AuthProvider>
  if (path === '/register/partner') return <AuthProvider><RegisterPartnerPage /></AuthProvider>
  if (path.startsWith('/partner')) {
    return (
      <AuthProvider>
        <Routes>
          <Route path="/partner" element={<PartnerLayout />}>
            <Route index element={<PartnerDashboardPage />} />
            <Route path="dashboard" element={<PartnerDashboardPage />} />
            <Route path="orders" element={<PartnerOrdersPage />} />
            <Route path="orders/:orderId" element={<PartnerOrderDetailPage />} />
            <Route path="projects" element={<PartnerProjectsPage />} />
            <Route path="projects/create" element={<PartnerProjectCreatePage />} />
            <Route path="quotation/create" element={<PartnerQuotationCreatePage />} />
            <Route path="quotation/history" element={<PartnerQuotationHistoryPage />} />
            <Route path="quotation/:quotationId" element={<PartnerQuotationDetailPage />} />
          </Route>
        </Routes>
      </AuthProvider>
    )
  }

  return (
    <AuthProvider>
    <div className="min-h-screen bg-background-light dark:bg-background-dark text-slate-900 dark:text-slate-100 flex flex-col">
      <AnnouncementBar />
      <Header />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/products" element={<ProductsPage />} />
        <Route path="/products/:id" element={<ProductDetailPage />} />
        <Route path="/cart" element={<CartPage />} />
        <Route path="/checkout" element={<CheckoutPage />} />
        <Route path="/checkout/success" element={<OrderSuccessPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/register/partner" element={<RegisterPartnerPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/account/orders" element={<AccountOrdersPage />} />
        <Route path="/account/orders/:orderId" element={<OrderDetailPage />} />
        <Route path="/account/warranty" element={<AccountWarrantyPage />} />
      </Routes>
      <Footer />
    </div>
    </AuthProvider>
  )
}
