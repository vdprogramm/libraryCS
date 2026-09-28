import api from "./axios";

export interface BorrowRecord {
  id: string;
  bookId: string;
  bookTitle: string;
  readerId: string;
  borrowDate: string;
  dueDate: string;
  returnDate: string | null;
  status: string;
}

export const borrowRecordsApi = {
  borrow: async (bookId: string) => {
    const response = await api.post(
      "/borrow-records",
      { bookId }
    );

    return response.data;
  },

  getMyRecords: async () => {
    const response = await api.get<BorrowRecord[]>(
      "/borrow-records/my"
    );

    return response.data;
  },

  getOverdue: async () => {
    const response = await api.get<BorrowRecord[]>(
      "/borrow-records/overdue"
    );

    return response.data;
  },

  returnBook: async (recordId: string) => {
    const response = await api.put(
      `/borrow-records/${recordId}/return`
    );

    return response.data;
  },

  getActive: async () => {
    const response = await api.get<BorrowRecord[]>(
      "/borrow-records/active"
    );

    return response.data;
  },
};
