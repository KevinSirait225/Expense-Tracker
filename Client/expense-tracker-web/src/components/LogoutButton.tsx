"use client";

import { logout } from "@/lib/auth";
import { useRouter } from "next/navigation";

export default function LogoutButton() {
    const router = useRouter();

    function handleLogout(){
        logout();
        router.push("/login");
    }

    return (
        <button
            onClick={handleLogout}
            className="text-sm text-red-600 hover:underline"
        >
            Logout
        </button>
    );
};
