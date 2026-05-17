"use client";

import { useState } from "react";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { Dropdown } from "primereact/dropdown";
import { Button } from "primereact/button";
import { ProgressSpinner } from "primereact/progressspinner";
import { Message } from "primereact/message";
import { Card } from "primereact/card";
import StatusBadge from "@/shared/components/StatusBadge";
import { useTransactions, useCreateTransaction } from "../hooks/useTransactions";
import type { TransactionResponse } from "../types/transaction";

const TYPE_OPTIONS = [
  { label: "Todos los tipos", value: "" },
  { label: "Desembolso", value: "Disbursement" },
  { label: "Pago", value: "Payment" },
];

const STATUS_OPTIONS = [
  { label: "Todos los estados", value: "" },
  { label: "Pendiente", value: "Pending" },
  { label: "Completado", value: "Completed" },
  { label: "Fallido", value: "Failed" },
];

const fmt = (v: number) => `$${v.toLocaleString("es-PE", { minimumFractionDigits: 2 })}`;

export default function TransactionListPage() {
  const [typeFilter, setTypeFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [error, setError] = useState<string | null>(null);

  const { data: transactions, isLoading, isError } = useTransactions(typeFilter || undefined, statusFilter || undefined);
  const createTx = useCreateTransaction();

  const simulatePayment = async () => {
    setError(null);
    try {
      await createTx.mutateAsync({
        idempotencyKey: `payment-${Date.now()}-${Math.random().toString(36).slice(2)}`,
        type: "Payment",
        amount: Math.round(Math.random() * 400 + 100),
        description: "Pago de cuota simulado",
      });
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Error al crear transacción");
    }
  };

    if (isLoading) return <div className="flex justify-center p-10"><ProgressSpinner /></div>;
  if (isError) return <Message severity="error" text="Error al carcar transacciones" className="w-full" />;

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-slate-800">Transacciones</h1>
        <Button label="Simular pago" icon="pi pi-plus" severity="success" className="button-color"
          loading={createTx.isPending} onClick={simulatePayment} />
      </div>

      {error && <Message severity="error" text={error} className="w-full" />}

      <Card className="shadow-sm">
        <div className="flex gap-4 mb-4">
          <Dropdown value={typeFilter} options={TYPE_OPTIONS} onChange={(e) => setTypeFilter(e.value)}
            placeholder="Tipo" className="w-48" />
          <Dropdown value={statusFilter} options={STATUS_OPTIONS} onChange={(e) => setStatusFilter(e.value)}
            placeholder="Estado" className="w-48" />
        </div>

        <DataTable value={transactions ?? []} stripedRows emptyMessage="No hay transacciones.">
          <Column field="id" header="ID" body={(r: TransactionResponse) => r.id.slice(0, 8) + "..."} />
          <Column field="type" header="Tipo" body={(r) =>
            r.type === "Disbursement" ? "Desembolso" : "Pago"} />
          <Column field="amount" header="Monto" body={(r) => fmt(r.amount)} />
          <Column field="status" header="Estado" body={(r) => <StatusBadge status={r.status} />} />
          <Column field="description" header="Descripción" />
          <Column field="createdAt" header="Fecha"
            body={(r) => new Date(r.createdAt).toLocaleDateString("es-PE")} />
        </DataTable>
      </Card>

    </div>
  );
}
