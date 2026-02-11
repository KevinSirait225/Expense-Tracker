"use client";

import { getErrorMessage } from "@/lib/error";
import { registerSchema } from "@/lib/validation";
import { authService } from "@/services/authServices";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { set } from "zod";

export default function RegisterPage() {
    const router = useRouter();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [errors, setErrors] = useState<Record<string, string>>({}); // Validation error
    const [formErrors, setFormErrors] = useState<string | null>(null); // Backend error
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e:React.SyntheticEvent<HTMLFormElement>) {
        e.preventDefault();
        setErrors({});
        setFormErrors(null);
        setLoading(true);

        // Schema register dipakai untuk validasi data
        const result = registerSchema.safeParse({
            email, password, confirmPassword
        });

        // jika result success set
        if(!result.success){
            const fieldErrors: Record<string, string> = {};

            result.error.issues.forEach((issue) => {
                const field = issue.path[0] as string;
                fieldErrors[field] = issue.message;
            });
            setErrors(fieldErrors);
            setLoading(false);
            return;
        }

        try{
            await authService.register({email,password});
            router.push("/login");
        } catch (err){
            setFormErrors(getErrorMessage(err));
        } finally {
            setLoading(false)
        }
    }

    return (
        <main className="min-h-screen flex items-center justify-center">
        <form
            onSubmit={handleSubmit}
            className="w-full max-w-sm space-y-4 border p-6 rounded"
        >
            <h1 className="text-xl font-bold">Register</h1>

            {formErrors && <p className="text-red-500">{formErrors}</p>}

            {/* Email */}
            <input
            type="email"
            placeholder="Email"
            className="w-full border p-2 rounded"
            value={email}
            onChange={(e) => {
             setEmail(e.target.value);
             
            //  saat diketik hapus error email yang ada sbelumnya 
            if(errors.email){
                setErrors((prev) => {
                    const newErrors = {...prev};
                    delete newErrors.email;
                    return newErrors;
                });
            }
            }}
            />
            {/* Error validation email */}
            {errors.email && (
                <p className="text-red-500 text-sm">{errors.email}</p>
            )}

            {/* Password */}
            <input
            type="password"
            placeholder="Password"
            className="w-full border p-2 rounded"
            value={password}
            onChange={(e) => {
                setPassword(e.target.value);

                //  saat diketik hapus error password yang ada sbelumnya 
                if(errors.password){
                    setErrors((prev) => {
                        const newErrors = {...prev};
                        delete newErrors.password;
                        return newErrors;
                    });
             }
            }}
            />
            {/* Error validation password */}
            {errors.password && (
                <p className="text-red-500 text-sm">{errors.password}</p>
            )}

            {/* Confirm Password */}
            <input
            type="password"
            placeholder="Confirm password"
            className="w-full border p-2 rounded"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            />
            {/* Error validation confirm password */}
            {errors.confirmPassword && (
                <p className="text-red-500 text-sm">{errors.confirmPassword}</p>
            )}

            <button
            disabled={loading}
            className="w-full bg-black text-white py-2 rounded"
            >
            {loading ? "Creating account..." : "Register"}
            </button>

            <p className="text-sm text-center">
            Already have an account?{" "}
            <a href="/login" className="underline">
                Login
            </a>
            </p>
        </form>
        </main>
    );
};
