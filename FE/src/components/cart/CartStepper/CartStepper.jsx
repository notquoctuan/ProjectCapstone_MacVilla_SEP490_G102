import { CART_STEPS } from '../../../data/cart'

/**
 * Step indicator: currentStep 1-based (1 = Giỏ hàng, 2 = Thông tin thanh toán, 3 = Hoàn tất).
 * Matches temp.html: progress line, active step with larger circle + ring.
 */
export function CartStepper({ currentStep = 1 }) {
  const totalSteps = CART_STEPS.length
  const progressWidth =
    totalSteps > 1 && currentStep > 1
      ? ((currentStep - 1) / (totalSteps - 1)) * 100
      : 0

  return (
    <div className="mb-10 max-w-2xl mx-auto">
      <div className="flex items-center justify-between relative">
        {/* Background line */}
        <div
          className="absolute top-1/2 left-0 w-full h-1 bg-slate-200 dark:bg-slate-700 -translate-y-1/2 -z-10 rounded-full"
          aria-hidden
        />
        {/* Progress line */}
        <div
          className="absolute top-1/2 left-0 h-1 bg-primary -translate-y-1/2 -z-10 rounded-full transition-all duration-300"
          style={{ width: `${progressWidth}%` }}
          aria-hidden
        />
        {CART_STEPS.map(({ step, label }) => {
          const isActive = step === currentStep
          const isPast = step < currentStep
          const isStep2 = step === 2
          return (
            <div
              key={step}
              className="flex flex-col items-center gap-2 relative z-10"
            >
              <div
                className={`rounded-full flex items-center justify-center font-bold text-sm ${
                  isActive
                    ? 'w-10 h-10 bg-primary text-white shadow-lg ring-4 ring-white dark:ring-slate-900'
                    : isPast
                      ? 'w-8 h-8 bg-primary text-white'
                      : 'w-8 h-8 bg-slate-200 dark:bg-slate-700 text-slate-500'
                }`}
              >
                {step}
              </div>
              <span
                className={`text-xs font-bold ${
                  isActive
                    ? 'text-primary uppercase tracking-wider'
                    : isPast
                      ? 'text-primary'
                      : 'font-medium text-slate-500 dark:text-slate-400'
                }`}
              >
                {label}
              </span>
            </div>
          )
        })}
      </div>
    </div>
  )
}
