import {FieldParameter, InputType, Rule} from "./FieldParameter";


function setRuleField<T>(value: T | undefined, errorMessage: string | undefined): { message: string; value: T } | undefined | T {
    if (!value) return undefined
    if (errorMessage) return {value: value, message: errorMessage}
    return value;
}

function setRuleFieldRegex(value: string | undefined, errorMessage: string | undefined): { message: string; value: RegExp } | undefined | RegExp {
    if (!value) return undefined
    if (errorMessage) return {value: new RegExp(value), message: errorMessage}
    return new RegExp(value);
}

export function factoryRules(field: FieldParameter): any {
    return {
        min: setRuleField(field.minValue, field.minValueErrorMessage),
        max: setRuleField(field.maxValue, field.maxValueErrorMessage),
        minLength: setRuleField(field.minLength, field.minLengthErrorMessage),
        maxLength: setRuleField(field.maxLength, field.maxLengthErrorMessage),
        pattern: setRuleFieldRegex(field.pattern, field.patternErrorMessage),
        required: setRuleField(field.required, field.requiredErrorMessage)
    } as Rule
}

export function getDefaultValue(type: InputType): any {
    if (type === InputType.INTEGER) return 0;
    if (type === InputType.DECIMAL) return 0;
    if (type === InputType.UNSIGNEDINTEGER) return 0;
    if (type === InputType.STRING) return "";
    if (type === InputType.BOOLEAN) return false;
    if (type === InputType.DATETIME) return new Date();
}