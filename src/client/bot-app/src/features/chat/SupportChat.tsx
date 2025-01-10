import React, { useCallback, useEffect, useState, memo } from 'react';
import { Avatar, Box, Button, CircularProgress, Container, Paper, TextField, Typography } from '@mui/material';
import Grid from '@mui/material/Grid';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ListItemText from '@mui/material/ListItemText';
import Divider from '@mui/material/Divider';
import { ChatRequest, ChatMessage, ChatSummary, creater, Chat } from './types';
import { streamChatCompletion } from './network';
import { useGetChats, useSaveChat } from './hooks';
import ReactPlaceholder from 'react-placeholder';
import { useForm } from "react-hook-form";

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
    display: 'flex',
    flexDirection: 'column',
    gap: 1,
  },
  chatMessage: (isUser: boolean) => ({
    p: 1,
    mb: 1,
    borderRadius: 2,
    maxWidth: '70%',
    alignSelf: isUser ? 'flex-end' : 'flex-start',
    backgroundColor: isUser ? 'primary.light' : 'grey.200',
  }),
  inputBox: {
    display: 'flex',
    alignItems: 'stretch', // Ensure child elements stretch to the full height
    p: 2,
    borderTop: 1,
    borderColor: 'divider',
  },
  textField: {
    mr: 2,
  },
  button: {
    height: '100%', // Match the button height to the TextField
  },
};


const SupportChat = () => {
  const [chatConversations, loading , ] = useGetChats() 
  const { saveChats, loading: saveLoading, error: saveError } = useSaveChat();
  const [isStreaming, setIsStreaming] = useState<boolean>(false);
  const [chatSummaries, setChatSummaries] = useState<ChatSummary[]>([]);
  const [selectedChatSummary, setSelectedChatSummary] = useState<ChatSummary | null>();
  const [currentChat, setCurrentChat] = useState<Chat>();
  const [currentMessage, setCurrentMessage] = useState<ChatMessage[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const [sendError, setSendError] = useState<boolean>(false);
  const [helperText, setHelperText] = useState("");
  const { handleSubmit, reset, formState: { errors }, } = useForm<Chat[]>();
  const [isDirty, setIsDirty] = useState<Boolean>(false)

  
  useEffect(() =>
    console.log(`IS Form dirty ${isDirty}`)
    , [isDirty])


  const onSubmit = async () => {    
    if (!currentChat) {
      alert('No messages to save.');
      return;
    }

    try {      
      await saveChats(currentChat, currentMessage);
      reset(); // Clear the form after successful submission
      setIsDirty(false)
      alert('Chat saved!');
    } catch (err) {
      console.error('Failed to save chat:', err);
    }
  };

  useEffect(() => {
    if(!loading && chatConversations !== null){            
      const chatSummaries: ChatSummary[] = chatConversations.map((h, i) => {
        var index = i + 1
        return {
          id: h.id, 
          primary: `Chat ${index}`,
          secondary: h.messages[0].content,
          avatar: '/static/images/avatar/5.jpg',
        }
      })

    setChatSummaries(chatSummaries)
    reset(chatConversations)
  }
  }, [chatConversations, loading, reset])
  
  const handleChatSelect = useCallback(
    (chat: ChatSummary) => {  
      setSelectedChatSummary(chat);      
      const targetChat = chatConversations.find(c => c.id === chat.id)
      if(targetChat){
        setCurrentChat(targetChat)
        setCurrentMessage(targetChat.messages)
      }
      else {
        const newChat: Chat = {
          id: -1,
          createrId: 1, 
          createdOn: new Date().toISOString(),
          modifiedOn: new Date().toISOString(),
          messages: [] 
        };
        setCurrentChat(newChat)
        setCurrentMessage(newChat.messages)
      }      
    },
    [chatConversations]
  );
  
   const handleSendMessage = useCallback(() => {    
    if (newMessage.trim() === "") {
      setSendError(true);
      setHelperText("Message cannot be empty");
      return;
    }

    setIsDirty(true)
    setSendError(false);
    setHelperText(""); // Clear any previous error message
    const id = currentChat?.messages.length ?? 0
    const userMessage = { id: id + 1, role: 'user' as creater, content: newMessage };
    setCurrentMessage((prev) => [...prev, userMessage]);
    setNewMessage("");

    const request: ChatRequest = {
      model: "gpt-4",
      messages: [
        ...currentMessage.map((msg) => ({
          role: msg.role === 'user' ? 'user' : 'assistant' as 'user' | 'assistant',
          content: msg.content,
        })),
        { role: "user", content: newMessage }, // Add the new user input
      ],
    };

    // Handle streaming response
    setIsStreaming(true);

    let accumulatedText = ""; // Accumulate streamed chunks
    streamChatCompletion(
      request,
      (chunk) => {
        accumulatedText += chunk; // Combine chunks
        //TODO: Mix with persisted chat.messages
        setCurrentMessage((prev) => {
          const assistantMessage = prev.find((msg) => msg.role === 'assistant' && msg.id === prev.length);
          if (assistantMessage) {
            // Update the existing assistant message
            return prev.map((msg) =>
              msg.id === assistantMessage.id ? { ...msg, content: accumulatedText } : msg
            );
          } else {
            // Add a new assistant message
            return [...prev, { id: prev.length + 1, role: 'assistant', content: accumulatedText }];
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
  }, [newMessage, currentChat?.messages.length, currentMessage]); 
  
  const handleStartNewChat = useCallback(() => {
    const highestId = chatConversations.length > 0
    ? Math.max(...chatConversations.flatMap(c => c.id))
    : 0; 

    const newChat: ChatSummary = {
      id: highestId + 1,
      primary: `New Chat ${chatSummaries.length + 1}`,
      avatar: "/static/images/avatar/placeholder.jpg",
    };
    setChatSummaries((prev) => [newChat, ...prev]);
    handleChatSelect(newChat);
  }, [chatConversations, chatSummaries.length, handleChatSelect]);

  const customPlaceHolder = (
    <>
    <Grid container spacing={2}>
      {/* Chat List Placeholder */}
      <Grid item xs={3}>
        <Box
          sx={{
            height: 'calc(100vh - 100px)',
            backgroundColor: 'grey.200',
            borderRadius: 1,
          }}
        ></Box>
      </Grid>

      {/* Current Chat Placeholder */}
      <Grid item xs={9}>
        <Box
          sx={{
            height: 'calc(100vh - 100px)',
            backgroundColor: 'grey.300',
            borderRadius: 1,
          }}
        ></Box>
      </Grid>
    </Grid>
    </>
  )

  return (
    <form onSubmit={handleSubmit(onSubmit)}
    style={{ display: 'flex', flexDirection: 'column', height: '100%', width: '100%' }}
    >
    <ReactPlaceholder
      showLoadingAnimation
      ready={!loading}
      customPlaceholder={customPlaceHolder}>
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
              {chatSummaries.length > 0 ? (
                chatSummaries.map((chat) => (
                  <React.Fragment key={chat.id}>
                    <ListItemButton
                      onClick={() => handleChatSelect(chat)}
                      selected={selectedChatSummary?.id === chat.id}
                      sx={muiStyles.listItemButton(selectedChatSummary?.id === chat.id)}
                    >
                      <ListItemAvatar>
                        <Avatar src={chat.avatar} />
                      </ListItemAvatar>
                      <ListItemText primary={chat.primary} secondary={chat.secondary || ""} />
                    </ListItemButton>
                    <Divider />
                  </React.Fragment>
                ))
              ) : (
                <Typography variant="body2" sx={{ textAlign: 'center', p: 2 }}>
                  No chats available.
                </Typography>
              )}
        </List>
            </Paper>
          </Grid>

          {/* Current Chat on the right */}
          <Grid item xs={9}>
            <Paper elevation={3} sx={muiStyles.currentChatPaper}>
              {/* Chat History */}
              <Box sx={muiStyles.chatHistoryBox}>
                {currentMessage.map((message) => (
                  <Box
                    key={currentChat?.id !== undefined ? currentChat.id + message.id : message.id}
                    sx={muiStyles.chatMessage(message.role !== 'user')}
                  >
                    <Typography variant="body2">                    
                      {message.content}
                      </Typography>
                  </Box>
                ))}
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
                  onKeyDown={(e) => 
                  {
                    if(e.key === "Enter"){
                      e.preventDefault()    
                      handleSendMessage()
                    }    
                  }
                  }
                  sx={muiStyles.textField}
                  error={sendError}
                  helperText={helperText}
                />
                 <Box sx={{ display: 'flex', gap: 1}}>
                <Button variant="contained" onClick={handleSendMessage} disabled={isStreaming}
                   sx={muiStyles.button}>
                  {isStreaming ? "Streaming..." : "Send"}
                </Button>
                <Button variant='contained' sx={muiStyles.button} disabled={!isDirty} type='submit'>
                  Save
                </Button>
                </Box>                
              </Box>
            </Paper>
          </Grid>
        </Grid>
      </Container>
    </ReactPlaceholder>
    </form>
  );
};

export default memo(SupportChat) 