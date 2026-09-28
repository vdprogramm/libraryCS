import { useState } from "react";
import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { Link } from "react-router-dom";
import {
  ArrowLeft,
  Search,
  ShieldCheck,
  UserRound,
} from "lucide-react";
import axios from "axios";

import {
  usersApi,
  type AdminUser,
} from "../../api/users";

function getErrorMessage(error: unknown) {
  if (axios.isAxiosError(error)) {
    return (
      error.response?.data?.detail ??
      error.response?.data?.message ??
      "Không thể thực hiện thao tác."
    );
  }

  return "Đã xảy ra lỗi.";
}

export default function UsersPage() {
  const queryClient = useQueryClient();

  const [keyword, setKeyword] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [message, setMessage] = useState("");

  const usersQuery = useQuery({
    queryKey: ["adminUsers", search, page],
    queryFn: () => usersApi.getAll(search, page),
  });

  const roleMutation = useMutation({
    mutationFn: ({
      id,
      role,
    }: {
      id: string;
      role: "READER" | "LIBRARIAN";
    }) => usersApi.changeRole(id, role),

    onSuccess: async () => {
      setMessage("Cập nhật vai trò thành công.");

      await queryClient.invalidateQueries({
        queryKey: ["adminUsers"],
      });
    },

    onError: (error) => {
      setMessage(getErrorMessage(error));
    },
  });

  const statusMutation = useMutation({
    mutationFn: ({
      id,
      isActive,
    }: {
      id: string;
      isActive: boolean;
    }) => usersApi.changeStatus(id, isActive),

    onSuccess: async () => {
      setMessage("Cập nhật trạng thái thành công.");

      await queryClient.invalidateQueries({
        queryKey: ["adminUsers"],
      });
    },

    onError: (error) => {
      setMessage(getErrorMessage(error));
    },
  });

  function handleChangeRole(user: AdminUser) {
    const currentRole = user.roles.includes("LIBRARIAN")
      ? "LIBRARIAN"
      : "READER";

    const nextRole =
      currentRole === "LIBRARIAN"
        ? "READER"
        : "LIBRARIAN";

    const confirmed = window.confirm(
      `Đổi quyền của "${user.username}" từ ${currentRole} sang ${nextRole}?`
    );

    if (!confirmed) return;

    setMessage("");
    roleMutation.mutate({
      id: user.id,
      role: nextRole,
    });
  }

  function handleChangeStatus(user: AdminUser) {
    const nextStatus = !user.isActive;

    const confirmed = window.confirm(
      `${nextStatus ? "Mở khóa" : "Khóa"} tài khoản "${user.username}"?`
    );

    if (!confirmed) return;

    setMessage("");
    statusMutation.mutate({
      id: user.id,
      isActive: nextStatus,
    });
  }

  const users = usersQuery.data?.items ?? [];

  const totalPages = Math.max(
    1,
    Math.ceil(
      (usersQuery.data?.totalCount ?? 0) /
        (usersQuery.data?.pageSize ?? 10)
    )
  );

  return (
    <div className="min-h-screen bg-slate-100 p-4 md:p-8">
      <div className="mx-auto max-w-7xl">
        <Link
          to="/admin"
          className="mb-5 inline-flex items-center gap-2 text-slate-500 hover:text-blue-600"
        >
          <ArrowLeft size={18} />
          Quay lại Dashboard
        </Link>

        <div className="mb-8 flex items-center gap-3">
          <UserRound size={32} className="text-blue-600" />

          <div>
            <h1 className="text-3xl font-bold text-slate-900">
              Quản lý người dùng
            </h1>

            <p className="mt-1 text-slate-500">
              Quản lý tài khoản và phân quyền hệ thống.
            </p>
          </div>
        </div>

        <form
          onSubmit={(event) => {
            event.preventDefault();
            setPage(1);
            setSearch(keyword.trim());
          }}
          className="mb-6 flex gap-3 rounded-xl bg-white p-4 shadow-sm"
        >
          <input
            value={keyword}
            onChange={(event) =>
              setKeyword(event.target.value)
            }
            placeholder="Tìm theo tên, username hoặc email..."
            className="min-w-0 flex-1 rounded-lg border border-slate-300 px-4 py-3 outline-blue-500"
          />

          <button
            type="submit"
            className="inline-flex items-center gap-2 rounded-lg bg-slate-900 px-5 py-3 text-white"
          >
            <Search size={18} />
            Tìm
          </button>
        </form>

        {message && (
          <p className="mb-5 rounded-lg bg-blue-50 p-4 text-blue-700">
            {message}
          </p>
        )}

        <div className="overflow-x-auto rounded-xl bg-white shadow-sm">
          {usersQuery.isPending ? (
            <p className="p-8">Đang tải tài khoản...</p>
          ) : usersQuery.isError ? (
            <p className="p-8 text-red-600">
              {getErrorMessage(usersQuery.error)}
            </p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50">
                <tr>
                  <th className="p-4">Người dùng</th>
                  <th className="p-4">Email</th>
                  <th className="p-4">Vai trò</th>
                  <th className="p-4">Trạng thái</th>
                  <th className="p-4 text-right">Thao tác</th>
                </tr>
              </thead>

              <tbody>
                {users.map((user) => {
                  const isAdmin = user.roles.includes("ADMIN");
                  const isLibrarian =
                    user.roles.includes("LIBRARIAN");

                  return (
                    <tr
                      key={user.id}
                      className="border-t border-slate-100"
                    >
                      <td className="p-4">
                        <p className="font-medium">
                          {user.fullName}
                        </p>
                        <p className="text-slate-500">
                          @{user.username}
                        </p>
                      </td>

                      <td className="p-4">{user.email}</td>

                      <td className="p-4">
                        <span className="inline-flex items-center gap-1 rounded-full bg-blue-50 px-3 py-1 text-blue-700">
                          {isAdmin && <ShieldCheck size={15} />}
                          {user.roles.join(", ")}
                        </span>
                      </td>

                      <td className="p-4">
                        <span
                          className={`rounded-full px-3 py-1 ${
                            user.isActive
                              ? "bg-green-100 text-green-700"
                              : "bg-red-100 text-red-700"
                          }`}
                        >
                          {user.isActive
                            ? "Hoạt động"
                            : "Đã khóa"}
                        </span>
                      </td>

                      <td className="p-4">
                        <div className="flex justify-end gap-2">
                          <button
                            onClick={() => handleChangeRole(user)}
                            disabled={
                              isAdmin ||
                              !user.isActive ||
                              roleMutation.isPending ||
                              statusMutation.isPending
                            }
                            className="rounded-lg border border-blue-200 px-3 py-2 text-blue-700 hover:bg-blue-50 disabled:cursor-not-allowed disabled:opacity-40"
                          >
                            {isLibrarian
                              ? "Chuyển Reader"
                              : "Cấp Librarian"}
                          </button>

                          <button
                            onClick={() => handleChangeStatus(user)}
                            disabled={
                              isAdmin ||
                              roleMutation.isPending ||
                              statusMutation.isPending
                            }
                            className={`rounded-lg px-3 py-2 text-white disabled:cursor-not-allowed disabled:opacity-40 ${
                              user.isActive
                                ? "bg-red-600 hover:bg-red-700"
                                : "bg-green-600 hover:bg-green-700"
                            }`}
                          >
                            {user.isActive
                              ? "Khóa"
                              : "Mở khóa"}
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}

                {users.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="p-10 text-center text-slate-500"
                    >
                      Không tìm thấy tài khoản nào.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          )}

          <div className="flex items-center justify-between border-t p-4">
            <span className="text-sm text-slate-500">
              Tổng: {usersQuery.data?.totalCount ?? 0} tài khoản
            </span>

            <div className="flex items-center gap-3">
              <button
                disabled={page <= 1}
                onClick={() => setPage((p) => p - 1)}
                className="rounded-lg border px-3 py-2 disabled:opacity-40"
              >
                Trước
              </button>

              <span className="text-sm">
                {page}/{totalPages}
              </span>

              <button
                disabled={page >= totalPages}
                onClick={() => setPage((p) => p + 1)}
                className="rounded-lg border px-3 py-2 disabled:opacity-40"
              >
                Sau
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
