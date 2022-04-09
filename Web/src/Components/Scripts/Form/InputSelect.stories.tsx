import React from 'react';
import {ComponentMeta, ComponentStory} from '@storybook/react';
import {InputSelect} from "./InputSelect";

export default {
    title: 'Scripts/Form/InputSelect',
    component: InputSelect,
} as ComponentMeta<typeof InputSelect>;

const Template: ComponentStory<typeof InputSelect> = (args) => <InputSelect/>;
export const Primary = Template.bind({});
Primary.args = {};