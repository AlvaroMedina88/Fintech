"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Menubar } from "primereact/menubar";
import { MenuItem } from "primereact/menuitem";

export default function Navbar() {
  const pathname = usePathname();

      const items: MenuItem[] = [
        { label: "Inicio", icon: "pi pi-home", url: "/" },
        { label: "Simular Préstamo", icon: "pi pi-calculator", url: "/loans/simulate" },
        { label: "Mis Préstamos", icon: "pi pi-list", url: "/loans" },
        { label: "Transacciones", icon: "pi pi-credit-card", url: "/transactions" },
      ];

  const start = (
    <Link href="/" className="flex items-center gap-2 font-bold text-blue-700 text-lg mr-6">
      
      <i className="pi pi-building-columns text-xl icono" />
      SGIP
    </Link>
  );

  return <Menubar model={items} start={start} className="border-0 border-b rounded-none shadow-sm px-4 navbar" />;
}
