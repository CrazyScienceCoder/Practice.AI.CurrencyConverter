import { getServerSession } from "next-auth";
import { redirect } from "next/navigation";
import { authOptions } from "@/lib/auth";
import { ChatInterface } from "@/components/chat/ChatInterface";

export default async function ChatPage() {
  const session = await getServerSession(authOptions);

  if (!session) {
    redirect("/login");
  }

  const userName =
    session.user?.name ?? session.user?.email ?? "User";

  return (
    <div className="flex h-screen flex-col bg-gray-50 dark:bg-gray-900">
      <ChatInterface userName={userName} />
    </div>
  );
}
