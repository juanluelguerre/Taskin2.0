# Grafana Alloy - Frontend Observability

Grafana Alloy acts as a proxy between the browser (Faro SDK) and Loki, handling CORS and forwarding frontend telemetry.

## Architecture

```
Browser (Faro SDK) → Alloy (port 12345) → Loki (port 3100)
```

## Configuration

- **Config file**: `alloy-config.alloy`
- **Faro receiver port**: 12345 (exposed to browser)
- **HTTP API port**: 12347 (internal monitoring)

## CORS Settings

Allowed origins:
- `http://localhost:4200` (Angular dev server)
- `http://localhost:5000` (Aspire dashboard)

## What Faro Collects

1. **Web Vitals**: TTFB, FCP, LCP, CLS, FID
2. **JavaScript Exceptions**: Errors and stack traces
3. **Custom Events**: User interactions
4. **Performance Traces**: Navigation and resource timing

## Verification

Check Alloy is running:
```bash
curl http://localhost:12347/metrics
```

Check data in Loki:
```bash
curl -G "http://localhost:3100/loki/api/v1/query_range" \
  --data-urlencode 'query={Application="Taskin-UI"}' \
  --data-urlencode 'limit=10'
```

## Frontend Configuration

In `ui/src/src/environments/environment.ts`:
```typescript
faro: {
  url: 'http://localhost:12345/collect',
  app: {
    name: 'Taskin-UI',
    version: '1.0.0',
    environment: 'development'
  }
}
```

## Grafana Dashboard

Dashboard UID: `CiroMopVz`
- View at: http://localhost:3000/d/CiroMopVz
- Panels: Web Vitals, Exceptions, Visits, Popular Browsers
