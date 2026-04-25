import { BRAND_APP_VERSION_LABEL, BRAND_COPYRIGHT } from "@/config/brand";
import styles from "./LoginFooter.module.css";

const DEFAULT_COPYRIGHT = BRAND_COPYRIGHT;
const DEFAULT_VERSION = BRAND_APP_VERSION_LABEL;

export function LoginFooter({
  copyright = DEFAULT_COPYRIGHT,
  version = DEFAULT_VERSION,
}) {
  return (
    <p className={styles.footer}>
      {copyright}
      <br />
      {version}
    </p>
  );
}
