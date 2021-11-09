#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mVARIABLE TESTS\e[m"
echo -e "\n# VARIABLE TESTS" >> test-results.md

echo -e "| Test | Expected | Actual | Pass |" >> test-results.md
echo -e "| ---- | -------- | ------ | ---- |" >> test-results.md

passed=0
failed=0

# USE VARIABLE
echo -e "\e[1;33;4;44mTEST: -t \${variable(myVar} -l 10 -V \"myVar='I am a variable'\"\e[m"
result=$($path_to_lignator -t '${variable(myVar)}' -l 10 -V 'myVar=I am a variable' -o /dev/stdout | head -n 1)
if [[ "$result" != "I am a variable" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'I am a variable' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|use variable|I am a variable|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'I am a variable' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|use variable|I am a variable|$result|✔️|" >> test-results.md
fi

# VARIABLE SAME PER LOG
echo -e "\e[1;33;4;44mTEST: -t \${variable(myVar}\${variable(myVar)} -V \"myVar='\${randomitem(a,b)}'\"\e[m"
result=$($path_to_lignator -t '${variable(myVar)}${variable(myVar)}' -V 'myVar=${randomitem(a,b)}' -o /dev/stdout | grep '[aa|bb]' | wc -l)
if [[ "$result" != 1 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|variable same per log|1|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|variable same per log|1|$result|✔️|" >> test-results.md
fi

# VARIABLE DIFFERENT PER LOG
echo -e "\e[1;33;4;44mTEST: -t \${variable(myVar} -V \"myVar='\${uuid}'\" -l 2\e[m"
result=$($path_to_lignator -t '${variable(myVar)}' -V 'myVar=${uuid}' -o /dev/stdout -l 2)
result1=$(echo -e "$result" | head -n 1)
result2=$(echo -e "$result" | tail -n 1)
if [[ $result1 -eq $result2 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m $result1 != $result2 \e[m actual: \e[31m $result1 == $result2 \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|variable different per log|$result1 != $result2|$result1 == $result2|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m $result1 != $result2 \e[m actual: \e[32m $result1 != $result2 \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|variable different per log|$result1 != $result2|$result1 != $result2|✔️|" >> test-results.md
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