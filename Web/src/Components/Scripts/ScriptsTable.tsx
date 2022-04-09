import {IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow,} from "@mui/material";

import 'react-toastify/dist/ReactToastify.css';

// use recoil js to store the scripts from the api
import {useRecoilState} from "recoil";
import {scriptsState} from "../../State/atoms";
import {useEffect} from "react";
import {ScriptApi} from "../../apiClient";
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import {useLocation, useNavigate} from 'react-router-dom';

export default function ScriptsTable() {
    const [scripts, setScriptsState] = useRecoilState(scriptsState);
    const location = useLocation();
    let navigate = useNavigate();
    useEffect(() => {
        // make api call to get all scripts and set the state
        const scriptsApi = new ScriptApi();
        scriptsApi.apiV1ScriptGet().then((response) => {
            setScriptsState(response.data);
        });
    }, []);

    return (
        <TableContainer component={Paper}>
            <Table sx={{minWidth: 650}} aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell>ID</TableCell>
                        <TableCell align="right">Filename</TableCell>
                        <TableCell align="right">Path</TableCell>
                        <TableCell align="right">Created At</TableCell>
                        <TableCell align="center">Actions</TableCell>

                    </TableRow>
                </TableHead>
                <TableBody>
                    {scripts.map((row) => (
                        <TableRow
                            key={row.id}
                            sx={{'&:last-child td, &:last-child th': {border: 0}}}
                        >
                            <TableCell component="th" scope="row">
                                {row.id}
                            </TableCell>
                            <TableCell align="right">{row.fileName}</TableCell>
                            <TableCell align="right">{row.fullPath}</TableCell>
                            <TableCell align="right">{row.createdAt}</TableCell>

                            <TableCell align="center">
                                <IconButton onClick={() => navigate(`${location.pathname}\\${row.id}\\run`)}>
                                    <PlayArrowIcon/>
                                </IconButton>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}
