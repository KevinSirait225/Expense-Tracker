"use client";

import { saveToken } from "@/lib/auth";
import { getErrorMessage } from "@/lib/error";
import { authService } from "@/services/authServices";
import { useRouter } from "next/navigation";
import { useState } from "react";

export default function LoginPage() {
    const router = useRouter();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e: React.SyntheticEvent<HTMLFormElement>) {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try{
            const res = await authService.login({email, password});
            saveToken(res.token);
            router.push("/");
        } catch (err){
            setError(getErrorMessage(err));
        } finally {
            setLoading(false);
        }
    }

    return (
        <main className="min-h-screen flex items-center justify-center">
        <form
            onSubmit={handleSubmit}
            className="w-full max-w-sm space-y-4 border p-6 rounded"
        >
            <h1 className="text-xl font-bold">Login</h1>

            {error && <p className="text-red-500">{error}</p>}

            <input
            type="email"
            placeholder="Email"
            className="w-full border p-2 rounded"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            />

            <input
            type="password"
            placeholder="Password"
            className="w-full border p-2 rounded"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            />

            <button
            disabled={loading}
            className="w-full bg-black text-white py-2 rounded"
            >
            {loading ? "Logging in..." : "Login"}
            </button>
        </form>
        </main>
    );
};
