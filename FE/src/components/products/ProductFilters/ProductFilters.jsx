import { useState } from 'react'
import { Icon } from '../../ui/Icon'
import { FilterGroup } from './FilterGroup'
import {
  FILTER_BRANDS,
  FILTER_PRICE_RANGES,
  FILTER_APPLIANCE_TYPES,
  FILTER_ENERGY,
} from '../../../data/productListingFilters'

export function ProductFilters() {
  const [brands, setBrands] = useState(
    FILTER_BRANDS.filter((b) => b.defaultChecked).map((b) => b.id)
  )
  const [priceRange, setPriceRange] = useState([])
  const [types, setTypes] = useState([])
  const [energy, setEnergy] = useState([])

  const toggleBrand = (id) => {
    setBrands((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    )
  }
  const togglePrice = (id) => {
    setPriceRange((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [id]
    )
  }
  const toggleType = (id) => {
    setTypes((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    )
  }
  const toggleEnergy = (id) => {
    setEnergy((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    )
  }

  const handleReset = () => {
    setBrands(FILTER_BRANDS.filter((b) => b.defaultChecked).map((b) => b.id))
    setPriceRange([])
    setTypes([])
    setEnergy([])
  }

  return (
    <aside className="w-full lg:w-64 flex-shrink-0 space-y-6">
      <div className="bg-white dark:bg-slate-900 rounded-xl p-5 border border-slate-200 dark:border-slate-800 shadow-sm">
        <div className="flex items-center justify-between mb-4">
          <h3 className="font-bold flex items-center gap-2">
            <Icon name="filter_list" className="text-primary" />
            Bộ lọc
          </h3>
          <button
            type="button"
            onClick={handleReset}
            className="text-xs text-primary font-semibold hover:underline"
          >
            Xóa bộ lọc
          </button>
        </div>
        <FilterGroup
          title="Thương hiệu"
          type="checkbox"
          options={FILTER_BRANDS}
          selectedIds={brands}
          onToggle={toggleBrand}
        />
        <FilterGroup
          title="Khoảng giá"
          type="button"
          options={FILTER_PRICE_RANGES}
          selectedIds={priceRange}
          onToggle={togglePrice}
        />
        <FilterGroup
          title="Loại thiết bị"
          type="checkbox"
          options={FILTER_APPLIANCE_TYPES}
          selectedIds={types}
          onToggle={toggleType}
        />
        <FilterGroup
          title="Tiết kiệm năng lượng"
          type="checkbox"
          options={FILTER_ENERGY}
          selectedIds={energy}
          onToggle={toggleEnergy}
        />
      </div>
    </aside>
  )
}
