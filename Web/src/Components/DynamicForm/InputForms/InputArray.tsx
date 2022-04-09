import {Box, Button, Grid, IconButton} from "@mui/material";
import React from "react";
import AddIcon from "@mui/icons-material/Add";
import RemoveIcon from "@mui/icons-material/Remove";
import {Controller, useFieldArray} from "react-hook-form";
import {getDefaultValue} from "../DynamicHelper";
import {DynamicInputFieldProps} from "../FieldParameter";
import {DynamicSingleInput} from "./DynamicSingleInput";

export function InputArray({field, form}: DynamicInputFieldProps): JSX.Element {
    let {name, label, helpMessage, defaultValue, type} = field;

    const {fields, append, remove, insert} = useFieldArray({
        control: form.control,
        name: name
    });

    return (
        <Grid container direction={"column"} spacing={1}>
            {
                fields.map((field, index) => {
                    return (
                        <Grid item>
                            <Box style={{display: "flex", alignItems: "center"}}>
                                <Controller
                                    name={`${name}.${index}`}
                                    control={form.control}
                                    defaultValue={defaultValue}
                                    render={({field, fieldState}) => (
                                        <DynamicSingleInput name={field.name}
                                                            label={field.name}
                                                            value={field.value}
                                                            error={Boolean(fieldState.error?.message)}
                                                            placeholder={""}
                                                            helperText={fieldState.error?.message || ""}
                                                            errorMessage={fieldState.error?.message || ""}
                                                            onBlur={field.onBlur}
                                                            onChange={field.onChange}
                                                            type={type}
                                                            fullWidth
                                        />
                                    )}
                                />
                                <IconButton onClick={() => {
                                    insert(index, getDefaultValue(type));
                                }}>
                                    <AddIcon style={{color: "green"}}/>
                                </IconButton>
                                <IconButton onClick={() => {
                                    remove(index);
                                }}>
                                    <RemoveIcon style={{color: "red"}}/>
                                </IconButton>
                            </Box>
                        </Grid>
                    )
                })}
            <Grid item>
                <Button fullWidth variant="contained" color="primary"
                        onClick={() => {
                            append(getDefaultValue(type));
                        }}>Add</Button>
            </Grid>
        </Grid>
    )
}