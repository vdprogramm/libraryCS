import { useState, type FormEvent } from "react";
import {
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import {
  BookOpen,
  Plus,
  Search,
  Pencil,
  Trash2,
  X,
} from "lucide-react";
import axios from "axios";

import { booksApi } from "../../api/books";
import type { Book, BookFormData } from "../../types/book";

const emptyForm: BookFormData = {
  title: "",
  isbn: "",
  categoryId: "",
  authorId: "",
  totalCopies: 1,
  price: 0,
};

function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    return (
      error.response?.data?.detail ||
      error.response?.data?.message ||
      "Không thể thực hiện thao tác."
    );
  }

  return "Đã xảy ra lỗi không xác định.";
}

export default function BooksManagementPage() {
  const queryClient = useQueryClient();

  const [keyword, setKeyword] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingBook, setEditingBook] = useState<Book | null>(null);
  const [form, setForm] = useState<BookFormData>(emptyForm);
  const [error, setError] = useState("");

  const booksQuery = useQuery({
    queryKey: ["books", search, page],
    queryFn: () => booksApi.getAll(search, page),
  });

  const categoriesQuery = useQuery({
    queryKey: ["categories"],
    queryFn: booksApi.getCategories,
  });

  const authorsQuery = useQuery({
    queryKey: ["authors"],
    queryFn: booksApi.getAuthors,
  });

  const saveMutation = useMutation({
    mutationFn: (data: BookFormData) =>
      editingBook
        ? booksApi.update(editingBook.id, data)
        : booksApi.create(data),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["books"],
      });

      closeModal();
    },

    onError: (err) => {
      setError(getErrorMessage(err));
    },
  });

  const deleteMutation = useMutation({
    mutationFn: booksApi.delete,

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["books"],
      });
    },

    onError: (err) => {
      window.alert(getErrorMessage(err));
    },
  });

  function openCreateModal() {
    setEditingBook(null);
    setForm({ ...emptyForm });
    setError("");
    setIsModalOpen(true);
  }

  function openEditModal(book: Book) {
    setEditingBook(book);

    setForm({
      title: book.title,
      isbn: book.isbn,
      categoryId: book.categoryId,
      authorId: book.authorId,
      totalCopies: book.totalCopies,
      price: book.price,
    });

    setError("");
    setIsModalOpen(true);
  }

  function closeModal() {
    setIsModalOpen(false);
    setEditingBook(null);
    setError("");
  }

  function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError("");

    if (!form.categoryId || !form.authorId) {
      setError("Vui lòng chọn thể loại và tác giả.");
      return;
    }

    if (form.totalCopies < 0 || form.price < 0) {
      setError("Số lượng và giá sách không được âm.");
      return;
    }

    saveMutation.mutate(form);
  }

  function handleDelete(book: Book) {
    const confirmed = window.confirm(
      `Bạn có chắc muốn xóa sách "${book.title}"?`
    );

    if (confirmed) {
      deleteMutation.mutate(book.id);
    }
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
            <div className="flex items-center gap-3">
              <BookOpen className="text-blue-600" size={32} />

              <h1 className="text-3xl font-bold text-slate-900">
                Quản lý sách
              </h1>
            </div>

            <p className="mt-2 text-slate-500">
              Thêm, chỉnh sửa và quản lý kho sách thư viện.
            </p>
          </div>

          <button
            onClick={openCreateModal}
            className="flex items-center gap-2 rounded-lg bg-blue-600 px-5 py-3 font-medium text-white hover:bg-blue-700"
          >
            <Plus size={20} />
            Thêm sách
          </button>
        </div>

        {/* Search */}
        <form
          onSubmit={(e) => {
            e.preventDefault();
            setPage(1);
            setSearch(keyword.trim());
          }}
          className="mb-6 flex gap-3 rounded-xl bg-white p-4 shadow-sm"
        >
          <input
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
            placeholder="Tìm theo tên sách hoặc ISBN..."
            className="min-w-0 flex-1 rounded-lg border border-slate-300 px-4 py-3 outline-blue-500"
          />

          <button
            type="submit"
            className="flex items-center gap-2 rounded-lg bg-slate-900 px-5 py-3 text-white hover:bg-slate-700"
          >
            <Search size={18} />
            Tìm kiếm
          </button>
        </form>

        {/* Table */}
        <div className="overflow-hidden rounded-xl bg-white shadow-sm">
          {booksQuery.isPending ? (
            <p className="p-8 text-center text-slate-500">
              Đang tải danh sách sách...
            </p>
          ) : booksQuery.isError ? (
            <p className="p-8 text-center text-red-600">
              {getErrorMessage(booksQuery.error)}
            </p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead className="bg-slate-50 text-slate-600">
                  <tr>
                    <th className="p-4">Tên sách</th>
                    <th className="p-4">ISBN</th>
                    <th className="p-4">Thể loại</th>
                    <th className="p-4">Tác giả</th>
                    <th className="p-4">Số lượng</th>
                    <th className="p-4">Giá</th>
                    <th className="p-4 text-right">Thao tác</th>
                  </tr>
                </thead>

                <tbody>
                  {books.map((book) => (
                    <tr
                      key={book.id}
                      className="border-t border-slate-100 hover:bg-slate-50"
                    >
                      <td className="p-4 font-medium">
                        {book.title}
                      </td>

                      <td className="p-4">{book.isbn}</td>

                      <td className="p-4">
                        {book.categoryName}
                      </td>

                      <td className="p-4">
                        {book.authorName}
                      </td>

                      <td className="p-4">
                        <span
                          className={
                            book.availableCopies > 0
                              ? "font-medium text-green-600"
                              : "font-medium text-red-600"
                          }
                        >
                          {book.availableCopies}
                        </span>
                        /{book.totalCopies}
                      </td>

                      <td className="p-4">
                        {book.price.toLocaleString("vi-VN")} đ
                      </td>

                      <td className="p-4">
                        <div className="flex justify-end gap-2">
                          <button
                            onClick={() => openEditModal(book)}
                            className="rounded-lg p-2 text-blue-600 hover:bg-blue-50"
                            title="Chỉnh sửa"
                          >
                            <Pencil size={18} />
                          </button>

                          <button
                            onClick={() => handleDelete(book)}
                            disabled={deleteMutation.isPending}
                            className="rounded-lg p-2 text-red-600 hover:bg-red-50 disabled:opacity-50"
                            title="Xóa sách"
                          >
                            <Trash2 size={18} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}

                  {books.length === 0 && (
                    <tr>
                      <td
                        colSpan={7}
                        className="p-10 text-center text-slate-500"
                      >
                        Không tìm thấy sách nào.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          )}

          {/* Pagination */}
          <div className="flex items-center justify-between border-t border-slate-100 p-4">
            <span className="text-sm text-slate-500">
              Tổng: {booksQuery.data?.totalCount ?? 0} sách
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

      {/* Create/Edit Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="max-h-[90vh] w-full max-w-xl overflow-y-auto rounded-2xl bg-white p-6 shadow-xl">
            <div className="mb-6 flex items-center justify-between">
              <h2 className="text-xl font-bold">
                {editingBook ? "Chỉnh sửa sách" : "Thêm sách"}
              </h2>

              <button
                onClick={closeModal}
                disabled={saveMutation.isPending}
                className="rounded-lg p-2 hover:bg-slate-100"
              >
                <X size={22} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="mb-1 block font-medium">
                  Tên sách
                </label>

                <input
                  required
                  value={form.title}
                  onChange={(e) =>
                    setForm({ ...form, title: e.target.value })
                  }
                  className="w-full rounded-lg border p-3"
                />
              </div>

              <div>
                <label className="mb-1 block font-medium">
                  ISBN
                </label>

                <input
                  required
                  value={form.isbn}
                  onChange={(e) =>
                    setForm({ ...form, isbn: e.target.value })
                  }
                  className="w-full rounded-lg border p-3"
                />
              </div>

              <div className="grid gap-4 sm:grid-cols-2">
                <div>
                  <label className="mb-1 block font-medium">
                    Thể loại
                  </label>

                  <select
                    required
                    value={form.categoryId}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        categoryId: e.target.value,
                      })
                    }
                    className="w-full rounded-lg border p-3"
                  >
                    <option value="">Chọn thể loại</option>

                    {(categoriesQuery.data ?? []).map(
                      (category) => (
                        <option
                          key={category.id}
                          value={category.id}
                        >
                          {category.name}
                        </option>
                      )
                    )}
                  </select>
                </div>

                <div>
                  <label className="mb-1 block font-medium">
                    Tác giả
                  </label>

                  <select
                    required
                    value={form.authorId}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        authorId: e.target.value,
                      })
                    }
                    className="w-full rounded-lg border p-3"
                  >
                    <option value="">Chọn tác giả</option>

                    {(authorsQuery.data ?? []).map((author) => (
                      <option key={author.id} value={author.id}>
                        {author.name}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="grid gap-4 sm:grid-cols-2">
                <div>
                  <label className="mb-1 block font-medium">
                    Tổng số bản
                  </label>

                  <input
                    type="number"
                    min={0}
                    required
                    value={form.totalCopies}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        totalCopies: Number(e.target.value),
                      })
                    }
                    className="w-full rounded-lg border p-3"
                  />
                </div>

                <div>
                  <label className="mb-1 block font-medium">
                    Giá sách (VNĐ)
                  </label>

                  <input
                    type="number"
                    min={0}
                    required
                    value={form.price}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        price: Number(e.target.value),
                      })
                    }
                    className="w-full rounded-lg border p-3"
                  />
                </div>
              </div>

              {error && (
                <p className="rounded-lg bg-red-50 p-3 text-sm text-red-600">
                  {error}
                </p>
              )}

              <div className="flex justify-end gap-3 pt-4">
                <button
                  type="button"
                  onClick={closeModal}
                  disabled={saveMutation.isPending}
                  className="rounded-lg border px-5 py-3"
                >
                  Hủy
                </button>

                <button
                  type="submit"
                  disabled={
                    saveMutation.isPending ||
                    categoriesQuery.isPending ||
                    authorsQuery.isPending
                  }
                  className="rounded-lg bg-blue-600 px-5 py-3 font-medium text-white hover:bg-blue-700 disabled:opacity-50"
                >
                  {saveMutation.isPending
                    ? "Đang lưu..."
                    : editingBook
                      ? "Lưu thay đổi"
                      : "Thêm sách"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
