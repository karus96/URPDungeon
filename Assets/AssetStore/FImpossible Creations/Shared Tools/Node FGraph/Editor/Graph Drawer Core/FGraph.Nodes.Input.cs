using UnityEditor;
using UnityEngine;

namespace FIMSpace.Graph
{
    public abstract partial class FGraphDrawerBase
    {

        Event nodeInputToUseAfterDraw = null;
        void HandleInputsForNode(FGraph_NodeBase node)
        {
            // Checking selection state which prevents some input events on the node
            bool isConnectingOrSelectingMulti = isConnectingNodes;
            if (!isConnectingOrSelectingMulti) isConnectingOrSelectingMulti = isSelectingMultiple;

            Event e = Event.current;

            bool selectedAndDragging = false;
            if (wasDraggingNode) if (lastestDraggedNode == node) selectedAndDragging = true;


            // Detecting if mouse entered on the node
            if (enteredOnNode is null)
                if (node._E_LatestRect.Contains(inGraphMousePos))
                {
                    enteredOnNode = node;
                    if (e.type == EventType.MouseUp)
                    {
                        NodeInput_MouseUp(e, node);
                    }
                }


            if (lastestDraggedNode != node || selectedAndDragging)
            {
                // If cursor is outside canvas, don't update node input
                if (!isMouseCursorInGraph) return;

                // If node is not selected + If cursor is outside node rect then don't update its input
                if (selectedNode != node) if (node._E_LatestRect.Contains(inGraphMousePos) == false) if (!selectedAndDragging) return;

                nodeInputToUseAfterDraw = null;

                if (e.type == EventType.MouseUp)
                {
                    NodeInput_MouseUnclickUp(e, node);
                }
                else if (e.type == EventType.MouseDrag)
                {

                    if (!isConnectingOrSelectingMulti)
                        if (e.button == 0)
                            NodeInput_MouseDrag(e, node);
                }
                else if (e.type == EventType.MouseDown)
                {
                    if (e.button == 0) NodeInput_MouseDown(e, node);
                }
            }


            if (e.button == 0)
            {
                if (selectedAndDragging)
                {
                    GUI.DragWindow();
                }
                else
                {

                    if (isConnectingOrSelectingMulti == false)
                    {
                        if (canDragNode)
                        {
                            if (isMouseCursorInGraph)
                            {
                                if (e.type == EventType.MouseDrag)
                                {
                                    wasDraggingNode = true;
                                    lastestDraggedNode = node;
                                }

                                GUI.DragWindow();
                            }
                        }

                        if (nodeInputToUseAfterDraw != null) nodeInputToUseAfterDraw.Use();
                    }
                }

            }

        }

    }
}