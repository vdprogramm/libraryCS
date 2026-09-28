import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { LayoutDashboard, Book, History, Clock, FileWarning, LogOut, Users } from "lucide-react";

export default function Dashboard({
  title,
}: {
  title: string;
}) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  return (
    <div className="min-h-screen p-4 md:p-8 animate-fade-in">
      <div className="mx-auto max-w-5xl glass rounded-3xl p-8 md:p-12 shadow-2xl relative overflow-hidden">
        <div className="absolute -right-32 -top-32 h-64 w-64 rounded-full bg-blue-400 opacity-20 blur-3xl pointer-events-none"></div>
        <div className="absolute -left-32 -bottom-32 h-64 w-64 rounded-full bg-purple-400 opacity-20 blur-3xl pointer-events-none"></div>

        <div className="flex flex-wrap items-center justify-between gap-6 border-b border-white/30 pb-8 relative z-10">
          <div className="flex items-center gap-5">
            <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-gradient-to-br from-blue-500 to-purple-600 shadow-lg">
              <LayoutDashboard className="text-white" size={32} />
            </div>
            <div>
              <h1 className="text-4xl font-black text-slate-800 tracking-tight">
                {title}
              </h1>
              <p className="mt-1 text-lg font-medium text-slate-600">
                Xin chào, <span className="font-bold text-purple-700">{user?.fullName}</span>
              </p>
            </div>
          </div>

          <button
            onClick={handleLogout}
            className="group flex items-center gap-2 rounded-xl bg-white/60 px-5 py-3 font-bold text-slate-700 backdrop-blur-md transition-all hover:bg-red-500 hover:text-white hover:shadow-lg"
          >
            <LogOut size={20} className="transition-transform group-hover:-translate-x-1" />
            Đăng xuất
          </button>
        </div>

        <div className="py-8 relative z-10">
          <div className="mb-8 inline-flex items-center gap-3 rounded-full bg-indigo-100/90 px-5 py-2.5 text-sm font-bold text-indigo-800 backdrop-blur-md shadow-sm border border-indigo-200">
            <span className="relative flex h-3 w-3">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-indigo-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-3 w-3 bg-indigo-500"></span>
            </span>
            Vai trò hiện tại: {user?.roles.join(", ")}
          </div>

          <div className="mt-4 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {user?.roles.includes("ADMIN") && (
              <>
                <DashboardCard to="/admin/users" icon={Users} color="from-blue-500 to-indigo-600" title="Quản lý người dùng" delay={0.1} />
                <DashboardCard to="/admin/books" icon={Book} color="from-emerald-400 to-teal-600" title="Quản lý sách" delay={0.2} />
              </>
            )}

            {user?.roles.includes("LIBRARIAN") && !user.roles.includes("ADMIN") && (
              <>
                <DashboardCard to="/librarian/books" icon={Book} color="from-emerald-400 to-teal-600" title="Quản lý sách" delay={0.1} />
                <DashboardCard to="/librarian/borrows" icon={Clock} color="from-orange-400 to-orange-600" title="Quản lý mượn trả" delay={0.2} />
                <DashboardCard to="/librarian/penalties" icon={FileWarning} color="from-red-400 to-rose-600" title="Quản lý tiền phạt" delay={0.3} />
              </>
            )}

            {user?.roles.includes("READER") && (
              <>
                <DashboardCard to="/reader/books" icon={Book} color="from-purple-500 to-fuchsia-600" title="Tìm và mượn sách" delay={0.1} />
                <DashboardCard to="/reader/my-borrows" icon={History} color="from-cyan-400 to-blue-600" title="Lịch sử mượn" delay={0.2} />
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

function DashboardCard({ to, icon: Icon, color, title, delay }: { to: string, icon: any, color: string, title: string, delay: number }) {
  return (
    <Link
      to={to}
      className={`animate-slide-up group relative overflow-hidden rounded-3xl bg-gradient-to-br ${color} p-6 shadow-lg transition-all hover:-translate-y-2 hover:shadow-2xl`}
      style={{ animationDelay: `${delay}s`, opacity: 0 }}
    >
      <div className="absolute -right-4 -bottom-4 opacity-20 transition-transform duration-500 group-hover:scale-150 group-hover:rotate-12">
        <Icon size={120} className="text-white" />
      </div>
      <div className="relative z-10">
        <div className="mb-4 inline-block rounded-2xl bg-white/20 p-4 backdrop-blur-md shadow-inner">
          <Icon size={32} className="text-white" />
        </div>
        <h3 className="text-2xl font-black text-white">{title}</h3>
      </div>
    </Link>
  );
}
