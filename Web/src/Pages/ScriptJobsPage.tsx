import React from "react";
import {Typography} from "@mui/material";
import ScriptsJobsTable from "../Components/Jobs/ScriptsJobsTable";


export default function ScriptJobsPage() {
    return (
        <>
            <Typography variant="h2" noWrap>
                Script Jobs
            </Typography>
            <ScriptsJobsTable/>
        </>
    );
}

