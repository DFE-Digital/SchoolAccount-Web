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

        document.querySelectorAll(
            '[data-analytics-event="census_guidance_link"]'
        ).forEach(link => {
            link.addEventListener("click", () => {
                trackEvent(appInsights, "census_guidance_link", {
                    action: "GuidanceLinkClicked",
                    label: link.textContent.trim(),
                    destination: link.origin + link.pathname,
                    page: "CensusJourney"
                });
            });
        });        

        document.querySelectorAll(
            '[data-analytics-event="census_status_details"]'
        ).forEach(details => {
            details.addEventListener("toggle", () => {
                trackEvent(appInsights, "census_status_details", {
                    action: details.open ? "Expanded" : "Collapsed",
                    label: "What your status means",
                    page: "CensusJourney"
                });
            });
        });

        document.querySelectorAll(
            '[data-analytics-event="census_external_navigation"]'
        ).forEach(link => {
            link.addEventListener("click", () => {
                trackEvent(appInsights, "census_external_navigation", {
                    action: "ExternalNavigation",
                    label: link.textContent.trim(),
                    destination: link.href,
                    page: "CensusJourney"
                });
            });
        });

        trackEvent(appInsights, "census_journey_test", {
            page: "CensusJourney",
            action: "PageLoaded",
            source: "SAB-527"
        });

        console.log("SAB-527: Test event sent to Application Insights.");
    }
}