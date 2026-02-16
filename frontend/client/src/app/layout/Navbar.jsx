import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import {useAuth} from '../../features/common/AuthProvider';

export default function Navigation() {
  const { user, isAuthenticated } = useAuth();

  return (
    <Navbar expand="lg" className="bg-body-tertiary" bg="dark" data-bs-theme="dark" sticky="top">
      <Container>
        <Navbar.Brand href="#home">React-Bootstrap</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            <Nav.Link href="/">Home</Nav.Link>
            <Nav.Link href="/titles">Titles</Nav.Link>
            <Nav.Link href="/names">Names</Nav.Link>
            {isAuthenticated &&
              <NavDropdown title="User" id="basic-nav-dropdown">
                <NavDropdown.Item href="/users/name-bookmarks">Name bookmarks </NavDropdown.Item>
                <NavDropdown.Item href="/users/rating-history">Rating history</NavDropdown.Item>
                <NavDropdown.Item href="/users/title-bookmarks">Title bookmarks</NavDropdown.Item>
                <NavDropdown.Item href="/users/search-history">Search history</NavDropdown.Item>
              </NavDropdown>
            }
            <Navbar.Toggle />
            {isAuthenticated &&
              <Navbar.Text>
                Signed in as: <a href="#login">{user}</a>
              </Navbar.Text>
             }
              {!isAuthenticated &&
              <Navbar.Text>
                Login <a href="/users/login">Login</a> or <a href="/users/account">create account</a>
              </Navbar.Text>
             }
          </Nav>
          <Form className="d-flex">
            <Form.Control
              type="search"
              placeholder="Search"
              className="me-2"
              aria-label="Search"
            />
            <Button variant="outline-success">Search</Button>
          </Form>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  )
}