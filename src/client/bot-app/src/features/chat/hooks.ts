import { useState, useCallback, useEffect } from 'react';
import { getChatHistory, saveChat } from './network';
import { ChatMessageDto, ChatMessage } from './types';


export const useSaveChat = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const saveChats = useCallback(async (request: ChatMessage[]) => {
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
  const [chatHistories, setChatHistories] = useState<ChatMessageDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let isMounted = true; // To prevent state updates on unmounted components
    setLoading(true);

    getChatHistory()
      .then(data => {
        if (isMounted) {
          setChatHistories(data);
          setError(null); // Clear error if successful
        }
      })
      .catch(err => {
        if (isMounted) {
          console.error("Error fetching chat histories:", err);
          setError(err);
        }
      })
      .finally(() => {
        if (isMounted) {
          setLoading(false);
        }
      });

    return () => {
      isMounted = false; // Cleanup function to prevent memory leaks
    };
  }, []);

  return [chatHistories, loading, error] as const;
};
function useCallBack(arg0: () => (() => void) | undefined, arg1: any[]) {
  throw new Error('Function not implemented.');
}

