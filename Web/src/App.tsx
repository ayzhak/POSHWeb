import React, {Suspense} from 'react';
import './App.css';
import {Route, Routes} from "react-router-dom";
import Layout from "./Layout/Default.ts";
import ScriptRunPage from "./Pages/ScriptRunPage";
import {LoadingTea} from "./Components/Loading/LoadingTea";
import ScriptsPage from "./Pages/ScriptPage";
import ScriptJobPage from "./Pages/ScirptJobPage";
import ScriptJobsPage from "./Pages/ScriptJobsPage";

function App() {
    return (
        <div className="App">
            <Layout>
                <Suspense fallback={<LoadingTea/>}>
                <Routes>
                    <Route path="/" element={<h1>Home</h1>}/>
                    <Route path="/scripts" element={<ScriptsPage/>}/>
                    <Route path="/loading" element={<LoadingTea/>}/>
                    <Route path="/scripts/:id/run" element={<ScriptRunPage/>}/>
                    <Route path="/jobs/:id" element={<ScriptJobPage/>}/>
                    <Route path="/jobs" element={<ScriptJobsPage/>}/>
                </Routes>
            </Suspense>
            </Layout>
        </div>
    );
}

export default App;
