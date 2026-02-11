import z from "zod";

// validation untuk register
export const registerSchema = z.object({
    // format email dgn min character 1, kalau salah message muncul
    email: z.email("Invalid email format").min(1, "Email is required"),
    password: z.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: z.string(),
})
.refine((data) => data.password === data.confirmPassword, {
    message: "Password do not match",
    path: ["confirmPassword"],
});

export const loginSchema = z.object({
    email: z.email("Invalid email format").min(1, "Email is required"),
    password: z.string().min(6, "Password must be at least 6 characters"),
});