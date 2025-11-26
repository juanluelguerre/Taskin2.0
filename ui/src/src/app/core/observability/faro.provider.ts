import { Provider, isDevMode } from '@angular/core';
import { getWebInstrumentations, initializeFaro } from '@grafana/faro-web-sdk';
import { TracingInstrumentation } from '@grafana/faro-web-tracing';
import { environment } from '@env/environment';

/**
 * Initializes Grafana Faro for Real User Monitoring (RUM)
 *
 * Faro collects:
 * - Web Vitals (TTFB, FCP, LCP, CLS, FID)
 * - JavaScript exceptions and errors
 * - Custom events and measurements
 * - Performance traces
 *
 * All data is sent to Loki for storage and visualization in Grafana dashboards.
 */
export function provideFaro(): Provider[] {
  return [
    {
      provide: 'FARO_INITIALIZER',
      useFactory: () => {
        // Only initialize Faro if URL is configured
        if (!environment.faro?.url) {
          console.warn('[Faro] Not initialized: Missing Loki endpoint URL in environment configuration');
          return null;
        }

        try {
          const faro = initializeFaro({
            url: environment.faro.url,
            app: {
              name: environment.faro.app.name,
              version: environment.faro.app.version,
              environment: environment.faro.app.environment,
            },
            instrumentations: [
              // Load default web instrumentations (errors, performance, web vitals)
              ...getWebInstrumentations({
                captureConsole: isDevMode(), // Only capture console logs in development
              }),
              // Add distributed tracing support
              new TracingInstrumentation(),
            ],
            // Configure batching and transport
            batching: {
              enabled: true,
              sendTimeout: 5000, // Send batch every 5 seconds
              itemLimit: 50, // Or when 50 items are collected
            },
          });

          console.log('[Faro] Initialized successfully', {
            app: environment.faro.app.name,
            environment: environment.faro.app.environment,
            url: environment.faro.url,
          });

          return faro;
        } catch (error) {
          console.error('[Faro] Initialization failed', error);
          return null;
        }
      },
      multi: true,
    },
  ];
}
