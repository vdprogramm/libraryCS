import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";
import api from "../api/axios";
import { UserPlus } from "lucide-react";

export default function RegisterPage() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    username: "",
    email: "",
    fullName: "",
    password: "",
    confirmPassword: "",
  });

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");

    if (form.password !== form.confirmPassword) {
      setError("Mật khẩu xác nhận không khớp.");
      return;
    }

    if (form.password.length < 8) {
      setError("Mật khẩu phải có ít nhất 8 ký tự.");
      return;
    }

    setLoading(true);

    try {
      await api.post("/auth/register", {
        username: form.username.trim(),
        email: form.email.trim(),
        fullName: form.fullName.trim(),
        password: form.password,
      });

      navigate("/login", {
        replace: true,
        state: { registered: true },
      });
    } catch (err) {
      if (axios.isAxiosError(err)) {
        setError(
          err.response?.data?.detail ??
            err.response?.data?.message ??
            "Không thể đăng ký. Kiểm tra thông tin tài khoản."
        );
      } else {
        setError("Đã xảy ra lỗi.");
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <form
        onSubmit={handleSubmit}
        className="glass animate-slide-up w-full max-w-lg rounded-3xl p-10 relative overflow-hidden"
      >
        <div className="absolute -right-20 -top-20 h-40 w-40 rounded-full bg-pink-400 opacity-20 blur-3xl"></div>
        <div className="absolute -left-20 -bottom-20 h-40 w-40 rounded-full bg-orange-400 opacity-20 blur-3xl"></div>

        <div className="mb-6 flex justify-center relative z-10">
          <div className="rounded-2xl bg-gradient-to-br from-pink-500 to-orange-400 p-4 shadow-xl">
            <UserPlus size={40} className="text-white" />
          </div>
        </div>

        <h1 className="mb-2 text-center text-4xl font-black text-slate-800 tracking-tight relative z-10">
          Tạo tài khoản mới
        </h1>

        <p className="mb-8 text-center font-medium text-slate-600 relative z-10">
          Khám phá hàng ngàn tựa sách hấp dẫn
        </p>

        <div className="space-y-5 relative z-10">
          <div>
            <label className="mb-2 block text-sm font-bold text-slate-700">Họ và tên</label>
            <input
              required
              value={form.fullName}
              onChange={(e) =>
                setForm({ ...form, fullName: e.target.value })
              }
              className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-pink-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-pink-500/20 shadow-sm"
              placeholder="Nguyễn Văn A"
            />
          </div>

          <div className="grid grid-cols-2 gap-5">
            <div>
              <label className="mb-2 block text-sm font-bold text-slate-700">Username</label>
              <input
                required
                value={form.username}
                onChange={(e) =>
                  setForm({ ...form, username: e.target.value })
                }
                className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-pink-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-pink-500/20 shadow-sm"
                placeholder="nguyenvana"
              />
            </div>
            <div>
              <label className="mb-2 block text-sm font-bold text-slate-700">Email</label>
              <input
                type="email"
                required
                value={form.email}
                onChange={(e) =>
                  setForm({ ...form, email: e.target.value })
                }
                className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-pink-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-pink-500/20 shadow-sm"
                placeholder="email@example.com"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-5">
            <div>
              <label className="mb-2 block text-sm font-bold text-slate-700">Mật khẩu</label>
              <input
                type="password"
                required
                minLength={8}
                value={form.password}
                onChange={(e) =>
                  setForm({ ...form, password: e.target.value })
                }
                className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-pink-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-pink-500/20 shadow-sm"
              />
            </div>
            <div>
              <label className="mb-2 block text-sm font-bold text-slate-700">Xác nhận</label>
              <input
                type="password"
                required
                value={form.confirmPassword}
                onChange={(e) =>
                  setForm({
                    ...form,
                    confirmPassword: e.target.value,
                  })
                }
                className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-pink-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-pink-500/20 shadow-sm"
              />
            </div>
          </div>
        </div>

        {error && (
          <div className="mt-5 rounded-xl border-l-4 border-red-500 bg-red-50/90 p-4 text-sm font-medium text-red-700 backdrop-blur-md relative z-10">
            {error}
          </div>
        )}

        <button
          disabled={loading}
          className="mt-8 w-full transform rounded-xl bg-gradient-to-r from-pink-500 to-orange-400 p-4 font-bold text-white shadow-lg transition-all hover:-translate-y-1 hover:shadow-xl focus:outline-none focus:ring-4 focus:ring-pink-500/50 disabled:opacity-50 relative z-10"
        >
          {loading ? "Đang xử lý..." : "Bắt đầu ngay"}
        </button>

        <p className="mt-6 text-center text-sm font-bold text-slate-600 relative z-10">
          Đã có tài khoản?{" "}
          <Link to="/login" className="text-pink-600 hover:text-pink-700 hover:underline">
            Đăng nhập
          </Link>
        </p>
      </form>
    </div>
  );
}
