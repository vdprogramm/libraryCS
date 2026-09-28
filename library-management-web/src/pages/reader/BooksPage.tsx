import { useState } from "react";
import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  BookOpen,
  Search,
  BookMarked,
  ArrowLeft,
} from "lucide-react";
import { Link } from "react-router-dom";
import axios from "axios";

import { booksApi } from "../../api/books";
import { borrowRecordsApi } from "../../api/borrowRecords";
import type { Book } from "../../types/book";

function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    return (
      error.response?.data?.detail ||
      error.response?.data?.message ||
      "Không thể mượn sách."
    );
  }

  return "Đã xảy ra lỗi.";
}

export default function BooksPage() {
  const queryClient = useQueryClient();

  const [keyword, setKeyword] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [selectedBook, setSelectedBook] = useState<Book | null>(
    null
  );

  const [message, setMessage] = useState("");

  const booksQuery = useQuery({
    queryKey: ["books", search, page],
    queryFn: () => booksApi.getAll(search, page),
  });

  const borrowMutation = useMutation({
    mutationFn: borrowRecordsApi.borrow,

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["books"],
        }),
        queryClient.invalidateQueries({
          queryKey: ["myBorrowRecords"],
        }),
      ]);

      setSelectedBook(null);
      setMessage("Mượn sách thành công!");
    },

    onError: (error) => {
      setMessage(getErrorMessage(error));
      setSelectedBook(null);
    },
  });

  function handleBorrow(book: Book) {
    setMessage("");
    setSelectedBook(book);
  }

  function confirmBorrow() {
    if (!selectedBook) return;

    borrowMutation.mutate(selectedBook.id);
  }

  const books = booksQuery.data?.items ?? [];
  const totalPages = Math.max(
    1,
    booksQuery.data?.totalPages ?? 1
  );

  return (
    <div className="min-h-screen bg-slate-100 p-4 md:p-8">
      <div className="mx-auto max-w-7xl">
        {/* Header */}
        <div className="mb-8 flex flex-wrap items-center justify-between gap-4">
          <div>
            <Link
              to="/reader"
              className="mb-4 inline-flex items-center gap-2 text-sm text-slate-500 hover:text-blue-600"
            >
              <ArrowLeft size={16} />
              Quay lại Dashboard
            </Link>

            <div className="flex items-center gap-3">
              <BookOpen size={32} className="text-blue-600" />

              <h1 className="text-3xl font-bold text-slate-900">
                Thư viện sách
              </h1>
            </div>

            <p className="mt-2 text-slate-500">
              Khám phá và mượn sách trực tuyến.
            </p>
          </div>

          <Link
            to="/reader/my-borrows"
            className="flex items-center gap-2 rounded-lg bg-blue-600 px-5 py-3 text-white hover:bg-blue-700"
          >
            <BookMarked size={19} />
            Sách đã mượn
          </Link>
        </div>

        {/* Search */}
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
            placeholder="Tìm theo tên sách hoặc ISBN..."
            className="min-w-0 flex-1 rounded-lg border border-slate-300 px-4 py-3 outline-blue-500"
          />

          <button
            type="submit"
            className="flex items-center gap-2 rounded-lg bg-slate-900 px-5 py-3 text-white"
          >
            <Search size={18} />
            Tìm kiếm
          </button>
        </form>

        {message && (
          <div className="mb-6 rounded-lg border border-blue-200 bg-blue-50 p-4 text-blue-700">
            {message}
          </div>
        )}

        {/* Book list */}
        {booksQuery.isPending ? (
          <p>Đang tải sách...</p>
        ) : booksQuery.isError ? (
          <p className="text-red-600">
            {getErrorMessage(booksQuery.error)}
          </p>
        ) : (
          <>
            <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
              {books.map((book) => (
                <div
                  key={book.id}
                  className="flex flex-col rounded-2xl bg-white p-6 shadow-sm transition hover:shadow-md"
                >
                  <div className="mb-5 flex h-32 items-center justify-center rounded-xl bg-blue-50">
                    <BookOpen
                      size={56}
                      className="text-blue-400"
                    />
                  </div>

                  <h2 className="text-xl font-bold text-slate-900">
                    {book.title}
                  </h2>

                  <p className="mt-2 text-sm text-slate-500">
                    Tác giả: {book.authorName}
                  </p>

                  <p className="mt-1 text-sm text-slate-500">
                    Thể loại: {book.categoryName}
                  </p>

                  <p className="mt-1 text-sm text-slate-500">
                    ISBN: {book.isbn}
                  </p>

                  <div className="mt-5 flex items-center justify-between">
                    <span
                      className={`rounded-full px-3 py-1 text-sm font-medium ${
                        book.availableCopies > 0
                          ? "bg-green-100 text-green-700"
                          : "bg-red-100 text-red-700"
                      }`}
                    >
                      {book.availableCopies > 0
                        ? `Còn ${book.availableCopies} bản`
                        : "Hết sách"}
                    </span>

                    <span className="text-sm text-slate-400">
                      Tổng: {book.totalCopies}
                    </span>
                  </div>

                  <button
                    onClick={() => handleBorrow(book)}
                    disabled={
                      book.availableCopies <= 0 ||
                      borrowMutation.isPending
                    }
                    className="mt-6 rounded-lg bg-blue-600 py-3 font-medium text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-slate-300"
                  >
                    {book.availableCopies > 0
                      ? "Mượn sách"
                      : "Không khả dụng"}
                  </button>
                </div>
              ))}
            </div>

            {books.length === 0 && (
              <p className="rounded-xl bg-white p-10 text-center text-slate-500">
                Không tìm thấy sách phù hợp.
              </p>
            )}

            {/* Pagination */}
            <div className="mt-8 flex items-center justify-center gap-4">
              <button
                disabled={page <= 1}
                onClick={() => setPage((p) => p - 1)}
                className="rounded-lg border bg-white px-4 py-2 disabled:opacity-40"
              >
                Trước
              </button>

              <span>
                Trang {page}/{totalPages}
              </span>

              <button
                disabled={page >= totalPages}
                onClick={() => setPage((p) => p + 1)}
                className="rounded-lg border bg-white px-4 py-2 disabled:opacity-40"
              >
                Sau
              </button>
            </div>
          </>
        )}
      </div>

      {/* Borrow confirmation */}
      {selectedBook && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl">
            <h2 className="text-xl font-bold">
              Xác nhận mượn sách
            </h2>

            <p className="mt-4 text-slate-600">
              Bạn muốn mượn cuốn sách:
            </p>

            <p className="mt-2 font-semibold text-blue-700">
              {selectedBook.title}
            </p>

            <p className="mt-4 text-sm text-slate-500">
              Thời hạn mượn: 14 ngày.
            </p>

            <div className="mt-6 flex justify-end gap-3">
              <button
                onClick={() => setSelectedBook(null)}
                disabled={borrowMutation.isPending}
                className="rounded-lg border px-5 py-2"
              >
                Hủy
              </button>

              <button
                onClick={confirmBorrow}
                disabled={borrowMutation.isPending}
                className="rounded-lg bg-blue-600 px-5 py-2 text-white disabled:opacity-50"
              >
                {borrowMutation.isPending
                  ? "Đang xử lý..."
                  : "Xác nhận"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
