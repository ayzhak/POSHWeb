import {IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow,} from "@mui/material";

// use recoil js to store the scripts from the api
import {useRecoilState, useRecoilValue} from "recoil";
import {scriptJobsQuery, scriptsJobsState} from "../../State/atoms";
import {useEffect} from "react";
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import {useLocation, useNavigate} from 'react-router-dom';
import {RenderState} from "../../Pages/ScirptJobPage";

export default function ScriptsJobsTable() {
    const [scriptJobs, setScriptsJobsState] = useRecoilState(scriptsJobsState);
    const queryScriptJobs = useRecoilValue(scriptJobsQuery)
    const location = useLocation();
    let navigate = useNavigate();
    useEffect(() => {
        setScriptsJobsState(queryScriptJobs)
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
                        <TableCell align="center">State</TableCell>
                        <TableCell align="center">Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {scriptJobs.map((row) => (
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
                            <TableCell align="center">{RenderState(row.state || "Created")}</TableCell>

                            <TableCell align="center">
                                <IconButton onClick={() => navigate(`${location.pathname}\\${row.id}`)}>
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
