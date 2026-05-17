"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { InputNumber } from "primereact/inputnumber";
import { Button } from "primereact/button";
import { Card } from "primereact/card";
import { Message } from "primereact/message";
import { useSimulateLoan, useCreateLoan } from "../hooks/useLoans";
import PaymentScheduleTable from "./PaymentScheduleTable";
import type { SimulateLoanResponse } from "../types/loan";
import { useState } from "react";
import { useRouter } from "next/navigation";

const schema = z.object({
  amount: z.number({ error: "Ingrese el monto" }).min(500, "Mínimo $500").max(50000, "Máximo $50,000"),
  term: z.number({ error: "Ingrese el plazo" }).int().min(6, "Mínimo 6 meses").max(60, "Máximo 60 meses"),
  monthlyIncome: z.number({ error: "Ingrese su ingreso" }).positive("Debe ser positivo"),
});

type FormValues = z.infer<typeof schema>;

export default function LoanSimulator() {
  const router = useRouter();
  const [result, setResult] = useState<SimulateLoanResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const simulate = useSimulateLoan();
  const createLoan = useCreateLoan();
  const { handleSubmit, setValue, watch, formState: { errors } } = useForm<FormValues>({
resolver: zodResolver(schema),
    defaultValues: { amount: undefined, term: undefined, monthlyIncome: undefined },
  });

  const values = watch();

  const onCalculate = handleSubmit(async (data) => {
    setError(null);
    try {
      const res = await simulate.mutateAsync({ amount: data.amount, term: data.term, tea: 0.24 });
      setResult(res);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Error al simular");
    }
  });

  const onApply = async () => {
    setError(null);
    try {
      await createLoan.mutateAsync({
        userId: "user-001",
        amount: values.amount,
        term: values.term,
        monthlyIncome: values.monthlyIncome,
        tea: 0.24,
      });
      router.push("/loans");
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Error al solicitar prestamo");
    }
  };

  return (
    <div className="flex flex-col gap-6">
      <Card title="" className="shadow-sm">
        <form onSubmit={onCalculate} className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-slate-700">Monto ($500 - $50,000)</label>
            <InputNumber
              value={values.amount ?? null}
              onValueChange={(e) => setValue("amount", e.value ?? 0)}
              mode="currency" currency="USD" locale="es-PE" currencyDisplay="narrowSymbol" min={500} max={50000}
              placeholder="Ej: 5000" className="w-full"
              invalid={!!errors.amount}
            />
            {errors.amount && <small className="text-red-500">{errors.amount.message}</small>}
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-slate-700">Plazo (6 - 60 meses)</label>
            <InputNumber
              value={values.term ?? null}
              onValueChange={(e) => setValue("term", e.value ?? 0)}
              suffix=" meses" min={6} max={60} placeholder="Ej: 12"
              className="w-full" invalid={!!errors.term}
            />
            {errors.term && <small className="text-red-500">{errors.term.message}</small>}
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-slate-700">Ingreso mensual</label>
            <InputNumber
              value={values.monthlyIncome ?? null}
              onValueChange={(e) => setValue("monthlyIncome", e.value ?? 0)}
              mode="currency" currency="USD" locale="es-PE" currencyDisplay="narrowSymbol" min={1}
              placeholder="Ej: 3000" className="w-full" invalid={!!errors.monthlyIncome}
            />
            {errors.monthlyIncome && <small className="text-red-500">{errors.monthlyIncome.message}</small>}
          </div>

          <div className="flex items-end">
            <Button label="Calcular" icon="pi pi-calculator" type="submit"
              loading={simulate.isPending} className="w-full" />
          </div>
        </form>

   {error && <Message severity="error" text={error} className="mt-4 w-full" />}
      </Card>

      {result && (
        <Card title="Resultado de Simulación" className="shadow-sm">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
            <Stat label="Monto" value={`$${result.amount.toLocaleString()}`} />
            <Stat label="Cuota mensual" value={`$${result.monthlyPayment.toFixed(2)}`} />
            <Stat label="TEA" value={`${(result.tea * 100).toFixed(0)}%`} />
            <Stat label="TEM" value={`${(result.tem * 100).toFixed(4)}%`} />
          </div>

  <PaymentScheduleTable schedule={result.schedule} />

          <div className="mt-4 flex justify-end">
            <Button label="Solicitar préstamo" icon="pi pi-send"
              severity="success" loading={createLoan.isPending} onClick={onApply} />
          </div>
        </Card>
      )}
    </div>
  );
}



function Stat({ label, value }: { label: string; value: string }) {
  return (
        <div className="bg-blue-50 rounded-lg p-3 text-center">
          <p className="text-xs text-slate-500 mb-1">{label}</p>
          <p className="text-lg font-bold text-blue-700">{value}</p>
        </div>
  );
}
