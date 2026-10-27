const sdkUrl = "https://js.monitor.azure.com/scripts/b/ai.3.gbl.min.js";

export async function initialiseAnalytics(connectionString) {
    if (!connectionString) {
        console.warn("Application Insights connection string is missing.");
        return null;
    }

    try {
        await loadScript(sdkUrl);

        const appInsights = new window.Microsoft.ApplicationInsights.ApplicationInsights({
            config: {
                connectionString,
                enableAutoRouteTracking: false,
                disableExceptionTracking: true,
                disableAjaxTracking: true,
                disableFetchTracking: true
            }
        });

        appInsights.loadAppInsights();

        return appInsights;
    } catch (error) {
        console.error("Failed to initialise Application Insights.", error);
        return null;
    }
}

export function trackEvent(appInsights, name, properties = {}) {
    appInsights.trackEvent({
        name,
        properties
    });
}

export function registerGovUkAnalytics(appInsights) {
    window.GOVUK = window.GOVUK || {};
    window.GOVUK.analytics = window.GOVUK.analytics || {};

    window.GOVUK.analytics.trackEvent = (category, action, options = {}) => {
        trackEvent(appInsights, category, {
            action,
            ...options
        });
    };
}

function loadScript(src) {
    return new Promise((resolve, reject) => {
        const script = document.createElement("script");
        script.src = src;
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}