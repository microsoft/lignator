#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mHEAD & TAIL TESTS\e[m"
echo -e "\n# HEAD & TAIL  TESTS" >> test-results.md

passed=0
failed=0

# HAS PLAIN HEAD
echo -e "\e[1;33;4;44mTEST: -t \${randomitem(world,universe)} -l 10 -H \"I am a title\"\e[m"
echo -e "\n## TEST: -t \$randomitem(world,universe)} -l 10 -H \"I am a title\"\n" >> test-results.md
result=$($path_to_lignator -t 'hello ${randomitem(world,universe)}' -l 10 -o /dev/stdout -H 'I am a title' | head -n 1)
if [[ "$result" != "I am a title" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'I am a title' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'I am a title' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'I am a title' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'I am a title' actual $result outcome: PASSED" >> test-results.md
fi

# HEAD WITH TOKENS HEAD
echo -e "\e[1;33;4;44mTEST: -t \${randomitem(world,universe)} -l 10 -H \"I am a \${randomitem(title,heading)}\"\e[m"
echo -e "\n## TEST: -t \$randomitem(world,universe)} -l 10 -H \"I am a \${randomitem(title,heading)}\"\n" >> test-results.md
result=$($path_to_lignator -t 'hello ${randomitem(world,universe)}' -l 10 -o /dev/stdout -H 'I am a ${randomitem(title,heading)}' | head -n 1 | grep "I am a \(title\|heading\)" | wc -l )
if [[ $result != 1 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 1 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 1 actual $result outcome: PASSED" >> test-results.md
fi

# HAS PLAIN TAIL
echo -e "\e[1;33;4;44mTEST: -t \${randomitem(world,universe)} -l 10 -T \"I am a tail\"\e[m"
echo -e "\n## TEST: -t \$randomitem(world,universe)} -l 10 -T \"I am a tail\"\n" >> test-results.md
result=$($path_to_lignator -t 'hello ${randomitem(world,universe)}' -l 10 -o /dev/stdout -T 'I am a tail' | tail -n 1)
if [[ "$result" != "I am a tail" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'I am a tail' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'I am a tail' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'I am a tail' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'I am a tail' actual $result outcome: PASSED" >> test-results.md
fi

# TAIL WITH TOKENS HEAD
echo -e "\e[1;33;4;44mTEST: -t \${randomitem(world,universe)} -l 10 -T \"I am a \${randomitem(tail,ending)}\"\e[m"
echo -e "\n## TEST: -t \$randomitem(world,universe)} -l 10 -T \"I am a \${randomitem(tail,ending)}\"\n" >> test-results.md
result=$($path_to_lignator -t 'hello ${randomitem(world,universe)}' -l 10 -o /dev/stdout -T 'I am a ${randomitem(tail,ending)}' | tail -n 1 | grep "I am a \(tail\|ending\)" | wc -l )
if [[ $result != 1 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 1 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 1 actual $result outcome: PASSED" >> test-results.md
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