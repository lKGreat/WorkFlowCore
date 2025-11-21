import React, { useCallback, useState } from 'react';
import {
  ReactFlow,
  MiniMap,
  Controls,
  Background,
  useNodesState,
  useEdgesState,
  addEdge,
  BackgroundVariant,
  type Edge,
  type Node,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { Button, Space, message } from 'antd';

const initialNodes: Node[] = [
  {
    id: 'start',
    type: 'input',
    data: { label: '开始' },
    position: { x: 250, y: 25 },
  },
];

const initialEdges: Edge[] = [];

export const ProcessDesigner: React.FC = () => {
  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
  const [nodeId, setNodeId] = useState(2);

  const onConnect = useCallback(
    (params: Connection) => setEdges((eds) => addEdge(params, eds)),
    [setEdges]
  );

  // 添加审批节点
  const addApprovalNode = () => {
    const newNode: Node = {
      id: `approval-${nodeId}`,
      type: 'default',
      data: { label: `审批节点 ${nodeId}` },
      position: { x: Math.random() * 400, y: Math.random() * 400 },
    };
    setNodes((nds) => nds.concat(newNode));
    setNodeId((id) => id + 1);
  };

  // 添加条件节点
  const addConditionNode = () => {
    const newNode: Node = {
      id: `condition-${nodeId}`,
      type: 'default',
      data: { label: `条件分支 ${nodeId}` },
      position: { x: Math.random() * 400, y: Math.random() * 400 },
      style: { backgroundColor: '#ffe7ba' },
    };
    setNodes((nds) => nds.concat(newNode));
    setNodeId((id) => id + 1);
  };

  // 添加结束节点
  const addEndNode = () => {
    const newNode: Node = {
      id: `end-${nodeId}`,
      type: 'output',
      data: { label: '结束' },
      position: { x: Math.random() * 400, y: Math.random() * 400 },
    };
    setNodes((nds) => nds.concat(newNode));
    setNodeId((id) => id + 1);
  };

  // 保存流程定义
  const saveProcess = async () => {
    const processData = {
      nodes,
      edges,
    };
    
    console.log('保存流程定义:', processData);
    message.success('流程已保存');
    
    // TODO: 调用后端 API 保存
  };

  // 清空画布
  const clearCanvas = () => {
    setNodes(initialNodes);
    setEdges(initialEdges);
    setNodeId(2);
    message.info('画布已清空');
  };

  return (
    <div style={{ width: '100vw', height: '100vh', display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '16px', backgroundColor: '#f0f0f0', borderBottom: '1px solid #d9d9d9' }}>
        <Space>
          <Button type="primary" onClick={addApprovalNode}>
            添加审批节点
          </Button>
          <Button onClick={addConditionNode}>添加条件节点</Button>
          <Button onClick={addEndNode}>添加结束节点</Button>
          <Button type="primary" onClick={saveProcess} style={{ marginLeft: '16px' }}>
            保存流程
          </Button>
          <Button danger onClick={clearCanvas}>
            清空
          </Button>
        </Space>
      </div>
      <div style={{ flexGrow: 1 }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          fitView
        >
          <Controls />
          <MiniMap />
          <Background variant={BackgroundVariant.Dots} gap={12} size={1} />
        </ReactFlow>
      </div>
    </div>
  );
};

