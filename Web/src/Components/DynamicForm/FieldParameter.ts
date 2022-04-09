import {SupportedPrimitives} from "./DynamicForm";
import {UseFormReturn} from "react-hook-form";

export enum InputType {
    STRING,
    INTEGER,
    DECIMAL,
    BOOLEAN,
    UNSIGNEDINTEGER,
    DATETIME,
    PHONE,
    PASSWORD
}

export interface FieldParameter {
    name: string;
    label: string;
    type: InputType;
    isArray: boolean;
    defaultValue: number | string | boolean | Date | string[] | number[] | Date[],
    selectableValues?: string[];
    minValue?: number;
    minValueErrorMessage?: string
    maxValue?: number;
    maxValueErrorMessage?: string;
    minLength?: number;
    minLengthErrorMessage?: string;
    maxLength?: number;
    maxLengthErrorMessage?: string;
    pattern?: string;
    patternErrorMessage?: string;
    minArrayCount?: number;
    minArrayCountErrorMessage?: string
    maxArrayCount?: number;
    maxArrayCountErrorMessage?: string
    minSelectCount?: number;
    minSelectCountErrorMessage?: string;
    maxSelectCount?: number;
    maxSelectCountErrorMessage?: string;
    required: boolean;
    requiredErrorMessage?: string
    helpMessage?: string;
}

export interface DynamicBaseInputProps {
    name: string;
    label: string;
    value: SupportedPrimitives;
    error: boolean;
    placeholder: string;
    helperText: string;
    errorMessage: string;
    onBlur: (event: any) => void
    onChange: (event: any) => void
    fullWidth?: boolean;
    width100?: boolean;
}

export interface DynamicInputFieldProps {
    field: FieldParameter;
    form: UseFormReturn;
}

export interface FieldResultNumber extends FieldParameter {
    value: any
}

export interface WithErrorMessage {
    value: number | boolean | string;
    message: string;
}

export interface Rule {
    min?: number | WithErrorMessage;
    max?: number | WithErrorMessage;
    required?: boolean | WithErrorMessage;
    minLength?: number | WithErrorMessage;
    maxLength?: number | WithErrorMessage;
    pattern?: string | WithErrorMessage;
}