// // context/AuthContext.tsx
// import { createContext, useContext, useState } from "react";

// interface AuthContextType {
//   userInfo: any;
//   setUserInfo: (user: any) => void;
// }

// const AuthContext = createContext<AuthContextType | undefined>(undefined);

// export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
//   const [userInfo, setUserInfo] = useState<any>(() => {
//     const raw = sessionStorage.getItem("userInfo");
//     return raw ? JSON.parse(raw) : null;
//   });

//   return (
//     <AuthContext.Provider value={{ userInfo, setUserInfo }}>
//       {children}
//     </AuthContext.Provider>
//   );
// };

// export const useAuth = () => {
//   const context = useContext(AuthContext);
//   if (!context) throw new Error("useAuth must be used within AuthProvider");
//   return context;
// };