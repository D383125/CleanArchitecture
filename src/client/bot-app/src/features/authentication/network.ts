import axios from "axios";

interface TokenResponse {
    access_token: string;
    expires_in: number;
    token_type: string;
  }
  
  export async function login(email: string, password: string): Promise<TokenResponse> {
    const data = new URLSearchParams();
    data.append("client_id", "react_client");
    data.append("client_secret", "react_secret");
    data.append("grant_type", "password");
    data.append("username", email);
    data.append("password", password);
  
    const response = await axios.post<TokenResponse>("http://localhost:5000/connect/token", data, {
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
    });
  
    return response.data;
  }
