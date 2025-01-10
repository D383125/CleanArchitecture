import { useState, useCallback } from 'react';
import { getChatHistory, saveChat } from './network';
import { Chat } from './types';
import { useQuery } from '@tanstack/react-query';


export const useSaveChat = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const saveChats = useCallback(async (request: Chat) => {
    setLoading(true);
    setError(null);
    try {
      const response = await saveChat(request);
      return response.data;
    } catch (err) {
      console.error('Error saving chat:', err);
      setError(err as Error);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return { saveChats, loading, error };
};


export const useGetChats = () => {
  const { data: chatHistories = [], isLoading, error } = useQuery<Chat[]>({
    queryKey: ['chatHistory'], // Unique query key
    queryFn: getChatHistory,  // Fetching function
    staleTime: 5 * 60 * 1000, // Cache data for 5 minutes
    retry: 3, // Retry failed requests up to 3 times
    refetchOnWindowFocus: false, // Disable refetching on window focus
  });

  return [chatHistories, isLoading, error] as const;
}
