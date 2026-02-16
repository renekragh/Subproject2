import { Outlet } from 'react-router-dom';
import Header from './Header';
import Navbar from './Navbar';
import Footer from './Footer';

export default function Layout() {
  return (
    <div>
      <header><Header /></header>
      <nav><Navbar /></nav>
      <main><Outlet /></main>
      <footer><Footer /></footer>
    </div>
  );
}