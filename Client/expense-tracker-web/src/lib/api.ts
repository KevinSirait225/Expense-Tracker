import { getToken } from "./auth";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

export interface ApiError {
  status: number;
  message: string;
}

export async function apiFetch<T>(
  url: string,
  options?: RequestInit
): Promise<T> {
  const token = typeof window !== "undefined" ? getToken() : null;

  const res = await fetch(`${API_BASE_URL}${url}`, {
    headers: {
      "Content-Type": "application/json",
      ...(token && { Authorization: `Bearer ${token}` }),
    },
    ...options,
  });

  if (!res.ok) {
    let message = "Something went wrong2";

    try {
      const data = await res.json();
      message = data?.Message ?? data?.error ?? message;
    } catch {
      // empty body / non-JSON
    }

    throw {
      status: res.status,
      message,
    } as ApiError;
  }

  if (res.status === 204) {
    return undefined as T;
  }

  return res.json();
}
