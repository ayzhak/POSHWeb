import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import {BrowserRouter} from "react-router-dom";
import {ToastContainer} from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import RecoilNexus from "recoil-nexus";
import axios from "axios"

import {RecoilRoot} from "recoil";
import SignalRProvider from "./SignalR/SignalRProvider";

import './i18n';
import {LocalizationProvider} from "@mui/lab";
import AdapterDateFns from '@mui/lab/AdapterDateFns';
import deLocale from 'date-fns/locale/de';

ReactDOM.render(
    <React.StrictMode>
        <RecoilRoot>
            <LocalizationProvider dateAdapter={AdapterDateFns} locale={deLocale}>
                <RecoilNexus/>
                <BrowserRouter>
                    <App/>
                    <ToastContainer
                        position="top-center"
                        autoClose={5000}
                        newestOnTop={false}
                        closeOnClick
                        pauseOnFocusLoss
                        pauseOnHover
                    />
                    <SignalRProvider/>
                </BrowserRouter>
            </LocalizationProvider>
        </RecoilRoot>
    </React.StrictMode>,
    document.getElementById('root')
)
;

reportWebVitals();
