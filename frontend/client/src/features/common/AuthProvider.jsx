import { createContext, useContext, useState } from "react"

const AuthContext = createContext();

export default function AuthProvider({children}) {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(localStorage.getItem('token') || '');
    const [isAuthenticated, setIsAuthenticated] = useState(() => {
        return localStorage.getItem('isAuthenticated') === 'true'
    });
    
    const login = async (credentials) => {
        try {
            const response = await fetch('http://localhost:5193/api/users', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(credentials)
            });
            if (!response.ok) throw new Error(`Login failed'${response.status, response.statusText}`);
            const user = await response.json();
            if (user === undefined) throw new Error('unexpected data');
            setUser(user);
            setToken(user.token);
            setIsAuthenticated(true);
            localStorage.setItem('token', user.token);
            localStorage.setItem('isAuthenticated', true);
            return;
        } catch(err) {
            console.log("Error: " + err);
        }
    };

    const logout = () => {
        setUser(null);
        setToken('');
        setIsAuthenticated(false);
        localStorage.removeItem('token');
    };

    

    return <AuthContext.Provider value={{login, user, token, isAuthenticated, logout}}> 
            {children}
           </AuthContext.Provider>
}

export const useAuth = () => {
    return useContext(AuthContext);
}