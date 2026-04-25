import { BRAND_LOGO_SRC, BRAND_NAME } from "@/config/brand";
import styles from "./LoginHeader.module.css";

const DEFAULT_TITLE = "Đăng nhập";
const DEFAULT_DESCRIPTION =
  "Nhập tài khoản nội bộ để vào cổng quản lý Macvilla.";

export function LoginHeader({ title = DEFAULT_TITLE, description = DEFAULT_DESCRIPTION }) {
  return (
    <>
      <div className={styles.mobileHeader}>
        <img src={BRAND_LOGO_SRC} alt="" className={styles.mobileLogoImg} width={48} height={48} />
        <h2 className={styles.mobileTitle}>{BRAND_NAME}</h2>
      </div>
      <div className={styles.intro}>
        <h2 className={styles.title}>{title}</h2>
        <p className={styles.description}>{description}</p>
      </div>
    </>
  );
}
