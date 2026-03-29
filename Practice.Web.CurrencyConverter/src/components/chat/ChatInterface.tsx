"use client";

import { useRef, useState, useCallback, useEffect } from "react";
import { useSession, signIn, signOut } from "next-auth/react";
import { MessageList } from "./MessageList";
import { MessageInput } from "./MessageInput";
import { cn } from "@/lib/cn";

export interface Message {
  id: string;
  role: "user" | "assistant";
  content: string;
  timestamp: Date;
}

interface ChatInterfaceProps {
  userName: string;
}

const MESSAGES_KEY = "chat_messages";
const CONVERSATION_KEY = "chat_conversation_id";

function loadFromStorage<T>(key: string): T | null {
  try {
    const raw = sessionStorage.getItem(key);
    return raw ? (JSON.parse(raw) as T) : null;
  } catch {
    return null;
  }
}

export function ChatInterface({ userName }: ChatInterfaceProps) {
  const { data: session, update: updateSession } = useSession();

  const [messages, setMessages] = useState<Message[]>(() => {
    if (typeof window === "undefined") return [];
    const saved = loadFromStorage<Message[]>(MESSAGES_KEY);
    return saved
      ? saved.map((m) => ({ ...m, timestamp: new Date(m.timestamp) }))
      : [];
  });

  const [isStreaming, setIsStreaming] = useState(false);
  const [tokenCopied, setTokenCopied] = useState(false);
  const [conversationId, setConversationId] = useState<string | undefined>(
    () =>
      typeof window !== "undefined"
        ? (loadFromStorage<string>(CONVERSATION_KEY) ?? undefined)
        : undefined
  );
  const [inputValue, setInputValue] = useState("");
  const [sessionExpired, setSessionExpired] = useState(false);
  const abortControllerRef = useRef<AbortController | null>(null);

  // Persist messages to sessionStorage whenever they change
  useEffect(() => {
    sessionStorage.setItem(MESSAGES_KEY, JSON.stringify(messages));
  }, [messages]);

  // Persist conversationId
  useEffect(() => {
    if (conversationId) {
      sessionStorage.setItem(CONVERSATION_KEY, conversationId);
    } else {
      sessionStorage.removeItem(CONVERSATION_KEY);
    }
  }, [conversationId]);

  // Detect session error set by NextAuth JWT callback after a failed token refresh
  useEffect(() => {
    if (session?.error === "RefreshAccessTokenError") {
      setSessionExpired(true);
    }
  }, [session?.error]);

  const handleReauthenticate = useCallback(() => {
    signIn("keycloak", { callbackUrl: "/chat" });
  }, []);

  const sendMessage = useCallback(
    async (userMessage: string) => {
      if (!userMessage.trim() || isStreaming) return;

      setSessionExpired(false);

      const userMsg: Message = {
        id: crypto.randomUUID(),
        role: "user",
        content: userMessage,
        timestamp: new Date(),
      };

      const assistantMsg: Message = {
        id: crypto.randomUUID(),
        role: "assistant",
        content: "",
        timestamp: new Date(),
      };

      setMessages((prev) => [...prev, userMsg, assistantMsg]);
      setIsStreaming(true);
      setInputValue("");

      abortControllerRef.current = new AbortController();

      const currentConversationId = conversationId ?? crypto.randomUUID();
      if (!conversationId) setConversationId(currentConversationId);

      try {
        const response = await fetch("/api/chat", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            message: userMessage,
            conversationId: currentConversationId,
          }),
          signal: abortControllerRef.current.signal,
        });

        if (response.status === 401) {
          // Prompt NextAuth to attempt a token refresh so the next request succeeds
          await updateSession();
          setSessionExpired(true);
          setMessages((prev) => {
            const updated = [...prev];
            const last = updated[updated.length - 1];
            if (last?.role === "assistant" && last.content === "") {
              updated[updated.length - 1] = {
                ...last,
                content:
                  "Your session has expired. Please sign in again to continue — your conversation will be preserved.",
              };
            }
            return updated;
          });
          return;
        }

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}: ${await response.text()}`);
        }

        const reader = response.body?.getReader();
        if (!reader) throw new Error("No response body");

        const decoder = new TextDecoder();
        let buffer = "";

        while (true) {
          const { done, value } = await reader.read();
          if (done) break;

          buffer += decoder.decode(value, { stream: true });
          const lines = buffer.split("\n");
          buffer = lines.pop() ?? "";

          for (const line of lines) {
            if (!line.startsWith("data: ")) continue;
            const data = line.slice(6);
            if (data === "[DONE]") continue;

            const text = data.replace(/\\n/g, "\n").replace(/\\r/g, "\r");

            setMessages((prev) => {
              const updated = [...prev];
              const last = updated[updated.length - 1];
              if (last?.role === "assistant") {
                updated[updated.length - 1] = {
                  ...last,
                  content: last.content + text,
                };
              }
              return updated;
            });
          }
        }
      } catch (err: unknown) {
        if (err instanceof Error && err.name === "AbortError") return;

        setMessages((prev) => {
          const updated = [...prev];
          const last = updated[updated.length - 1];
          if (last?.role === "assistant" && last.content === "") {
            updated[updated.length - 1] = {
              ...last,
              content: "Sorry, something went wrong. Please try again.",
            };
          }
          return updated;
        });
      } finally {
        setIsStreaming(false);
        abortControllerRef.current = null;
      }
    },
    [conversationId, isStreaming, updateSession]
  );

  const handleStop = useCallback(() => {
    abortControllerRef.current?.abort();
    setIsStreaming(false);
  }, []);

  const handleCopyToken = useCallback(async () => {
    if (!session?.accessToken) return;
    await navigator.clipboard.writeText(session.accessToken);
    setTokenCopied(true);
    setTimeout(() => setTokenCopied(false), 2000);
  }, [session?.accessToken]);

  const handleNewConversation = useCallback(() => {
    setMessages([]);
    setConversationId(undefined);
    setInputValue("");
    setSessionExpired(false);
    sessionStorage.removeItem(MESSAGES_KEY);
    sessionStorage.removeItem(CONVERSATION_KEY);
  }, []);

  return (
    <div className="flex h-full flex-col">
      {/* Header */}
      <header className="flex items-center justify-between border-b border-gray-200 bg-white px-6 py-3 dark:border-gray-700 dark:bg-gray-800">
        <div className="flex items-center gap-2">
          <span className="text-2xl">💱</span>
          <div>
            <h1 className="text-lg font-semibold text-gray-900 dark:text-white">
              Currency Converter AI
            </h1>
            {conversationId && (
              <p className="text-xs text-gray-500 dark:text-gray-400 font-mono">
                {conversationId.slice(0, 8)}...
              </p>
            )}
          </div>
        </div>

        <div className="flex items-center gap-3">
          <button
            onClick={handleNewConversation}
            className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm text-gray-700 hover:bg-gray-50 dark:border-gray-600 dark:text-gray-300 dark:hover:bg-gray-700"
          >
            New Chat
          </button>
          <button
            onClick={handleCopyToken}
            disabled={!session?.accessToken}
            title="Copy Bearer token for use in Postman or other API clients"
            className="rounded-lg border border-blue-300 px-3 py-1.5 text-sm text-blue-600 hover:bg-blue-50 disabled:cursor-not-allowed disabled:opacity-40 dark:border-blue-600 dark:text-blue-400 dark:hover:bg-blue-900/20"
          >
            {tokenCopied ? "Copied!" : "Copy Token"}
          </button>
          <div className="flex items-center gap-2">
            <span className="text-sm text-gray-600 dark:text-gray-400">
              {userName}
            </span>
            <button
              onClick={() =>
                signOut({ callbackUrl: "/api/auth/keycloak-logout" })
              }
              className="rounded-lg bg-red-50 px-3 py-1.5 text-sm text-red-600 hover:bg-red-100 dark:bg-red-900/20 dark:text-red-400"
            >
              Sign out
            </button>
          </div>
        </div>
      </header>

      {/* Session expired banner */}
      {sessionExpired && (
        <div className="flex items-center justify-between gap-3 bg-amber-50 px-6 py-3 text-sm text-amber-800 dark:bg-amber-900/30 dark:text-amber-300">
          <span>Your session has expired. Sign in again to keep chatting — your messages are saved.</span>
          <button
            onClick={handleReauthenticate}
            className="shrink-0 rounded-lg bg-amber-100 px-3 py-1.5 font-medium hover:bg-amber-200 dark:bg-amber-800 dark:hover:bg-amber-700"
          >
            Sign in again
          </button>
        </div>
      )}

      {/* Messages */}
      <div
        className={cn(
          "flex-1 overflow-y-auto",
          messages.length === 0 && "flex items-center justify-center"
        )}
      >
        {messages.length === 0 ? (
          <WelcomeScreen onSuggestionClick={(s) => sendMessage(s)} />
        ) : (
          <MessageList messages={messages} isStreaming={isStreaming} />
        )}
      </div>

      {/* Input */}
      <MessageInput
        value={inputValue}
        onChange={setInputValue}
        onSend={sendMessage}
        onStop={handleStop}
        isStreaming={isStreaming}
      />
    </div>
  );
}

function WelcomeScreen({
  onSuggestionClick,
}: {
  onSuggestionClick: (s: string) => void;
}) {
  const suggestions = [
    "What is 500 USD in EUR right now?",
    "Show me USD to JPY rates for the last 7 days",
    "Convert 1000 GBP to CHF",
    "What are the current EUR exchange rates?",
  ];

  return (
    <div className="w-full max-w-2xl px-4 text-center">
      <div className="mb-6 text-6xl">💱</div>
      <h2 className="mb-2 text-2xl font-bold text-gray-800 dark:text-white">
        Currency Converter AI
      </h2>
      <p className="mb-8 text-gray-600 dark:text-gray-400">
        Ask me anything about exchange rates, currency conversions, or
        historical data.
      </p>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        {suggestions.map((suggestion) => (
          <button
            key={suggestion}
            onClick={() => onSuggestionClick(suggestion)}
            className="rounded-xl border border-gray-200 bg-white px-4 py-3 text-left text-sm text-gray-700 shadow-sm hover:border-blue-300 hover:bg-blue-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300 dark:hover:border-blue-500 dark:hover:bg-blue-900/20"
          >
            {suggestion}
          </button>
        ))}
      </div>
    </div>
  );
}
