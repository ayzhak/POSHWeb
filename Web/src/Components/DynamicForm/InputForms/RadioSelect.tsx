import {FormControl, FormControlLabel, FormHelperText, FormLabel, Radio, RadioGroup} from "@mui/material";
import React from "react";
import {Controller} from "react-hook-form";
import {factoryRules} from "../DynamicHelper";
import {DynamicInputFieldProps} from "../FieldParameter";

export function RadioSelect({field, form}: DynamicInputFieldProps): JSX.Element {
    let {name, label, helpMessage, defaultValue} = field;
    return (
        <Controller
            name={name}
            control={form.control}
            defaultValue={defaultValue}
            rules={factoryRules(field)}
            render={({field: controllerField, fieldState}) => (
                <FormControl sx={{m: 3}} variant="outlined" error={Boolean(fieldState.error?.message)}>
                    <FormLabel component="legend">{label}</FormLabel>
                    <RadioGroup
                        {...controllerField}
                        aria-labelledby="demo-error-radios"
                    >
                        {field.selectableValues?.map((option, index) => (
                            <FormControlLabel
                                key={`input-radio-${name}-${index}`}
                                value={option}
                                control={<Radio key={`${name}-input-${index}`}/>}
                                label={option}
                            />
                        ))}
                    </RadioGroup>
                    {(fieldState.error?.message || helpMessage) &&
                        <FormHelperText>{fieldState.error?.message || helpMessage}</FormHelperText>}
                </FormControl>
            )}
        />)
}