import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'

const PARTNER_HERO_IMAGE =
  'https://lh3.googleusercontent.com/aida-public/AB6AXuAU0NoBEHuKk4bPkwFS41GvnMdRQPh4qzrqzpX7Ywea5wRW6dR2TEjtb9pxJp7VAmTEO5jtkHGXdBwhvHDAnBuynEjAT4dITmBnfCZIf2e4Z4srbdHfqz8c_Om2FBw3t2M79l2wsRKlMW1D9EXUJslcHcOy-d0c_wTRd6z3BqHkEC51eQ-21dd624rLLOzh_C61-ahiSk3tD8upW1m_qTGdhZawbqZaVt-0ykJt1RaP12XOtfMw_1ZzqcCDvmHgpitqpPd-zYrCivXM'

const PARTNER_BENEFITS = [
  {
    icon: 'percent',
    title: 'Chiết khấu ưu đãi',
    desc: 'Chính sách giá hấp dẫn nhất thị trường cho đại lý và nhà thầu.',
  },
  {
    icon: 'support_agent',
    title: 'Hỗ trợ kỹ thuật 24/7',
    desc: 'Đội ngũ kỹ sư giàu kinh nghiệm sẵn sàng tư vấn giải pháp thi công.',
  },
  {
    icon: 'local_shipping',
    title: 'Ưu tiên vận chuyển',
    desc: 'Hệ thống logistic chuyên nghiệp, đảm bảo đúng tiến độ dự án.',
  },
]

const ACTIVITY_FIELDS = [
  { value: '', label: 'Chọn lĩnh vực hoạt động', disabled: true },
  { value: 'construction', label: 'Thi công xây dựng' },
  { value: 'interior', label: 'Thiết kế nội thất' },
  { value: 'commerce', label: 'Thương mại vật liệu xây dựng' },
  { value: 'architect', label: 'Kiến trúc & Tư vấn' },
  { value: 'other', label: 'Lĩnh vực khác' },
]

export function RegisterPartnerPage() {
  const [showPassword, setShowPassword] = useState(false)

  const handleSubmit = (e) => {
    e.preventDefault()
    // TODO: integrate partner registration
  }

  return (
    <div className="flex min-h-screen bg-background-light dark:bg-background-dark text-slate-900 dark:text-slate-100">
      {/* Cột trái: Hình ảnh & Lợi ích */}
      <div className="hidden lg:flex lg:w-1/2 relative flex-col justify-between p-12 overflow-hidden bg-primary">
        <div className="absolute inset-0 z-0">
          <div className="absolute inset-0 bg-primary/80 z-10" />
          <div
            className="w-full h-full bg-cover bg-center"
            style={{ backgroundImage: `url("${PARTNER_HERO_IMAGE}")` }}
            role="img"
            aria-label="Kiến trúc văn phòng cao cấp"
          />
        </div>
        <div className="relative z-20">
          <div className="flex items-center gap-3 mb-12">
            <div className="size-10 bg-white rounded-lg flex items-center justify-center text-primary">
              <Icon name="kitchen" className="size-6" />
            </div>
            <h2 className="text-white text-2xl font-bold tracking-tight">
              HDG Việt Hàn
            </h2>
          </div>
          <div className="max-w-md">
            <h1 className="text-white text-4xl font-black leading-tight mb-6">
              Hợp tác phát triển bền vững cùng HDG Việt Hàn
            </h1>
            <p className="text-white/80 text-lg mb-10">
              Gia nhập mạng lưới đối tác chiến lược để nhận các giải pháp nhôm
              kính và nội thất cao cấp hàng đầu Việt Nam.
            </p>
          </div>
        </div>
        <div className="relative z-20 grid gap-6">
          {PARTNER_BENEFITS.map((item) => (
            <div
              key={item.icon}
              className="flex items-start gap-4 p-4 rounded-xl bg-white/10 backdrop-blur-sm border border-white/10"
            >
              <div className="bg-white/20 p-2 rounded-lg text-white">
                <Icon name={item.icon} className="text-xl" />
              </div>
              <div>
                <h3 className="text-white font-bold">{item.title}</h3>
                <p className="text-white/70 text-sm">{item.desc}</p>
              </div>
            </div>
          ))}
        </div>
        <div className="relative z-20 pt-8 border-t border-white/20">
          <p className="text-white/60 text-sm">
            © 2024 HDG Việt Hàn. Tất cả quyền được bảo lưu.
          </p>
        </div>
      </div>

      {/* Cột phải: Form đăng ký */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center p-6 md:p-12 lg:p-20 overflow-y-auto custom-scrollbar">
        <div className="w-full max-w-xl">
          {/* Logo mobile */}
          <Link to="/" className="lg:hidden flex items-center gap-3 mb-8">
            <div className="size-8 bg-primary rounded flex items-center justify-center text-white">
              <Icon name="kitchen" className="size-5" />
            </div>
            <h2 className="text-slate-900 dark:text-white text-xl font-bold tracking-tight">
              HDG Việt Hàn
            </h2>
          </Link>

          <div className="mb-8">
            <h2 className="text-3xl font-black text-slate-900 dark:text-white mb-2">
              Đăng ký Đối tác
            </h2>
            <p className="text-slate-500 dark:text-slate-400">
              Điền đầy đủ thông tin bên dưới để gửi yêu cầu tham gia mạng lưới
              đối tác công trình.
            </p>
          </div>

          <form className="grid grid-cols-1 md:grid-cols-2 gap-6" onSubmit={handleSubmit}>
            <div className="md:col-span-2 space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Tên doanh nghiệp <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="apartment"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg"
                />
                <input
                  type="text"
                  placeholder="Công ty CP Xây dựng ABC"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Mã số thuế <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="badge"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg"
                />
                <input
                  type="text"
                  placeholder="0101234567"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Người đại diện <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="person"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg"
                />
                <input
                  type="text"
                  placeholder="Nguyễn Văn A"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Số điện thoại <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="call"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg"
                />
                <input
                  type="tel"
                  placeholder="0901 234 567"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Email công ty <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="mail"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg"
                />
                <input
                  type="email"
                  placeholder="contact@company.vn"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="md:col-span-2 space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Lĩnh vực hoạt động <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="category"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg pointer-events-none"
                />
                <select className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all appearance-none text-slate-900 dark:text-slate-100">
                  {ACTIVITY_FIELDS.map((opt) => (
                    <option
                      key={opt.value || 'placeholder'}
                      value={opt.value}
                      disabled={opt.disabled}
                    >
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div className="md:col-span-2 space-y-2">
              <label className="text-sm font-semibold text-slate-700 dark:text-slate-300 ml-1">
                Mật khẩu <span className="text-red-500">*</span>
              </label>
              <div className="relative">
                <Icon
                  name="lock"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-lg"
                />
                <input
                  type={showPassword ? 'text' : 'password'}
                  placeholder="••••••••"
                  className="w-full pl-10 pr-12 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all text-slate-900 dark:text-slate-100"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((v) => !v)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-primary"
                  aria-label={showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'}
                >
                  <Icon name={showPassword ? 'visibility_off' : 'visibility'} className="text-lg" />
                </button>
              </div>
              <p className="text-xs text-slate-500 mt-1">
                Sử dụng ít nhất 8 ký tự bao gồm chữ cái và số.
              </p>
            </div>

            <div className="md:col-span-2 flex items-start gap-3 mt-2">
              <input
                id="terms-partner"
                type="checkbox"
                className="mt-1 size-4 rounded border-slate-300 text-primary focus:ring-primary"
              />
              <label
                htmlFor="terms-partner"
                className="text-sm text-slate-600 dark:text-slate-400 leading-tight"
              >
                Tôi đồng ý với các{' '}
                <a href="#" className="text-primary font-medium hover:underline">
                  Điều khoản dịch vụ
                </a>{' '}
                và{' '}
                <a href="#" className="text-primary font-medium hover:underline">
                  Chính sách bảo mật
                </a>{' '}
                dành cho đối tác của HDG Việt Hàn.
              </label>
            </div>

            <div className="md:col-span-2 pt-4">
              <button
                type="submit"
                className="w-full py-4 bg-primary text-white font-bold rounded-xl shadow-lg shadow-primary/30 hover:bg-primary/90 transition-all flex items-center justify-center gap-2"
              >
                <span>Gửi yêu cầu đăng ký</span>
                <Icon name="arrow_forward" />
              </button>
            </div>

            <div className="md:col-span-2 text-center mt-6">
              <p className="text-slate-600 dark:text-slate-400">
                Đã có tài khoản đối tác?{' '}
                <Link to="/login" className="text-primary font-bold hover:underline">
                  Đăng nhập ngay
                </Link>
              </p>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}
