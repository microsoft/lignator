#! /bin/bash

path_to_lignator=$1
rm -fr ./logs

echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mENV TESTS\e[m"
echo -e "\n# ENV TESTS" >> test-results.md

passed=0
failed=0

echo -e "| Test | Expected | Actual | Pass |" >> test-results.md
echo -e "| ---- | -------- | ------ | ---- |" >> test-results.md

# lignator_template
echo -e "\e[1;33;4;44mTEST: lignator_template\e[m"
export lignator_template='static template'
result=$($path_to_lignator -o /dev/stdout | head -n 1)

if [[ "$result" != "static template" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 'static template' \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_template|static template|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 'static template' \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "lignator_template|static template|$result|✔️|" >> test-results.md
fi
unset lignator_template

# lignator_logs
echo -e "\e[1;33;4;44mTEST: lignator_logs\e[m"
export lignator_logs=30
result=$($path_to_lignator -t 'uuid: ${uuid}' -o /dev/stdout | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)

if [[ $result != "30" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_logs|30|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_logs|30|$result|✔️|" >> test-results.md
fi
unset lignator_logs

# lignator_runs
echo -e "\e[1;33;4;44mTEST: lignator_runs\e[m"
export lignator_runs=20
result=$($path_to_lignator -t 'uuid: ${uuid}' -o /dev/stdout | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)

if [[ $result != "20" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 20 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_runs|20|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 20 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_runs|20|$result|✔️|" >> test-results.md
fi
unset lignator_runs

# lignator_output
echo -e "\e[1;33;4;44mTEST: lignator_output\e[m"
export lignator_output='/dev/stdout'
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 10 | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)

if [[ $result != "10" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 10 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_output|10|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 10 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_output|10|$result|✔️|" >> test-results.md
fi
unset lignator_output

# lignator_clean
echo -e "\e[1;33;4;44mTEST: lignator_clean\e[m"
export lignator_clean=true
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 10 | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)

if [[ -f "./logs/lignator.log" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m No logs \e[m actual: \e[31m Logs found \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_clean|No logs|Logs found|❌| " >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m No logs \e[m actual: \e[32m No logs \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_clean|No logs|No logs|✔️| " >> test-results.md
fi
unset lignator_clean

# lignator_infinite
echo -e "\e[1;33;4;44mTEST: lignator_infinite\e[m"
export lignator_infinite=true
export lignator_log_level=Information
$path_to_lignator -t 'uuid: ${uuid}' -l 10 > ./output.test &
pid=$(pidof $path_to_lignator)
sleep 1
kill $pid
result=$(cat ./output.test | egrep "infinite flag == true, will run until the process is stopped$" | wc -l)
if [[ $result != 1 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m warning for infinite \e[m actual: \e[31m No info log \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_infinite|warning for infinite|No info log|❌| " >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m warning for infinite \e[m actual: \e[32m warning for infinite \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_infinite|warning for infinite|warning for infinite|✔️| " >> test-results.md
fi
rm -f ./output.test
unset lignator_infinite
export lignator_log_level=None

# lignator_extension
echo -e "\e[1;33;4;44mTEST: lignator_extension\e[m"
export lignator_extension='test'
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 10 | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)
if [[ ! -f "./logs/lignator.test" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m lignator.$lignator_extension \e[m actual: \e[31m lignator.$lignator_extension \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_extension|lignator.$lignator_extension| file not found|❌| " >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m lignator.$lignator_extension \e[m actual: \e[32m lignator.$lignator_extension \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_extension|lignator.$lignator_extension|lignator.$lignator_extension|✔️| " >> test-results.md
fi
unset lignator_extension

# lignator_multiline
echo -e "\e[1;33;4;44mTEST: lignator_multiline\e[m"
echo -e "hello\nworld" > ./input.test
export lignator_multiline=true
$path_to_lignator -t '$PWD/input.test'
result=$(cat $PWD/input.test | egrep "hello|world" | wc -l)

if [[ $result != "2" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 2 logs \e[m actual: \e[31m $result logs \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_multiline|2 logs|$result logs|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 2 logs \e[m actual: \e[32m $result logs \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_multiline|2 logs|$result logs|✔️|" >> test-results.md
fi
rm -f ./input.test
rm -fr ./logs
unset lignator_multiline

# lignator_head
echo -e "\e[1;33;4;44mTEST: lignator_head\e[m"
export lignator_head='Not the tail'
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 10  && head -n 1 ./logs/lignator.log)
if [[ $result != $lignator_head ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m $lignator_head \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_head|$lignator_head|$result|❌| " >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m $lignator_head \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_head|$lignator_head|$result|✔️| " >> test-results.md
fi
unset lignator_head

# lignator_tail
echo -e "\e[1;33;4;44mTEST: lignator_tail\e[m"
export lignator_tail='Not the head'
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 10 && tail -n 1 ./logs/lignator.log)
if [[ $result != $lignator_tail ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m $lignator_tail \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_tail|$lignator_tail|$result|❌| " >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m $lignator_tail \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_tail|$lignator_tail|$result|✔️| " >> test-results.md
fi
unset lignator_tail

# lignator_variables
rm -fr ./logs
echo -e "\e[1;33;4;44mTEST: lignator_variables\e[m"
export lignator_variables='one=Hello;two=World'
result=$($path_to_lignator -t '${variable(one)} ${variable(two)}' && head -n 1 ./logs/lignator.log)
if [[ $result != "Hello World" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m Hello World \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|lignator_variables|Hello World|$result|❌| " >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m Hello World \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|lignator_variables|Hello World|$result|✔️| " >> test-results.md
fi
rm -fr ./logs
unset lignator_variables

total=$(($passed+$failed))
echo -e "\e[1;33;4;44mRESULTS\e[m"
echo -e "\e[32m Passed: $passed\e[m"
echo -e "\e[31m Failed: $failed\e[m"
echo -e "\e[32m Total:  $total\e[m"
echo -e "|**Results**|✔️ Passed: $passed|❌ Failed: $failed|🏁 Total: $total|" >> test-results.md

rm -fr ./logs

if [[ $failed !=  0 ]];
then
  exit 1
fi
