#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mOUTPUT TESTS\e[m"
echo -e "\n# OUTPUT TESTS" >> test-results.md

passed=0
failed=0

# /dev/stdout
echo -e "\e[1;33;4;44mTEST: -t 'this is a log' -o /dev/stdout \e[m"
echo -e "\n## TEST: -t 'this is a log' -o /dev/stdout \n" >> test-results.md
result=$($path_to_lignator  -t 'this is a log' -o /dev/stdout | head -n 1)
if [[ "$result" != "this is a log" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'this is a log' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'this is a log' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'this is a log' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'this is a log' actual $result outcome: PASSED" >> test-results.md
fi

# ./logs/lignator.log
echo -e "\e[1;33;4;44mTEST: -t 'this is a log' \e[m"
echo -e "\n## TEST: -t 'this is a log' \n" >> test-results.md
$path_to_lignator -t 'this is a log'
result=$(tail -n 1 ./logs/lignator.log)
if [[ "$result" != "this is a log" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'this is a log' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'this is a log' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'this is a log' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'this is a log' actual $result outcome: PASSED" >> test-results.md
fi

# ./logs/lignator.json
echo -e "\e[1;33;4;44mTEST: -t 'this is a json log' -e json\e[m"
echo -e "\n## TEST: -t 'this is a json log' -e json\n" >> test-results.md
$path_to_lignator -t 'this is a json log' -e json
result=$(tail -n 1 ./logs/lignator.json)
if [[ "$result" != "this is a json log" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'this is a json log' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'this is a json log' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'this is a json log' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'this is a json log' actual $result outcome: PASSED" >> test-results.md
fi

# ./testlogs/lignator.log
echo -e "\e[1;33;4;44mTEST: -t 'this is a test log' -o testlogs \e[m"
echo -e "\n## TEST: -t 'this is a test log'  -o testlogs \n" >> test-results.md
$path_to_lignator -t 'this is a test log' -o testlogs
result=$(tail -n 1 ./testlogs/lignator.log)
if [[ "$result" != "this is a test log" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'this is a test log' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'this is a test log' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'this is a test log' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'this is a test log' actual $result outcome: PASSED" >> test-results.md
fi

# output ./testlogss/*
echo -e "\e[1;33;4;44mTEST: -t './integration-tests/samples' -o testlogs2 \e[m"
echo -e "\n## TEST: -t './integration-tests/samples'  -o testlogs2 \n" >> test-results.md
$path_to_lignator -t './integration-tests/samples' -o testlogs2
result=$(ls ./testlogs2 | wc -l)
if [[ "$result" != 3 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 3 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 3 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 3 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 3 actual $result outcome: PASSED" >> test-results.md
fi


total=$(($passed+$failed))
echo -e "\e[1;33;4;44mRESULTS\e[m"
echo -e "\n# RESULTS\n" >> test-results.md
echo -e "\e[32m Passed: $passed\e[m"
echo -e "✔️ Passed: $passed" >> test-results.md
echo -e "\e[31m Failed: $failed\e[m"
echo -e "❌ Failed: $failed" >> test-results.md
echo -e "\e[32m Total:  $total\e[m"
echo -e "🏁 Total: $total" >> test-results.md

# clean up
rm -fr ./logs
rm -fr ./testlogs
rm -fr ./testlogs2

if [[ $failed !=  0 ]];
then
  exit 1
fi