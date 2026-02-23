import { createContext, useContext, useState } from "react"

const AuthContext = createContext();

export default function AuthProvider({children}) {
    const [user, setUser] = useState(localStorage.getItem('user') || '');
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
            if (user === null) throw new Error('unexpected data');
            setUser(user.username);
            setToken(user.token);
            setIsAuthenticated(true);
            localStorage.setItem('user', user.username);
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
        localStorage.removeItem('user');
        localStorage.removeItem('isAuthenticated');
    };

    return <AuthContext.Provider value={{login, user, token, isAuthenticated, logout}}> 
            {children}
           </AuthContext.Provider>
}

export const useAuth = () => {
    return useContext(AuthContext);
}