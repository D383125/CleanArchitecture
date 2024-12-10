import { useState, useEffect } from 'react';
import { getChatHistory } from './network';
import { ChatHistory } from './types';


export const useGetChats = () => {
  const [chatHistories, setChatHistories] = useState<ChatHistory[]>([]);
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
