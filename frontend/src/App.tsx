import { Suspense } from 'react';
import { BrowserRouter, useRoutes } from 'react-router-dom';
import { Spin } from 'antd';
import { ErrorBoundary } from './components/ErrorBoundary';
import { routes } from './router';
import './App.css';

// 全局加载占位符
const PageLoading = () => (
  <div style={{ 
    display: 'flex', 
    justifyContent: 'center', 
    alignItems: 'center', 
    height: '100vh' 
  }}>
    <Spin size="large" tip="加载中..." />
  </div>
);

function AppRoutes() {
  return useRoutes(routes);
}

function App() {
  return (
    <ErrorBoundary>
      <BrowserRouter>
        <Suspense fallback={<PageLoading />}>
          <AppRoutes />
        </Suspense>
      </BrowserRouter>
    </ErrorBoundary>
  );
}

export default App;
