import { apiFetch } from "@/lib/api";
import { CreateExpenseDto, Expense, UpdateExpenseDto } from "@/types/expense";

export const expenseService = {
    getAll(): Promise<Expense[]> {
        return apiFetch("/expenses");
    },

    create(data: CreateExpenseDto): Promise<Expense> {
        return apiFetch("/expenses", {
            method: "POST",
            body:JSON.stringify(data),
        });
    },
    update(id:number, data: UpdateExpenseDto): Promise<void> {
        return apiFetch(`/expenses/${id}`, {
            method: "PUT",
            body:JSON.stringify(data),
        });
    },
    remove(id:number): Promise<void> {
        return apiFetch(`/expenses/${id}`, {
            method: "DELETE",
        });
    },
}