import React from "react";
import {Divider, Drawer, Toolbar} from "@mui/material";
import Navigation from "../Navigation";

const drawerWidth = 240;

export default function Sidebar() {
    return (
        <Drawer
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                '& .MuiDrawer-paper': {
                    width: drawerWidth,
                    boxSizing: 'border-box',
                },
            }}
            variant="permanent"
            anchor="left"
        >
            <Toolbar/>
            <Divider/>
            <Navigation/>
        </Drawer>
    );
}
