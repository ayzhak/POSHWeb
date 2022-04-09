import {Box, Grid, Input, Slider, Typography} from "@mui/material";
import {VolumeUp} from "@mui/icons-material";
import React from "react";

export function NumberSlider() {
    let [sliderValeue, setSliderValue] = React.useState(0);
    let onSliderChange = (event: Event, newValue: number | number[]) => {
        setSliderValue(newValue as number);
    };
    let onInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSliderValue(event.target.value === '' ? 0 : Number(event.target.value));
    };
    let onSliderBlur = () => {
        if (sliderValeue < 0) {
            setSliderValue(sliderValeue as number);
        } else if (sliderValeue > 100) {
            setSliderValue(sliderValeue as number);
        }
    };

    return (
        <Box>
            <Typography id="input-slider" gutterBottom>
                Volume
            </Typography>
            <Grid container spacing={2} alignItems="center">
                <Grid item>
                    <VolumeUp/>
                </Grid>
                <Grid item xs>
                    <Slider
                        value={sliderValeue}
                        onChange={onSliderChange}
                        marks
                        aria-labelledby="input-slider"
                    />
                </Grid>
                <Grid item>
                    <Input
                        value={sliderValeue}
                        size="small"
                        onChange={onInputChange}
                        onBlur={onSliderBlur}
                        inputProps={{
                            step: 10,
                            min: 0,
                            max: 100,
                            type: "number",
                            "aria-labelledby": "input-slider",
                        }}
                    />
                </Grid>
            </Grid>
        </Box>
    )
}