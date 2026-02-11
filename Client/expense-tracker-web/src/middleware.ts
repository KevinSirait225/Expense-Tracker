import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

export function middleware(request: NextRequest){
    const token = request.cookies.get("access_token")?.value;
    const {pathname} = request.nextUrl;

    // kalo path /login, lanjut
    if(pathname.startsWith("/login") || pathname.startsWith("/register")){
        return NextResponse.next();
    }
    // kalo token kosong, redirect to login page
    if(!token){
        const loginUrl =  new URL("/login", request.url);
        return NextResponse.redirect(loginUrl);
    }

    // token ga kosong, lanjut
    return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico).*)"],
};