"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { loanService } from "../services/loanService";
import type { CreateLoanRequest, SimulateLoanRequest } from "../types/loan";

const LOANS_KEY = "loans";

export function useLoans(userId?: string) {
  return useQuery({
    queryKey: [LOANS_KEY, userId],
    queryFn: () => loanService.getAll(userId),
  });
}

export function useLoan(id: string) {
  return useQuery({
    queryKey: [LOANS_KEY, id],
    queryFn: () => loanService.getById(id),
    enabled: !!id,
  });
}

export function useSimulateLoan() {
  return useMutation({
    mutationFn: (data: SimulateLoanRequest) => loanService.simulate(data),
  });
}

export function useCreateLoan() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateLoanRequest) => loanService.create(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: [LOANS_KEY] }),
  });
}

export function useApproveLoan() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => loanService.approve(id),

    onSuccess: () => qc.invalidateQueries({ queryKey: [LOANS_KEY] }),
  });
}

export function useRejectLoan() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => loanService.reject(id),
    
    onSuccess: () => qc.invalidateQueries({ queryKey: [LOANS_KEY] }),
  });
}
