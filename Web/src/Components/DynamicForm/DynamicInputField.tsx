import {DynamicInputFieldProps} from "./FieldParameter";
import {Controller} from "react-hook-form";
import {DynamicSingleInput} from "./InputForms/DynamicSingleInput";
import {SelectArray} from "./InputForms/SelectArray";
import {factoryRules} from "./DynamicHelper";
import {RadioSelect} from "./InputForms/RadioSelect";
import {InputArray} from "./InputForms/InputArray";

export function DynamicInputField({field, form}: DynamicInputFieldProps): JSX.Element {
    if (field.isArray && Array.isArray(field.selectableValues)) {
        return (<SelectArray field={field} form={form}/>)
    } // Array of Checkboxes
    if (!field.isArray && Array.isArray(field.selectableValues)) {
        return (<RadioSelect field={field} form={form}/>)
    } // Radio or Select List
    if (field.isArray && !Array.isArray(field.selectableValues)) {
        return (<InputArray field={field} form={form}/>)
    } // Array List of free Inputs
    if (!field.isArray && !Array.isArray(field.selectableValues)) {
        return (<Controller
            control={form.control}
            name={field.name}

            defaultValue={field.defaultValue}
            rules={factoryRules(field)}
            render={({field: {name, value, onBlur, onChange}}) => (
                <DynamicSingleInput
                    name={name}
                    value={value}
                    onBlur={onBlur}
                    onChange={onChange}
                    type={field.type}
                    helperText={field?.helpMessage || ""}
                    error={Boolean(form.formState.errors[field.name])}
                    errorMessage={form.formState.errors[field.name]?.message}
                    label={`${field.name}${field.required ? " *" : ""}`}
                    placeholder={"Please put something in"}
                    fullWidth
                />
            )}
        />)
    } // Single Input
    return (<div>Error</div>);
}
