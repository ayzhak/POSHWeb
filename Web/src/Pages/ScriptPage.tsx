import React from "react";
import {Typography} from "@mui/material";
import ScriptsTable from "../Components/Scripts/ScriptsTable";


export default function ScriptsPage() {
    return (
        <>
            <Typography variant="h2" noWrap>
                Scripts Run
            </Typography>
            <ScriptsTable/>
        </>
    );
}
