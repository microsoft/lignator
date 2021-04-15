#! /bin/bash
path_to_lignator=$1
echo -e "Using: $path_to_lignator"
echo -e "# lignator integration tests\n" > test-results.md

export lignator_no_banner=true
export lignator_log_level=None

./integration-tests/token-tests.sh $path_to_lignator
./integration-tests/head-tail-tests.sh $path_to_lignator
./integration-tests/variable-tests.sh $path_to_lignator
./integration-tests/input-tests.sh $path_to_lignator
./integration-tests/output-tests.sh $path_to_lignator

export lignator_no_banner=false
export lignator_log_level=Information

exit 0