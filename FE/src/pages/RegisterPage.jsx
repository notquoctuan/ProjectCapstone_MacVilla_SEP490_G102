import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Icon } from '../components/ui/Icon'

const REGISTER_HERO_IMAGE =
  'https://lh3.googleusercontent.com/aida-public/AB6AXuCSIqbGesgtDQfGxltUnWreISY-YSJNwRFDGAen8rA0z404Jc6OT8xCQOM7c_JSKVCYdPiBKK6r4Jcqhozj0dBqjTN5X6hHDAtTJIvJYkPNFKpBG58sf71kCgyh03f6jSa6QV_BgQdJlLf1IG4BNMZA0ovzP0fLtc0pIrpSlJoaKuW2VyeNOWdT4wQmrgJMMLVG9XCNWLTBCtKEyp2OIk2jxDylGKcOycHISqLUCOWjx7B_5OdM86C5GxUyyF-xcpOc8AD9e--DHcsW'

const GOOGLE_ICON =
  'https://lh3.googleusercontent.com/aida-public/AB6AXuDoMOiMlqqysWphmahfENUViNtq2h0VEaN8XhFg7ajamLDPXasaYcgnhoUNp4mNpEk0EQ4_xOGxaxSGWJ2LCUvTBHmZSBpHZUPPYqKh2bB4UxMJJPU68E4Ak4foAOWIg9QuWCm4yVI7iSZkmdb_IndCUeo3s3pYf2_Zw-XL2mN3Z7YIOw8jIEIU8UDDgp7PkDr5QSIVAx0j3hmSNvoEfBYQQV8UU-253qYCRZTmvg803RZl7VJNAdZSv6hcUDXkRZbLzZOhwrqufLtI'
const FACEBOOK_ICON =
  'https://lh3.googleusercontent.com/aida-public/AB6AXuB3Y3YKoJIpwd5LfKt7GBHcALZgHU82KCUBepDjmzgkhbQOs1-9MfNFOy38TABET3QB7bua1NsPcfWBeeIvwbbhD2sTY_XB4wMDRmvIuQKhQg3XOAl3C5M06i44hwbEFeZmPkGfgUkYWmb9pHQqh-4LPhn-r-8eXxaLlCI-Y0WDA9qid2X0iWcuK94XU1XjTuP86-DSrr5qqdGH6nrqeuFsFWyWcBXmoKMtJxuBO0PPSZpOFwlxPN_aim5hXWWCLXGbFkk-__eMdWR5'

export function RegisterPage() {
  const [showPassword, setShowPassword] = useState(false)

  const handleSubmit = (e) => {
    e.preventDefault()
    // TODO: integrate registration
  }

  return (
    <div className="flex min-h-screen flex-col lg:flex-row bg-background-light dark:bg-background-dark text-slate-900 dark:text-slate-100">
      {/* Left: Hero - hidden on mobile */}
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden bg-primary">
        <div
          className="absolute inset-0 bg-cover bg-center"
          style={{ backgroundImage: `url("${REGISTER_HERO_IMAGE}")` }}
          role="img"
          aria-label="Phòng tắm sang trọng với đá cẩm thạch và thiết bị vàng"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-primary/80 to-transparent" />
        <div className="relative z-10 flex flex-col justify-end p-20 text-white">
          <div className="flex items-center gap-3 mb-8">
            <div className="size-10 bg-white rounded-lg flex items-center justify-center p-2">
              <Icon name="kitchen" className="text-primary text-2xl" />
            </div>
            <span className="text-2xl font-bold tracking-tight">
              HDG Việt Hàn
            </span>
          </div>
          <h1 className="text-5xl font-black leading-tight mb-6">
            Nâng tầm không gian sống đẳng cấp
          </h1>
          <p className="text-xl text-slate-100 max-w-lg">
            Gia nhập cộng đồng HDG Việt Hàn để sở hữu những giải pháp thiết bị
            phòng tắm và nhà bếp tinh tế nhất.
          </p>
        </div>
      </div>

      {/* Right: Form */}
      <div className="flex flex-1 flex-col justify-center px-6 py-12 lg:px-20 xl:px-32">
        <div className="mx-auto w-full max-w-md">
          {/* Mobile logo */}
          <Link
            to="/"
            className="lg:hidden flex items-center gap-3 mb-10"
          >
            <div className="size-8 bg-primary rounded flex items-center justify-center p-1.5">
              <Icon name="kitchen" className="text-white text-xl" />
            </div>
            <span className="text-xl font-bold tracking-tight text-primary">
              HDG Việt Hàn
            </span>
          </Link>

          <div className="mb-10">
            <h2 className="text-3xl font-black text-slate-900 dark:text-slate-50 tracking-tight">
              Tạo tài khoản mới
            </h2>
            <p className="mt-2 text-slate-600 dark:text-slate-400">
              Khám phá thế giới nội thất cao cấp ngay hôm nay.
            </p>
          </div>

          <form className="space-y-5" onSubmit={handleSubmit}>
            <div>
              <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
                Họ và tên
              </label>
              <div className="relative">
                <Icon
                  name="person"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xl"
                />
                <input
                  type="text"
                  placeholder="Nhập họ và tên"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition-all outline-none text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
                  Email
                </label>
                <div className="relative">
                  <Icon
                    name="mail"
                    className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xl"
                  />
                  <input
                    type="email"
                    placeholder="email@viethan.vn"
                    className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition-all outline-none text-slate-900 dark:text-slate-100"
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
                  Số điện thoại
                </label>
                <div className="relative">
                  <Icon
                    name="call"
                    className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xl"
                  />
                  <input
                    type="tel"
                    placeholder="09xx xxx xxx"
                    className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition-all outline-none text-slate-900 dark:text-slate-100"
                  />
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
                Mật khẩu
              </label>
              <div className="relative">
                <Icon
                  name="lock"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xl"
                />
                <input
                  type={showPassword ? 'text' : 'password'}
                  placeholder="••••••••"
                  className="w-full pl-10 pr-12 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition-all outline-none text-slate-900 dark:text-slate-100"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((v) => !v)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-primary"
                  aria-label={showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'}
                >
                  <Icon name={showPassword ? 'visibility_off' : 'visibility'} />
                </button>
              </div>
            </div>

            <div>
              <label className="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-1.5">
                Xác nhận mật khẩu
              </label>
              <div className="relative">
                <Icon
                  name="shield_lock"
                  className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xl"
                />
                <input
                  type="password"
                  placeholder="••••••••"
                  className="w-full pl-10 pr-4 py-3 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent transition-all outline-none text-slate-900 dark:text-slate-100"
                />
              </div>
            </div>

            <div className="flex items-start gap-3">
              <input
                id="terms"
                type="checkbox"
                className="mt-1 h-4 w-4 rounded border-slate-300 text-primary focus:ring-primary"
              />
              <label
                htmlFor="terms"
                className="text-sm text-slate-600 dark:text-slate-400"
              >
                Tôi đồng ý với{' '}
                <a
                  href="#"
                  className="text-primary font-semibold hover:underline"
                >
                  Điều khoản dịch vụ
                </a>{' '}
                và{' '}
                <a
                  href="#"
                  className="text-primary font-semibold hover:underline"
                >
                  Chính sách bảo mật
                </a>{' '}
                của HDG Việt Hàn.
              </label>
            </div>

            <button
              type="submit"
              className="w-full py-4 bg-primary text-white rounded-lg font-bold text-lg hover:bg-primary/90 transition-all shadow-lg shadow-primary/20"
            >
              ĐĂNG KÝ NGAY
            </button>
          </form>

          <div className="relative my-8">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-slate-200 dark:border-slate-700" />
            </div>
            <div className="relative flex justify-center text-sm">
              <span className="bg-background-light dark:bg-background-dark px-4 text-slate-500">
                Hoặc đăng ký bằng
              </span>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <button
              type="button"
              className="flex items-center justify-center gap-2 py-3 border border-slate-200 dark:border-slate-700 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors text-slate-600 dark:text-slate-300"
            >
              <img src={GOOGLE_ICON} alt="Google" className="size-5" />
              <span className="text-sm font-semibold">Google</span>
            </button>
            <button
              type="button"
              className="flex items-center justify-center gap-2 py-3 border border-slate-200 dark:border-slate-700 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors text-slate-600 dark:text-slate-300"
            >
              <img src={FACEBOOK_ICON} alt="Facebook" className="size-5" />
              <span className="text-sm font-semibold">Facebook</span>
            </button>
          </div>

          <p className="mt-10 text-center text-slate-600 dark:text-slate-400">
            Đã có tài khoản?{' '}
            <Link to="/login" className="text-primary font-bold hover:underline">
              Đăng nhập ngay
            </Link>
          </p>

          <footer className="mt-12 flex flex-wrap justify-center gap-6 text-xs text-slate-400 border-t border-slate-100 dark:border-slate-800 pt-8">
            <a href="#" className="hover:text-primary transition-colors">
              Điều khoản
            </a>
            <a href="#" className="hover:text-primary transition-colors">
              Bảo mật
            </a>
            <a href="#" className="hover:text-primary transition-colors">
              Hỗ trợ
            </a>
            <span>© 2024 HDG Việt Hàn</span>
          </footer>
        </div>
      </div>
    </div>
  )
}
