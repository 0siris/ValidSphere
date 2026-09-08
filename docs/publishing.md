# Publishing ValidSphere

ValidSphere is published to NuGet.org by the `Publish NuGet` GitHub Actions workflow. Releases are cut from the protected `release` branch, which accepts merges only via pull request with a green CI build. Publishing a GitHub release triggers the workflow; saving a draft does not.

## One-time setup

### GitHub repository variable

In `Settings` → `Secrets and variables` → `Actions` → `Variables`, create:

| Name | Value |
| --- | --- |
| `NUGET_USER` | NuGet.org profile name, currently `0siris` |

This is a configuration variable, not an API key.

### NuGet.org trusted publishing

In the NuGet.org account menu, open `Trusted Publishing` and add a GitHub policy with:

| Field | Value |
| --- | --- |
| Repository owner | `0siris` |
| Repository | `ValidSphere` |
| Workflow file | `publish.yml` |
| Environment | Empty |
| Package scope | `ValidSphere` |

Enter only `publish.yml`, not `.github/workflows/publish.yml`. The workflow uses OIDC to obtain a short-lived NuGet key; no permanent API key is stored in GitHub.

See [NuGet trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) for the upstream setup documentation.

## Create a preview release

1. Prepare the release on a short-lived branch from `main`: update `<Version>` in `ValidSphere.csproj` and the installation example in `Readme.md`, then open a pull request `main` → `release`.
2. Merge the pull request once the `build` check is green. GitHub blocks the merge otherwise — `release` accepts no direct pushes, not even for administrators.
3. Confirm that `origin/release` contains the merge commit:

   ```powershell
   git fetch origin
   git status --short --branch
   git log -1 --oneline origin/release
   dotnet build ValidSphere.csproj --configuration Release
   dotnet pack ValidSphere.csproj --configuration Release --output artifacts
   ```

4. Create a draft targeting that exact commit. Replace the version for each release:

   ```powershell
   $version = "0.1.0-preview.2"
   $tag = "v$version"
   $commit = git rev-parse origin/release

   gh release create $tag `
     --repo 0siris/ValidSphere `
     --target $commit `
     --title "ValidSphere $version" `
     --generate-notes `
     --draft `
     --prerelease `
     --latest=false
   ```

5. Review the draft title, notes, tag, target commit, and pre-release flag on GitHub.
6. Publish the reviewed draft:

   ```powershell
   gh release edit $tag --repo 0siris/ValidSphere --draft=false
   ```

Publishing creates the tag and starts `.github/workflows/publish.yml`. The tag without its leading `v` becomes the NuGet package version.

For a stable release, use a stable semantic version such as `1.0.0` and omit `--prerelease` when creating the draft.

## Verify the publication

Check the workflow from the command line:

```powershell
$runId = gh run list --repo 0siris/ValidSphere --workflow publish.yml --limit 1 --json databaseId --jq '.[0].databaseId'
gh run watch $runId --repo 0siris/ValidSphere
```

After the workflow succeeds, verify the version on [NuGet.org](https://www.nuget.org/packages/ValidSphere).

## Failure handling

- If OIDC authentication fails, verify `NUGET_USER` and the trusted-publishing policy values above.
- If the published release fails because its source needs a change, fix the source and create a new package version; do not move the release tag.
- A published NuGet version cannot be overwritten. Fix the problem and publish a new version instead.
- Re-running the workflow is safe when the same package version already exists because the push uses `--skip-duplicate`.
