import React, { useCallback, useEffect, useState } from 'react';
import { Avatar, Box, Button, CircularProgress, Container, Paper, TextField, Typography } from '@mui/material';
import Grid from '@mui/material/Grid';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ListItemText from '@mui/material/ListItemText';
import Divider from '@mui/material/Divider';
import { ChatCompletionRequest } from './types';
import { postChatCompletion } from './network';
import { useGetChats } from './hooks';
import ReactPlaceholder from 'react-placeholder';

interface ChatSummaryMessage {
  id: number;
  creater?: string;
  primary: string;
  secondary?: string;
  avatar: string;
}

export type creater = "user" | "assistant"

interface ChatMessage {
  id: number
  creater: creater
  text: string
}

/*
const initialChatMessages: ChatSummaryMessage[] = [
  { id: 1, primary: 'Chat 1', secondary: 'Blah blah blah', avatar: '/static/images/avatar/5.jpg' },
  { id: 2, primary: 'Chat 2', secondary: 'Lorem ipsum dolor', avatar: '/static/images/avatar/6.jpg' },
];*/

const muiStyles = {
  container: {
    py: 2,
  },
  chatListPaper: {
    height: 'calc(100vh - 100px)',
    overflowY: 'auto',
  },
  chatListHeader: {
    p: 2,
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  listItemButton: (isSelected: boolean) => ({
    backgroundColor: isSelected ? 'grey.300' : 'inherit',
    '&:hover': { backgroundColor: 'grey.200' },
  }),
  currentChatPaper: {
    height: 'calc(100vh - 100px)',
    display: 'flex',
    flexDirection: 'column',
  },
  chatHistoryBox: {
    flex: 1,
    p: 2,
    overflowY: 'auto',
    display: 'flex', // Add flex layout
    flexDirection: 'column', // Stack messages vertically
    gap: 1, // Optional spacing between messages
  },
  chatMessage: (isUser: boolean) => ({
    p: 1,
    mb: 1,
    borderRadius: 2,
    maxWidth: '70%',
    alignSelf: isUser ? 'flex-end' : 'flex-start', // Align based on the sender
    backgroundColor: isUser ? 'primary.light' : 'grey.200',
  }),
  inputBox: {
    display: 'flex',
    alignItems: 'center',
    p: 2,
    borderTop: 1,
    borderColor: 'divider',
  },
  textField: {
    mr: 2,
  },
};


const Chat = () => {
  const [chatHistory, loading , ] = useGetChats()        
  const [currentMessage, setCurrentMessage] = useState<string>(''); // For accumulated streamed message
  const [isStreaming, setIsStreaming] = useState<boolean>(false);
  const [chatMessages, setChatMessages] = useState<ChatSummaryMessage[]>([]);
  //const [chatMessages, setChatMessages] =  useState(chatHistory);
  const [selectedChat, setSelectedChat] = useState<ChatSummaryMessage | null>();
  const [currentChatHistory, setCurrentChatHistory] = useState<ChatMessage[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const [error, setError] = useState<boolean>(false);
  const [helperText, setHelperText] = useState("");

useEffect(() => {
if(!loading && chatHistory !== null)
{
  const initialChatMessages: ChatSummaryMessage[] = chatHistory.map((h, i) => {
    var index = i + 1
    return {
      id: index, 
      primary: `Chat ${index}`,
      secondary: h.message, 
      avatar: '/static/images/avatar/5.jpg',
    }
  })

  setChatMessages(initialChatMessages)
}
}, [chatHistory, loading])

  console.log(`Chat Histry is ${JSON.stringify(chatHistory)}`)  

  useEffect(() => {
    console.log("Chat Component mounted.");

    return () => {
      console.log("Chat Component unmounted. Virtual DOM flusing to DOM. Cleaning up.");
    };
  }, []);

  

  const handleChatSelect = useCallback(
    (chat: ChatSummaryMessage) => {
      setSelectedChat(chat);
      setCurrentChatHistory([
        { id: 1, creater: 'user', text: `${chat.primary}` },
        ...(chat.secondary ? [{ id: 2, creater: 'user' as creater, text: chat.secondary }] : []),
      ]);
    },
    []
  );

   const handleSendMessage = useCallback(() => {    
    if (newMessage.trim() === "") {
      setError(true);
      setHelperText("Message cannot be empty");
      return;
    }

    setError(false);
    setHelperText(""); // Clear any previous error message

    const userMessage = { id: currentChatHistory.length + 1, creater: 'user' as creater, text: newMessage };
    setCurrentChatHistory((prev) => [...prev, userMessage]);
    setNewMessage("");

    const request: ChatCompletionRequest = {
      model: "gpt-4",
      messages: [
        ...currentChatHistory.map((msg) => ({
          role: msg.creater === 'user' ? 'user' : 'assistant' as 'user' | 'assistant',
          content: msg.text,
        })),
        { role: "user", content: newMessage }, // Add the new user input
      ],
    };

    // Handle streaming response
    setIsStreaming(true);

    let accumulatedText = ""; // Accumulate streamed chunks
    postChatCompletion(
      request,
      (chunk) => {
        accumulatedText += chunk; // Combine chunks
        setCurrentChatHistory((prev) => {
          const assistantMessage = prev.find((msg) => msg.creater === 'assistant' && msg.id === prev.length);
          if (assistantMessage) {
            // Update the existing assistant message
            return prev.map((msg) =>
              msg.id === assistantMessage.id ? { ...msg, text: accumulatedText } : msg
            );
          } else {
            // Add a new assistant message
            return [...prev, { id: prev.length + 1, creater: 'assistant', text: accumulatedText }];
          }
        });
      },
      () => {
        console.log('Done')        
        setIsStreaming(false);
      },
      () => {
        setIsStreaming(false); // End streaming
      }
    );
  }, [newMessage, currentChatHistory]); 
  

  const handleStartNewChat = useCallback(() => {
    const newChat: ChatSummaryMessage = {
      id: chatMessages.length + 1,
      primary: `New Chat ${chatMessages.length + 1}`,
      avatar: "/static/images/avatar/placeholder.jpg",
    };
    setChatMessages((prev) => [newChat, ...prev]);
    handleChatSelect(newChat);
  }, [chatMessages.length, handleChatSelect]);

  

  return (
    <ReactPlaceholder showLoadingAnimation ready={!loading} type="media" rows={2} >
    <Container maxWidth="lg" sx={muiStyles.container}>
      <Grid container spacing={2}>
        {/* Chat List on the left */}
        <Grid item xs={3}>
          <Paper elevation={3} sx={muiStyles.chatListPaper}>
            <Box sx={muiStyles.chatListHeader}>
              <Typography variant="h6">Chats</Typography>
              <Button variant="contained" size="small" onClick={handleStartNewChat}>
                Start a Chat
              </Button>
            </Box>
            <Divider />
            <List>
              {chatMessages.map((chat) => (
                <React.Fragment key={chat.id}>
                  <ListItemButton
                    onClick={() => handleChatSelect(chat)}
                    selected={selectedChat?.id === chat.id}
                    sx={muiStyles.listItemButton(selectedChat?.id === chat.id)}
                  >
                    <ListItemAvatar>
                      <Avatar src={chat.avatar} />
                    </ListItemAvatar>
                    <ListItemText
                      primary={chat.primary}
                      secondary={chat.secondary || ""}
                    />
                  </ListItemButton>
                  <Divider />
                </React.Fragment>
              ))}
            </List>
          </Paper>
        </Grid>

        {/* Current Chat on the right */}
        <Grid item xs={9}>
          <Paper elevation={3} sx={muiStyles.currentChatPaper}>
            {/* Chat History */}
            <Box sx={muiStyles.chatHistoryBox}>
              {currentChatHistory.map((message) => (
                <Box
                  key={message.id}
                  sx={muiStyles.chatMessage(message.creater !== 'user')}
                >
                  <Typography variant="body2">                    
                    {message.text}
                    </Typography>
                </Box>
              ))}

      {/* TODO:       
      2. Fix colour of message to go with theme      
      4. Setup AWS infra
      */}
        {isStreaming && (
          <Box sx={muiStyles.chatMessage(false)}>
            <Typography variant="body2">              
              {isStreaming && <CircularProgress size={12} sx={{ ml: 1 }} />}
            </Typography>
          </Box>
        )}
            </Box>
            {/* Message Input */}
            <Box sx={muiStyles.inputBox}>
              <TextField
                fullWidth
                variant="outlined"
                placeholder="Type your message..."
                value={newMessage}
                onChange={(e) => setNewMessage(e.target.value)}
                onKeyPress={(e) => e.key === "Enter" && handleSendMessage()}
                sx={muiStyles.textField}
                error={error}
                helperText={helperText}
              />
              <Button variant="contained" onClick={handleSendMessage} disabled={isStreaming}>
                {isStreaming ? "Streaming..." : "Send"}
              </Button>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
    </ReactPlaceholder>
  );
};

export default Chat;

