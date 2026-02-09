"use client";

import { useEffect, useState } from "react";
import { expenseService } from "@/services/expenseService";
import { Expense } from "@/types/expense";
import { getErrorMessage } from "@/lib/error";

export default function Home() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    expenseService
      .getAll()
      .then(setExpenses)
      .catch((err) => {
        if (err?.status === 401) {
          window.location.href = "/login";
        } else {
          setError(getErrorMessage(err));
        }
      });
  }, []);

  if (error) return <p className="text-red-500">{error}</p>;

  return (
    <main className="p-6 max-w-xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">Expenses</h1>

      <ul className="space-y-2">
        {expenses.map((e) => (
          <li key={e.id} className="border p-3 rounded">
            ${e.amount} – {e.description}
          </li>
        ))}
      </ul>
    </main>
  );
}
