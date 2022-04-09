import {TextField} from "@mui/material";
import React from "react";
import {DynamicBaseInputProps} from "../../FieldParameter";
import MuiPhoneNumber from "material-ui-phone-number-2";

export function DynamicPhoneNumber(props: DynamicBaseInputProps): JSX.Element {
    let {name, value, label, error, placeholder, errorMessage, helperText, onBlur, onChange, fullWidth, width100} = props;
    return (
        <MuiPhoneNumber
            fullWidth={fullWidth}
            key={`input-phone-number-${name}`}
            defaultCountry={'ch'}
            variant="outlined"
            onlyCountries={['de','ch','it','fr']}
            onChange={onChange}
            name={name}
            value={value}
            label={label}
            error={error}

            helperText={error ? errorMessage : helperText}
        />
    )
}