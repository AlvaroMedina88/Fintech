"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { transactionService } from "../services/transactionService";
import type { CreateTransactionRequest } from "../types/transaction";

const TX_KEY = "transactions";

export function useTransactions(type?: string, status?: string) {
  return useQuery({
    queryKey: [TX_KEY, type, status],
    queryFn: () => transactionService.getAll(type, status),
  });
}

export function useCreateTransaction() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateTransactionRequest) => transactionService.create(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: [TX_KEY] }),
  });
}
