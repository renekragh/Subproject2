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

export default function App() {
  const [movies, setMovies] = useState([]);
  const [selectedMovie, setSelectedMovie] = useState();
  const [refresh, setRefresh] = useState(false);
  const idempotencyKeyRef = useRef(crypto.randomUUID());
 // let temp = localStorage.getItem('token');
  const token = 'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSm9obiBEb2UiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTc3MTI3NzkyNH0.mzY7KMYQLIn1xG0HciD7a4VoNWN3rOxkjp2fM4ASrVzVl7dUrxFgRfYAuOk42_jnqhffqaZcFxPuEdllhK0aNw';
  //const token = '';
  const [ratingHistory, setRatingHistory] = useState([]);
  const [bookmarks, setBookmarks] = useState([]);
  //const [error, setError] = useState(null);
 
  //const { isAuthenticated } = useAuth();
  const isAuthenticated = true//useAuth();

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

  async function ApiAddBookmark(movieId, note) {
      try {
        const response = await fetch(`http://localhost:5193/api/titles/${movieId}/bookmarks`, {
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
        setRefresh(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiUpdateBookmark(movieId, note) {
      try {
        const response = await fetch(`http://localhost:5193/api/bookmark-titles/${movieId}`, {
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
        setRefresh(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  async function ApiDeleteBookmark(movieId) {
      try {
        const response = await fetch(`http://localhost:5193/api/bookmark-titles/${movieId}`, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          }
        });
        if (response.status !== 200) throw new Error(`Updating bookmark failed: '${response.status + ' ' + response.statusText}`);
        const data = await response.json(); 
        if (data === undefined) throw new Error('unexpected data');
        setRefresh(true);
        return;
      } catch (err) {
          throw new Error(err);
        } 
  }

  const handleBookmark = (movieId, note) => {
    const index = bookmarks.findIndex(x => x.id === movieId);
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

/*
  const handleRate = (movieId, rate) => {

    const index = ratings.findIndex(x => x.id === movieId) 
    index > -1 ? UpdateRate(index) : AddRate();

    async function AddRate() {
      try {
        await ApiAddRate(movieId, rate);
        setRatings(// Replace the state
          [ // with a new array
            ...ratings, // that contains all the old items
            { id: movieId, rating: rate } // and one new item at the end
          ]
        );
      } catch (err) {
        console.log("Error: " + err.message);
        } 
    };

    async function UpdateRate(index) {
      try {
        await ApiUpdateRate(movieId, rate);
        const updatedRate = ratings.map((v, i) => {
          if (i === index) return { id: movieId, rating: rate } 
          return v;
        });
        setRatings(updatedRate);
      } catch (err) {
          console.log("Error: " + err.message);
        } 
    };
  };
 

  const handleDeleteRate = async (movieId) => {
    try {
      await ApiDeleteRate(movieId);
      setRatings(
        ratings.filter(x => x.id !== movieId)
      );
    } catch (err) {
          console.log("Error: " + err.message);
      } 
  }; 
*/ 

    //console.log('App --> ratings.length: '+ratings.length)
  
  return (
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Layout />}>
                <Route index element={<Home />} />
                <Route path='/login' element={<Login />} />
                <Route path='/search' element={<Search />} />
                <Route element={<ProtectedRoute />}>
                  <Route
                    path="/users/name-bookmarks"
                    element={
                      <NameBookmarks/>
                    }
                  />
                  <Route
                    path="/users/title-bookmarks"
                    element={
                      <TitleBookmarks />
                    }
                  />
                  <Route
                    path="/users/rating-history"
                    element={
                      <RatingHistory />
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
                      <NameDashboard />
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

