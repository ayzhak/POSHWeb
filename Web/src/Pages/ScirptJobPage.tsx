import React, {useEffect} from "react";
import {Box, Chip, Grid, Paper, Table, TableBody, TableCell, TableRow, Typography} from "@mui/material";
import {useParams} from "react-router-dom";
import {useRecoilState, useRecoilValue} from "recoil";
import {scriptJobQuery, scriptJobState} from "../State/atoms";
import {JobParameterState, JobState} from "../apiClient";
import ReactAnsi from 'react-ansi'

export function RenderState(state: JobState | JobParameterState) {
    switch (state) {
        case "Created":
            return (<Chip label={state}/>)
        case "Finished":
            return (<Chip color="success" label={state}/>)
        case "Failed":
            return (<Chip color="error" label={state}/>)
        case "Running":
            return (<Chip color="info" label={state}/>)
        case "Valid":
            return (<Chip color="success" label={state}/>)
        default:
            return <Chip color="error" label={state}/>;
    }
}

export default function ScriptJobPage() {
    const params = useParams<{ id: string }>();
    const [scriptJob, setScriptJob] = useRecoilState(scriptJobState)
    const queryScript = useRecoilValue(scriptJobQuery(params.id))
    useEffect(() => {
        setScriptJob(queryScript)
    }, [])

    return (
        <>
            <Typography variant="h2" noWrap>
                Job {scriptJob?.id}
            </Typography>
            <Grid container spacing={3}>
                <Grid item xs={4}>
                    <Paper>
                        <Table>
                            <TableBody>
                                <TableRow>
                                    <TableCell align="left"><Typography
                                        style={{fontWeight: "bolder"}}>Filename</Typography></TableCell>
                                    <TableCell align="left">{scriptJob?.fileName}</TableCell>
                                </TableRow>
                                <TableRow>
                                    <TableCell align="left"><Typography
                                        style={{fontWeight: "bolder"}}>Status</Typography></TableCell>
                                    <TableCell
                                        align="left">{RenderState(scriptJob?.state || JobState.Created)} </TableCell>
                                </TableRow>
                            </TableBody>
                        </Table>
                    </Paper>
                </Grid>
                <Grid item xs={8}>
                    <Box width="100%" height="500px" textAlign="left">
                        <ReactAnsi
                            log={scriptJob?.log || ""}
                            linkify
                            virtual
                            logStyle={{height: 600}}

                        />
                    </Box>
                </Grid>
                <Grid item xs={4}>
                    <Paper>
                        <Table>
                            <TableBody>
                                {scriptJob?.parameters?.map(value => (
                                    <>
                                        <TableRow>
                                            <TableCell align="left"><Typography
                                                style={{fontWeight: "bolder"}}>{value.name}</Typography></TableCell>
                                            <TableCell
                                                align="left">{value.state} </TableCell>
                                            <TableCell align="left">{value.value}</TableCell>
                                        </TableRow>
                                    </>
                                ))}
                            </TableBody>
                        </Table>
                    </Paper>
                </Grid>
            </Grid>
        </>
    );
}