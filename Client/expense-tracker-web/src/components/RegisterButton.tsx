"use client";

import { useRouter } from "next/navigation";

export default function RegisterButton() {
    const router = useRouter();

    function handleRegister(){
        router.push("/register");
    }

    return (
        <button
            onClick={handleRegister}
            className="text-sm text-white hover:underline"
        >
            Register
        </button>
    );
};
