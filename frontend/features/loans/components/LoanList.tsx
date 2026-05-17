"use client";

import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { Button } from "primereact/button";
import { ProgressSpinner } from "primereact/progressspinner";
import { Message } from "primereact/message";
import StatusBadge from "@/shared/components/StatusBadge";
import { useLoans } from "../hooks/useLoans";
import { useRouter } from "next/navigation";
import type { LoanResponse } from "../types/loan";

const fmt = (v: number) => `$${v.toLocaleString("es-PE", { minimumFractionDigits: 2 })}`;

export default function LoanList() {
  const router = useRouter();
  const { data: loans, isLoading, error } = useLoans("user-001");

  if (isLoading) return <div className="flex justify-center p-10"><ProgressSpinner /></div>;
  if (error) return <Message severity="error" text={error.message} className="w-full" />;

  return (
    <DataTable
      value={loans ?? []}
      emptyMessage="No se tiene prestamos aún."

      stripedRows
      rowClassName={() => "cursor-pointer"}
      onRowClick={(e) => router.push(`/loans/${(e.data as LoanResponse).id}`)}
    >
      <Column field="id" header="ID" body={(r: LoanResponse) => r.id.slice(0, 8) + "..."} />
        <Column field="amount" header="Monto" body={(r) => fmt(r.amount)} />
        <Column field="term" header="Plazo" body={(r) => `${r.term} meses`} />
        <Column field="monthlyPayment" header="Cuota mensual" body={(r) => fmt(r.monthlyPayment)} />
       <Column field="status" header="Estado" body={(r) => <StatusBadge status={r.status} />} />
       <Column field="createdAt" header="Fecha" body={(r) => new Date(r.createdAt).toLocaleDateString("es-PE")} />
      <Column body={(r: LoanResponse) => (
        <Button icon="pi pi-eye" rounded text onClick={(e) => { e.stopPropagation(); router.push(`/loans/${r.id}`); }} />
      )} />
    </DataTable>
  );
}
