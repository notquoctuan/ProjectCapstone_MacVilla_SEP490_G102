import { Icon } from '../../ui/Icon'

export function Logo() {
  return (
    <div className="flex items-center gap-2 text-white shrink-0">
      <Icon name="water_damage" className="text-4xl" />
      <div className="flex flex-col">
        <h1 className="text-xl font-bold leading-none tracking-tight">
          HDG VIET HAN
        </h1>
        <span className="text-[10px] opacity-80 uppercase tracking-widest">
          Thiết bị cao cấp
        </span>
      </div>
    </div>
  )
}
