import { apiFetch } from "@/lib/api";
import { AuthResponse, LoginDto } from "@/types/auth";

export const authService ={
    login(data: LoginDto): Promise<AuthResponse> {
        return apiFetch<AuthResponse>("/auth/login", {
            method:"POST",
            body: JSON.stringify(data),
        });
    },
};