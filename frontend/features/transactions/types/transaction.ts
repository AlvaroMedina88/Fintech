export interface TransactionResponse {
  id: string;
  idempotencyKey: string;
  type: string;
  amount: number;
  status: string;
  loanId: string | null;
  description: string;
  createdAt: string;
}

  export interface CreateTransactionRequest {
    idempotencyKey: string;
    type: string;
    amount: number;
    loanId?: string;
    description: string;
  }
