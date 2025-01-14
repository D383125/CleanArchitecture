import { useEffect, useState } from "react";

// Define a breakpoint for mobile screens
const MOBILE_BREAKPOINT = 768; // px

/**
 * Hook to detect if the screen is small (mobile size).
 * Rerenders the component when the screen size transitions between desktop and mobile.
 *
 * @returns {boolean} - `true` if the screen is small (mobile), `false` otherwise.
 */
const useSmallScreen = (): boolean => {
  const [isSmallScreen, setIsSmallScreen] = useState<boolean>(
    window.innerWidth <= MOBILE_BREAKPOINT,
  );

  useEffect(() => {
    const handleResize = () => {
      const isNowSmall = window.innerWidth <= MOBILE_BREAKPOINT;
      setIsSmallScreen(isNowSmall);
    };

    // Add event listener for window resize
    window.addEventListener("resize", handleResize);

    // Clean up the event listener on unmount
    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  return isSmallScreen;
};

export default useSmallScreen;
