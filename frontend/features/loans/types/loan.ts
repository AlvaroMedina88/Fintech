export interface PaymentScheduleDto {
  paymentNumber: number;
  dueDate: string;
  totalPayment: number;
  principal: number;
  interest: number;
  remainingBalance: number;
  status: string;
}

export interface LoanResponse {
  id: string;
  userId: string;
  amount: number;
  term: number;
  interestRate: number;
  loanType: string;
  status: string;
  monthlyPayment: number;
  createdAt: string;
}

export interface LoanDetailResponse extends LoanResponse {
  schedule: PaymentScheduleDto[];
}

export interface SimulateLoanRequest {
  amount: number;
  term: number;
  tea: number;
}

  export interface SimulateLoanResponse {
    amount: number;
    term: number;
    tea: number;
    tem: number;
    monthlyPayment: number;
    schedule: PaymentScheduleDto[];
  }

export interface CreateLoanRequest {
  userId: string;
  amount: number;
  term: number;
  monthlyIncome: number;
  tea: number;
}
