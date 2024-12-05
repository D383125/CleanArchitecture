import { ChatCompletionRequest } from "./types";

//TODO: Put url in env var
const url = 'https://localhost:8081/'
const baseUrl = `${url}chat`
  
  // Stream response handling
  export const postChatCompletion = async (
    request: ChatCompletionRequest,
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
        console.error(`NETWORK: Error: ${error}`)
        onError(error instanceof Error ? error.message : 'Unknown error');
      }
    }
  };
  