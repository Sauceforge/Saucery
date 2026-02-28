# Copilot Instructions

## Project Guidelines
- User prefers pipeline jobs to build once, upload compiled artifacts to the artifact named 'build-artifacts', have test jobs download that artifact into the '${{ env.BUILD_ARTIFACTS }}' folder, and include a cleanup job to delete build artifacts for the run. Apply this pattern to tunit-pipeline.yml.