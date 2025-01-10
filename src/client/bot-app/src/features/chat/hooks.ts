import { useState, useCallback } from 'react';
import { getChatHistory, saveChat } from './network';
import { Chat, ChatDto, ChatMessage } from './types';
import { useQuery, useQueryClient } from '@tanstack/react-query';

const chatCacheKey = 'chatHistory'

export const useSaveChat = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const queryClient = useQueryClient();

  const saveChats = useCallback(
    async (hostChat: Chat, newMessages: ChatMessage[]) => {
      setLoading(true);
      setError(null);

      const chatDto: ChatDto = {
        id: hostChat.id,
        creatorId: hostChat.createrId,
        createdOn: new Date(hostChat.createdOn),
        lastModifiedOn: new Date(hostChat.modifiedOn),
        message: JSON.stringify(newMessages),
      };

      try {
        const response = await saveChat(chatDto);
        queryClient.invalidateQueries({ queryKey: [chatCacheKey] });
        return response.data;
      } catch (err) {
        console.error('Error saving chat:', err);
        setError(err as Error);
        throw err;
      } finally {
        setLoading(false);
      }
    },
    [queryClient]
  );

  return { saveChats, loading, error };
};


export const useGetChats = () => {
  const { data: chatHistories = [], isLoading, error } = useQuery<Chat[]>({
    queryKey: [chatCacheKey], 
    queryFn: getChatHistory,  
    staleTime: 5 * 60 * 1000, // Cache data for 5 minutes
    retry: 3, // Retry failed requests up to 3 times
    refetchOnWindowFocus: true, // Disable refetching on window focus
  });

  return [chatHistories, isLoading, error] as const;
}
