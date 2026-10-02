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
* `Serilog.Sinks.OpenTelemetry` with the OpenTelemetry endpoints that Container Apps provides

## Decision Outcome

Chosen option: "The Azure Monitor OpenTelemetry distro, with Serilog forwarding to it", because it sends metrics to
Application Insights as well as logs and traces, which the managed agent can't, and it needs nothing extra to run.

How it works:

* When `APPLICATIONINSIGHTS_CONNECTION_STRING` is set, the distro sends logs, traces and metrics to Application
  Insights. When it isn't set nothing is registered, so local runs and the tests are unaffected.
* Serilog forwards its events to the OpenTelemetry logger provider, so logs line up with their traces. The default
  logging providers are cleared so the console isn't printed twice.
* When `OTEL_EXPORTER_OTLP_ENDPOINT` is set, which Rider does for runs from the IDE, logs, traces and metrics also go to
  that endpoint over OTLP.
* The role name and instance come from the distro's Container Apps detector (the container app name and the replica),
  not from our code.
* Serilog's request logging is off. Application Insights already records the requests.

### Consequences

* Good, because logs, traces and metrics reach Application Insights from one in-process pipeline.
* Good, because Serilog stays as it is, and Rider shows all three signals locally.
* Bad, because forwarding Serilog through the logger provider is fiddly. The default providers have to be cleared, and
  with `OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf` every log is exported twice.
* Bad, because the distro's default sampling can drop a burst of requests and the logs that go with them.

### Confirmation

Unit tests cover when the distro and the OTLP export are registered. The first deployment should show telemetry in
Application Insights with the container app name as the role and the replica as the instance.

## Pros and Cons of the Options

### The Azure Monitor OpenTelemetry distro, with Serilog forwarding to it

* Good, because it sends metrics as well as logs and traces.
* Good, because it needs no extra infrastructure.
* Good, because it can authenticate with Entra if local authentication is turned off.
* Bad, because of the Serilog forwarding quirks above.

### `Serilog.Sinks.OpenTelemetry` with the OpenTelemetry endpoints that Container Apps provides

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

Worth revisiting if the managed agent starts sending metrics to Application Insights, or if the platform team wants us
on the agent. In that case the distro would be replaced by plain OTLP and `Serilog.Sinks.OpenTelemetry`.

The metrics limitation is listed under known limitations in
[Collect and read OpenTelemetry data in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents).