import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { ProcessDesigner } from './components/ProcessDesigner';
import { ProcessDefinitionList } from './pages/ProcessDefinitionList';
import { VersionHistory } from './pages/VersionHistory';
import { ProcessInstanceList } from './pages/ProcessInstanceList';
import { FileUploadDemo } from './pages/FileUploadDemo';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* 带布局的路由 */}
        <Route path="/" element={<Layout />}>
          <Route index element={<ProcessDefinitionList />} />
          <Route path="instances" element={<ProcessInstanceList />} />
          <Route path="versions/:key" element={<VersionHistory />} />
          <Route path="file-upload" element={<FileUploadDemo />} />
        </Route>
        
        {/* 全屏流程设计器（无布局） */}
        <Route path="designer" element={<ProcessDesigner mode="create" />} />
        <Route path="designer/:id" element={<ProcessDesigner mode="edit" />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
