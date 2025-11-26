export const environment = {
  production: true,
  baseUrl: '',
  useHash: false,
  apiUrl: 'https://your-production-api-url.com',
  faro: {
    url: 'https://your-loki-endpoint/loki/api/v1/push',
    app: {
      name: 'Taskin-UI',
      version: '1.0.0',
      environment: 'production'
    }
  }
};
