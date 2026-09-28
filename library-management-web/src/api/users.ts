import api from "./axios";

export interface AdminUser {
  id: string;
  username: string;
  email: string;
  fullName: string;
  isActive: boolean;
  roles: string[];
}

export interface PagedUsersResult {
  items: AdminUser[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
}

export const usersApi = {
  getAll: async (
    keyword = "",
    pageNumber = 1,
    pageSize = 10
  ) => {
    const response = await api.get<PagedUsersResult>(
      "/users",
      { params: { keyword, pageNumber, pageSize } }
    );

    return response.data;
  },

  changeRole: async (id: string, role: "READER" | "LIBRARIAN") => {
    const response = await api.put<AdminUser>(
      `/users/${id}/role`,
      { role }
    );

    return response.data;
  },

  changeStatus: async (id: string, isActive: boolean) => {
    const response = await api.put<AdminUser>(
      `/users/${id}/status`,
      { isActive }
    );

    return response.data;
  },
};
