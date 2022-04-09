import {TextField} from "@mui/material";
import {DynamicBaseInputProps} from "../../FieldParameter";

export function DynamicNumberInput(props: DynamicBaseInputProps): JSX.Element {
    let {name, value, label, error, placeholder, errorMessage, helperText, onBlur, onChange, fullWidth, width100} = props;
    return (
        <TextField
            fullWidth={fullWidth || false}
            style={{width: width100 ? "100%" : undefined}}
            type="number"
            key={`input-number-${name}`}
            name={name}
            label={label}
            value={value}
            error={error}
            placeholder={placeholder}
            onChange={event => {
                // @ts-ignore
                onChange(Number(event.target.value))
            }}
            onBlur={onBlur}
            helperText={error ? errorMessage : helperText}
        />
    )
}