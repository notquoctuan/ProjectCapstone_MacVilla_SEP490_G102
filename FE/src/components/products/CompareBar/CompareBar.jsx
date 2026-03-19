import { Icon } from '../../ui/Icon'

const MAX_COMPARE = 3
// Ảnh từ trang chính (Hot Sale - Bếp Bosch)
const PLACEHOLDER_THUMB =
  'https://lh3.googleusercontent.com/aida-public/AB6AXuAjTps2hcpaZ25QqEb2dyvGsKU07tT5-axWTgociQKByXjR_DMlQr_radBemqLaHbcU8R5YkPO_Fyp879PBl9HUTAXMsjVKgjzlpplqYHHelyT_aHnCyIg5nSVaAlaKVWCn9R7TEIHFXgJz_nZBZVFQqibbPwUffEFXo1CiYSg_QcnRqHu4JCvnpHADybLIBpdx15kNqEWhTAGnoxfjXSc2cbgmeSNHLVovKm1Gsz7LiBqUjVxRgGW9kQrnUXP26KeubkPqXgLS2I_L'

/**
 * Floating compare bar (shown when at least one product in compare)
 * Mock: 1 item in compare, 2 empty slots
 */
export function CompareBar({ compareCount = 1, maxCompare = MAX_COMPARE }) {
  if (compareCount === 0) return null

  const slots = []
  for (let i = 0; i < compareCount; i++) {
    slots.push({ type: 'item', image: PLACEHOLDER_THUMB })
  }
  for (let i = compareCount; i < maxCompare; i++) {
    slots.push({ type: 'empty' })
  }

  return (
    <div className="fixed bottom-6 left-1/2 -translate-x-1/2 z-50 px-6 py-3 bg-slate-900/95 backdrop-blur-md rounded-full shadow-2xl border border-white/10 flex items-center gap-6 hidden lg:flex">
      <div className="flex items-center gap-3">
        {slots.map((slot, index) =>
          slot.type === 'item' ? (
            <div
              key={index}
              className="size-10 rounded-lg bg-slate-800 border border-white/20 overflow-hidden relative"
            >
              <img
                src={slot.image}
                alt=""
                className="w-full h-full object-cover"
              />
              <button
                type="button"
                className="absolute top-0 right-0 bg-red-500 text-white leading-none p-0.5 rounded-bl-md"
                aria-label="Xóa khỏi so sánh"
              >
                <Icon name="close" className="text-[10px]" />
              </button>
            </div>
          ) : (
            <div
              key={index}
              className="size-10 rounded-lg bg-slate-700/50 border border-white/10 flex items-center justify-center border-dashed"
            >
              <Icon name="add" className="text-slate-500" />
            </div>
          )
        )}
      </div>
      <div className="h-8 w-px bg-white/10" />
      <button
        type="button"
        className="bg-primary text-white px-6 py-2 rounded-full text-sm font-bold hover:bg-primary/90 transition-colors"
      >
        So sánh ({compareCount}/{maxCompare})
      </button>
    </div>
  )
}
