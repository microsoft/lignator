#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mVARIABLE TESTS\e[m"
echo -e "\n# VARIABLE TESTS" >> test-results.md

passed=0
failed=0

# USE VARIABLE
echo -e "\e[1;33;4;44mTEST: -t \${variable(myVar} -l 10 -V \"myVar='I am a variable'\"\e[m"
echo -e "\n## TEST:  -t \${variable(myVar} -l 10 -V \"myVar='I am a variable'\"\n" >> test-results.md
result=$($path_to_lignator -t '${variable(myVar)}' -l 10 -V 'myVar=I am a variable' -o /dev/stdout | head -n 1)
if [[ "$result" != "I am a variable" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'I am a variable' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'I am a variable' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'I am a variable' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'I am a variable' actual $result outcome: PASSED" >> test-results.md
fi

# VARIABLE SAME PER LOG
echo -e "\e[1;33;4;44mTEST: -t \${variable(myVar}\${variable(myVar)} -V \"myVar='\${randomitem(a,b)}'\"\e[m"
echo -e "\n## TEST:  -t \${variable(myVar)}\${variable(myVar)}  -V \"myVar='\${randomitem(a,b)}'\"\n" >> test-results.md
result=$($path_to_lignator -t '${variable(myVar)}${variable(myVar)}' -V 'myVar=${randomitem(a,b)}' -o /dev/stdout | grep '[aa|bb]' | wc -l)
if [[ "$result" != 1 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 1 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 1 actual $result outcome: PASSED" >> test-results.md
fi

# VARIABLE DIFFERENT PER LOG
echo -e "\e[1;33;4;44mTEST: -t \${variable(myVar} -V \"myVar='\${uuid}'\" -l 2\e[m"
echo -e "\n## TEST:  -t \${variable(myVar)}  -V \"myVar='\${uuid}'\" -l 2\n" >> test-results.md
result=$($path_to_lignator -t '${variable(myVar)}' -V 'myVar=${uuid}' -o /dev/stdout -l 2)
result1=$(echo -e "$result" | head -n 1)
result2=$(echo -e "$result" | tail -n 1)
if [[ $result1 -eq $result2 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m $result1 != $result2 \e[m actual: \e[31m $result1 == $result2 \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: $result1 != $result2 actual $result1 == $result2 outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m $result1 != $result2 \e[m actual: \e[32m $result1 != $result2 \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: $result1 != $result2 actual $result1 != $result2 outcome: PASSED" >> test-results.md
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