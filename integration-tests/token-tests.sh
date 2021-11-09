#! /bin/bash
path_to_lignator=$1
echo "Using: $path_to_lignator"

echo -e "\e[1;33;4;44mTOKEN TESTS\e[m"
echo -e "\n# TOKEN TESTS" >> test-results.md

echo -e "| Test | Expected | Actual | Pass |" >> test-results.md
echo -e "| ---- | -------- | ------ | ---- |" >> test-results.md

passed=0
failed=0

# RANDOMITEM
echo -e "\e[1;33;4;44mTEST: -t \${randomitem(world,universe)} -l 10\e[m"
result=$($path_to_lignator -t 'hello ${randomitem(world,universe)}' -l 10 -o /dev/stdout | grep "hello \(world\|universe\)" | wc -l)
if [[ $result != "10" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 10 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|randomitem|10|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 10 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|randomitem|10|$result|✔️|" >> test-results.md
fi

# RANDOMITEM not all duplicates
echo -e "\e[1;33;4;44mTEST: randomitem not all duplicates\e[m"
result=$($path_to_lignator -t 'hello ${randomitem(world,universe,galaxy)}' -l 100 -o /dev/stdout | grep "hello \(world\|universe\|galaxy\)" | sort | uniq -d | wc -l)
if [[ $result != "3" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 3 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|randomitem no duplications|3|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 3 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|randomitem no duplications|3|$result|✔️|" >> test-results.md
fi

# RANDOMBETWEEN
echo -e "\e[1;33;4;44mTEST: -t \${randombetween(1,5)} -l 20\e[m"
result=$($path_to_lignator -t 'number: ${randombetween(1,5)}' -l 20 -o /dev/stdout | grep  "number: \(1\|2\|3\|4\|5\)" | wc -l)
if [[ $result != "20" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 20 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|randombetween|20|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 20 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|randombetween|20|$result|✔️|" >> test-results.md
fi

# RANDOMBETWEEN not all duplicates
echo -e "\e[1;33;4;44mTEST: randombetween not all duplicates\e[m"
result=$($path_to_lignator -t 'number: ${randombetween(1,5)}' -l 100 -o /dev/stdout | grep  "number: \(1\|2\|3\|4\|5\)" | sort | uniq -d |wc -l)
if [[ $result != "5" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 5 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|randombetween no duplications|5|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 5 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|randombetween no duplications|5|$result|✔️|" >> test-results.md
fi

# UUID
echo -e "\e[1;33;4;44mTEST: -t 'uuid: \${uuid}' -l 30\e[m"
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 30 -o /dev/stdout | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | wc -l)
if [[ $result != "30" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|uuid|30|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|uuid|30|$result|✔️|" >> test-results.md
fi

# UUID all unique
echo -e "\e[1;33;4;44mTEST: UUID all unique\e[m"
result=$($path_to_lignator -t 'uuid: ${uuid}' -l 30 -o /dev/stdout | egrep "^uuid: [[:alnum:]]{8}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{4}\b-[[:alnum:]]{12}$" | sort | uniq -u | wc -l)
if [[ $result != "30" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|uuid all unique|30|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 30 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|uuid all unique|30|$result|✔️|" >> test-results.md
fi

# UTCNOW
echo -e "\e[1;33;4;44mTEST: -t 'timestamp: \${utcnow()}' -l 40\e[m"
result=$($path_to_lignator -t 'timestamp: ${utcnow()}' -l 40 -o /dev/stdout | egrep "^timestamp: [0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{3}$" | wc -l)
if [[ $result != "40" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 40 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|utcnow|40|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 40 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|utcnow|40|$result|✔️|" >> test-results.md
fi

# UTCNOW WITH FORMAT STRING
echo -e "\e[1;33;4;44mTEST: -t 'timestamp: \${utcnow(dd MMM yyyy)}' -l 50\e[m"
result=$($path_to_lignator -t 'timestamp: ${utcnow(dd MMM yyyy)}' -l 50 -o /dev/stdout | egrep "^timestamp: [0-9]{2} [A-Z]{1}[a-z]{2} [0-9]{4}$" | wc -l)
if [[ $result != "50" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 50 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|utcnow with format string|50|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 50 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|utcnow with format string|50|$result|✔️|" >> test-results.md
fi

# UTCNOW all unique
echo -e "\e[1;33;4;44mTEST: UTCNOW all unique\e[m"
result=$($path_to_lignator -t 'timestamp: ${utcnow(yyyy-MM-dd HH:mm:ss.fffffff)}' -l 60 -o /dev/stdout | egrep "^timestamp: [0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{7}$" | sort | uniq -u | wc -l)
if [[ $result != "60" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 60 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|utcnow all unique|60|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 60 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|utcnow all unique|60|$result|✔️|" >> test-results.md
fi

# COMBINE TOKENS (ALL)
echo -e "\e[1;33;4;44mTEST: -t 'hello \${randomitem(world,universe)} number: \${randombetween(1,5)} uuid: \${uuid} timestamp: \${utcnow(dd MMM yyyy)} timestamp: \${utcnow(dd MMM yyyy)}' -l 100\e[m"
result=$($path_to_lignator -t 'hello ${randomitem(world,universe)} number: ${randombetween(1,5)} uuid: ${uuid} timestamp: ${utcnow(dd MMM yyyy)} timestamp: ${utcnow(dd MMM yyyy)}' -l 100 -o /dev/stdout | egrep "^hello .* uuid: .* timestamp: .* timestamp: .*$" | wc -l)
if [[ $result != "100" ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 100 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|all tokens|100|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 100 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|all tokens|100|$result|✔️|" >> test-results.md
fi

# LINESFROMFILE
echo -e "\e[1;33;4;44mTEST: LINESFROMFILE\e[m"
result=$($path_to_lignator -t '${linefromfile(./integration-tests/samples/basic_mutliline.txt)}' -o /dev/stdout | grep '\(Hello\|World\)' | wc -l)
if [[ $result != 1 ]];
then
  failed=$(($failed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[31m $result \e[m outcome: \e[31mFAILED\e[m"
  echo -e "|line from file|1|$result|❌|" >> test-results.md
else
  passed=$(($passed+1))
  echo -e "expected: \e[32m 1 \e[m actual: \e[32m $result \e[m outcome: \e[32mPASSED\e[m"
  echo -e "|line from file|1|$result|✔️|" >> test-results.md
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