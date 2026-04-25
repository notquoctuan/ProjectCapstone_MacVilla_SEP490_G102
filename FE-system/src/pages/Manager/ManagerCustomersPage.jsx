import { CustomersListPage } from "@/pages/customers/CustomersListPage";

export function ManagerCustomersPage() {
  return (
    <CustomersListPage
      customersBase="/manager/sales/customers"
      formIdPrefix="manager-customer"
      listDescription="Tra cứu B2B/B2C, công nợ — `khach-hang-va-cong-no.md`. Cùng API `/api/admin/customers`."
    />
  );
}
