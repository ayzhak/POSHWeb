import {IconButton, InputAdornment, TextField} from "@mui/material";
import React, {useState} from "react";
import {DynamicBaseInputProps} from "../../FieldParameter";
import {Visibility, VisibilityOff} from "@mui/icons-material";

export function DynamicPassword(props: DynamicBaseInputProps): JSX.Element {
    let {
        name,
        value,
        label,
        error,
        placeholder,
        errorMessage,
        helperText,
        onBlur,
        onChange,
        fullWidth,
        width100
    } = props;
    let [showPasswort, setShowPassword] = useState<boolean>(false)
    return (
        <TextField
            fullWidth={fullWidth || false}
            style={{width: width100 ? "100%" : undefined}}
            type={showPasswort ? 'text' : 'password'}
            key={`input-text-${name}`}
            name={name}
            label={label}
            value={value}
            error={error}
            placeholder={placeholder}
            onChange={onChange}
            onBlur={onBlur}
            helperText={error ? errorMessage : helperText}
            onMouseLeave={()=> setShowPassword(false)}
            InputProps={{
                endAdornment: (
                    <InputAdornment position="end">
                        <IconButton
                            aria-label="toggle password visibility"
                            onClick={() => setShowPassword(!showPasswort)}
                            edge="end"
                        >
                            {showPasswort ? <VisibilityOff/> : <Visibility/>}
                        </IconButton>
                    </InputAdornment>
                )
            }}
        />
    )
}