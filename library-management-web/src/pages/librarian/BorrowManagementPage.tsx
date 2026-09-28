import { useState } from "react";
import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { ArrowLeft, BookCheck, Clock3 } from "lucide-react";
import axios from "axios";

import {
  borrowRecordsApi,
  type BorrowRecord,
} from "../../api/borrowRecords";

function formatDate(value: string) {
  return new Date(value).toLocaleDateString("vi-VN");
}

function errorMessage(error: unknown) {
  if (axios.isAxiosError(error)) {
    return (
      error.response?.data?.detail ??
      error.response?.data?.message ??
      "Không thể thực hiện thao tác."
    );
  }

  return "Đã xảy ra lỗi.";
}

export default function BorrowManagementPage() {
  const queryClient = useQueryClient();
  const [message, setMessage] = useState("");

  const overdueQuery = useQuery({
    queryKey: ["activeBorrowRecords"],
    queryFn: borrowRecordsApi.getActive,
  });

  const returnMutation = useMutation({
    mutationFn: borrowRecordsApi.returnBook,

    onSuccess: async () => {
      setMessage("Đã xác nhận trả sách thành công.");

      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["activeBorrowRecords"],
        }),
        queryClient.invalidateQueries({
          queryKey: ["books"],
        }),
        queryClient.invalidateQueries({
          queryKey: ["penalties"],
        }),
      ]);
    },

    onError: (error) => {
      setMessage(errorMessage(error));
    },
  });

  function handleReturn(record: BorrowRecord) {
    const confirmed = window.confirm(
      `Xác nhận độc giả đã trả sách "${record.bookTitle}"?`
    );

    if (!confirmed) return;

    setMessage("");
    returnMutation.mutate(record.id);
  }

  const records = overdueQuery.data ?? [];

  return (
    <div className="min-h-screen bg-slate-100 p-4 md:p-8">
      <div className="mx-auto max-w-6xl">
        <Link
          to="/librarian"
          className="mb-5 inline-flex items-center gap-2 text-slate-500 hover:text-blue-600"
        >
          <ArrowLeft size={18} />
          Dashboard
        </Link>

        <div className="mb-8 flex items-center gap-3">
          <Clock3 size={32} className="text-orange-600" />

          <div>
            <h1 className="text-3xl font-bold">
              Quản lý mượn trả
            </h1>
            <p className="mt-1 text-slate-500">
              Danh sách sách đang được mượn.
            </p>
          </div>
        </div>

        {message && (
          <p className="mb-5 rounded-lg bg-blue-50 p-4 text-blue-700">
            {message}
          </p>
        )}

        <div className="overflow-x-auto rounded-xl bg-white shadow-sm">
          {overdueQuery.isPending ? (
            <p className="p-8">Đang tải dữ liệu...</p>
          ) : overdueQuery.isError ? (
            <p className="p-8 text-red-600">
              {errorMessage(overdueQuery.error)}
            </p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50">
                <tr>
                  <th className="p-4">Sách</th>
                  <th className="p-4">Mã độc giả</th>
                  <th className="p-4">Ngày mượn</th>
                  <th className="p-4">Hạn trả</th>
                  <th className="p-4">Thao tác</th>
                </tr>
              </thead>

              <tbody>
                {records.map((record) => (
                  <tr
                    key={record.id}
                    className="border-t border-slate-100"
                  >
                    <td className="p-4 font-medium">
                      {record.bookTitle}
                    </td>

                    <td className="p-4 text-slate-500">
                      {record.readerId}
                    </td>

                    <td className="p-4">
                      {formatDate(record.borrowDate)}
                    </td>

                    <td
                      className={`p-4 ${
                        new Date(record.dueDate).getTime() < Date.now()
                          ? "font-medium text-red-600"
                          : "text-slate-700"
                      }`}
                    >
                      {formatDate(record.dueDate)}
                    </td>

                    <td className="p-4">
                      <button
                        onClick={() => handleReturn(record)}
                        disabled={returnMutation.isPending}
                        className="inline-flex items-center gap-2 rounded-lg bg-green-600 px-4 py-2 text-white hover:bg-green-700 disabled:opacity-50"
                      >
                        <BookCheck size={17} />
                        Xác nhận trả
                      </button>
                    </td>
                  </tr>
                ))}

                {records.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="p-10 text-center text-slate-500"
                    >
                      Không có lượt mượn đang hoạt động.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}
