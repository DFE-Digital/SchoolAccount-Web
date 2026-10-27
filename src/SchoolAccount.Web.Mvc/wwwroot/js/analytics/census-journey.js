import {
    initialiseAnalytics,
    trackEvent,
    registerGovUkAnalytics
} from "./analytics.js";

const configElement = document.getElementById("census-analytics-config");

if (configElement) {
    const { connectionString } = JSON.parse(configElement.textContent);

    const appInsights = await initialiseAnalytics(connectionString);

    if (appInsights) {
        registerGovUkAnalytics(appInsights);

        trackEvent(appInsights, "census_journey_test", {
            page: "CensusJourney",
            action: "PageLoaded",
            source: "SAB-527"
        });

        console.log("SAB-527: Test event sent to Application Insights.");
    }
}