// untuk cek errornya JSON object dari backend, unexpected error, atau real error
export function getErrorMessage(error: unknown): string {
    // Kalo error dari JS/library, jalan yg ini
    if (error instanceof Error) return error.message;
    
    // Cek error dari object (contoh: key json dari backend), disini cek yg ada key error dan message
    if (typeof error === "object" && error !== null) {
        // kalo key nya message
        if ("message" in error) {
            return String((error as { message: unknown }).message);
        }
        // kalo keynya error
        if ("error" in error) {
            return String((error as { error: unknown }).error);
        }
  }

  return "Something went wrong";
}
