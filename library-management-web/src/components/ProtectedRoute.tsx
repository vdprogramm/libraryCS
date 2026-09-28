import { Navigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import type { ReactNode } from "react";

type Role = "ADMIN" | "LIBRARIAN" | "READER";

export default function ProtectedRoute({
  children,
  role,
}: {
  children: ReactNode;
  role: Role;
}) {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (!user.roles.includes(role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
}
