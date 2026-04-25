import { Outlet, useNavigate } from "react-router-dom";
import { DashboardSidebar } from "@/components/Dashboard/DashboardSidebar/DashboardSidebar";
import { useAuth } from "@/context/AuthContext";
import styles from "../SalerLayout/SalerLayout.module.css";

function avatarUrlForName(name) {
  const q = encodeURIComponent(name || "User");
  return `https://ui-avatars.com/api/?size=128&background=004a99&color=fff&bold=true&name=${q}`;
}

/**
 * @param {{ navItems: import("@/config/actorNav.config").ActorNavEntry[]; brandSub: string }} props
 */
export function ActorShellLayout({ navItems, brandSub }) {
  const navigate = useNavigate();
  const { user: authUser, logout } = useAuth();

  const displayName = authUser?.fullName?.trim() || authUser?.username || "Người dùng";
  const sidebarUser = {
    name: displayName,
    role: authUser?.roleName || "—",
    avatarUrl: avatarUrlForName(displayName),
  };

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  return (
    <div className={styles.layout}>
      <DashboardSidebar navItems={navItems} brandSub={brandSub} user={sidebarUser} onLogout={handleLogout} />
      <main className={styles.main}>
        <Outlet />
      </main>
    </div>
  );
}
