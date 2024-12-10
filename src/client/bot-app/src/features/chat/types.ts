import { z } from "zod";


const chatCompletionMessageSchema = z.object({
  role: z.enum(["system", "user", "assistant"]),
  content: z.string(),
});

// Define ChatCompletionRequest schema
const chatCompletionRequestSchema = z.object({
  model: z.string().default("gpt-4o"), // Default value
  messages: z.array(chatCompletionMessageSchema).default([]), // Default to an empty array
});

export const chatMessage = z.object({
  message: z.string(),
  createrId: z.number().nullish(),
  createdOn: z.date().nullish(),
  modiifedOn: z.date().nullish(),
})

// TypeScript types inferred from Zod schemas
export type ChatCompletionMessage = z.infer<typeof chatCompletionMessageSchema>;
export type ChatCompletionRequest = z.infer<typeof chatCompletionRequestSchema>;
export type ChatHistory = z.infer<typeof chatMessage>
