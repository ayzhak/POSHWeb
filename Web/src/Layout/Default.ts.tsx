import React, {PropsWithChildren} from "react";
import Sidebar from "../Components/Sidebar/Sidebar";
import {alpha, AppBar, Box, Breadcrumbs, IconButton, InputBase, styled, Toolbar, Typography} from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import useReactRouterBreadcrumbs from "use-react-router-breadcrumbs";
import {NavLink} from "react-router-dom";
import {StatusDot, StatusDotType} from "../Components/Status/StatusDot";
import {useRecoilValue} from "recoil";
import {ConnectionState, signalRConnectionState} from "../State/atoms";
import SearchIcon from '@mui/icons-material/Search';

let statusMapping = {
    [ConnectionState.CONNECTED]: StatusDotType.Green,
    [ConnectionState.CONNECTING]: StatusDotType.Yellow,
    [ConnectionState.DISCONNECTED]: StatusDotType.Red
};

const Search = styled('div')(({theme}) => ({
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: alpha(theme.palette.common.white, 0.15),
    '&:hover': {
        backgroundColor: alpha(theme.palette.common.white, 0.25),
    },
    marginRight: theme.spacing(2),
    marginLeft: 0,
    width: '100%',
    [theme.breakpoints.up('sm')]: {
        marginLeft: theme.spacing(3),
        width: 'auto',
    },
}));

const SearchIconWrapper = styled('div')(({theme}) => ({
    padding: theme.spacing(0, 2),
    height: '100%',
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
}));

const StyledInputBase = styled(InputBase)(({theme}) => ({
    color: 'inherit',
    '& .MuiInputBase-input': {
        padding: theme.spacing(1, 1, 1, 0),
        // vertical padding + font size from searchIcon
        paddingLeft: `calc(1em + ${theme.spacing(4)})`,
        transition: theme.transitions.create('width'),
        width: '100%',
        [theme.breakpoints.up('md')]: {
            width: '20ch',
        },
    },
}));

export default function Layout({children}: PropsWithChildren<{}>) {
    const signalRConnectionStatus = useRecoilValue(signalRConnectionState);
    const breadcrumbs = useReactRouterBreadcrumbs();
    return (
        <>
            <Box sx={{display: 'flex'}}>
                <AppBar position="fixed" sx={{zIndex: (theme) => theme.zIndex.drawer + 1}}>
                    <Toolbar variant="dense">
                        <IconButton edge="start" color="inherit" aria-label="menu" sx={{mr: 2}}>
                            <MenuIcon/>
                        </IconButton>
                        <Typography variant="h6" color="inherit" component="div">
                            POSHWeb
                        </Typography>

                        <Search>
                            <SearchIconWrapper>
                                <SearchIcon/>
                            </SearchIconWrapper>
                            <StyledInputBase
                                placeholder="Search…"
                                inputProps={{'aria-label': 'search'}}
                            />
                        </Search>
                        <Box sx={{flexGrow: 1}}/>
                        <Box sx={{display: {xs: 'none', md: 'flex'}}}>
                            <StatusDot status={statusMapping[signalRConnectionStatus]} popoverText="Test"/>
                        </Box>
                    </Toolbar>
                </AppBar>
                <Sidebar/>
                <Box component="main" sx={{flexGrow: 1, p: 3}}>
                    <Toolbar/>
                    <Box display="flex" justifyContent="center" alignItems="center">
                        <Breadcrumbs aria-label="breadcrumb">
                            {breadcrumbs.map(({breadcrumb, match}, index) => (
                                <NavLink key={index} style={{textDecoration: "none"}}
                                         to={match.pathname}>{breadcrumb}</NavLink>
                            ))}
                        </Breadcrumbs>
                    </Box>
                    {children}
                </Box>
            </Box>
        </>
    );
}
