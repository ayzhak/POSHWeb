import {
    FormControl,
    FormControlLabel,
    FormHelperText,
    FormLabel,
    Radio,
    RadioGroup,
    SelectChangeEvent
} from "@mui/material";
import React from "react";

export function InputSelect() {
    const [age, setAge] = React.useState('');
    const handleChange = (event: SelectChangeEvent) => {
        setAge(event.target.value as string);
    };
    return (
        <FormControl sx={{m: 3}} variant="standard">
            <FormLabel id="demo-error-radios">Pop quiz: MUI is...</FormLabel>
            <RadioGroup
                aria-labelledby="demo-error-radios"
                name="quiz"
            >
                <FormControlLabel value="best" control={<Radio/>} label="The best!"/>
                <FormControlLabel value="worst" control={<Radio/>} label="The worst."/>
            </RadioGroup>
            <FormHelperText>This is a helper Text</FormHelperText>
        </FormControl>
    )
}