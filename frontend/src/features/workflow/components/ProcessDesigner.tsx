import React, { useCallback, useState, useEffect } from 'react';
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
  type Connection,
} from '@xyflow/react';
import '@xyflow/react/dist/style.css';
import { Button, Space, message, Modal, Form, Input } from 'antd';
import { useNavigate, useParams } from 'react-router-dom';
import { processDefinitionService } from '../services/processDefinitionService';

const { TextArea } = Input;

const initialNodes: Node[] = [
  {
    id: 'start',
    type: 'input',
    data: { label: '开始' },
    position: { x: 250, y: 25 },
  },
];

const initialEdges: Edge[] = [];

type ProcessDesignerProps = {
  mode?: 'create' | 'edit';
};

export const ProcessDesigner: React.FC<ProcessDesignerProps> = ({ mode = 'create' }) => {
  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
  const [nodeId, setNodeId] = useState(2);
  const [processName, setProcessName] = useState('');
  const [processKey, setProcessKey] = useState('');
  const [processDescription, setProcessDescription] = useState('');
  const [processId, setProcessId] = useState<string | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [form] = Form.useForm();
  
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  // 加载流程定义
  useEffect(() => {
    if (id) {
      loadProcessDefinition(id);
    }
  }, [id]);

  const loadProcessDefinition = async (definitionId: string) => {
    try {
      const definition = await processDefinitionService.getProcessDefinition(definitionId);
      setProcessId(String(definition.id));
      setProcessName(definition.name);
      setProcessKey(definition.key);
      setProcessDescription(definition.description || '');
      
      // TODO: 解析流程内容（暂时跳过，因为 definition 类型没有 content/definition 字段）
      // if (definition.definition) {
      //   const flowData = JSON.parse(definition.definition);
      //   if (flowData.nodes && flowData.edges) {
      //     setNodes(flowData.nodes);
      //     setEdges(flowData.edges);
      //     
      //     // 更新nodeId以避免ID冲突
      //     const maxId = flowData.nodes.reduce((max: number, node: Node) => {
      //       const match = node.id.match(/\d+$/);
      //       if (match) {
      //         const num = parseInt(match[0]);
      //         return Math.max(max, num);
      //       }
      //       return max;
      //     }, 1);
      //     setNodeId(maxId + 1);
      //   }
      // }
      
      message.success('流程定义加载成功');
    } catch (err: unknown) {
      const error = err as { message?: string };
      message.error(error.message || '加载流程定义失败');
    }
  };

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

  // 显示保存对话框
  const showSaveDialog = () => {
    form.setFieldsValue({
      name: processName,
      key: processKey,
      description: processDescription,
    });
    setIsModalOpen(true);
  };

  // 保存流程定义
  const saveProcess = async (createNewVersion: boolean = false) => {
    try {
      await form.validateFields();
      const values = form.getFieldsValue();
      
      setIsSaving(true);
      
      const processData = {
        nodes,
        edges,
      };
      
      const content = JSON.stringify(processData);
      
      if (processId && mode === 'edit') {
        // 更新现有流程
        const result = await processDefinitionService.updateProcessDefinition(
          processId,
          {
            name: values.name,
            description: values.description,
            definition: content,
            isEnabled: true,
          }
        );
        
        setProcessId(String(result.id));
        setProcessName(result.name);
        setProcessKey(result.key);
        setProcessDescription(result.description || '');
        
        message.success(createNewVersion ? '新版本创建成功' : '流程已更新');
      } else {
        // 创建新流程
        const result = await processDefinitionService.createProcessDefinition({
          name: values.name,
          key: values.key,
          description: values.description,
          definition: content,
          isEnabled: true,
        });
        
        setProcessId(String(result.id));
        setProcessName(result.name);
        setProcessKey(result.key);
        setProcessDescription(result.description || '');
        
        message.success('流程已创建');
        
        // 切换到编辑模式
        navigate(`/designer/${result.id}`, { replace: true });
      }
      
      setIsModalOpen(false);
    } catch (err: unknown) {
      const error = err as { message?: string };
      message.error(error.message || '保存失败');
    } finally {
      setIsSaving(false);
    }
  };

  // 清空画布
  const clearCanvas = () => {
    Modal.confirm({
      title: '确认清空',
      content: '确定要清空画布吗？此操作不可恢复。',
      onOk: () => {
        setNodes(initialNodes);
        setEdges(initialEdges);
        setNodeId(2);
        message.info('画布已清空');
      },
    });
  };

  return (
    <div style={{ width: '100%', height: '100vh', display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '16px', backgroundColor: '#f0f0f0', borderBottom: '1px solid #d9d9d9' }}>
        <Space>
          <Button type="primary" onClick={addApprovalNode}>
            添加审批节点
          </Button>
          <Button onClick={addConditionNode}>添加条件节点</Button>
          <Button onClick={addEndNode}>添加结束节点</Button>
          <Button type="primary" onClick={showSaveDialog} style={{ marginLeft: '16px' }}>
            保存流程
          </Button>
          {processId && (
            <Button onClick={() => saveProcess(true)}>
              保存为新版本
            </Button>
          )}
          <Button danger onClick={clearCanvas}>
            清空
          </Button>
          <Button onClick={() => navigate('/')}>
            返回列表
          </Button>
        </Space>
        {processName && (
          <div style={{ marginTop: '8px', fontSize: '14px', color: '#666' }}>
            当前流程: <strong>{processName}</strong> ({processKey})
          </div>
        )}
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

      <Modal
        title={processId ? '保存流程' : '创建流程'}
        open={isModalOpen}
        onOk={() => saveProcess(false)}
        onCancel={() => setIsModalOpen(false)}
        confirmLoading={isSaving}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            name="name"
            label="流程名称"
            rules={[{ required: true, message: '请输入流程名称' }]}
          >
            <Input placeholder="请输入流程名称" />
          </Form.Item>
          <Form.Item
            name="key"
            label="流程Key"
            rules={[{ required: true, message: '请输入流程Key' }]}
          >
            <Input placeholder="请输入流程Key（英文标识）" disabled={!!processId} />
          </Form.Item>
          <Form.Item name="description" label="流程描述">
            <TextArea rows={4} placeholder="请输入流程描述" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
