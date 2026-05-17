import api from "@/shared/lib/api";
import type {
  CreateLoanRequest, LoanDetailResponse, LoanResponse,
  SimulateLoanRequest, SimulateLoanResponse, PaymentScheduleDto
} from "../types/loan";

export const loanService = {
  simulate: (data: SimulateLoanRequest) =>
    api.post<SimulateLoanResponse>("/api/loans/simulate", data).then(r => r.data),

  create: (data: CreateLoanRequest) =>
    api.post<LoanResponse>("/api/loans", data).then(r => r.data),

  getAll: (userId?: string) =>
    api.get<LoanResponse[]>("/api/loans", { params: userId ? { userId } : {} }).then(r => r.data),

  getById: (id: string) =>
    api.get<LoanDetailResponse>(`/api/loans/${id}`).then(r => r.data),

  getSchedule: (id: string) =>
    api.get<PaymentScheduleDto[]>(`/api/loans/${id}/schedule`).then(r => r.data),

  approve: (id: string) =>
    api.patch<LoanResponse>(`/api/loans/${id}/approve`).then(r => r.data),

  reject: (id: string) =>
    api.patch<LoanResponse>(`/api/loans/${id}/reject`).then(r => r.data),
};
