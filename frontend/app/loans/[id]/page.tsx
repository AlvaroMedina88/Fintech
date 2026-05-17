"use client";

import LoanDetailPage from "@/features/loans/pages/LoanDetailPage";
import { use } from "react";

export default function Page({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  return <LoanDetailPage id={id} />;
}
