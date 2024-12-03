import React from 'react';
import logo from './logo.svg';
import './App.css';
import { AppProvider } from '@toolpad/core';
import DashboardLayoutBasic from './pages/DashboardLayout/components/DashboardLayoutBasic';





function App() {
  return (
    <AppProvider>
      <DashboardLayoutBasic />    
    </AppProvider>
  );
}

export default App;
