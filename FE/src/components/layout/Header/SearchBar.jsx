import { Icon } from '../../ui/Icon'

const PLACEHOLDER = 'Bạn cần tìm thiết bị gì?'

export function SearchBar() {
  return (
    <div className="hidden md:flex flex-1 max-w-2xl bg-white rounded-lg items-center px-3 py-1.5 gap-2">
      <Icon name="search" className="text-slate-400" />
      <input
        type="text"
        className="w-full border-none focus:ring-0 text-slate-800 text-sm"
        placeholder={PLACEHOLDER}
        aria-label={PLACEHOLDER}
      />
    </div>
  )
}
