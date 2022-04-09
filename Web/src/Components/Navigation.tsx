import React from "react";
import {Link} from "react-router-dom";
import {Divider, List, ListItem, ListItemIcon, ListItemText, Typography} from "@mui/material";
import MailIcon from "@mui/icons-material/Mail";
import SourceIcon from '@mui/icons-material/Source';
import TerminalIcon from '@mui/icons-material/Terminal';
import WorkIcon from '@mui/icons-material/Work';
enum NavigationType {
    LINK = "LINK",
    LIST = "LIST",
    DIVIDER = "DIVIDER",
}

type NavigationItem = {
    type: NavigationType,
    label?: string,
    link?: string,
    icon?: any,
    list?: NavigationItem[],
}

let structure: NavigationItem[] = [{
    type: NavigationType.LIST, list: [
        {type: NavigationType.LINK, label: "Scripts", link: "/scripts", icon: <SourceIcon/>},
        {type: NavigationType.LINK, label: "Jobs", link: "/jobs", icon: <WorkIcon/>},
        {type: NavigationType.DIVIDER},
        {
            type: NavigationType.LIST, label: "Settings", link: "/settings", icon: <MailIcon/>, list: [
                {type: NavigationType.LINK, label: "General", link: "/settings/general", icon: <MailIcon/>},
                {type: NavigationType.DIVIDER},
            ]
        },
    ]
},
]

export default function Navigation() {

    const CreateList = (item: NavigationItem, index: number) => {
        switch (item.type) {
            case NavigationType.LIST:
                return (
                    <List key={index}>
                        {item.label && <Typography align="left">{item.label}</Typography>}
                        {item.list ? item.list.map(CreateList) : ""}
                    </List>
                )
            case NavigationType.LINK:
                return (
                    // @ts-ignore
                    <ListItem component={Link} key={index} to={item.link}>
                        <ListItemIcon>{item.icon}</ListItemIcon>
                        <ListItemText primary={item.label}/>
                    </ListItem>
                )
            case NavigationType.DIVIDER:
                return <Divider key={index}/>
        }
    };
    const items = structure.map(CreateList);
    return (
        <List>
            {items}
        </List>
    );
}