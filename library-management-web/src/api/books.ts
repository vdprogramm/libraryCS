import api from "./axios";

import type {
  Book,
  BookFormData,
  Category,
  Author,
  PagedResult,
} from "../types/book";

export const booksApi = {
  getAll: async (keyword = "", pageNumber = 1) => {
    const response = await api.get<PagedResult<Book>>("/books", {
      params: {
        keyword,
        pageNumber,
        pageSize: 10,
      },
    });

    return response.data;
  },

  create: async (data: BookFormData) => {
    const response = await api.post<Book>("/books", data);
    return response.data;
  },

  update: async (id: string, data: BookFormData) => {
    const response = await api.put<Book>(`/books/${id}`, { ...data, id });
    return response.data;
  },

  delete: async (id: string) => {
    await api.delete(`/books/${id}`);
  },

  getCategories: async () => {
    const response = await api.get<Category[]>("/categories");
    return response.data;
  },

  getAuthors: async () => {
    const response = await api.get<Author[]>("/authors");
    return response.data;
  },
};
