import api from "./axios";

export interface Penalty {
  id: string;
  borrowRecordId: string;
  readerId: string;
  bookTitle: string;
  fineAmount: number;
  paymentStatus: string;
  reason: string;
  paidAt: string | null;
}

export const penaltiesApi = {
  getAll: async (paymentStatus = "Unpaid") => {
    const response = await api.get<Penalty[]>("/penalties", {
      params: { paymentStatus },
    });

    return response.data;
  },

  markAsPaid: async (id: string) => {
    const response = await api.put(`/penalties/${id}/pay`);

    return response.data;
  },
};
