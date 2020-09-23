import React from 'react';
import {
  BrowserRouter as Router,
  Switch,
  Route
} from "react-router-dom";
import './App.css';
import Home from './Home'
import Callback from './Callback'

function App() {
  return (
    <div className="App">
      <header className="App-header">
        React OAuth Test Client
      </header>
        <Router>
          <Switch>
            <Route path="/callback">
              <Callback />
            </Route>
            <Route path="/">
              <Home />
            </Route>
          </Switch>
        </Router>
    </div>
  );
}

export default App;
