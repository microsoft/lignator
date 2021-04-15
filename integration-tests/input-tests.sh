#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mINPUT TESTS\e[m"
echo -e "\n# INPUT TESTS" >> test-results.md

passed=0
failed=0

# INLINE
echo -e "\e[1;33;4;44mTEST: -t 'inline template'\e[m"
echo -e "\n## TEST: -t 'inline template'\n" >> test-results.md
result=$($path_to_lignator  -t 'inline template' -o /dev/stdout | head -n 1)
if [[ "$result" != "inline template" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'inline template' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'inline template' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'inline template' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'inline template' actual $result outcome: PASSED" >> test-results.md
fi

# FROM FILE
echo -e "\e[1;33;4;44mTEST: -t 'FILE template'\e[m"
echo -e "\n## TEST: -t 'FILE template'\n" >> test-results.md
result=$($path_to_lignator  -t './integration-tests/samples/file.template' -o /dev/stdout | head -n 1)
if [[ "$result" != "Hello from file" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'Hello from file' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'Hello from file' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'Hello from file' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'Hello from file' actual $result outcome: PASSED" >> test-results.md
fi

# FROM FILE WITH TWO LINES
echo -e "\e[1;33;4;44mTEST: -t 'FILE WITH LINES template'\e[m"
echo -e "\n## TEST: -t 'FILE WITH LINES template'\n" >> test-results.md
result=$($path_to_lignator  -t './integration-tests/samples/file3.template' -o /dev/stdout -l 100 | sort | uniq | wc -l)
if [[ "$result" != 2 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 2 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 2 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 2 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 2 actual $result outcome: PASSED" >> test-results.md
fi

# FROM DIR
echo -e "\e[1;33;4;44mTEST: -t 'DIR template'\e[m"
echo -e "\n## TEST: -t 'DIR template'\n" >> test-results.md
result=$($path_to_lignator  -t './integration-tests/samples' -l 100 -o /dev/stdout | sort | uniq | wc -l)
if [[ "$result" != 4 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 4 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 4 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 4 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 4 actual $result outcome: PASSED" >> test-results.md
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

if [[ $failed !=  0 ]];
then
  exit 1
fi