import { Tag } from "primereact/tag";

const severityMap: Record<string, "success" | "info" | "warning" | "danger" | "secondary"> = {
  Active: "success",
  Approved: "success",
  Completed: "success",
  Pending: "warning",
  Paid: "info",
  Rejected: "danger",
  Failed: "danger",
};

export default function StatusBadge({ status }: Readonly<{ status: string }>) {
  return <Tag value={status} severity={severityMap[status] ?? "secondary"} />;
}
