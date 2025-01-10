import * as React from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { createTheme } from '@mui/material/styles';
import DashboardIcon from '@mui/icons-material/Dashboard';
import ChatIcon from '@mui/icons-material/Chat';
import BarChartIcon from '@mui/icons-material/BarChart';
import DescriptionIcon from '@mui/icons-material/Description';
import LayersIcon from '@mui/icons-material/Layers';
import { AppProvider, Router, type Navigation } from '@toolpad/core/AppProvider';
import { DashboardLayout } from '@toolpad/core/DashboardLayout';
import { Route, Routes } from 'react-router-dom';
import  Chat from '../../../features/chat/SupportChat';


const NAVIGATION: Navigation = [
  {
    kind: 'header',
    title: 'Main items',
  },
  {
    segment: 'dashboard',
    title: 'Dashboard',
    icon: <DashboardIcon />,
  },
  {
    segment: 'chat',
    title: 'Chat',
    icon: <ChatIcon />,    

  },  
  {
    segment: 'train',
    title: 'Train Model',
    icon: <DescriptionIcon />,
  },
  {
    kind: 'divider',
  },
/*   {
    kind: 'header',
    title: 'Analytics',
  },
  {
    segment: 'reports',
    title: 'Reports',
    icon: <BarChartIcon />,
    children: [
      {
        segment: 'sales',
        title: 'Sales',
        icon: <DescriptionIcon />,
      },
      {
        segment: 'traffic',
        title: 'Traffic',
        icon: <DescriptionIcon />,
      },
    ],
  },
  {
    segment: 'integrations',
    title: 'Integrations',
    icon: <LayersIcon />,
  }, */
];

const demoTheme = createTheme({
  cssVariables: {
    colorSchemeSelector: 'data-toolpad-color-scheme',
  },
  colorSchemes: { light: true, dark: true },
  breakpoints: {
    values: {
      xs: 0,
      sm: 600,
      md: 600,
      lg: 1200,
      xl: 1536,
    },
  },
});


interface MasterPageContentProps {
  router: Router
}

const MasterPageContent = ({router } : MasterPageContentProps) => {
    return (
      <Box
        sx={{
          py: 4,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          textAlign: 'center',
        }}
      >        
        {router.pathname === '/chat' ? <Chat /> : <></>}
      </Box>
    );
  }
  
  interface DashboardLayoutBasicProps {
    /**
     * Injected by the documentation to work in an iframe.
     * Remove this when copying and pasting into your project.
     */
    window?: () => Window;
  }
  
  export default function DashboardLayoutBasic(props: DashboardLayoutBasicProps) {
    const { window } = props;
  
    // Custom router implementation
    const router = useRouter('/dashboard');
    
  
    // Remove this const when copying and pasting into your project.
    const demoWindow = window !== undefined ? window() : undefined;
  
    return (
      <AppProvider
        navigation={NAVIGATION}
        router={router}
        theme={demoTheme}
        window={demoWindow}
      >
        <DashboardLayout>
          {/* Pass the current pathname to MasterPageContent */}
          <MasterPageContent router={router} />          
        </DashboardLayout>
      </AppProvider>
    );
  }
  
  function useRouter(initialPath: string): Router {
    const [pathname, setPathname] = React.useState(initialPath);
  
    const router = React.useMemo(() => {
      return {
        pathname,
        searchParams: new URLSearchParams(),
        navigate: (path: string | URL) => setPathname(String(path)),
      };
    }, [pathname]);
  
    return router;
  }
  