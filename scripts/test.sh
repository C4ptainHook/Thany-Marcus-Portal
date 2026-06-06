#!/usr/bin/env bash
# Canonical entrypoint for the Portal test suite (unit + integration lanes).
#
# Why this exists: as of .NET 10.0.201 + xunit.v3.mtp-v2 3.x, `dotnet test`
# does not discover MTP-style tests (reports "Zero tests ran"). The reliable
# invocation is `dotnet run` on the test project. See plans/portal-001-handoff.md.
#
# The suite is split into a fast, Docker-free unit project and a
# Testcontainers-backed integration project (plans/test-split-unit-integration-handoff.md).
# Both run by default; pick one with LANE:  LANE=unit scripts/test.sh
# Any extra args are forwarded to the runner, e.g. scripts/test.sh --filter ...

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck source=./_testcontainers-env.sh
source "${repo_root}/scripts/_testcontainers-env.sh"

lane="${LANE:-all}"

run_lane() {
  dotnet run \
    --project "${repo_root}/tests/ThanyMarcus.Portal.$1" \
    -c "${CONFIGURATION:-Release}" \
    -- "${@:2}"
}

case "$lane" in
  unit)        run_lane UnitTests "$@" ;;
  integration) run_lane IntegrationTests "$@" ;;
  all)         run_lane UnitTests "$@"; run_lane IntegrationTests "$@" ;;
  *) echo "usage: LANE=[unit|integration|all] scripts/test.sh [runner args]" >&2; exit 2 ;;
esac
