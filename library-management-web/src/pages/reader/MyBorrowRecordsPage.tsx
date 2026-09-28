import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { ArrowLeft, BookMarked } from "lucide-react";
import { borrowRecordsApi } from "../../api/borrowRecords";

function formatDate(value: string | null) {
  if (!value) return "—";

  return new Date(value).toLocaleDateString("vi-VN");
}

export default function MyBorrowRecordsPage() {
  const { data, isPending, isError } = useQuery({
    queryKey: ["myBorrowRecords"],
    queryFn: borrowRecordsApi.getMyRecords,
  });

  const records = data ?? [];

  return (
    <div className="min-h-screen bg-slate-100 p-4 md:p-8">
      <div className="mx-auto max-w-6xl">
        <Link
          to="/reader/books"
          className="mb-5 inline-flex items-center gap-2 text-slate-500 hover:text-blue-600"
        >
          <ArrowLeft size={18} />
          Quay lại thư viện
        </Link>

        <div className="mb-8 flex items-center gap-3">
          <BookMarked size={32} className="text-blue-600" />

          <h1 className="text-3xl font-bold">
            Lịch sử mượn sách
          </h1>
        </div>

        <div className="overflow-x-auto rounded-xl bg-white shadow-sm">
          {isPending ? (
            <p className="p-8">Đang tải lịch sử...</p>
          ) : isError ? (
            <p className="p-8 text-red-600">
              Không thể tải lịch sử mượn sách.
            </p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50">
                <tr>
                  <th className="p-4">Tên sách</th>
                  <th className="p-4">Ngày mượn</th>
                  <th className="p-4">Hạn trả</th>
                  <th className="p-4">Ngày trả</th>
                  <th className="p-4">Trạng thái</th>
                </tr>
              </thead>

              <tbody>
                {records.map((record) => {
                  const isOverdue =
                    record.returnDate === null &&
                    new Date(record.dueDate).getTime() < Date.now();

                  return (
                    <tr
                      key={record.id}
                      className="border-t border-slate-100"
                    >
                      <td className="p-4 font-medium">
                        {record.bookTitle}
                      </td>

                      <td className="p-4">
                        {formatDate(record.borrowDate)}
                      </td>

                      <td className="p-4">
                        {formatDate(record.dueDate)}
                      </td>

                      <td className="p-4">
                        {formatDate(record.returnDate)}
                      </td>

                      <td className="p-4">
                        <span
                          className={`rounded-full px-3 py-1 text-xs font-medium ${
                            isOverdue || record.status === "Overdue"
                              ? "bg-red-100 text-red-700"
                              : record.status === "Borrowing"
                                ? "bg-blue-100 text-blue-700"
                                : "bg-green-100 text-green-700"
                          }`}
                        >
                          {isOverdue || record.status === "Overdue"
                            ? "Đang quá hạn"
                            : record.status === "Borrowing"
                              ? "Đang mượn"
                              : "Đã trả"}
                        </span>
                      </td>
                    </tr>
                  );
                })}

                {records.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="p-10 text-center text-slate-500"
                    >
                      Bạn chưa mượn cuốn sách nào.
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
