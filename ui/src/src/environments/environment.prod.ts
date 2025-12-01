export const environment = {
  production: true,
  baseUrl: '',
  useHash: false,
  apiUrl: 'https://your-production-api-url.com',
  faro: {
    // In production, use Grafana Cloud or your Alloy endpoint
    url: 'https://your-alloy-endpoint/collect',
    app: {
      name: 'Taskin-UI',
      version: '1.0.0',
      environment: 'production'
    }
  }
};
