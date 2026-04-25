import { NavLink, useLocation } from "react-router-dom";
import { BRAND_LOGO_SRC, BRAND_NAME, BRAND_SALER_SUB } from "@/config/brand";
import { SALER_NAV_ITEMS, isActorNavGroup } from "@/config/actorNav.config";
import styles from "./DashboardSidebar.module.css";

const DEFAULT_USER = {
  name: "Nguyen Saler",
  role: "Sale",
  avatarUrl:
    "https://lh3.googleusercontent.com/aida-public/AB6AXuBDBIxERU3V5wGnQOiNrluifEq-6eYcqpkbolK_gmi0h-jypwsO2YlLp2Sq1KTTNsYb5uLrVP3bty_Gb16JywSbMY0xLQqQ4ZGLitmyYJSTEhnlvv93DYWLUbHmMLdB50pjCWtt7n9rJnvdHPAbwu_ayWND6EFb-8C5dk7nrXVmCAn36uIba4Bjo-4d0jrVuJAAC6nc23Xv9eiwLglDaaRLue1NBx7WeTovu0XkPgLbvEBg2lfypWlN3T_3Gfx3zjrTUqfCqIVmmIkm",
};

/**
 * @param {{ item: import("@/config/actorNav.config").ActorNavItem; inGroup?: boolean }} props
 */
function SidebarNavLink({ item, inGroup }) {
  const location = useLocation();
  return (
    <NavLink
      to={item.to}
      end={item.end ?? false}
      className={({ isActive: navIsActive }) => {
        const active =
          typeof item.isActive === "function" ? item.isActive(location.pathname) : navIsActive;
        return [styles.link, inGroup ? styles.linkInGroup : "", active ? styles.linkActive : ""].filter(Boolean).join(" ");
      }}
    >
      <span className="material-symbols-outlined" aria-hidden>
        {item.icon}
      </span>
      <span>{item.label}</span>
    </NavLink>
  );
}

/**
 * Sidebar khu actor (Sales, Manager, StockManager, Worker).
 * @param {{
 *   navItems?: import("@/config/actorNav.config").ActorNavEntry[];
 *   brandSub?: string;
 *   user?: typeof DEFAULT_USER;
 *   onLogout?: () => void;
 * }} props
 */
export function DashboardSidebar({
  navItems = SALER_NAV_ITEMS,
  brandSub = BRAND_SALER_SUB,
  user = DEFAULT_USER,
  onLogout,
}) {
  return (
    <aside className={styles.sidebar}>
      <div className={styles.header}>
        <div className={styles.logoIcon}>
          <img src={BRAND_LOGO_SRC} alt={BRAND_NAME} className={styles.logoImg} />
        </div>
        <div className={styles.brand}>
          <h1 className={styles.brandTitle}>{BRAND_NAME}</h1>
          <p className={styles.brandSub}>{brandSub}</p>
        </div>
      </div>
      <nav className={styles.nav} aria-label="Main">
        <ul className={styles.navList}>
          {navItems.map((entry, idx) => {
            if (isActorNavGroup(entry)) {
              return (
                <li key={`group-${entry.title}-${idx}`} className={styles.navGroup}>
                  <p className={styles.navGroupTitle}>{entry.title}</p>
                  <ul className={styles.navSubList}>
                    {entry.items.map((item) => (
                      <li key={item.to + item.label}>
                        <SidebarNavLink item={item} inGroup />
                      </li>
                    ))}
                  </ul>
                </li>
              );
            }
            return (
              <li key={entry.to + entry.label}>
                <SidebarNavLink item={entry} />
              </li>
            );
          })}
        </ul>
      </nav>
      <div className={styles.userBlock}>
        <div className={styles.userCard}>
          <div className={styles.avatar}>
            <img src={user.avatarUrl} alt="" />
          </div>
          <div className={styles.userCardText} style={{ minWidth: 0, flex: 1 }}>
            <p className={styles.userName}>{user.name}</p>
            <p className={styles.userRole}>{user.role}</p>
          </div>
        </div>
        {typeof onLogout === "function" && (
          <button type="button" className={styles.logoutBtn} onClick={onLogout}>
            <span className="material-symbols-outlined" aria-hidden style={{ fontSize: "1.125rem" }}>
              logout
            </span>
            Đăng xuất
          </button>
        )}
      </div>
    </aside>
  );
}
