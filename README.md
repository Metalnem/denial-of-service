# Denial of service attack demo using Azure Functions

This repo contains an Azure Functions app vulnerable to `StackOverflowException` and a malicious client
app that can perform denial of service attack against it by flooding it with highly nested JSON payloads.
Read my blog post to learn more:
[How StackOverflowException can bring down an expensive compute cluster](https://mijailovic.net/2023/04/01/denial-of-service/).
