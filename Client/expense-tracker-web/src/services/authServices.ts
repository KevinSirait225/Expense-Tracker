import { apiFetch } from "@/lib/api";
import { AuthResponse, LoginDto, RegisterDto } from "@/types/auth";

export const authService ={
    // Login, authresponse ambil token dari hasil login
    login(data: LoginDto): Promise<AuthResponse> {
        return apiFetch<AuthResponse>("/auth/login", {
            method:"POST",
            body: JSON.stringify(data),
        });
    },

    register(data: RegisterDto): Promise<void> {
        return apiFetch<void>("/auth/register", {
            method:"POST",
            body: JSON.stringify(data),
        });
    },
};