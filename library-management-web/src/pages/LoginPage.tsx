import { useState, type FormEvent } from "react";
import { useNavigate, Link, useLocation } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import axios from "axios";
import { BookOpenText } from "lucide-react";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const registered = location.state?.registered === true;

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const user = await login(username, password);

      if (user.roles.includes("ADMIN")) {
        navigate("/admin", { replace: true });
      } else if (user.roles.includes("LIBRARIAN")) {
        navigate("/librarian", { replace: true });
      } else {
        navigate("/reader", { replace: true });
      }
    } catch (err) {
      if (axios.isAxiosError(err) && !err.response) {
        setError("Không kết nối được Backend. Kiểm tra API và CORS.");
      } else {
        setError("Đăng nhập thất bại. Kiểm tra thông tin tài khoản.");
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <form
        onSubmit={handleSubmit}
        className="glass animate-slide-up w-full max-w-md rounded-3xl p-10 relative overflow-hidden"
      >
        <div className="absolute -right-20 -top-20 h-40 w-40 rounded-full bg-blue-400 opacity-20 blur-3xl"></div>
        <div className="absolute -left-20 -bottom-20 h-40 w-40 rounded-full bg-purple-400 opacity-20 blur-3xl"></div>

        <div className="mb-6 flex justify-center relative z-10">
          <div className="rounded-2xl bg-gradient-to-br from-blue-500 to-purple-600 p-4 shadow-xl">
            <BookOpenText size={40} className="text-white" />
          </div>
        </div>
        
        <h1 className="mb-2 text-center text-4xl font-black text-slate-800 tracking-tight relative z-10">
          Library Hub
        </h1>

        <p className="mb-8 text-center font-medium text-slate-600 relative z-10">
          Cánh cửa tri thức của bạn
        </p>

        <div className="space-y-5 relative z-10">
          <div>
            <label className="mb-2 block text-sm font-bold text-slate-700">Tên đăng nhập</label>
            <input
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-purple-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-purple-500/20 shadow-sm"
              placeholder="Nhập tên đăng nhập"
            />
          </div>

          <div>
            <label className="mb-2 block text-sm font-bold text-slate-700">Mật khẩu</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="w-full rounded-xl border-2 border-white/60 bg-white/50 p-4 text-slate-800 placeholder-slate-500 backdrop-blur-md transition-all focus:border-purple-500 focus:bg-white/80 focus:outline-none focus:ring-4 focus:ring-purple-500/20 shadow-sm"
              placeholder="Nhập mật khẩu"
            />
          </div>
        </div>

        {error && (
          <div className="mt-5 rounded-xl border-l-4 border-red-500 bg-red-50/90 p-4 text-sm font-medium text-red-700 backdrop-blur-md relative z-10">
            {error}
          </div>
        )}

        {registered && (
          <div className="mt-5 rounded-xl border-l-4 border-green-500 bg-green-50/90 p-4 text-sm font-medium text-green-700 backdrop-blur-md relative z-10">
            Đăng ký thành công. Vui lòng đăng nhập.
          </div>
        )}

        <button
          disabled={loading}
          className="mt-8 w-full transform rounded-xl bg-gradient-to-r from-blue-600 to-purple-600 p-4 font-bold text-white shadow-lg transition-all hover:-translate-y-1 hover:shadow-xl focus:outline-none focus:ring-4 focus:ring-purple-500/50 disabled:opacity-50 relative z-10"
        >
          {loading ? "Đang xử lý..." : "Đăng nhập ngay"}
        </button>

        <p className="mt-6 text-center text-sm font-bold text-slate-600 relative z-10">
          Chưa có tài khoản?{" "}
          <Link to="/register" className="text-purple-700 hover:text-purple-800 hover:underline">
            Đăng ký ngay
          </Link>
        </p>
      </form>
    </div>
  );
}
