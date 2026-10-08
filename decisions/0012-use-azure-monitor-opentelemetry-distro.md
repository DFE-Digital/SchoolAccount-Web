---
status: "proposed"
date: 2026-10-02
decision-makers: Paul Custance
---

# Use the Azure Monitor OpenTelemetry distro for telemetry

## Context and Problem Statement

We want logs, traces and metrics from the web app in Azure Monitor, and we already log through Serilog. How do we get
telemetry into Application Insights without replacing Serilog, and how do we see the same telemetry locally in Rider?

## Decision Drivers

* Keep Serilog as the logging API, along with what we already write to the console and Seq.
* Get metrics into Application Insights, not just logs and traces.
* No extra infrastructure to run.
* See the same signals in Rider's OpenTelemetry window while developing.

## Considered Options

* The Azure Monitor OpenTelemetry distro, with Serilog forwarding to it
* Plain OTLP straight to Application Insights' OTLP ingestion
* `Serilog.Sinks.OpenTelemetry` with the OpenTelemetry endpoints that the Container Apps managed agent provides

## Decision Outcome

Chosen option: "The Azure Monitor OpenTelemetry distro, with Serilog forwarding to it", because it sends metrics to
Application Insights as well as logs and traces, which the managed agent can't, it needs nothing extra to run, and it
works with our existing Application Insights resources and their connection strings, which OTLP ingestion doesn't.

How it works:

* Telemetry goes to one destination, never both.
* When `APPLICATIONINSIGHTS_CONNECTION_STRING` is set, the distro sends logs, traces and metrics to Application
  Insights.
* When it isn't set but `OTEL_EXPORTER_OTLP_ENDPOINT` is, which Rider does for runs from the IDE, logs, traces and
  metrics go to that endpoint over OTLP instead.
* When both are set, Application Insights wins. The Container Apps agent can inject `OTEL_EXPORTER_OTLP_ENDPOINT` into a
  deployed app, and that mustn't move production telemetry away from Application Insights.
* When neither is set nothing is registered, so the tests are unaffected.
* Serilog forwards its events to the OpenTelemetry logger provider, so logs line up with their traces. The default
  logging providers are cleared so the console isn't printed twice.
* The role name and instance come from the distro's Container Apps detector (the container app name and the replica),
  not from our code.
* Serilog's request logging is on, so each request is also written as one summary log event, alongside the request
  Application Insights records.

### Consequences

* Good, because logs, traces and metrics reach Application Insights from one in-process pipeline.
* Good, because Serilog stays as it is, and Rider shows all three signals locally.
* Bad, because forwarding Serilog through the logger provider is fiddly. The default providers have to be cleared, and
  with `OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf` every log is exported twice.
* Bad, because to see telemetry in Rider the connection string has to be unset locally.
* Bad, because the distro's default sampling can drop a burst of requests and the logs that go with them.

### Confirmation

Unit tests cover which destination is registered, including that Application Insights wins when both are configured.
The first deployment should show telemetry in Application Insights with the container app name as the role and the
replica as the instance.

## Pros and Cons of the Options

### The Azure Monitor OpenTelemetry distro, with Serilog forwarding to it

* Good, because it sends metrics as well as logs and traces.
* Good, because it needs no extra infrastructure.
* Good, because it can authenticate with Entra if local authentication is turned off.
* Bad, because of the Serilog forwarding quirks above.

### Plain OTLP straight to Application Insights' OTLP ingestion

Application Insights can ingest OTLP for logs, traces and metrics when OTLP support is turned on for the resource. The
app would send OTLP to the resource's endpoint for each signal, authenticated with a Microsoft Entra token.

* Good, because it sends metrics as well as logs and traces.
* Good, because the app only needs plain OpenTelemetry packages, so local and deployed use the same exporter.
* Bad, because our Application Insights resources would need OTLP support turned on, and the container app's identity
  would need the Monitoring Metrics Publisher role on the resource's data collection rule.
* Bad, because the endpoints only accept a Microsoft Entra token. Microsoft documents this path for an OpenTelemetry
  Collector, so an app exporting directly needs its own token handling.
* Bad, because metrics land in an Azure Monitor workspace (a Prometheus store), and logs and traces in Log Analytics
  using the OpenTelemetry schema, rather than in the classic Application Insights tables.
* Bad, because we'd lose what the distro adds, such as Live Metrics, profiling and sampling.

### `Serilog.Sinks.OpenTelemetry` with the OpenTelemetry endpoints that the Container Apps managed agent provides

Container Apps can run a managed OpenTelemetry agent in the environment. It injects the agent's OTLP endpoints into each
app (`OTEL_EXPORTER_OTLP_ENDPOINT`, plus a `CONTAINERAPP_OTEL_*_GRPC_ENDPOINT` variable for each signal). Serilog and the
OpenTelemetry SDK would send OTLP to those endpoints, and the agent would forward to Application Insights.

* Good, because the app only needs plain OTLP, so local and deployed use the same path.
* Bad, because the endpoints don't get metrics into Application Insights. The agent's Application Insights destination
  only accepts logs and traces, so metrics would need another destination such as Datadog or a separate OTLP backend.
* Bad, because it's a preview feature that runs as a single replica and drops data if the destination is down for more
  than about five minutes.
* Bad, because it can't be used if the Application Insights resource has local authentication turned off.

## More Information

We expect to move to OTLP ingestion in a follow-up, once our Application Insights resources have OTLP support turned on
and the container app's identity has the role it needs. That change would supersede this decision.

Worth revisiting if the managed agent starts sending metrics to Application Insights, or if the platform team wants us
on the agent. In that case the distro would be replaced by plain OTLP and `Serilog.Sinks.OpenTelemetry`.

The metrics limitation is listed under known limitations in
[Collect and read OpenTelemetry data in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents).