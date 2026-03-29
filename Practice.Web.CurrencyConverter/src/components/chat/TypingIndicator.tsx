"use client";

export function TypingIndicator() {
  return (
    <div className="flex items-center gap-1 py-1">
      {[0, 1, 2].map((i) => (
        <span
          key={i}
          className="h-2 w-2 rounded-full bg-gray-400 animate-bounce dark:bg-gray-500"
          style={{ animationDelay: `${i * 150}ms` }}
        />
      ))}
    </div>
  );
}
