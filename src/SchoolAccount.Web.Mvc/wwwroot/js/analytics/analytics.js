
const sdkUrl = "https://js.monitor.azure.com/scripts/b/ai.3.gbl.min.js";

export async function initialiseAnalytics(mode, connectionString) {
    if (mode === "Console") {
        console.info("SAB-527: Console analytics enabled.");

        let organisationContext = {};

        return {
            trackEvent: ({ name, properties = {} }) => {
                console.log(`[Analytics] ${name}`, {
                    ...organisationContext,
                    ...properties
                });
            },

            setAuthenticatedUserContext: (userId) => {
                organisationContext.authenticatedId = userId;
            },

            addTelemetryInitializer: (initializer) => {
                const envelope = {
                    data: {
                        baseData: {
                            properties: {}
                        }
                    }
                };

                initializer(envelope);

                organisationContext = {
                    ...organisationContext,
                    ...envelope.data.baseData.properties
                };
            }
        };
    }

    if (mode !== "ApplicationInsights") {
        return null;
    }

    if (!connectionString) {
        console.warn("Application Insights connection string is missing.");
        return null;
    }

    try {
        await loadScript(sdkUrl);

        const appInsights =
            new window.Microsoft.ApplicationInsights.ApplicationInsights({
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

export function setOrganisationContext(
    appInsights,
    organisationId,
    organisationType,
    analyticsUserId,
    organisationCategory
) {
    if (analyticsUserId) {
        appInsights.setAuthenticatedUserContext(
            analyticsUserId,
            undefined,
            false
        );
    } else {
        console.warn("SAB-527: Analytics user identifier unavailable.");
    }

    if (!organisationId || !organisationType) {
        console.warn("SAB-527: Organisation identifier unavailable.");
        return;
    }

    appInsights.addTelemetryInitializer(envelope => {
        envelope.data = envelope.data || {};
        envelope.data.baseData = envelope.data.baseData || {};
        envelope.data.baseData.properties =
            envelope.data.baseData.properties || {};

        Object.assign(envelope.data.baseData.properties, {
            organisationId,
            organisationType,
            organisationCategory
        });
    });

    console.log(
        `SAB-527: Organisation context configured (${organisationType}).`
    );
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
