import { getServerSession } from "next-auth";
import { authOptions } from "@/lib/auth";
import { NextRequest } from "next/server";

const AI_AGENT_API_URL =
  process.env.AI_AGENT_API_URL ??
  process.env.NEXT_PUBLIC_AI_AGENT_API_URL ??
  "http://localhost:5264";

/**
 * POST /api/chat
 * Proxies the user message to the AI Agent API and streams the SSE response
 * back to the browser. Attaches the Keycloak JWT as a Bearer token.
 */
export async function POST(req: NextRequest) {
  const session = await getServerSession(authOptions);

  if (!session?.accessToken) {
    return new Response("Unauthorized", { status: 401 });
  }

  if (session.error === "RefreshAccessTokenError") {
    return new Response("Session expired", { status: 401 });
  }

  const body = await req.json();
  const { message, conversationId } = body as {
    message: string;
    conversationId?: string;
  };

  if (!message?.trim()) {
    return new Response("Message is required", { status: 400 });
  }

  const upstream = await fetch(`${AI_AGENT_API_URL}/api/v1.0/chat/message`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session.accessToken}`,
    },
    body: JSON.stringify({ message, conversationId }),
  });

  if (!upstream.ok) {
    return new Response(await upstream.text(), { status: upstream.status });
  }

  // Stream the SSE response back directly to the client
  return new Response(upstream.body, {
    headers: {
      "Content-Type": "text/event-stream",
      "Cache-Control": "no-cache",
      "X-Accel-Buffering": "no",
    },
  });
}
