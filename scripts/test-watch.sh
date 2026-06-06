#!/usr/bin/env bash
# TDD loop — re-runs a test lane on every save.
#
# Defaults to the fast, Docker-free unit project (plans/test-split-unit-integration-handoff.md),
# so the inner loop has no Testcontainers startup cost. Watch the integration
# lane instead with LANE=integration; note its Postgres container is
# collection-scoped, so it restarts once per save (~3-5s/cycle) — narrow with
# a filter for tight loops, e.g.:
#   LANE=integration scripts/test-watch.sh -- --filter "FullyQualifiedName~Smoke"

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck source=./_testcontainers-env.sh
source "${repo_root}/scripts/_testcontainers-env.sh"

case "${LANE:-unit}" in
  unit)        project=UnitTests ;;
  integration) project=IntegrationTests ;;
  *) echo "usage: LANE=[unit|integration] scripts/test-watch.sh [-- <runner args>]" >&2; exit 2 ;;
esac

exec dotnet watch \
  --project "${repo_root}/tests/ThanyMarcus.Portal.${project}" \
  -- run -c "${CONFIGURATION:-Debug}" -- "$@"
