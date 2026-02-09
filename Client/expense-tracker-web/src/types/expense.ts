export interface Expense {
    id: number;
    amount: number;
    description: string;
    date: string;
}

export interface CreateExpenseDto{
    amount: number;
    description: string;
}

export interface UpdateExpenseDto{
    amount: number;
    description: string;
}