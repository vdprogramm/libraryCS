import api from "./axios";

export interface DashboardStats {
  totalUsers: number;
  totalBooks: number;
  availableCopies: number;
  activeBorrows: number;
  overdueBorrows: number;
  unpaidPenalties: number;
  unpaidFineAmount: number;
}

export async function getDashboardStats() {
  const response = await api.get<DashboardStats>(
    "/admin/dashboard"
  );

  return response.data;
}
