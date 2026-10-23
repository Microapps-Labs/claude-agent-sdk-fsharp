# Local capture

Captured on 23 October 2026 from base revision `63c21e66bc6856b3a91ff6d63825df6ec4e28312` with the documented fixes and examples in this commit.

NET SDK 8.0.426 on Linux ARM64. The SDK build initially failed on FS3511 with warnings treated as errors. Input enumeration was moved into a statically compiled task without disabling the compiler check. A local subprocess fixture then exposed dropped buffered stdout after child exit; both transport readers now drain to EOF. Two regression cases force a child to exit before reading. All 102 tests passed, and BasicUsage compiled with one existing resumable-code warning. The offline example invokes a local handler and validates two serialized inputs and one response from a fake CLI. No Claude CLI, account or model was used. No complete upstream parity or cross-platform qualification is claimed. The PNG is actual stdout rendered as a transcript.

Commands from the repository root after installing its documented dependencies:

```sh
dotnet test tests/ClaudeAgentSdk.Tests/ClaudeAgentSdk.Tests.fsproj -c Release
dotnet build examples/BasicUsage/BasicUsage.fsproj -c Release
dotnet run --project examples/OfflineDemo -c Release -- examples/OfflineDemo/fake-cli.sh
```
