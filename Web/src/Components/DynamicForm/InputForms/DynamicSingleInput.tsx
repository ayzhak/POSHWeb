import {DynamicBaseInputProps, InputType} from "../FieldParameter";
import {DynamicInputText} from "./Inputs/DynamicInputText";
import {DynamicNumberInput} from "./Inputs/DynamicNumberInput";
import {DynamicSwitchInput} from "./Inputs/DynamicSwitchInput";
import {DynamicUnsignedNumberInput} from "./Inputs/DynamicUnsignedNumberInput";
import {DynamicDateTime} from "./Inputs/DynamicDateTime";
import {DynamicPhoneNumber} from "./Inputs/DynamicPhoneNumber";
import {DynamicPassword} from "./Inputs/DynamicPassword";

export function DynamicSingleInput(props: DynamicBaseInputProps & { type: InputType }): JSX.Element {
    switch (props.type) {
        case InputType.STRING:
            return (
                <DynamicInputText {...props}/>
            );
        case InputType.INTEGER:
            return (
                <DynamicNumberInput {...props}/>
            )
        case InputType.BOOLEAN:
            return (
                <DynamicSwitchInput {...props}/>
            )
        case InputType.UNSIGNEDINTEGER:
            return (
                <DynamicUnsignedNumberInput {...props}/>
            )
        case InputType.DATETIME:
            return (
                <DynamicDateTime {...props}/>
            )
        case InputType.PHONE:
            return (
                <DynamicPhoneNumber {...props}/>
            )
        case InputType.PASSWORD:
            return (
                <DynamicPassword {...props}/>
            )
    }

    return (<div>Unknown input type</div>);
}