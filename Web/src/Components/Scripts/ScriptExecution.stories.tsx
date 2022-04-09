import React from 'react';
import {ComponentMeta, ComponentStory} from '@storybook/react';
import ScriptExecution from "./ScriptExecution";
import AdapterDateFns from "@mui/lab/AdapterDateFns";
import {LocalizationProvider} from "@mui/lab";
import deLocale from "date-fns/locale/de";

export default {
    title: 'Scripts/Form',
    component: ScriptExecution
} as ComponentMeta<typeof ScriptExecution>;

const Template: ComponentStory<typeof ScriptExecution> = (args) => {
    return (
        <LocalizationProvider dateAdapter={AdapterDateFns} locale={deLocale}>
            <ScriptExecution/>
        </LocalizationProvider>
    )
}
export const Primary = Template.bind({});
Primary.args = {};