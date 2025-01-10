import { ChatRequest, ChatMessage, Chat, chatDto, ChatDto } from "./types";
import axios from 'axios';

const url = process.env.REACT_APP_API_URL
const baseUrl = `${url}chat`

export const getChatHistory = async (): Promise<Chat[]> => {
  try {
    const response = await axios.get(baseUrl);
    const rawData = response.data;

    console.log(`Received raw data: ${JSON.stringify(rawData)}`);

    const chats: Chat[] = rawData.map((dto: any) => {
      const parsedDto = chatDto.parse(dto); // Validate using Zod      
      const messageArray = JSON.parse(parsedDto.message); 
      
      const messages: ChatMessage[] = messageArray.map((msg: any) => ({
        id: msg.id,
        role: msg.role as ChatMessage['role'], // Ensure type safety
        content: msg.content,
      }));

      return {
        id: parsedDto.id,
        creatorId: parsedDto.creatorId ?? -1,
        createdOn: parsedDto.createdOn?.toISOString() || "",
        modifiedOn: parsedDto.lastModifiedOn?.toISOString() || "",
        messages,
      };
    });

    return chats;
  } catch (error) {
    console.error("Error fetching chat histories:", error);
    throw error;
  }
};



export const saveChat = async (
  request: ChatDto
) => {
return await axios.put(baseUrl, request)
  .then(response => response.data)
}

 export const streamChatCompletion = async (
    request: ChatRequest,
    onUpdateMessage: (updatedText: string) => void,
    onComplete?: () => void,
    onError?: (error: string) => void
  ) => {
    try {
      const response = await fetch(baseUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      });
  
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }
  
      const reader = response.body?.getReader();
      const decoder = new TextDecoder();
  
      if (!reader) {
        throw new Error('Failed to get response reader.');
      }
  
      let done = false;  
      while (!done) {
        const { value, done: readerDone } = await reader.read();
        done = readerDone;
  
        if (value) {
          const chunk = decoder.decode(value, { stream: true });        
          onUpdateMessage(chunk)
        }
      }
  
      if (onComplete) onComplete(); // Notify when streaming is complete
    } catch (error) {
      if (onError) {
        console.log(`NETWORK: Error: ${error}`)
        onError(error instanceof Error ? error.message : 'Unknown error');        
      }
      throw error;
    }
  };
  