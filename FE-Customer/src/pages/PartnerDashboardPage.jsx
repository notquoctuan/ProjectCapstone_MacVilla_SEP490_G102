import { useState } from 'react'
import { Icon } from '../components/ui/Icon'
import { PartnerHeaderUser } from '../components/partner/PartnerHeaderUser'
import { PartnerCompanySummary } from '../components/partner/PartnerCompanySummary'
import {
  B2B_STATS,
  B2B_PROJECT_ACTIVITIES,
  B2B_QUICK_ACTIONS,
  B2B_ACCOUNT_MANAGER,
} from '../data/b2bDashboard'
import { useAuth } from '../contexts/AuthContext'

function formatToday() {
  return new Date().toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export function PartnerDashboardPage() {
  const [search, setSearch] = useState('')
  const today = formatToday()
  const { user } = useAuth()
  const welcomeName =
    (user?.companyName || '').trim() ||
    (user?.fullName || user?.name || '').trim() ||
    'quý đối tác'

  return (
    <>
      <header className="h-16 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 flex items-center justify-between px-8 sticky top-0 z-40">
          <div className="flex items-center gap-4">
            <h2 className="text-lg font-bold text-slate-800 dark:text-slate-100">Customer Dashboard</h2>
          </div>
          <div className="flex items-center gap-6">
            <div className="relative hidden lg:block">
              <Icon name="search" className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-xl" />
              <input
                type="text"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Tìm kiếm đơn hàng, vật tư..."
                className="pl-10 pr-4 py-1.5 bg-slate-100 dark:bg-slate-800 border-none rounded-lg text-sm w-64 focus:ring-1 focus:ring-primary text-slate-900 dark:text-slate-100 placeholder-slate-400"
              />
            </div>
            <div className="flex items-center gap-4 border-r pr-6 border-slate-200 dark:border-slate-700">
              <button type="button" className="relative text-slate-500 hover:text-primary transition-colors">
                <Icon name="notifications" className="text-2xl" />
                <span className="absolute top-0 right-0 w-2 h-2 bg-red-500 rounded-full border-2 border-white dark:border-slate-900" />
              </button>
              <button type="button" className="text-slate-500 hover:text-primary transition-colors">
                <Icon name="settings" className="text-2xl" />
              </button>
            </div>
            <PartnerHeaderUser size="sm" hideTextOnMobile={false} />
          </div>
        </header>

        {/* Content */}
        <div className="p-8">
          {/* Welcome */}
          <div className="flex flex-wrap justify-between items-end gap-4 mb-8">
            <div>
              <h3 className="text-3xl font-black text-slate-900 dark:text-slate-50 tracking-tight">Tổng quan Dashboard</h3>
              <p className="text-slate-500 mt-1">
                Chào mừng quay trở lại, {welcomeName}
              </p>
            </div>
            <div className="flex gap-3">
              <div className="flex items-center gap-2 bg-white dark:bg-slate-800 px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 shadow-sm text-sm font-medium text-slate-700 dark:text-slate-200">
                <Icon name="calendar_today" className="text-slate-400" />
                <span>Hôm nay: {today}</span>
              </div>
            </div>
          </div>

          <PartnerCompanySummary className="mb-8" />

          {/* Stats */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            {B2B_STATS.map((stat) => (
              <div
                key={stat.id}
                className="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 flex flex-col justify-between"
              >
                <div className="flex justify-between items-start mb-4">
                  <div className={`p-2 rounded-lg ${stat.iconClass}`}>
                    <Icon name={stat.icon} className="text-2xl" />
                  </div>
                  <span className={`text-xs font-bold px-2 py-1 rounded-full ${stat.trendClass}`}>{stat.trend}</span>
                </div>
                <div>
                  <p className="text-slate-500 text-sm font-medium">{stat.label}</p>
                  <p className="text-2xl font-bold text-slate-900 dark:text-slate-50 mt-1">{stat.value}</p>
                </div>
              </div>
            ))}
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Activities Table */}
            <div className="lg:col-span-2">
              <div className="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden">
                <div className="px-6 py-5 border-b border-slate-100 dark:border-slate-700 flex justify-between items-center">
                  <h4 className="font-bold text-slate-800 dark:text-slate-100 text-lg">Hoạt động công trình gần đây</h4>
                  <button type="button" className="text-primary text-sm font-semibold hover:underline">
                    Xem tất cả
                  </button>
                </div>
                <div className="overflow-x-auto">
                  <table className="w-full text-left">
                    <thead className="bg-slate-50 dark:bg-slate-900/50">
                      <tr>
                        <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Project Name</th>
                        <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Equipment Status</th>
                        <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider">Progress</th>
                        <th className="px-6 py-4 text-xs font-bold text-slate-500 uppercase tracking-wider text-right">Update Date</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 dark:divide-slate-700">
                      {B2B_PROJECT_ACTIVITIES.map((row) => (
                        <tr key={row.id}>
                          <td className="px-6 py-4">
                            <p className="text-sm font-bold text-slate-800 dark:text-slate-100">{row.name}</p>
                            <p className="text-xs text-slate-400">ID: {row.id}</p>
                          </td>
                          <td className="px-6 py-4">
                            <span className={`px-2 py-1 text-[10px] font-bold rounded uppercase ${row.statusClass}`}>
                              {row.status}
                            </span>
                          </td>
                          <td className="px-6 py-4">
                            <div className="w-full bg-slate-100 dark:bg-slate-700 rounded-full h-1.5 max-w-[120px]">
                              <div
                                className="bg-primary h-1.5 rounded-full transition-all"
                                style={{ width: `${row.progress}%` }}
                              />
                            </div>
                            <span className="text-[10px] font-medium text-slate-500 mt-1 block">{row.progress}% Hoàn thành</span>
                          </td>
                          <td className="px-6 py-4 text-right">
                            <span className="text-sm text-slate-500">{row.updateDate}</span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            {/* Right Column */}
            <div className="space-y-8">
              {/* Quick Actions */}
              <div className="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 p-6">
                <h4 className="font-bold text-slate-800 dark:text-slate-100 text-lg mb-4">Quick Actions</h4>
                <div className="space-y-3">
                  {B2B_QUICK_ACTIONS.map((action) => (
                    <button
                      key={action.label}
                      type="button"
                      className={`w-full flex items-center justify-between px-4 py-3 rounded-lg transition-colors group ${
                        action.primary
                          ? 'bg-primary/5 hover:bg-primary/10 text-primary'
                          : 'bg-slate-50 dark:bg-slate-900/50 hover:bg-slate-100 dark:hover:bg-slate-900 text-slate-700 dark:text-slate-200'
                      }`}
                    >
                      <div className="flex items-center gap-3">
                        <Icon name={action.icon} className="text-xl" />
                        <span className="text-sm font-bold">{action.label}</span>
                      </div>
                      <Icon name="chevron_right" className="text-lg group-hover:translate-x-1 transition-transform" />
                    </button>
                  ))}
                </div>
              </div>

              {/* Account Manager */}
              <div className="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 p-6">
                <h4 className="font-bold text-slate-800 dark:text-slate-100 text-lg mb-4">Your Account Manager</h4>
                <div className="flex flex-col items-center">
                  <div className="w-24 h-24 rounded-full border-4 border-primary/10 overflow-hidden mb-4 p-1">
                    <img
                      src={B2B_ACCOUNT_MANAGER.avatar}
                      alt=""
                      className="w-full h-full object-cover rounded-full"
                    />
                  </div>
                  <h5 className="text-lg font-bold text-slate-900 dark:text-slate-50">{B2B_ACCOUNT_MANAGER.name}</h5>
                  <p className="text-sm text-slate-500 font-medium">{B2B_ACCOUNT_MANAGER.title}</p>
                  <div className="w-full mt-6 space-y-3">
                    <div className="flex items-center gap-3 text-sm">
                      <Icon name="call" className="text-primary text-xl" />
                      <span className="text-slate-600 dark:text-slate-400 font-medium">{B2B_ACCOUNT_MANAGER.phone}</span>
                    </div>
                    <div className="flex items-center gap-3 text-sm">
                      <Icon name="mail" className="text-primary text-xl" />
                      <span className="text-slate-600 dark:text-slate-400 font-medium">{B2B_ACCOUNT_MANAGER.email}</span>
                    </div>
                    <div className="flex items-center gap-3 text-sm">
                      <Icon name="schedule" className="text-primary text-xl" />
                      <span className="text-slate-600 dark:text-slate-400 font-medium">{B2B_ACCOUNT_MANAGER.schedule}</span>
                    </div>
                  </div>
                  <button
                    type="button"
                    className="w-full mt-6 flex items-center justify-center gap-2 bg-slate-900 dark:bg-slate-700 text-white py-2 rounded-lg font-bold text-sm hover:opacity-90 transition-opacity"
                  >
                    <Icon name="chat" className="text-lg" />
                    Chat với chuyên viên
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
    </>
  )
}
