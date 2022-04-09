import {FieldParameter} from "./FieldParameter";
import {SubmitHandler, useForm} from "react-hook-form";
import {Box, Button, Grid, InputLabel} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import SendIcon from "@mui/icons-material/Send";
import React, {useEffect} from "react";
import {DynamicInputField} from "./DynamicInputField";

export interface DynamicFormProps {
    fields: Array<FieldParameter>;
    onSubmit: (value: any) => void;
}

export type SupportedPrimitives = number | string | boolean | Date;

interface Map {
    [key: string]: SupportedPrimitives | string[] | number[] | Date[]
}

function transformToDefaultValues(fields: Array<FieldParameter>): Map {
    let values: Map = {}
    fields.forEach(fields => values[fields.name] = fields.defaultValue)
    return values;
}

export function DynamicForm({fields, onSubmit}: DynamicFormProps) {
    const form = useForm({defaultValues: transformToDefaultValues(fields), mode: "all"});
    useEffect(() => {form.reset(transformToDefaultValues(fields))},[fields])
    return (
        <>
            <Grid container rowSpacing={1} columnSpacing={{xs: 1, sm: 2, md: 3}}>
                {
                    fields.map(field => (<>
                            <Grid item xs={4}>
                                <InputLabel>{field.label}</InputLabel>
                            </Grid>
                            <Grid item xs={8}>
                                <DynamicInputField field={field} form={form}/>
                            </Grid>
                        </>
                    ))
                }
            </Grid>
            <Box style={{display: "flex", justifyContent: "center"}}>
                <Button type="reset"
                        variant="outlined"
                        startIcon={<DeleteIcon/>}
                        onClick={() => form.reset()}
                        style={{width: "100%", margin: "5px"}}>
                    Abort
                </Button>
                <Button type="submit"
                        variant="contained"
                        endIcon={<SendIcon/>}
                        onClick={form.handleSubmit(onSubmit)}
                        style={{width: "100%", margin: "5px"}}>
                    Run
                </Button>
            </Box>
        </>
    )
}