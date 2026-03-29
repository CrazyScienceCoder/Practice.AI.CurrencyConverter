"use client";

import { useEffect, useRef } from "react";
import { cn } from "@/lib/cn";
import type { Message } from "./ChatInterface";
import { TypingIndicator } from "./TypingIndicator";

interface MessageListProps {
  messages: Message[];
  isStreaming: boolean;
}

export function MessageList({ messages, isStreaming }: MessageListProps) {
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  return (
    <div className="space-y-4 px-4 py-6 max-w-3xl mx-auto w-full">
      {messages.map((message) => (
        <MessageBubble key={message.id} message={message} isStreaming={isStreaming} />
      ))}
      <div ref={bottomRef} />
    </div>
  );
}

function MessageBubble({
  message,
  isStreaming,
}: {
  message: Message;
  isStreaming: boolean;
}) {
  const isUser = message.role === "user";
  const isLastAssistant =
    !isUser && isStreaming && message.content === "";

  return (
    <div
      className={cn(
        "flex gap-3",
        isUser ? "flex-row-reverse" : "flex-row"
      )}
    >
      {/* Avatar */}
      <div
        className={cn(
          "flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-sm font-medium",
          isUser
            ? "bg-blue-600 text-white"
            : "bg-gray-200 text-gray-700 dark:bg-gray-700 dark:text-gray-300"
        )}
      >
        {isUser ? "U" : "AI"}
      </div>

      {/* Bubble */}
      <div
        className={cn(
          "max-w-[75%] rounded-2xl px-4 py-2.5 text-sm leading-relaxed",
          isUser
            ? "rounded-tr-sm bg-blue-600 text-white"
            : "rounded-tl-sm bg-white text-gray-800 shadow-sm dark:bg-gray-800 dark:text-gray-200"
        )}
      >
        {isLastAssistant ? (
          <TypingIndicator />
        ) : (
          <span className="whitespace-pre-wrap">{message.content}</span>
        )}
      </div>
    </div>
  );
}
