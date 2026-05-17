import api from "@/shared/lib/api";
import type { CreateTransactionRequest, TransactionResponse } from "../types/transaction";

export const transactionService = {
  create: (data: CreateTransactionRequest) =>
    api.post<TransactionResponse>("/api/transactions", data).then(r => r.data),

  getAll: (type?: string, status?: string) =>
    api.get<TransactionResponse[]>("/api/transactions", { params: { type, status } }).then(r => r.data),

  getById: (id: string) =>
    api.get<TransactionResponse>(`/api/transactions/${id}`).then(r => r.data),
};
