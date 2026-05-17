"use client";

import { Button } from "primereact/button";
import { useRouter } from "next/navigation";
import LoanList from "../components/LoanList";

export default function LoanListPage() {
  const router = useRouter();
  return (
    <div>
      <div className="flex items-center justify-between mb-6">
            <h1 className="text-2xl font-bold text-slate-800">Mis Préstamos</h1>
            <Button label="Simular nuevo" className="button-color" icon="pi pi-plus" onClick={() => router.push("/loans/simulate")} />
      </div>
      <LoanList />
    </div>
  );
}
