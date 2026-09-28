import { useQuery } from "@tanstack/react-query";
import { Link, useNavigate } from "react-router-dom";
import {
  BookOpen,
  BookCheck,
  Clock3,
  Library,
  Users,
  Wallet,
  AlertTriangle,
  LogOut,
  TrendingUp
} from "lucide-react";
import { getDashboardStats } from "../../api/dashboard";
import { useAuth } from "../../contexts/AuthContext";

export default function AdminDashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const { data, isPending, isError } = useQuery({
    queryKey: ["adminDashboard"],
    queryFn: getDashboardStats,
  });

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  const cards = data
    ? [
        {
          label: "Người dùng",
          value: data.totalUsers,
          icon: Users,
          color: "from-blue-400 to-blue-600",
        },
        {
          label: "Đầu sách",
          value: data.totalBooks,
          icon: BookOpen,
          color: "from-emerald-400 to-emerald-600",
        },
        {
          label: "Bản sách còn",
          value: data.availableCopies,
          icon: Library,
          color: "from-teal-400 to-teal-600",
        },
        {
          label: "Đang mượn",
          value: data.activeBorrows,
          icon: BookCheck,
          color: "from-indigo-400 to-indigo-600",
        },
        {
          label: "Đang quá hạn",
          value: data.overdueBorrows,
          icon: Clock3,
          color: "from-orange-400 to-orange-600",
        },
        {
          label: "Khoản phạt chưa trả",
          value: data.unpaidPenalties,
          icon: AlertTriangle,
          color: "from-red-400 to-rose-600",
        },
      ]
    : [];

  return (
    <div className="min-h-screen p-4 md:p-8">
      <div className="mx-auto max-w-7xl animate-fade-in relative">
        <header className="mb-10 flex flex-wrap items-center justify-between gap-6 glass rounded-3xl p-8 shadow-2xl relative overflow-hidden">
          <div className="absolute right-0 top-0 h-64 w-64 rounded-full bg-blue-400 opacity-10 blur-3xl pointer-events-none"></div>
          
          <div className="flex items-center gap-5 relative z-10">
            <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-gradient-to-br from-indigo-500 to-purple-600 shadow-lg">
              <TrendingUp className="text-white" size={32} />
            </div>
            <div>
              <h1 className="text-4xl font-black text-slate-800 tracking-tight">
                Tổng quan hệ thống
              </h1>
              <p className="mt-1 text-lg font-medium text-slate-600">
                Chào mừng quản trị viên, <span className="font-bold text-indigo-700">{user?.fullName}</span>
              </p>
            </div>
          </div>

          <button
            onClick={handleLogout}
            className="group flex items-center gap-2 rounded-xl bg-white/60 px-5 py-3 font-bold text-slate-700 backdrop-blur-md transition-all hover:bg-red-500 hover:text-white hover:shadow-lg relative z-10"
          >
            <LogOut size={20} className="transition-transform group-hover:-translate-x-1" />
            Đăng xuất
          </button>
        </header>

        <nav className="mb-10 flex flex-wrap gap-4 animate-slide-up" style={{ animationDelay: '0.1s', opacity: 0 }}>
          <Link
            to="/admin/users"
            className="group relative overflow-hidden rounded-2xl bg-gradient-to-r from-blue-600 to-indigo-600 px-8 py-4 font-bold text-white shadow-lg transition-all hover:-translate-y-1 hover:shadow-2xl"
          >
            <div className="absolute inset-0 bg-white/20 opacity-0 transition-opacity group-hover:opacity-100"></div>
            <div className="flex items-center gap-3 text-lg">
              <Users size={24} />
              Quản lý tài khoản
            </div>
          </Link>

          <Link
            to="/admin/books"
            className="group relative overflow-hidden rounded-2xl bg-gradient-to-r from-emerald-500 to-teal-600 px-8 py-4 font-bold text-white shadow-lg transition-all hover:-translate-y-1 hover:shadow-2xl"
          >
            <div className="absolute inset-0 bg-white/20 opacity-0 transition-opacity group-hover:opacity-100"></div>
            <div className="flex items-center gap-3 text-lg">
              <BookOpen size={24} />
              Quản lý sách
            </div>
          </Link>
        </nav>

        {isPending ? (
          <div className="flex h-64 items-center justify-center">
            <div className="h-16 w-16 animate-spin rounded-full border-4 border-indigo-500 border-t-transparent"></div>
          </div>
        ) : isError ? (
          <div className="glass rounded-3xl p-8 border-l-4 border-red-500 shadow-xl">
            <h3 className="text-xl font-black text-red-800">Đã xảy ra lỗi</h3>
            <p className="font-medium text-red-700 mt-2">Không thể tải thống kê. Vui lòng kiểm tra kết nối Backend và quyền Admin.</p>
          </div>
        ) : (
          <div className="animate-slide-up" style={{ animationDelay: '0.2s', opacity: 0 }}>
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {cards.map((card, index) => {
                const Icon = card.icon;

                return (
                  <div
                    key={card.label}
                    className="group glass relative overflow-hidden rounded-3xl p-8 transition-all hover:-translate-y-2 hover:shadow-2xl"
                    style={{ animationDelay: `${0.2 + index * 0.1}s` }}
                  >
                    <div className="absolute -right-6 -top-6 h-32 w-32 rounded-full bg-gradient-to-br opacity-20 blur-2xl transition-all duration-500 group-hover:scale-150 group-hover:opacity-40"></div>
                    
                    <div className="mb-6 flex items-center justify-between relative z-10">
                      <p className="text-lg font-bold text-slate-600">
                        {card.label}
                      </p>

                      <div className={`rounded-2xl bg-gradient-to-br ${card.color} p-4 shadow-lg`}>
                        <Icon className="text-white" size={28} />
                      </div>
                    </div>

                    <p className="relative z-10 text-5xl font-black text-slate-800 tracking-tight">
                      {card.value.toLocaleString("vi-VN")}
                    </p>
                  </div>
                );
              })}
            </div>

            <div className="mt-8 glass rounded-3xl p-10 transition-all hover:shadow-2xl animate-slide-up relative overflow-hidden" style={{ animationDelay: '0.8s', opacity: 0 }}>
              <div className="absolute -right-20 -bottom-20 h-64 w-64 rounded-full bg-red-400 opacity-10 blur-3xl pointer-events-none"></div>

              <div className="flex flex-wrap items-center justify-between gap-6 relative z-10">
                <div>
                  <div className="flex items-center gap-4 mb-3">
                    <div className="rounded-2xl bg-gradient-to-br from-red-500 to-rose-600 p-4 shadow-lg">
                      <Wallet className="text-white" size={32} />
                    </div>
                    <h2 className="text-2xl font-black text-slate-700">
                      Tổng tiền phạt chờ thanh toán
                    </h2>
                  </div>
                  <p className="text-lg text-slate-500 font-medium">Khoản thu dự kiến từ các phiếu mượn quá hạn</p>
                </div>

                <div className="text-right">
                  <p className="text-6xl font-black text-transparent bg-clip-text bg-gradient-to-r from-red-600 to-rose-500 tracking-tight drop-shadow-sm">
                    {data!.unpaidFineAmount.toLocaleString("vi-VN")} <span className="text-3xl font-bold">VNĐ</span>
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
