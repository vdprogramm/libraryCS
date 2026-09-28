export interface Book {
  id: string;
  title: string;
  isbn: string;
  categoryId: string;
  categoryName: string;
  authorId: string;
  authorName: string;
  totalCopies: number;
  availableCopies: number;
  price: number;
}

export interface Category {
  id: string;
  name: string;
}

export interface Author {
  id: string;
  name: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface BookFormData {
  title: string;
  isbn: string;
  categoryId: string;
  authorId: string;
  totalCopies: number;
  price: number;
}
