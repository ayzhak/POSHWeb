import {FormControl, FormControlLabel, FormHelperText, FormLabel, Switch} from "@mui/material";
import React from "react";
import {DynamicBaseInputProps} from "../../FieldParameter";

export function DynamicSwitchInput(props: DynamicBaseInputProps): JSX.Element {
    let {name, value, label, error, placeholder, errorMessage, helperText, onBlur, onChange} = props;
    return (
        <FormControl error={error} fullWidth>
            <FormLabel component="legend">{label}</FormLabel>
            <FormControlLabel
                style={{width: "100%"}}
                control={<Switch name={name}
                                 key={`input-switch-${name}`}
                                 onChange={event => onChange(Boolean(event.target.checked))}
                                 onBlur={onBlur} value={value}/>}
                label={label}
            />
            {(helperText || error) && <FormHelperText>{error ? errorMessage : helperText}</FormHelperText>}
        </FormControl>
    )
}