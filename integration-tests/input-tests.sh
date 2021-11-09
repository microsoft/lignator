#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mINPUT TESTS\e[m"
echo -e "\n# INPUT TESTS" >> test-results.md

echo -e "| Test | Expected | Actual | Pass |" >> test-results.md
echo -e "| ---- | -------- | ------ | ---- |" >> test-results.md

passed=0
failed=0

# INLINE
echo -e "\e[1;33;4;44mTEST: -t 'inline template'\e[m"
result=$($path_to_lignator  -t 'inline template' -o /dev/stdout | head -n 1)
if [[ "$result" != "inline template" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'inline template' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|Inline|inline template|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'inline template' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|Inline|inline template|$result|✔️|" >> test-results.md
fi

# FROM FILE
echo -e "\e[1;33;4;44mTEST: -t 'FILE template'\e[m"
result=$($path_to_lignator  -t './integration-tests/samples/file.template' -o /dev/stdout | head -n 1)
if [[ "$result" != "Hello from file" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'Hello from file' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|From file|Hello from file|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'Hello from file' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|From file|Hello from file|$result|✔️|" >> test-results.md
fi

# FROM FILE WITH TWO LINES
echo -e "\e[1;33;4;44mTEST: -t 'FILE WITH LINES template'\e[m"
result=$($path_to_lignator  -t './integration-tests/samples/file3.template' -o /dev/stdout -l 100 | sort | uniq | wc -l)
if [[ "$result" != 2 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 2 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|FROM FILE WITH TWO LINES|2|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 2 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|FROM FILE WITH TWO LINES|2|$result|✔️|" >> test-results.md
fi

# FROM DIR
echo -e "\e[1;33;4;44mTEST: -t 'DIR template'\e[m"
result=$($path_to_lignator  -t './integration-tests/samples' -l 100 -o /dev/stdout | sort | uniq | wc -l)
if [[ "$result" != 4 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 4 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|FROM DIR|4|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 4 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|FROM DIR|4|$result|✔️|" >> test-results.md
fi

total=$(($passed+$failed))
echo -e "\e[1;33;4;44mRESULTS\e[m"
echo -e "\e[32m Passed: $passed\e[m"
echo -e "\e[31m Failed: $failed\e[m"
echo -e "\e[32m Total:  $total\e[m"
echo -e "|**Results**|✔️ Passed: $passed|❌ Failed: $failed|🏁 Total: $total|" >> test-results.md

if [[ $failed !=  0 ]];
then
  exit 1
fi