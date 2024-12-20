import { ChatCompletionRequest, ChatMessage } from "./types";
import React from 'react';
import axios from 'axios';

const url = process.env.REACT_APP_API_URL
const baseUrl = `${url}chat`
  

export const getChatHistory = ()  => {
  return axios.get(baseUrl)
  .then(resp =>         
    resp.data  
  )
  .then(d => 
{
  console.log(`Recieved ${JSON.stringify(d)}`)
  return d
})
}

export const saveChat = async (
  request: ChatMessage[]
) => {
return await axios.put(baseUrl, request)
  .then(response => response.data)
}

 export const streamChatCompletion = async (
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
        console.log(`NETWORK: Error: ${error}`)
        onError(error instanceof Error ? error.message : 'Unknown error');        
      }
      throw error;
    }
  };
  