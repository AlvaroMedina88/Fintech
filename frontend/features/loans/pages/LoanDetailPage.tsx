"use client";

import { useLoan, useApproveLoan, useRejectLoan } from "../hooks/useLoans";
import PaymentScheduleTable from "../components/PaymentScheduleTable";
import StatusBadge from "@/shared/components/StatusBadge";
import { Button } from "primereact/button";
import { Card } from "primereact/card";
import { ProgressSpinner } from "primereact/progressspinner";
import { Message } from "primereact/message";
import { useRouter } from "next/navigation";
import { useState } from "react";

const fmt = (v: number) => `$${v.toLocaleString("es-PE", { minimumFractionDigits: 2 })}`;

export default function LoanDetailPage({ id }: { id: string }) {
  const router = useRouter();
  const { data: loan, isLoading, error } = useLoan(id);
  const approve = useApproveLoan();
  const reject = useRejectLoan();
  const [actionError, setActionError] = useState<string | null>(null);

  if (isLoading) return <div className="flex justify-center p-10"><ProgressSpinner /></div>;
  if (error || !loan) return <Message severity="error" text="Préstamo no encontrado" className="w-full" />;

  const handleApprove = async () => {
    try { await approve.mutateAsync(id); }
    catch (e: unknown) { setActionError(e instanceof Error ? e.message : "Error"); }
  };

  const handleReject = async () => {
    try { await reject.mutateAsync(id); }
    catch (e: unknown) { setActionError(e instanceof Error ? e.message : "Error"); }
  };

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center gap-3">
        <Button icon="pi pi-arrow-left" text onClick={() => router.back()} />
        <h1 className="text-2xl font-bold text-slate-800">Detalle del Préstamo</h1>
        <StatusBadge status={loan.status} />
      </div>

      {actionError && <Message severity="error" text={actionError} className="w-full" />}

      <Card className="shadow-sm">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
          <Stat label="Monto" value={fmt(loan.amount)} />
          <Stat label="Plazo" value={`${loan.term} meses`} />
          <Stat label="Cuota mensual" value={fmt(loan.monthlyPayment)} />
          <Stat label="TEA" value={`${(loan.interestRate * 100).toFixed(0)}%`} />
          <Stat label="Tipo" value={loan.loanType} />
          <Stat label="Estado" value={loan.status} />
          <Stat label="Usuario" value={loan.userId} />
          <Stat label="Fecha" value={new Date(loan.createdAt).toLocaleDateString("es-PE")} />
        </div>

        {loan.status === "Pending" && (
          <div className="flex gap-3 mt-6">
            <Button label="Aprobar" icon="pi pi-check" severity="success"
              loading={approve.isPending} onClick={handleApprove} />
            <Button label="Rechazar" icon="pi pi-times" severity="danger"
              loading={reject.isPending} onClick={handleReject} />
          </div>
        )}
      </Card>

      <Card title="Cronograma de Pagos" className="shadow-sm">
        <PaymentScheduleTable schedule={loan.schedule ?? []} />
      </Card>
    </div>
  );
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-xs text-slate-500 mb-1">{label}</p>
      <p className="font-semibold text-slate-800">{value}</p>
    </div>
  );
}
