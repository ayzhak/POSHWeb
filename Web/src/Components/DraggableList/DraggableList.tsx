import * as React from 'react';
import {PropsWithChildren, ReactNode} from 'react';
import {DragDropContext, Draggable, Droppable, OnDragEndResponder} from 'react-beautiful-dnd';
import {ListItem} from "@mui/material";

export type Item = {
    id: string;
};

export type DraggableListItemProps = {
    item: Item;
    index: number;
};

const DraggableListItem = ({item, index, children}: PropsWithChildren<DraggableListItemProps>) => {
    return (
        <Draggable draggableId={item.id} index={index}>
            {(provided, snapshot) => (
                <ListItem
                    ref={provided.innerRef}
                    {...provided.draggableProps}
                    {...provided.dragHandleProps}
                    style={snapshot.isDragging ? {background: 'rgb(235,235,235)'} : {}}
                >
                    {children}
                </ListItem>
            )}
        </Draggable>
    );
};

export type DraggableListProps = {
    items: Item[];
    onDragEnd: OnDragEndResponder;
    component: ReactNode;
};

const DraggableList = React.memo(({items, onDragEnd, component}: DraggableListProps) => {
    return (
        <DragDropContext onDragEnd={onDragEnd}>
            <Droppable droppableId="droppable-list">
                {provided => (
                    <div ref={provided.innerRef} {...provided.droppableProps}>
                        {items.map((item, index) => (
                            <DraggableListItem item={item} index={index} key={item.id}>
                                {component}
                            </DraggableListItem>
                        ))}
                        {provided.placeholder}
                    </div>
                )}
            </Droppable>
        </DragDropContext>
    );
});

export default DraggableList;
