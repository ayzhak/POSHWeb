import React from 'react';
import {ComponentMeta, ComponentStory} from '@storybook/react';
import {NumberSlider} from "./NumberSlider";

export default {
    title: 'Scripts/Form/NumberSlider',
    component: NumberSlider,
} as ComponentMeta<typeof NumberSlider>;

const Template: ComponentStory<typeof NumberSlider> = (args) => <NumberSlider/>;
export const Primary = Template.bind({});
Primary.args = {};