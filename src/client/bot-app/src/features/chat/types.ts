import { z } from "zod";

const chatCompletionMessageSchema = z.object({
  role: z.enum(["system", "user", "assistant"]),
  content: z.string(),
});

const chatCompletionRequestSchema = z.object({
  model: z.string().default("gpt-4o"), // Default value
  messages: z.array(chatCompletionMessageSchema).default([]), // Default to an empty array
});

export const chatMessageDto = z.object({
  message: z.string(),
  createrId: z.number().nullish(),
  createdOn: z.date().nullish(),
  modiifedOn: z.date().nullish(),
})

export type creater = "user" | "assistant"

export interface ChatMessage {
  id: number
  role: creater
  content: string
}

export interface ChatSummary {
  id: number;
  creater?: string;
  primary: string;
  secondary?: string;
  avatar: string;
}

// TypeScript types inferred from Zod schemas
export type ChatCompletionMessage = z.infer<typeof chatCompletionMessageSchema>;
export type ChatCompletionRequest = z.infer<typeof chatCompletionRequestSchema>;
export type ChatMessageDto = z.infer<typeof chatMessageDto>
