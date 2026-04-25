/**
 * Trang thông tin chi tiết / dashboard tài khoản
 */

export const ACCOUNT_QUICK_LINKS = [
  { icon: 'military_tech', label: 'Hạng thành viên', href: '#', colorClass: 'bg-primary/10 text-primary group-hover:bg-primary group-hover:text-white' },
  { icon: 'sell', label: 'Mã giảm giá', href: '#', colorClass: 'bg-accent-orange/10 text-accent-orange group-hover:bg-accent-orange group-hover:text-white' },
  { icon: 'history', label: 'Lịch sử', href: '/account/orders', colorClass: 'bg-primary/10 text-primary group-hover:bg-primary group-hover:text-white' },
  { icon: 'location_on', label: 'Sổ địa chỉ', href: '#', colorClass: 'bg-primary/10 text-primary group-hover:bg-primary group-hover:text-white' },
]

export const ACCOUNT_SIDEBAR_NAV = [
  { icon: 'dashboard', label: 'Tổng quan', href: '/account' },
  { icon: 'receipt_long', label: 'Lịch sử mua hàng', href: '/account/orders' },
  { icon: 'verified_user', label: 'Tra cứu bảo hành', href: '/account/warranty' },
  { icon: 'redeem', label: 'Hội viên & Ưu đãi', href: '#' },
  { icon: 'manage_accounts', label: 'Thông tin tài khoản', href: '/account' },
]

export const ACCOUNT_RECENT_ORDERS = [
  {
    id: 'MV12345',
    title: 'Bếp từ Bosch PID675DC1E',
    image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDGaUFp6SOeoIQj9qwr_aI5Lo1YuqLz5j-YhOktblKLRDIvZFbLUn3rVI9FA_W0Na6r5o8RC38m7hvo4CTFJswBhaMeWNA0-C9G_1sur5zCnMFd3HeYER53QSoo5IvDF_AHCa5vj-AQlFPjchuM0Z4TBywe4Of1zFAeZzBbEtlo_9NdNlKiKePlDbsz-M-uTb8bUFQTiQYQWr9FO9cICfWcH0DKofgsc2iz5KBKcaSXH-8TNEovLQJHZY4W8swD2KZZLOBZXGECCy_w',
    imageAlt: 'Bếp từ Bosch',
    date: '24/10/2023',
    status: 'Delivered',
    statusLabel: 'Đã giao',
    statusClass: 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400',
    total: 24500000,
  },
  {
    id: 'MV12352',
    title: 'Chậu rửa treo tường TOTO',
    image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDg33_TTM3H7_tdEd3q1ra-RCrKydRNlEF8MTkkc3pdnjfkmJTV8MkZG2dywvgvS0C8y-XFgHnVQjzJmDP6ACH6Qc2mATIY86sTDM1n9dl7NjK2vSI_A7fFaw4tu0hUzmi_9DGTszRBW_53Wq19v3tjMuY4D4CJHsgUaxMPhvACUmbNuw66G5v4wtq_V4sq0PXLy3nr7IffkwJB3KMElqoSRAmJ0-qbk6f0F6n36jPX8nzZDjD2rNb7lxA5YeOO5QBf8S2zp4hjGu0z',
    imageAlt: 'Chậu rửa TOTO',
    date: '02/11/2023',
    status: 'Shipping',
    statusLabel: 'Đang giao',
    statusClass: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400',
    total: 8200000,
  },
  {
    id: 'MV12360',
    title: 'Bình nóng lạnh Ariston Andris2 RS 15L',
    image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBSRx74Qtyaic6R0XQkDBjGz10uzDjoT-vbwK8YYlYZb322RRkslcEuhsso8d_KK06cTZWsIl7LlMKGr2GcrO9Vto0YIGitYKmkWZeRt09G26-iJ_4Rqzi7K6pUsPyJlTYiqaslgxL1stb4QJjOxRjvg791MxeB4woYulPwIil9IbqDjRnaENELAQm7K-Gq1hqe8_CpqlLXdxfGs3wNOvxx-5TUTbhaLudlCtUH6U3rY9rC6xUe3CQLmStaBdy2XD5cAvirDSPp-_hL',
    imageAlt: 'Bình nóng lạnh Ariston',
    date: '15/11/2023',
    status: 'Delivered',
    statusLabel: 'Đã giao',
    statusClass: 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400',
    total: 4150000,
  },
]

const DEFAULT_AVATAR = 'https://lh3.googleusercontent.com/aida-public/AB6AXuC900buh_phqmBdEB3m5RUVqr6bFQrfRRylbEvDV_lH-gCl1CNEqQ-YWq_lYjl-2r3WrSLjM1-KrYb86GDrrHup4V5tImD0P7DGJDpjhTfbp3u0anAnYI9dK0foNgMaiu2E3BUiFfFcr7NvXLhIr0ZFl1qzNioeg6qBWvUAr-jbqBmw7luP3TWKm3B3HMXPPLhzDWyXdmbKuWYb6K-VtMAKU_qwaejJp49sQ0iRrjWKmORcugHSVrUljgwjfmVouDoIuESOQVqKhmTu'
const PROFILE_AVATAR = 'https://lh3.googleusercontent.com/aida-public/AB6AXuDzCPmz4WpK_-T9NmRsnlXhzizq2s4kL6-CWI0_8LYcB2U2ZL_okLp-OX8zaM4KSGBlPr4yCvnTY8NInRD17uJq8yVzovP0QDCygLHibAAN8IgA7Tnz-p65fYo5Xa8N1TlYc5zejTHEaQb081DSB-xetmEbLkbKDdCsi-YbKARE0d0i_iMTc6oxonxyA4KaAKA23HHRn9NhL-P4p4lrh6xpobEeK_Es99N8fCtB5K6FyNf-ohATDH6QYzsowkhgwHwi7jOsHbWDqiIc'

/** Lịch sử mua hàng - đơn đầy đủ (header + sản phẩm + actions) */
export const PURCHASE_HISTORY_FILTERS = [
  { id: 'all', label: 'Tất cả' },
  { id: 'pending', label: 'Chờ xác nhận' },
  { id: 'shipping', label: 'Đang giao' },
  { id: 'delivered', label: 'Đã giao' },
  { id: 'cancelled', label: 'Đã hủy' },
]

export const PURCHASE_HISTORY_ORDERS = [
  {
    id: 'MV-882910',
    date: '12/05/2024',
    status: 'delivered',
    statusLabel: 'ĐÃ GIAO THÀNH CÔNG',
    statusIcon: 'check_circle',
    statusClass: 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400',
    items: [
      { name: 'Máy lọc không khí Macvilla Pure Air X1 - Trắng', qty: 1, price: 4500000, image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBri26AUkpt2feZdHxWJ7CUXWBxfjC4SvDz0SXCkKsYWwSgckMy6rCwrb734ZPOY3u6Fqaw9wtiWdz9dITVnk1Q0HvjjaBLmj1IyL3FSVB-kQ6eDWMa2NkKIrzAh4F9CGtQTW2R4yKjcf1KnRww5MkY_uVY2e2tf03l0nDJlVDX0Nj5Ifl8OiNpSeGy9kvz9VrSoIBLjoq82sdUf6VXMFPlAqWTMesKIZe9KdbOlhdW8xF-lVnxTrgnoNAPAqVFmN77NRprzuRO-mDR', imageAlt: 'Máy lọc không khí' },
    ],
    total: 4500000,
    actions: ['detail', 'warranty', 'reorder'],
  },
  {
    id: 'MV-771204',
    date: '20/05/2024',
    status: 'shipping',
    statusLabel: 'ĐANG GIAO HÀNG',
    statusIcon: 'local_shipping',
    statusClass: 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-400',
    items: [
      { name: 'Bếp từ đôi Macvilla Smart Cook Pro S2', qty: 1, price: 12900000, image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDB_zrj1PrelBcr-3Zhj6TI9K0Z2JI1Q90q4u_BKCzuBa2yCbBUpwmg7xbzlPK5GKFYKFaEqPzfIbN7XvnNPNyUpDjI-HLDVvLalFZNcEa9eEUar7JJjEQceaklkFTBsBSpbP4XiWLXSwLeh6zCy_dXmmALjLjy49lUKQn-SZG9R6d7CaYOEsxz97-7zkiO9yzXkXM0u2Ovm2kxSxRRBtdsPIV0zDIsDvTzOX0akkhW0vaW7nW650tXiqgNqNnqoe_xl0I6nvkcVTu-', imageAlt: 'Bếp từ đôi' },
    ],
    total: 12900000,
    actions: ['track', 'detail'],
  },
  {
    id: 'MV-662512',
    date: '05/05/2024',
    status: 'cancelled',
    statusLabel: 'ĐÃ HỦY',
    statusIcon: 'cancel',
    statusClass: 'bg-slate-100 dark:bg-slate-800 text-slate-500 dark:text-slate-400',
    items: [
      { name: 'Ấm siêu tốc Macvilla Eco Heat - 1.7L', qty: 2, price: 600000, image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuA1KKSJ9bb9dMWLSkPDHUeFE2Z5LXgO6fSpEwKIRZB3UL1BwuKNIDhBjBMOSEsI2e_Mz9RzbJFRG1d8t044IYqN5eayeA1CkP3cF_QLTMqZcu6zBSBpq91M1lSPn_mz3ijCPc-eGTAp230eZ7mDVuONlD6hBYc7T-FYNaD9s8yIS0c_e99yy24cnF7lxTiq2j73i8qHXAUOKs8ohONiEZwJzJ6VZHyZCkePgFHukEnufMWO_LHjyMw6eCkdA1agyo284Hw0dGIiPM8e', imageAlt: 'Ấm siêu tốc' },
    ],
    total: 2400000,
    actions: ['detail', 'reorder'],
  },
]

export { DEFAULT_AVATAR, PROFILE_AVATAR }
