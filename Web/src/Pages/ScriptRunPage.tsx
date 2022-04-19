import React, {useEffect} from "react";
import {Card, Typography} from "@mui/material";
import {useNavigate, useParams} from "react-router-dom";
import {useRecoilState, useRecoilValue} from "recoil";
import {scriptQuery, scriptRunState, scriptsJobsState} from "../State/atoms";
import {DynamicForm} from "../Components/DynamicForm/DynamicForm";
import {FieldParameter, InputType} from "../Components/DynamicForm/FieldParameter";
import {isBoolean, isNumber} from "util";
import {LoadingTea} from "../Components/Loading/LoadingTea";
import {InputParameter, PSParameter, ScriptApi} from "../apiClient";

function ValueConverter(type: InputType, isArray: boolean, value: any): any {
    if (isArray) {
        return []
    }
    if (type === InputType.INTEGER) return Number(value);
    if (type === InputType.DECIMAL) return Number(value);
    if (type === InputType.UNSIGNEDINTEGER) return Number(value);
    if (type === InputType.STRING) return String(value);
    if (type === InputType.BOOLEAN) return Boolean(value);
    if (type === InputType.DATETIME) return Date.parse(value) || "";
}

function TypeConverter(type: string, name: string): InputType {
    let TypeMapping: {[key: string]: InputType} = {
        "String": InputType.STRING,
        "Int32": InputType.INTEGER,
        "Boolean": InputType.BOOLEAN,
        "SwitchParameter": InputType.BOOLEAN,
        "UInt32": InputType.UNSIGNEDINTEGER,
        "Double": InputType.INTEGER,
        "DateTime": InputType.DATETIME
    }
    let evaluatedType = TypeMapping[type.replace("[]", "")]
    if(evaluatedType === InputType.STRING && name.toLowerCase().startsWith("phone")) return InputType.PHONE
    if(evaluatedType === InputType.STRING && name.toLowerCase().startsWith("password")) return InputType.PASSWORD
    return evaluatedType
}

function ParameterToFieldsConverter(parameters: Array<PSParameter> | null | undefined): Array<FieldParameter> {
    if (parameters === null || parameters === undefined) {
        return [];
    }

    return parameters.map(parameter => {
        // @ts-ignore
        const type = TypeConverter(parameter.type, parameter.name)
        return {
            name: parameter.name || "",
            type: type,
            defaultValue: ValueConverter(type, parameter.type?.endsWith("[]") || false, parameter.default),
            label: parameter.name || "",
            isArray: parameter.type?.endsWith("[]") || false,
            selectableValues: parameter.options?.validValues || undefined,
            helpMessage: parameter.helpMessage || undefined,
            required: parameter.mandatory || false,
            requiredErrorMessage: "This is required",
            minLength: parameter.options?.minLength || undefined,
            minLengthErrorMessage: `This field has a minimum of ${parameter.options?.minLength || undefined} letters`,
            maxLength: parameter.options?.maxLength || undefined,
            maxLengthErrorMessage: `This field has a maximum of ${parameter.options?.maxLength || undefined} letters`,
            minValue: parameter.options?.minValue || undefined,
            minValueErrorMessage: `This field has a minimum of ${parameter.options?.minValue || undefined}`,
            maxValue: parameter.options?.maxValue || undefined,
            maxValueErrorMessage: `This field has a maximum of ${parameter.options?.maxValue || undefined}`,
            pattern: parameter.options?.regexString|| undefined,
            patternErrorMessage: `Must be in the pattern of ${parameter.options?.regexString || undefined}`,
        }
    });
}

export default function ScriptRunPage() {
    const params = useParams<{ id: string }>();
    let navigate = useNavigate();
    const [script, setScript] = useRecoilState(scriptRunState)
    const [jobs, setScripJobs] = useRecoilState(scriptsJobsState)
    const queryScript = useRecoilValue(scriptQuery(params.id))
    useEffect(() => {
        setScript(queryScript)
    }, [])

    async function hanldeSubmit(values: object) {
        let request: InputParameter[] = []
        for (const [key, value] of Object.entries(values)) {
            if(!value || value == "") continue;
            let value2 = value;
            if(Array.isArray(value)) value2 = value.join(",")
            if(isBoolean(value2)) value2 = String(value2)
            if(isNumber(value)) value2 = String(value)
            request.push({name: key, value: value2})
        }
        if(!script?.id) return;
        const scriptsApi = new ScriptApi();
        const response = await scriptsApi.apiV1ScriptIdRunPost(script.id, request)
        setScript(null)
        navigate(`/jobs/${response.data.id}`)
    }
    return (
        <>
            <Typography variant="h2" noWrap>
                Scripts Run
            </Typography>
            <Card style={{padding: "10px"}}>
                {script && <DynamicForm fields={ParameterToFieldsConverter(script.parameters)} onSubmit={hanldeSubmit}/> || <LoadingTea/>}
            </Card>
        </>
    );
}
