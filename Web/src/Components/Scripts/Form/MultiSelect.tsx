// make react component
import React from 'react';
import {Checkbox, FormControl, FormControlLabel, FormGroup, FormHelperText, FormLabel} from "@mui/material";

export function MultiSelect() {
    return (
        <FormControl sx={{m: 3}} component="fieldset" variant="standard">
            <FormLabel component="legend">Assign responsibility</FormLabel>
            <FormGroup>
                <FormControlLabel
                    control={
                        <Checkbox name="gilad"/>
                    }
                    label="Gilad Gray"
                />
                <FormControlLabel
                    control={
                        <Checkbox name="jason"/>
                    }
                    label="Jason Killian"
                />
                <FormControlLabel
                    control={
                        <Checkbox name="antoine"/>
                    }
                    label="Antoine Llorca"
                />
            </FormGroup>
            <FormHelperText>Be careful</FormHelperText>
        </FormControl>
    )
}