import { useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';

/**
 * InnerLink - 内链组件
 * 用于在iframe中显示外部链接
 */
export default function InnerLink() {
  const location = useLocation();
  const [url, setUrl] = useState('');

  useEffect(() => {
    // 从路由state中获取URL
    const state = location.state as { link?: string };
    if (state?.link) {
      setUrl(state.link);
    }
  }, [location]);

  if (!url) {
    return <div style={{ padding: '20px' }}>无效的链接</div>;
  }

  return (
    <iframe
      src={url}
      style={{
        width: '100%',
        height: 'calc(100vh - 64px)',
        border: 'none'
      }}
      title="内链页面"
    />
  );
}

