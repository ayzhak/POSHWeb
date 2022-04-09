import './StatusDot.css'
import {useState} from "react";
import {Popover, Typography} from "@mui/material";

export enum StatusDotType {
    Green = 'green',
    Yellow = 'yellow',
    Red = 'red',
    Orange = 'orange',
}

type StatusDotProps = {
    status: StatusDotType
    popoverText: string
}

export const StatusDot: React.FC<StatusDotProps> = ({children, status, popoverText}) => {
    const [anchorEl, setAnchorEl] = useState(null);

    const handleClick = (event: any) => {
        setAnchorEl(event?.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const open = Boolean(anchorEl);
    const id = open ? 'status-popover' : undefined;

    return (
        <>
            <div className={`status-dot status-dot--${status}`} onClick={handleClick}>
            </div>
            <Popover
                id={id}
                open={open}
                anchorEl={anchorEl}
                onClose={handleClose}
                anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'left',
                }}
            >
                <Typography sx={{p: 2}}>{popoverText}</Typography>
            </Popover>
        </>

    )
}