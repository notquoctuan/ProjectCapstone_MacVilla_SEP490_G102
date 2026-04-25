import { useState } from "react";
import { FormInput } from "../FormInput/FormInput";
import styles from "./LoginForm.module.css";

const ICON_SIZE = { fontSize: "1.25rem" };

export function LoginForm({ onSubmit, submitting = false }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [remember, setRemember] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (submitting) return;
    onSubmit?.({ username, password, remember });
  };

  return (
    <form className={styles.form} method="post" onSubmit={handleSubmit}>
      <FormInput
        id="username"
        label="Tên đăng nhập"
        type="text"
        name="username"
        placeholder="Nhập tên đăng nhập"
        autoComplete="username"
        required
        disabled={submitting}
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        leftIcon={
          <span className="material-symbols-outlined" style={ICON_SIZE}>
            person
          </span>
        }
      />
      <FormInput
        id="password"
        label="Password"
        type="password"
        name="password"
        placeholder="••••••••"
        autoComplete="current-password"
        required
        disabled={submitting}
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        leftIcon={
          <span className="material-symbols-outlined" style={ICON_SIZE}>
            lock
          </span>
        }
      />
      <div className={styles.row}>
        <div className={styles.checkboxWrap}>
          <input
            id="remember-me"
            name="remember-me"
            type="checkbox"
            className={styles.checkbox}
            checked={remember}
            onChange={(e) => setRemember(e.target.checked)}
            disabled={submitting}
          />
          <label className={styles.checkboxLabel} htmlFor="remember-me">
            Remember me
          </label>
        </div>
        <a className={styles.forgotLink} href="#">
          Forgot password?
        </a>
      </div>
      <div>
        <button type="submit" className={styles.submitBtn} disabled={submitting}>
          {submitting ? "Đang đăng nhập…" : "Login to Dashboard"}
          <span className={`material-symbols-outlined ${styles.submitIcon}`}>
            arrow_forward
          </span>
        </button>
      </div>
    </form>
  );
}
