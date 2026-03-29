"use client";

import { KeyboardEvent, useRef } from "react";
import { cn } from "@/lib/cn";

interface MessageInputProps {
  value: string;
  onChange: (value: string) => void;
  onSend: (message: string) => void;
  onStop: () => void;
  isStreaming: boolean;
}

export function MessageInput({
  value,
  onChange,
  onSend,
  onStop,
  isStreaming,
}: MessageInputProps) {
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      if (!isStreaming && value.trim()) {
        onSend(value);
      }
    }
  };

  const handleInput = () => {
    const ta = textareaRef.current;
    if (ta) {
      ta.style.height = "auto";
      ta.style.height = `${Math.min(ta.scrollHeight, 160)}px`;
    }
  };

  return (
    <div className="border-t border-gray-200 bg-white px-4 py-3 dark:border-gray-700 dark:bg-gray-800">
      <div className="mx-auto flex max-w-3xl items-end gap-3">
        <textarea
          ref={textareaRef}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={handleKeyDown}
          onInput={handleInput}
          placeholder="Ask about exchange rates… (Shift+Enter for new line)"
          rows={1}
          disabled={isStreaming}
          className={cn(
            "flex-1 resize-none rounded-xl border border-gray-300 bg-gray-50 px-4 py-2.5",
            "text-sm text-gray-900 placeholder-gray-400 outline-none",
            "focus:border-blue-500 focus:ring-2 focus:ring-blue-200",
            "dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400",
            "dark:focus:border-blue-500 dark:focus:ring-blue-800",
            "disabled:opacity-50 transition-all"
          )}
        />

        {isStreaming ? (
          <button
            onClick={onStop}
            className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-red-500 text-white hover:bg-red-600 transition-colors"
            title="Stop generating"
          >
            <StopIcon />
          </button>
        ) : (
          <button
            onClick={() => value.trim() && onSend(value)}
            disabled={!value.trim()}
            className={cn(
              "flex h-10 w-10 shrink-0 items-center justify-center rounded-xl transition-colors",
              value.trim()
                ? "bg-blue-600 text-white hover:bg-blue-700"
                : "bg-gray-200 text-gray-400 cursor-not-allowed dark:bg-gray-700 dark:text-gray-500"
            )}
            title="Send message (Enter)"
          >
            <SendIcon />
          </button>
        )}
      </div>

      <p className="mt-1.5 text-center text-xs text-gray-400">
        AI responses may be inaccurate. Always verify rates before financial decisions.
      </p>
    </div>
  );
}

function SendIcon() {
  return (
    <svg viewBox="0 0 24 24" className="h-5 w-5" fill="currentColor">
      <path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z" />
    </svg>
  );
}

function StopIcon() {
  return (
    <svg viewBox="0 0 24 24" className="h-5 w-5" fill="currentColor">
      <rect x="6" y="6" width="12" height="12" rx="1" />
    </svg>
  );
}
