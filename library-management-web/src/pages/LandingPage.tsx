import { Link } from "react-router-dom";
import { BookOpen, Sparkles, Library, ArrowRight, ShieldCheck } from "lucide-react";

export default function LandingPage() {
  return (
    <div className="min-h-screen relative overflow-hidden flex flex-col">
      {/* Background decorations */}
      <div className="absolute -left-32 top-0 h-96 w-96 rounded-full bg-blue-500 opacity-20 blur-3xl pointer-events-none animate-pulse-slow"></div>
      <div className="absolute -right-32 bottom-0 h-96 w-96 rounded-full bg-purple-500 opacity-20 blur-3xl pointer-events-none animate-pulse-slow" style={{ animationDelay: '1.5s' }}></div>
      <div className="absolute left-1/2 top-1/2 h-64 w-64 -translate-x-1/2 -translate-y-1/2 rounded-full bg-pink-500 opacity-20 blur-3xl pointer-events-none"></div>

      {/* Navigation Bar */}
      <header className="glass m-4 rounded-full px-6 py-4 sticky top-4 z-50 animate-slide-up shadow-xl">
        <div className="mx-auto flex max-w-7xl items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="rounded-full bg-gradient-to-br from-blue-600 to-purple-600 p-2 shadow-lg">
              <Library className="text-white" size={24} />
            </div>
            <span className="text-2xl font-black text-slate-800 tracking-tight">Library Hub</span>
          </div>
          
          <div className="flex items-center gap-4">
            <Link 
              to="/login" 
              className="px-5 py-2.5 font-bold text-slate-700 hover:text-purple-700 transition-colors"
            >
              Đăng nhập
            </Link>
            <Link 
              to="/register" 
              className="rounded-full bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-2.5 font-bold text-white shadow-lg transition-transform hover:-translate-y-0.5 hover:shadow-xl"
            >
              Đăng ký ngay
            </Link>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <main className="flex-1 flex items-center justify-center p-4 z-10">
        <div className="mx-auto max-w-5xl text-center animate-fade-in">
          <div className="mb-6 inline-flex items-center gap-2 rounded-full bg-white/40 border border-white/60 px-5 py-2 font-bold text-purple-800 backdrop-blur-md shadow-sm">
            <Sparkles size={18} className="text-purple-600" />
            Nền tảng quản lý thư viện số 1
          </div>
          
          <h1 className="mb-8 text-6xl md:text-8xl font-black text-slate-900 tracking-tighter drop-shadow-sm leading-tight">
            Khám phá tri thức <br />
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-600 via-purple-600 to-pink-500">
              không giới hạn
            </span>
          </h1>
          
          <p className="mx-auto mb-12 max-w-2xl text-xl font-medium text-slate-700 leading-relaxed">
            Hệ thống thư viện hiện đại giúp bạn dễ dàng tìm kiếm, mượn sách và quản lý tài liệu cá nhân mọi lúc mọi nơi.
          </p>
          
          <div className="flex flex-wrap items-center justify-center gap-4">
            <Link 
              to="/register" 
              className="group flex items-center gap-2 rounded-full bg-slate-900 px-8 py-4 font-bold text-white shadow-2xl transition-all hover:-translate-y-1 hover:shadow-purple-500/25"
            >
              Bắt đầu hành trình
              <ArrowRight size={20} className="transition-transform group-hover:translate-x-1" />
            </Link>
            <Link 
              to="/login" 
              className="flex items-center gap-2 rounded-full glass px-8 py-4 font-bold text-slate-800 transition-all hover:-translate-y-1 hover:bg-white/80"
            >
              Đã có tài khoản?
            </Link>
          </div>

          <div className="mt-20 grid grid-cols-1 md:grid-cols-3 gap-6 text-left">
            <FeatureCard 
              icon={BookOpen} 
              title="Hàng ngàn đầu sách" 
              description="Thư viện cung cấp kho sách khổng lồ và đa dạng mọi thể loại."
              color="from-blue-500 to-cyan-500"
            />
            <FeatureCard 
              icon={Library} 
              title="Quản lý mượn trả dễ dàng" 
              description="Theo dõi lịch sử và gia hạn trực tuyến chỉ với vài thao tác."
              color="from-purple-500 to-pink-500"
            />
            <FeatureCard 
              icon={ShieldCheck} 
              title="Bảo mật tuyệt đối" 
              description="Hệ thống được thiết kế an toàn, bảo vệ dữ liệu độc giả tối đa."
              color="from-orange-500 to-red-500"
            />
          </div>
        </div>
      </main>
    </div>
  );
}

function FeatureCard({ icon: Icon, title, description, color }: { icon: any, title: string, description: string, color: string }) {
  return (
    <div className="glass rounded-3xl p-6 transition-all hover:-translate-y-2 hover:shadow-xl group">
      <div className={`mb-4 inline-flex rounded-2xl bg-gradient-to-br ${color} p-4 shadow-lg transition-transform group-hover:scale-110`}>
        <Icon className="text-white" size={28} />
      </div>
      <h3 className="mb-2 text-xl font-bold text-slate-800">{title}</h3>
      <p className="font-medium text-slate-600">{description}</p>
    </div>
  );
}
