import { SidebarNav } from '../components/main/SidebarNav'
import { HeroBanner } from '../components/main/HeroBanner'
import { SubBanners } from '../components/main/SubBanners'
import { HotSaleSection } from '../components/main/HotSaleSection'
import { BrandSection } from '../components/main/BrandSection'
import { SmartHomeSection } from '../components/main/SmartHomeSection'

export function HomePage() {
  return (
    <main className="max-w-7xl mx-auto px-4 py-6">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <SidebarNav />
        <div className="lg:col-span-9">
          <HeroBanner />
          <SubBanners />
        </div>
      </div>
      <HotSaleSection />
      <BrandSection />
      <SmartHomeSection />
    </main>
  )
}
