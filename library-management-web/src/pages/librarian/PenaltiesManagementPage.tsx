import { useState } from "react";
import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { ArrowLeft, Wallet } from "lucide-react";
import axios from "axios";

import {
  penaltiesApi,
  type Penalty,
} from "../../api/penalties";

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

export default function PenaltiesManagementPage() {
  const queryClient = useQueryClient();
  const [message, setMessage] = useState("");

  const penaltiesQuery = useQuery({
    queryKey: ["penalties", "Unpaid"],
    queryFn: () => penaltiesApi.getAll("Unpaid"),
  });

  const paymentMutation = useMutation({
    mutationFn: penaltiesApi.markAsPaid,

    onSuccess: async () => {
      setMessage("Đã ghi nhận thanh toán thành công.");

      await queryClient.invalidateQueries({
        queryKey: ["penalties"],
      });
    },

    onError: (error) => {
      setMessage(errorMessage(error));
    },
  });

  function handlePayment(penalty: Penalty) {
    const confirmed = window.confirm(
      `Xác nhận đã nhận ${penalty.fineAmount.toLocaleString(
        "vi-VN"
      )} VNĐ cho sách "${penalty.bookTitle}"?`
    );

    if (!confirmed) return;

    setMessage("");
    paymentMutation.mutate(penalty.id);
  }

  const penalties = penaltiesQuery.data ?? [];

  const totalUnpaid = penalties.reduce(
    (sum, penalty) => sum + penalty.fineAmount,
    0
  );

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
          <Wallet size={32} className="text-blue-600" />

          <h1 className="text-3xl font-bold">
            Quản lý tiền phạt
          </h1>
        </div>

        <div className="mb-6 rounded-xl bg-white p-6 shadow-sm">
          <p className="text-slate-500">
            Tổng tiền phạt chưa thanh toán
          </p>

          <p className="mt-2 text-3xl font-bold text-red-600">
            {totalUnpaid.toLocaleString("vi-VN")} VNĐ
          </p>
        </div>

        {message && (
          <p className="mb-5 rounded-lg bg-blue-50 p-4 text-blue-700">
            {message}
          </p>
        )}

        <div className="overflow-x-auto rounded-xl bg-white shadow-sm">
          {penaltiesQuery.isPending ? (
            <p className="p-8">Đang tải tiền phạt...</p>
          ) : penaltiesQuery.isError ? (
            <p className="p-8 text-red-600">
              {errorMessage(penaltiesQuery.error)}
            </p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50">
                <tr>
                  <th className="p-4">Sách</th>
                  <th className="p-4">Mã độc giả</th>
                  <th className="p-4">Lý do</th>
                  <th className="p-4">Tiền phạt</th>
                  <th className="p-4">Thao tác</th>
                </tr>
              </thead>

              <tbody>
                {penalties.map((penalty) => (
                  <tr
                    key={penalty.id}
                    className="border-t border-slate-100"
                  >
                    <td className="p-4 font-medium">
                      {penalty.bookTitle}
                    </td>

                    <td className="p-4 text-slate-500">
                      {penalty.readerId}
                    </td>

                    <td className="p-4">
                      {penalty.reason}
                    </td>

                    <td className="p-4 font-semibold text-red-600">
                      {penalty.fineAmount.toLocaleString(
                        "vi-VN"
                      )}{" "}
                      đ
                    </td>

                    <td className="p-4">
                      <button
                        onClick={() => handlePayment(penalty)}
                        disabled={paymentMutation.isPending}
                        className="rounded-lg bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 disabled:opacity-50"
                      >
                        Xác nhận thanh toán
                      </button>
                    </td>
                  </tr>
                ))}

                {penalties.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="p-10 text-center text-slate-500"
                    >
                      Không có khoản phạt chưa thanh toán.
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
