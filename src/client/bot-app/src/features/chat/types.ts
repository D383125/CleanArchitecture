import { z } from "zod";

const chatMessageSchema = z.object({
  role: z.enum(["system", "user", "assistant"]),
  content: z.string(),
});

const chatRequestSchema = z.object({
  model: z.string().default("gpt-4o"),
  messages: z.array(chatMessageSchema).default([]),
  createrId: z.number().nullish(),
  createdOn: z.date().nullish(),
  modifedOn: z.date().nullish(),
});

export const chatDto = z.object({
  id: z.number(),
  message: z.string(),
  creatorId: z.number().nullish(),
  createdOn: z
    .string()
    .nullable()
    .transform((val) => (val ? new Date(val) : null)),
  lastModifiedOn: z
    .string()
    .nullable()
    .transform((val) => (val ? new Date(val) : null)),
});

// In Memory //
export type creater = "user" | "assistant";

export interface ChatMessage {
  id: number; // FOr React list key
  role: creater;
  content: string;
}

export interface Chat {
  id: number;
  createrId: number;
  createdOn: string;
  modifiedOn: string;
  messages: ChatMessage[];
}

export interface ChatSummary {
  id: number;
  creater?: string;
  primary: string;
  secondary?: string;
  avatar: string;
}
// In Memory //

export type ChatRequest = z.infer<typeof chatRequestSchema>;
export type ChatDto = z.infer<typeof chatDto>;
