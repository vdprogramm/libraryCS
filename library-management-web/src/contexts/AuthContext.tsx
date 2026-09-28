import {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from "react";
import api from "../api/axios";

type Role = "ADMIN" | "LIBRARIAN" | "READER";

type User = {
  id: string;
  username: string;
  email: string;
  fullName: string;
  roles: Role[];
};

type AuthContextType = {
  user: User | null;
  login: (username: string, password: string) => Promise<User>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    try {
      const saved = sessionStorage.getItem("user");
      return saved ? (JSON.parse(saved) as User) : null;
    } catch {
      return null;
    }
  });

  async function login(username: string, password: string) {
    const response = await api.post<{
      accessToken: string;
      user: User;
    }>("/auth/login", { username, password });

    const { accessToken, user } = response.data;

    sessionStorage.setItem("accessToken", accessToken);
    sessionStorage.setItem("user", JSON.stringify(user));

    setUser(user);
    return user;
  }

  function logout() {
    sessionStorage.removeItem("accessToken");
    sessionStorage.removeItem("user");
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider");
  }

  return context;
}
