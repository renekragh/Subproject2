import { useState, useRef } from 'react';
import { Form, Button } from 'react-bootstrap';
import { useNavigate, replace } from "react-router-dom";
import './register.css';

  const createAccount = async (idempotencyKeyRef, accountDetails) => {
    try {
        const response = await fetch('http://localhost:5193/api/users', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Idempotency-Key': idempotencyKeyRef.current,
          },
          body: JSON.stringify(accountDetails)
        });
        if (response.status !== 201) throw new Error(`Creating user account failed: '${response.status + ' ' + response.statusText}`);
        idempotencyKeyRef.current = crypto.randomUUID();
        return;
      } catch (err) {
          throw new Error(err);
    } 
  };

export default function Register() {
    const idempotencyKeyRef = useRef(crypto.randomUUID());
    const [email, setEmail] = useState('');
    const [name, setName] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [password2, setPassword2] = useState('');
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const ACCOUNT_ICON = <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-person-fill-add" viewBox="0 0 16 16">
                            <path d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7m.5-5v1h1a.5.5 0 0 1 0 1h-1v1a.5.5 0 0 1-1 0v-1h-1a.5.5 0 0 1 0-1h1v-1a.5.5 0 0 1 1 0m-2-6a3 3 0 1 1-6 0 3 3 0 0 1 6 0"/>
                            <path d="M2 13c0 1 1 1 1 1h5.256A4.5 4.5 0 0 1 8 12.5a4.5 4.5 0 0 1 1.544-3.393Q8.844 9.002 8 9c-5 0-6 3-6 4"/>
                        </svg>

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        if ( password !== password2) {
            setError('Passwords do not match!')
            return;
        }

        try {
          await createAccount(idempotencyKeyRef, {name, email, username, password});
          navigate('/users/login', replace);
        } catch(err) {
            setError(err.message || 'An error occurred during account creation');
        } finally {
            setLoading(false);
        }
    };

    return (
    <div className="login-wrapper">
        <h2>{ACCOUNT_ICON} Create Account</h2>
        {error && <div className="error-message">{error}</div>}
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3" controlId="formBasicEmail">
              <Form.Control
                required
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                disabled={loading}
              />
              <Form.Control.Feedback type="invalid">
                Please enter your email.
              </Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicName">
              <Form.Control
                required
                type="text"
                placeholder="Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                disabled={loading}
              />
              <Form.Control.Feedback type="invalid">
                Please enter your name.
              </Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicUsername">
              <Form.Control
                required
                type="text"
                placeholder="Username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                disabled={loading}
              />
              <Form.Control.Feedback type="invalid">
                Please choose a username.
              </Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicPassword1">
              <Form.Control
                required
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={loading}
              />
              <Form.Control.Feedback type="invalid">
                Please choose a password.
              </Form.Control.Feedback>
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicPassword2">
              <Form.Control
                required
                type="password"
                placeholder="Repeat password"
                value={password2}
                onChange={(e) => setPassword2(e.target.value)}
                disabled={loading}
              />
              <Form.Control.Feedback type="invalid">
                Please enter same password.
              </Form.Control.Feedback>
            </Form.Group>

            <Button variant="primary" type="submit" className="register-button" disabled={loading}>
              {loading ? 'Creating account...' : 'Submit'}
            </Button>
          </Form>
      </div>
  );
}