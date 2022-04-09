import {
    Checkbox,
    FormControl,
    FormControlLabel,
    FormGroup,
    FormHelperText,
    FormLabel,
    MenuItem,
    Select
} from "@mui/material";
import React from "react";
import {Controller, useFieldArray} from "react-hook-form";
import {factoryRules} from "../DynamicHelper";
import {DynamicInputFieldProps} from "../FieldParameter";

export function SelectArray({field, form}: DynamicInputFieldProps): JSX.Element {
    let {name, label, helpMessage, defaultValue} = field;
    const {append, remove, fields} = useFieldArray({
        control: form.control,
        name: name,
    });

    const error = form.formState.errors[name]

    function handleChange(event: any) {
        const name = event.target.name;
        const checked = event.target.checked;
        if (checked) append(name)
        if (!checked) {
            let list = form.getValues(`${field.name}`)
            let index = list.findIndex((value: any) => value === name)
            remove(index)
        }
    }

    function isChecked(option: string): boolean {
        let list: Array<string> = form.getValues(`${name}`)
        return list.includes(option)
    }

    return (
        <Controller
            control={form.control}
            name={field.name}
            defaultValue={defaultValue}
            rules={factoryRules(field)}
            render={({field: {name, value, onBlur, onChange}}) => (
                <FormControl component="fieldset" variant="standard" fullWidth error={Boolean(error?.message)}>
                    <FormLabel component="legend">{label}</FormLabel>
                    <FormGroup>
                        {field.selectableValues?.map((option, index) => (
                            <FormControlLabel
                                key={index}
                                control={
                                    <Checkbox
                                        key={`input-checkbox-${name}-${index}`}
                                        name={option}
                                        onChange={handleChange}
                                        onBlur={() => onBlur()}
                                        checked={isChecked(option)}
                                    />
                                }
                                label={option}
                            />
                        ))}

                    </FormGroup>
                    {(error?.message || helpMessage) &&
                        <FormHelperText>{error?.message || helpMessage}</FormHelperText>}
                </FormControl>
            )}
        />
    )
}