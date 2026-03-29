"use client";

import { SessionProvider as NextSessionProvider, useSession, signOut } from "next-auth/react";
import { useEffect } from "react";
import type { Session } from "next-auth";

function SessionErrorWatcher() {
  const { data: session } = useSession();

  useEffect(() => {
    if (session?.error === "RefreshAccessTokenError") {
      signOut({ callbackUrl: "/login" });
    }
  }, [session?.error]);

  return null;
}

export function SessionProvider({
  children,
  session,
}: {
  children: React.ReactNode;
  session: Session | null;
}) {
  return (
    <NextSessionProvider session={session}>
      <SessionErrorWatcher />
      {children}
    </NextSessionProvider>
  );
}
