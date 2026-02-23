import { useEffect, useState, useRef } from 'react'
import TitleDashboard from "../../features/titles/dashboard/TitleDashboard";
import Login from '../../features/users/Login';
import ProtectedRoute from '../../features/common/ProtectedRoute';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import AuthProvider from '../../features/common/AuthProvider';
import Home from '../Home';
import Layout from '../layout/Layout';
import Search from '../../features/common/Search';
import TitleBookmarks from '../../features/users/TitleBookmarks';
import RatingHistory from '../../features/users/RatingHistory';
import SearchHistory from '../../features/users/SearchHistory';
import NameBookmarks from '../../features/users/NameBookmarks';
import NameDashboard from '../../features/names/dashboard/NameDashboard';
import Register from '../../features/users/Register';
import 'bootstrap/dist/css/bootstrap.css';
import Account from '../../features/users/Account';

export default function App() {
  const [movies, setMovies] = useState([]);
  const [names, setNames] = useState([]);
  const [selectedMovie, setSelectedMovie] = useState();
  const [selectedName, setSelectedName] = useState();
  const [refresh, setRefresh] = useState(false);
  const [refreshAfterBookmarkedNames, setRefreshAfterBookmarkedNames] = useState(false);
  const idempotencyKeyRef = useRef(crypto.randomUUID());
  const [bookmarkNames, setBookmarkNames] = useState([]);
  const token = localStorage.getItem('token');
  const [ratingHistory, setRatingHistory] = useState([]);
  const [bookmarks, setBookmarks] = useState([]);
  const isAuthenticated = localStorage.getItem('isAuthenticated');

   //**************************************** NAMES ************************************************************************ */ 

    useEffect(() => {
    async function load() {
      try {
        const res = await fetch('http://localhost:5193/api/names');
        if (!res.ok) throw new Error("status = " + res.status);
        const data = await res.json(); 
        if (data.items === undefined) throw new Error('unexpected data');
        setNames(data.items);
        setRefreshAfterBookmarkedNames(false);
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    }
    load();
  },[isAuthenticated, refreshAfterBookmarkedNames]);

    useEffect(() => {
    async function load() {
      try {
        const res = await fetch('http://localhost:5193/api/bookmark-names',
                      {
                        headers: 
                        {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        }
                      }
                  );
        if (!res.ok) throw new Error("status = " + res.status);
        const data = await res.json(); 
        if (data.items === undefined) throw new Error('unexpected data');
        setBookmarkNames(data.items);
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    }
    load();
  },[token, names]);

 //**************************************** TITLES ************************************************************************ */ 

  useEffect(() => {
    async function load() {
      try {
        const res = await fetch('http://localhost:5193/api/titles');
        if (!res.ok) throw new Error("status = " + res.status);
        const data = await res.json(); 
        if (data.items === undefined) throw new Error('unexpected data');
        setMovies(data.items);
        if (isAuthenticated) await ApiGetRatingHistoryAndBookmarks();
        setRefresh(false);
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    }
    load();
  },[isAuthenticated, refresh]);

  async function ApiGetRatingHistoryAndBookmarks() {
    try {
          const response = await Promise.all([
            fetch('http://localhost:5193/api/ratings-history', 
              {
                headers: 
                {
                  'Content-Type': 'application/json',
                  'Authorization': `Bearer ${token}`
                }
              }
            ),
            fetch('http://localhost:5193/api/bookmark-titles', 
              {
                headers: {
                          'Content-Type': 'application/json',
                          'Authorization': `Bearer ${token}`
                        }
              }
            )                
          ]);
          const responseStatus = response.find(x => !x.ok);
          if (responseStatus !== undefined) throw new Error(responseStatus.url + ' ' + responseStatus.status + ' ' + responseStatus.statusText);
          const [data0, data1] = await Promise.all(response.map(async (x) => await x.json()));
          if ([data0, data1].length !== response.length) throw new Error('unexpected data');
          setRatingHistory(data0.items);
          setBookmarks(data1.items);
          return;
      } catch (error) {
          throw new Error(error);
        }
    }

  const handleSelectMovie = async (url) => {
    await fetch(`${url}`)
    .then(response => response.json())
    .then(data => setSelectedMovie(data))
  }

  const handleCancelSelectMovie = () => {
    setSelectedMovie(undefined);
  }

//******************************************** BOOKMARKS ************************************************************************* */ 

  async function ApiAddBookmark(movieId, note) {
    const id = movieId.slice(0,1);
      try {
        const response = await fetch(id === 't' ? `http://localhost:5193/api/titles/${movieId}/bookmarks` : `http://localhost:5193/api/names/${movieId}/bookmarks`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Idempotency-Key': idempotencyKeyRef.current,
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify(note)
        });
        if (response.status !== 201) throw new Error(`Creating bookmark failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        idempotencyKeyRef.current = crypto.randomUUID();
        id === 't' ? setRefresh(true) : setRefreshAfterBookmarkedNames(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiUpdateBookmark(movieId, note) {
    const id = movieId.slice(0,1);
      try {
        const response = await fetch(id === 't' ? `http://localhost:5193/api/bookmark-titles/${movieId}` : `http://localhost:5193/api/bookmark-names/${movieId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify(note)
        });
        if (response.status !== 200) throw new Error(`Updating bookmark failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        id === 't' ? setRefresh(true) : setRefreshAfterBookmarkedNames(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiDeleteBookmark(movieId) {
    const id = movieId.slice(0,1);
      try {
        const response = await fetch(id === 't' ? `http://localhost:5193/api/bookmark-titles/${movieId}` : `http://localhost:5193/api/bookmark-names/${movieId}`, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          }
        });
        if (response.status !== 200) throw new Error(`Updating bookmark failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        id === 't' ? setRefresh(true) : setRefreshAfterBookmarkedNames(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiDeleteAllBookmarks(id) {
        try {
            const response = await fetch(id === 't' ? 'http://localhost:5193/api/bookmark-titles' : 'http://localhost:5193/api/bookmark-names', {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            });
            if (response.status !== 200) throw new Error(`Deleting all title bookmarks failed: '${response.status + ' ' + response.statusText}`);
            const data = await response.json(); 
            if (data === undefined) throw new Error('unexpected data');
            id === 't' ? setRefresh(true) : setRefreshAfterBookmarkedNames(true);
        } catch (err) {
          console.log(err);
        } 
    }

  const handleBookmark = (movieId, note) => {
    const id = movieId.slice(0,1);
    const index = id === 't' ? bookmarks.findIndex(x => x.id === movieId) : bookmarkNames.findIndex(x => x.id === movieId);
    index > -1 ? UpdateBookmark() : AddBookmark();

    async function AddBookmark() {
      try {
        await ApiAddBookmark(movieId, note);
      } catch (err) {
        console.log("Error: " + err.message);
        } 
    };

    async function UpdateBookmark() {
      try {
        await ApiUpdateBookmark(movieId, note);
      } catch (err) {
        console.log("Error: " + err.message);
        }  
    };
  }

  const handleDeleteBookmark = async (movieId) => {
    try {
        await ApiDeleteBookmark(movieId);
    } catch (err) {
        console.log("Error: " + err.message);
      }   
  }

  const handleDeleteAllTitleBookmarks = async () => {
    try {
        await ApiDeleteAllBookmarks('t');
    } catch (err) {
        console.log("Error: " + err.message);
      }   
  }

  const handleDeleteAllNameBookmarks = async () => {
    try {
        await ApiDeleteAllBookmarks('n');
    } catch (err) {
        console.log("Error: " + err.message);
      }   
  }

  const handleSelectName = async (url) => {
    await fetch(`${url}`)
    .then(response => response.json())
    .then(data => setSelectedName(data))
  }

//************************************************* RATINGS ******************************************************************** */ 

  async function ApiAddRate(movieId, rate) {
      try {
        const response = await fetch(`http://localhost:5193/api/titles/${movieId}/ratings`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Idempotency-Key': idempotencyKeyRef.current,
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify(rate)
        });
        if (response.status !== 201) throw new Error(`Creating rate failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        idempotencyKeyRef.current = crypto.randomUUID();
        setRefresh(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiUpdateRate(movieId, rate) {
      try {
        const response = await fetch(`http://localhost:5193/api/titles/${movieId}/ratings`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify(rate)
        });
        if (response.status !== 200) throw new Error(`Updating rate failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        setRefresh(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiDeleteRate(movieId) {
      try {
        const response = await fetch(`http://localhost:5193/api/titles/${movieId}/ratings`, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          }
        });
        if (response.status !== 200) throw new Error(`Deleting rate failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        setRefresh(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  const handleDeleteAllRatings = async () => {
    try {
        //await ApiDeleteAllRatings(); / not implemented yet
    } catch (err) {
        console.log("Error: " + err.message);
      }   
  }

  const handleRate = (movieId, rate) => {

    const index = ratingHistory.findIndex(x => x.id === movieId);
    index > -1 ? UpdateRate() : AddRate();

    async function AddRate() {
      try {
        await ApiAddRate(movieId, rate);
      } catch (err) {
        console.log("Error: " + err.message);
        } 
    };

    async function UpdateRate() {
      try {
        await ApiUpdateRate(movieId, rate);
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    };
  }

  const handleDeleteRate = async (movieId) => {

    try {
      await ApiDeleteRate(movieId);
    } catch (err) {
          console.log("Error: " + err.message);
      } 
  };  

  return (
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Layout />}>
                <Route index element={<Home />} />
                <Route path='/users/login' element={<Login />} />
                <Route path='/users/register' element={<Register />} />
                <Route path='/search' element={
                  <Search
                    bookmarks={bookmarks}
                    handleBookmark={handleBookmark}
                    handleDeleteBookmark={handleDeleteBookmark}
                    ratingHistory={ratingHistory}
                    handleRate={handleRate}
                    handleDeleteRate={handleDeleteRate}
                  />
                } 
                />
                <Route element={<ProtectedRoute />}>
                  <Route path='/users/account' element={<Account />} />
                  <Route
                    path="/users/name-bookmarks"
                    element={
                      <NameBookmarks
                        bookmarkNames={bookmarkNames}
                        handleBookmark={handleBookmark}
                        handleDeleteBookmark={handleDeleteBookmark}
                        handleDeleteAllNameBookmarks={handleDeleteAllNameBookmarks}
                        selectName={handleSelectName}
                        selectedName={selectedName}
                      />
                    }
                  />
                  <Route
                    path="/users/title-bookmarks"
                    element={
                      <TitleBookmarks 
                        bookmarks={bookmarks}
                        handleBookmark={handleBookmark}
                        handleDeleteBookmark={handleDeleteBookmark}
                        handleDeleteAllTitleBookmarks={handleDeleteAllTitleBookmarks}
                        ratingHistory={ratingHistory}
                        handleRate={handleRate}
                        handleDeleteRate={handleDeleteRate}
                        selectMovie={handleSelectMovie}
                        selectedMovie={selectedMovie}
                      />
                    }
                  />
                  <Route
                    path="/users/rating-history"
                    element={
                      <RatingHistory 
                        ratingHistory={ratingHistory}
                        handleRate={handleRate}
                        handleDeleteRate={handleDeleteRate}
                        bookmarks={bookmarks}
                        handleBookmark={handleBookmark}
                        handleDeleteBookmark={handleDeleteBookmark}
                        handleDeleteAllRatings={handleDeleteAllRatings}
                        selectMovie={handleSelectMovie}
                        selectedMovie={selectedMovie}
                      />
                    }
                  />
                  <Route
                    path="/users/search-history"
                    element={
                      <SearchHistory />
                    }
                  />
                  <Route
                    path="/names"
                    element={
                      <NameDashboard
                        names={names} 
                        bookmarks={bookmarkNames}
                        handleBookmark={handleBookmark}
                        handleDeleteBookmark={handleDeleteBookmark}
                      />
                    }
                  />
                  <Route
                    path="/titles"
                    element={
                      <TitleDashboard 
                        movies={movies} 
                        selectMovie={handleSelectMovie}
                        cancelSelectMovie={handleCancelSelectMovie}
                        selectedMovie={selectedMovie}
                        handleRate={handleRate}
                        handleDeleteRate={handleDeleteRate}
                        ratingHistory={ratingHistory}
                        bookmarks={bookmarks}
                        handleBookmark={handleBookmark}
                        handleDeleteBookmark={handleDeleteBookmark}
                      />
                    }   
                  />
                </Route>
              </Route>
            </Routes>       
          </BrowserRouter>
        </AuthProvider> 
  )
}

