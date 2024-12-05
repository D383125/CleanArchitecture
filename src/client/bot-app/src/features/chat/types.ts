import { z } from "zod";

// Define ChatCompletionMessage schema


const ChatCompletionMessageSchema = z.object({
  role: z.enum(["system", "user", "assistant"]),
  content: z.string(),
});

// Define ChatCompletionRequest schema
const ChatCompletionRequestSchema = z.object({
  model: z.string().default("gpt-4o"), // Default value
  messages: z.array(ChatCompletionMessageSchema).default([]), // Default to an empty array
});

// TypeScript types inferred from Zod schemas
export type ChatCompletionMessage = z.infer<typeof ChatCompletionMessageSchema>;
export type ChatCompletionRequest = z.infer<typeof ChatCompletionRequestSchema>;


