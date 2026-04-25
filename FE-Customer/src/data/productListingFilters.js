/**
 * Filter options for product listing page
 */
export const FILTER_BRANDS = [
  { id: 'macvilla', label: 'Macvilla', defaultChecked: true },
  { id: 'bosch', label: 'Bosch Cao cấp' },
  { id: 'samsung', label: 'Samsung Gia đình' },
  { id: 'lg', label: 'LG Nhà bếp' },
]

export const FILTER_PRICE_RANGES = [
  { id: 'under500', label: 'Dưới 500$' },
  { id: '500-1500', label: '500$ - 1.500$' },
  { id: '1500-3000', label: '1.500$ - 3.000$' },
  { id: 'over3000', label: 'Trên 3.000$' },
]

export const FILTER_APPLIANCE_TYPES = [
  { id: 'fridges', label: 'Tủ lạnh thông minh' },
  { id: 'induction', label: 'Bếp từ' },
  { id: 'steam', label: 'Lò hơi nước' },
]

export const FILTER_ENERGY = [
  { id: 'a+++', label: 'Tiết kiệm điện A+++' },
  { id: 'energystar', label: 'Energy Star' },
]
