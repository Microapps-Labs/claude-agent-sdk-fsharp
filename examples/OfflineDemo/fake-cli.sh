#!/bin/sh
# Local protocol fixture. It ignores CLI flags and records only synthetic input.
set -eu
cat > "$DEMO_INPUT_LOG"
printf '%s\n' '{"type":"result","subtype":"success","duration_ms":0,"duration_api_ms":0,"is_error":false,"num_turns":1,"session_id":"offline-demo","result":"Local fixture only"}'
