import {PSScript} from "../../apiClient";
import React from "react";
import {Box, FormControlLabel, Slider, Stack, Switch, TextField} from "@mui/material";
import {NumberSlider} from "./Form/NumberSlider";
import {InputSelect} from "./Form/InputSelect";
import {MultiSelect} from "./Form/MultiSelect";
import {DatePicker, DateTimePicker} from "@mui/lab";

type ScriptExecutionProps = {
    script: PSScript;
};

function valuetext(value: number) {
    return `${value}°C`;
}

export default function ScriptExecution() {
    const [value, setValue] = React.useState<Date | null>(null);
    return (<>
        <Box>
            <Stack>
                <TextField
                    error
                    id="outlined-error-helper-text"
                    label="Error"
                    defaultValue="Hello World"
                    helperText="Incorrect entry."
                />
                <FormControlLabel control={<Switch defaultChecked/>} label="Label"/>
                <Slider
                    aria-label="Temperature"
                    defaultValue={30}
                    getAriaValueText={valuetext}
                    valueLabelDisplay="auto"
                    step={1}
                    marks
                    min={10}
                    max={20}
                />
                <NumberSlider/>
                <InputSelect/>
                <MultiSelect/>
                <DatePicker
                    mask="__.__.____"
                    label="Basic example"
                    value={value}
                    onChange={(newValue) => {
                        setValue(newValue);
                    }}
                    renderInput={(params) => <TextField {...params} />}
                />
                <DateTimePicker
                    mask="__.__.____ __:__"
                    renderInput={(params) => <TextField {...params} />}
                    value={value}
                    onChange={(newValue) => {
                        setValue(newValue);
                    }}
                />
            </Stack>
        </Box>
    </>)
}