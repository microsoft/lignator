#! /bin/bash

path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mENV TESTS\e[m"
echo -e "\n# ENV TESTS" >> test-results.md

passed=0
failed=0

# lignator_template
echo -e "\e[1;33;4;44mTEST: lignator_template\e[m"
echo -e "\n## TEST: lignator_template\n" >> test-results.md
export lignator_template='static template'
result=$($path_to_lignator -o /dev/stdout | head -n 1)
if [[ "$result" != "static template" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'static template' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 'static template' actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'static template' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 'static template' actual $result outcome: PASSED" >> test-results.md
fi
unset lignator_template

# lignator_logs
echo -e "\e[1;33;4;44mTEST: lignator_logs\e[m"
echo -e "\n## TEST: lignator_logs\n" >> test-results.md
export lignator_logs=30
result=$($path_to_lignator -t 'uuid: ${uuid}' -o /dev/stdout | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)
if [[ $result != "30" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 30 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 30 actual $result outcome: PASSED" >> test-results.md
fi
unset lignator_logs

# lignator_runs
echo -e "\e[1;33;4;44mTEST: lignator_runs\e[m"
echo -e "\n## TEST: lignator_runs\n" >> test-results.md
export lignator_runs=20
result=$($path_to_lignator -t 'uuid: ${uuid}' -o /dev/stdout | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)
if [[ $result != "20" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 20 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 20 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 20 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 20 actual $result outcome: PASSED" >> test-results.md
fi
unset lignator_runs

# lignator_output
echo -e "\e[1;33;4;44mTEST: lignator_output\e[m"
echo -e "\n## TEST: lignator_output\n" >> test-results.md
export lignator_output='/dev/stdout'
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 10 | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)
if [[ $result != "10" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 10 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "❌ expected: 10 actual $result outcome: FAILED" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 10 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "✔️ expected: 10 actual $result outcome: PASSED" >> test-results.md
fi
unset lignator_output

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