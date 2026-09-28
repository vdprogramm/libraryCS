import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
} from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import { AuthProvider } from "./contexts/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";
import LandingPage from "./pages/LandingPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import Dashboard from "./pages/Dashboard";
import AdminDashboard from "./pages/admin/AdminDashboard";
import BooksManagementPage from "./pages/librarian/BooksManagementPage";
import BooksPage from "./pages/reader/BooksPage";
import MyBorrowRecordsPage from "./pages/reader/MyBorrowRecordsPage";
import BorrowManagementPage from "./pages/librarian/BorrowManagementPage";
import PenaltiesManagementPage from "./pages/librarian/PenaltiesManagementPage";
import UsersPage from "./pages/admin/UsersPage";

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <Routes>
            <Route path="/" element={<LandingPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            <Route
              path="/admin"
              element={
                <ProtectedRoute role="ADMIN">
                  <AdminDashboard />
                </ProtectedRoute>
              }
            />

            <Route
              path="/admin/books"
              element={
                <ProtectedRoute role="ADMIN">
                  <BooksManagementPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/admin/users"
              element={
                <ProtectedRoute role="ADMIN">
                  <UsersPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/librarian"
              element={
                <ProtectedRoute role="LIBRARIAN">
                  <Dashboard title="Librarian Dashboard" />
                </ProtectedRoute>
              }
            />

            <Route
              path="/librarian/books"
              element={
                <ProtectedRoute role="LIBRARIAN">
                  <BooksManagementPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/librarian/borrows"
              element={
                <ProtectedRoute role="LIBRARIAN">
                  <BorrowManagementPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/librarian/penalties"
              element={
                <ProtectedRoute role="LIBRARIAN">
                  <PenaltiesManagementPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/reader"
              element={
                <ProtectedRoute role="READER">
                  <Dashboard title="Reader Dashboard" />
                </ProtectedRoute>
              }
            />
            
            <Route
              path="/reader/books"
              element={
                <ProtectedRoute role="READER">
                  <BooksPage />
                </ProtectedRoute>
              }
            />
            
            <Route
              path="/reader/my-borrows"
              element={
                <ProtectedRoute role="READER">
                  <MyBorrowRecordsPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/unauthorized"
              element={
                <div className="p-10 text-center text-red-600">
                  Bạn không có quyền truy cập trang này.
                </div>
              }
            />

            <Route
              path="*"
              element={<Navigate to="/" replace />}
            />
          </Routes>
        </AuthProvider>
      </BrowserRouter>
    </QueryClientProvider>
  );
}
