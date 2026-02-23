import { useState, useRef } from 'react';
import { Form, Button } from 'react-bootstrap';
import { useNavigate, replace } from "react-router-dom";

export default function Account() {
  return <div>
    <h1>User account</h1>
    <ul>
      <li>Update account info</li>
      <li>Delete account</li>
    </ul>
   
  </div>
/*
    const [state, setState] = useState({email, name, username, password});
    const [password2, setPassword2] = useState('');
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setLoading(true);
        
        if ( state.password !== password2) {
            setError('Passwords do not match!')
            return;
        }
        
        try {
            await updateAccount(idempotencyKeyRef, {name, email, username, password});
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
                value={state.email}
                onChange={(e) => setState(state.email === e.target.value)}
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
                value={state.name}
                onChange={(e) => setState(state.name === e.target.value)}
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
                value={state.username}
                onChange={(e) => setState(state.username === e.target.value)}
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
                value={state.password}
                onChange={(e) => setState(state.password === e.target.value)}
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
  */
}