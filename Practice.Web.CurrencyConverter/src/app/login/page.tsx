"use client";

import { signIn } from "next-auth/react";
import { useEffect } from "react";
import { useSearchParams } from "next/navigation";

export default function LoginPage() {
  const searchParams = useSearchParams();
  const error = searchParams.get("error");
  const loggedOut = searchParams.get("logged_out") === "true";

  useEffect(() => {
    // Don't auto-redirect after an explicit logout or when there's an error
    if (loggedOut || error) return;
    signIn("keycloak", { callbackUrl: "/chat" });
  }, [error, loggedOut]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 dark:bg-gray-900">
      <div className="text-center">
        <div className="mb-4 text-4xl">💱</div>
        <h1 className="mb-2 text-2xl font-bold text-gray-900 dark:text-white">
          Currency Converter AI
        </h1>
        {error ? (
          <div className="mt-4">
            <p className="text-red-600 dark:text-red-400 font-medium">
              Auth error: {error}
            </p>
            <button
              onClick={() => signIn("keycloak", { callbackUrl: "/chat" })}
              className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Try again
            </button>
          </div>
        ) : loggedOut ? (
          <div className="mt-4">
            <p className="text-gray-600 dark:text-gray-400">
              You have been signed out.
            </p>
            <button
              onClick={() => signIn("keycloak", { callbackUrl: "/chat" })}
              className="mt-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              Sign in
            </button>
          </div>
        ) : (
          <>
            <p className="text-gray-600 dark:text-gray-400">
              Redirecting to login...
            </p>
            <div className="mt-4 h-1 w-32 mx-auto rounded-full bg-blue-200 overflow-hidden">
              <div className="h-full bg-blue-600 animate-pulse rounded-full" />
            </div>
          </>
        )}
      </div>
    </div>
  );
}
