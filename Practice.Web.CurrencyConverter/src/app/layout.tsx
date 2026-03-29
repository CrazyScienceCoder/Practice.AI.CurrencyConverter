import type { Metadata } from "next";
import { getServerSession } from "next-auth";
import { SessionProvider } from "@/components/SessionProvider";
import { authOptions } from "@/lib/auth";
import "./globals.css";

export const metadata: Metadata = {
  title: "Currency Converter AI",
  description: "AI-powered currency conversion chatbot",
};

export default async function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const session = await getServerSession(authOptions);

  return (
    <html lang="en">
      <body>
        <SessionProvider session={session}>{children}</SessionProvider>
      </body>
    </html>
  );
}
