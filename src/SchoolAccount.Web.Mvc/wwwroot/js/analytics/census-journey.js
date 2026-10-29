import {
    initialiseAnalytics,
    trackEvent,
    registerGovUkAnalytics,
    setOrganisationContext
} from "./analytics.js";

const configElement = document.getElementById("census-analytics-config");

if (configElement) {
    const { connectionString } = JSON.parse(configElement.textContent);
    
    const appInsights = await initialiseAnalytics(connectionString);

    if (appInsights) {
        // Organisation identification (LAESTAB or UKPRN)
        const organisationElement = document.getElementById(
            "census-organisation-config"
        );
        
        if (organisationElement) {
            const { organisationId, organisationType } =
                JSON.parse(organisationElement.textContent);

            setOrganisationContext(
                appInsights,
                organisationId,
                organisationType
            );
        }
        
        // Forward existing GOV.UK Step-by-step events
        registerGovUkAnalytics(appInsights);

        // Track ordinary hyperlinks within Step-by-step content.
        // Exclude .js-link elements already handled by the GOV.UK component.
        document.querySelectorAll(
            '.app-step-nav .js-panel a[href]:not(.js-link)'
        ).forEach(link => {
            link.addEventListener("click", () => {
                trackEvent(appInsights, "census_step_navigation_link", {
                    action: "StepLinkClicked",
                    label: link.textContent.trim(),
                    destination: link.origin + link.pathname,
                    page: "CensusJourney"
                });
            });
        });

        // Census Journey page visit
        trackEvent(appInsights, "census_journey_page_view", {
            page: "CensusJourney",
            action: "PageViewed",
            source: "SAB-527"
        });

        // Scroll depth tracking
        const scrollMilestones = [25, 50, 75, 100];
        const trackedMilestones = new Set();
        
        function trackScrollDepth() {
            const documentHeight = document.documentElement.scrollHeight;
            const viewportHeight = window.innerHeight;
            const scrollableHeight = documentHeight - viewportHeight;

            if (scrollableHeight <= 0) {
                return;
            }

            const scrollPercentage = Math.min(
                100,
                Math.round((window.scrollY / scrollableHeight) * 100)
            );

            scrollMilestones.forEach(milestone => {
                if (
                    scrollPercentage >= milestone &&
                    !trackedMilestones.has(milestone)
                ) {
                    trackedMilestones.add(milestone);

                    trackEvent(appInsights, "census_scroll_depth", {
                        action: "ScrollDepthReached",
                        depth: milestone,
                        page: "CensusJourney"
                    });
                }
            });
        }

        window.addEventListener("scroll", trackScrollDepth, {
            passive: true
        });

        // Guidance hyperlink tracking
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

        // Status descriptions expand/collapse tracking
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

        // Go to Collect external navigation tracking
        document.querySelectorAll(
            '[data-analytics-event="census_external_navigation"]'
        ).forEach(link => {
            link.addEventListener("click", () => {
                trackEvent(appInsights, "census_external_navigation", {
                    action: "ExternalNavigation",
                    label: link.textContent.trim(),
                    destination: link.origin + link.pathname,
                    page: "CensusJourney"
                });
            });
        });

        console.log("SAB-527: Census Journey analytics initialised.");
    }
}
