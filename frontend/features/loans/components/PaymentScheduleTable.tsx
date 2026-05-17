"use client";

import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import StatusBadge from "@/shared/components/StatusBadge";
import type { PaymentScheduleDto } from "../types/loan";

const fmt = (v: number) => `$${v.toLocaleString("es-PE", { minimumFractionDigits: 2 })}`;
const fmtDate = (d: string) => new Date(d).toLocaleDateString("es-PE");

export default function PaymentScheduleTable({ schedule }: { schedule: PaymentScheduleDto[] }) {
  return (
    <DataTable value={schedule} size="small" stripedRows scrollable scrollHeight="400px"
      emptyMessage="Sin cronograma disponible">
    <Column field="paymentNumber" header="#" style={{ width: "60px" }} />
    <Column field="dueDate" header="Fecha" body={(r) => fmtDate(r.dueDate)} />
    <Column field="totalPayment" header="Cuota" body={(r) => fmt(r.totalPayment)} />
    <Column field="principal" header="Capital" body={(r) => fmt(r.principal)} />
    <Column field="interest" header="Interés" body={(r) => fmt(r.interest)} />
    <Column field="remainingBalance" header="Saldo" body={(r) => fmt(r.remainingBalance)} />
    <Column field="status" header="Estado" body={(r) => <StatusBadge status={r.status} />} />
    </DataTable>
  );
}
