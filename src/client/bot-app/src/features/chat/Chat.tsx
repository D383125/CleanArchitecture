import React, { useCallback, useEffect, useState } from 'react';
import { Avatar, Box, Button, Container, Paper, TextField, Typography } from '@mui/material';
import Grid from '@mui/material/Grid';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ListItemText from '@mui/material/ListItemText';
import Divider from '@mui/material/Divider';

interface ChatMessage {
  id: number;
  creater?: string;
  primary: string;
  secondary?: string;
  avatar: string;
}

const initialChatMessages: ChatMessage[] = [
  { id: 1, primary: 'Chat 1', secondary: 'Blah blah blah', avatar: '/static/images/avatar/5.jpg' },
  { id: 2, primary: 'Chat 2', secondary: 'Lorem ipsum dolor', avatar: '/static/images/avatar/6.jpg' },
];

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
  const [chatMessages, setChatMessages] = useState(initialChatMessages);
  const [selectedChat, setSelectedChat] = useState<ChatMessage | null>(initialChatMessages[0]);
  const [currentChatHistory, setCurrentChatHistory] = useState([
    { id: 1, text: 'Hello, how are you?' },
    { id: 2, text: 'I’m doing well, thanks!' },
  ]);
  const [newMessage, setNewMessage] = useState('');

 useEffect(() => {
    console.log(`Chat Compoment mounted.`)

    return () => {
      console.log(`Chat Compoment unmounted. Cleaning up`)
    }
  }, 
 [])  

  const handleChatSelect = useCallback( (chat: ChatMessage) => {
    setSelectedChat(chat);
    setCurrentChatHistory([
      { id: 1, text: `${chat.primary}` },
      ...(chat.secondary ? [{ id: 2, text: chat.secondary }] : []),
    ]);
  }, []);

  const handleSendMessage = useCallback(() => {
    if (newMessage.trim()) {
      setCurrentChatHistory((prev) => [...prev, { id: prev.length + 1, text: newMessage }]);
      setNewMessage('');
    }
  }, [newMessage]);

  const handleStartNewChat = useCallback(() => {
    const newChat: ChatMessage = {
      id: chatMessages.length + 1,
      primary: `New Chat ${chatMessages.length + 1}`,
      avatar: '/static/images/avatar/placeholder.jpg',
    };
    setChatMessages((prev) => [newChat, ...prev]);
    handleChatSelect(newChat);
  }, [chatMessages.length, handleChatSelect]);

  return (
    <Container maxWidth="lg" sx={muiStyles.container}>
      <Grid container spacing={2}>
        {/* Chat List on the left */}
        <Grid item xs={3}>
          <Paper elevation={3} sx={muiStyles.chatListPaper}>
            <Box sx={muiStyles.chatListHeader}>
                {/* As a ListHEader elemnt */}
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
                      secondary={chat.secondary || ''}
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
                <Paper
                  key={message.id}
                  elevation={1}
                  sx={muiStyles.chatMessage(message.id % 2 === 0)}
                >
                  {message.text}
                </Paper>
              ))}
            </Box>

            {/* Message Input */}
            <Box sx={muiStyles.inputBox}>
              <TextField
                fullWidth
                variant="outlined"
                placeholder="Type your message..."
                value={newMessage}
                onChange={(e) => setNewMessage(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
                sx={muiStyles.textField}
              />
              <Button variant="contained" onClick={handleSendMessage}>
                Send
              </Button>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default Chat;
